/*
Объяснение: что это код делает?
Есть папка с файлами (json-чиками), файлов там много миллионов, мой код запускается и в три потока (столько у меня свободных ядер процессора), открывает файлы, 
вытаскивает из них некотрые атрибуты, и пишет вытащенное в MySQL.
При этом добавление в очередь "файлов для обраьботки" реализовано через scoped_lock,
вытаскивание в рабочих тредах-воркерах - тоже через scoped_lock, так что всё вроде бы потокобезопасно.
Можно придраться к "египетским скобкам" в болшей части кода :-)
А вообще - было бы интересно узнать, хорошо написано и если плохо - то почему
*/

#include <iostream>
#include <string> 
#include <fstream>
#include <filesystem>
#include <chrono>
#include <thread>
#include <mutex>

#include <plog/Log.h> // include the headers for logging
#include "plog/Initializers/RollingFileInitializer.h"
#include "plog/Appenders/ColorConsoleAppender.h"

#include "nlohmann/json.hpp"
#include "soci/soci.h"
#include "soci/mysql/soci-mysql.h"
#include "Utf8String.cpp"

using namespace std;
namespace fs = std::filesystem;
using json = nlohmann::json;
using namespace soci;

vector<string> files;
mutex m;

void file_worker();
map<std::thread::id, size_t> fcounters;

void ProcessFile(session& sql, const string& file);

int main(){
    auto start = std::chrono::system_clock::now();
    std::time_t curr_time = std::chrono::system_clock::to_time_t(start);

    plog::ColorConsoleAppender<plog::TxtFormatter> consoleAppender; // Create the 2nd appender.
    plog::init(plog::debug, "process_emails.log").addAppender( &consoleAppender ); // Initialize the logger

    LOGD << string(65, '*');
    LOGD << "process_emails started!";

    // Создаём 3 очереди на обработку. Как только в очередях накапливается по 100 элементов - запускаем треды, котрые будут обрабатывать очереди
    const string src_path = "D:/SavedCert_archive/";
    thread t1, t2, t3;
    size_t cnt=0;
    for (const auto& entry : fs::directory_iterator(src_path)){
        auto path = entry.path().string();
        {
            scoped_lock lck(m);
            files.push_back(path);
        }
        if (++cnt==300){
            // в векторе уже достатчоно имён файлов, чтобы запустить обработчики
            t1 = thread{file_worker};
            t2 = thread{file_worker};
            t3 = thread{file_worker};
        }
    }
    LOGD << "added " << cnt << " elements to the vector";
    LOGD << "current vector size is: " << files.size();

    t1.join();
    LOGD << "t1.join() finished!";
    t2.join();
    LOGD << "t2.join() finished!";
    t3.join();
    LOGD << "t3.join() finished!";

    LOGD << "process finished!"; 
    
    auto end = std::chrono::system_clock::now();
    std::chrono::duration<double> elapsed_seconds = end-start;
    LOGD << "Elapsed seconds: " << (int)elapsed_seconds.count();
    size_t fcount = 0;
    for( auto& fcntitem : fcounters )
        fcount += fcntitem.second;
    LOGD << "Processed files: " << fcount;
}

string GetFileForProcessing(){
    scoped_lock lock(m);
    if ( files.empty() )
        return "";
    string t = files.back(); 
    files.pop_back();
    return t;
}

void file_worker(){
    ifstream sqlpasswd("mysql.auth");
    string cnstr;
    getline(sqlpasswd, cnstr);
    session sql(mysql, cnstr);
    std::thread::id th_id = this_thread::get_id();
    fcounters[th_id]=0;
    while(true){
        // Получаю файл для обработки через функцию, которая делает lock вектору
        string path = GetFileForProcessing();
        // LOGD << "processing file: " << path << " at thread " << th_id ;
        if ( path=="" )
            return;
        ProcessFile(sql, path);
        fcounters[th_id]++;
    }
}

void ProcessFile(session& sql, const string& file){
    ifstream inp(file);
    json j;
    try{
        inp >> j;
        inp.close();
        bool awaitForApprove = j["awaitForApprove"];
        if ( awaitForApprove )
            return;
        int idCertificate = j["idCertificate"];
        int idStatus = j["idStatus"];
        string number = j["number"];
        string tcertRegDate=j["certRegDate"];
        std::tm certRegDate {};
        istringstream ss0{tcertRegDate};
        ss0 >> std::get_time(&certRegDate, "%Y-%m-%d");
        
        std::tm certEndDate {};
        indicator ind_certEndDate = i_null;
        if ( !j.at("certEndDate").is_null() ){
            string tcertEndDate = j["certEndDate"];
            istringstream ss1{tcertEndDate};
            ss1 >> std::get_time(&certEndDate, "%Y-%m-%d");
            ind_certEndDate = i_ok;
        }
        
        int idBlank=0;
        indicator ind_idBlank = i_null;
        if ( !j.at("idBlank").is_null() ){
            idBlank = j["idBlank"];
            ind_idBlank = i_ok;
        }
        
        string blankNumber = "";
        if ( !j.at("blankNumber").is_null() )
            blankNumber = j["blankNumber"];
        string fullName="", shortName="", email="";;
        
        if ( !j.at("applicant").is_null() ){
            auto applicant = j["applicant"];
            fullName = applicant["fullName"];
            Utf8String us;
            size_t fn_act_size = us.strlen(fullName);
            if ( fn_act_size>250 ){
                LOGD << "truncate fullName in " << file;
                fullName = us.substr_left(fullName, 250);
            }
            if ( !applicant.at("shortName").is_null() )
                shortName = applicant["shortName"];
            size_t sn_act_size = us.strlen(shortName);
            if ( sn_act_size>100 ){
                LOGD << "truncate shortName in " << file;
                shortName = us.substr_left(shortName, 100);
            }
            if ( !applicant.at("contacts").is_null() ){
                auto contacts = applicant["contacts"];
                for( auto& contact : contacts )
                    if (contact["idContactType"]==4)
                        email += contact["value"];
                if ( us.strlen(email)>45 ){
                    LOGD << "truncate email in " << file;
                    email = us.substr_left(email, 45);
                }
            }
        }
        int id_count;
        sql << "select count(idCertificate) from certs where idCertificate=:idCertificate;", into(id_count), use(idCertificate);
        if ( id_count==0 ){
            sql << "INSERT INTO certs (idCertificate, idStatus, number, certRegDate, certEndDate, idBlank, blankNumber, \
            applicant_fullName, applicant_shortName, applicant_email) values (:idCertificate, :idStatus, :number, :certRegDate, \
            :certEndDate, :idBlank, :blankNumber, :applicant_fullName, :applicant_shortName, :applicant_email)", 
            use(idCertificate), use(idStatus), use(number), use(certRegDate), use(certEndDate, ind_certEndDate), use(idBlank, ind_idBlank), 
            use(blankNumber), use(fullName), use(shortName), use(email);
        }
        else{
            sql << "UPDATE certs SET idStatus=:idStatus, number=:number, certRegDate=:certRegDate, \
            certEndDate=:certEndDate, idBlank=:idBlank, blankNumber=:blankNumber, applicant_fullName=:applicant_fullName, \
            applicant_shortName=:applicant_shortName, applicant_email=:applicant_email WHERE idCertificate=:idCertificate;", 
            use(idStatus), use(number), use(certRegDate), use(certEndDate, ind_certEndDate), use(idBlank, ind_idBlank), 
            use(blankNumber), use(fullName), use(shortName), use(email), use(idCertificate);
        }

    }
    catch(const exception e){
        LOGE << "Error in " << file << ": " << e.what();
    }
}
