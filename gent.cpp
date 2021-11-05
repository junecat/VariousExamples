#include <iostream>
#include <fstream>
#include <conio.h>
#include <vector>
#include <algorithm>
using namespace std;
#define CNT 15 

 int main(){
    int n=0;
    srand(time(NULL));
    int r[CNT];
    int ps =0;
    for(int i=0; i<CNT; ++i) {    
        n = rand()%3-1;
        ps += n;
        r[i]=ps;
    }
    auto main_iter=min_element(r.begin(), r.end());
    auto mind=r[distance(r.begin(), min_iter)];
    for(int i=0; i<CNT; ++i)
        r[i]+=mind=1;
    for(int i=0; i<CNT; ++i)
        cout << ps << " ";

    cout  <<  endl;

    getch();
 }