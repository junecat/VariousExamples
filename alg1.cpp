#include <iostream>
#include <fstream>
#include <vector>
#include <algorithm>
#include <numeric>
#include <conio.h>

#define MAXL 6 

using namespace std;

int R[MAXL];
int b[MAXL];

void Generate(){
    int n=0, ps=0;
    for(int i=0; i<MAXL; ++i) {    
        n = rand()%5-2;
        ps += n;
        R[i]= abs(ps);
    }
}

void PrintR(){
    for( auto& x : R)
        cout << x << " ";
    cout << endl;
}

int main(){
    //srand(time(NULL));

    // generate ratings
    for (int i=0; i<25;++i){
        Generate();
        PrintR();
    }   
    
    
    getch();    
}