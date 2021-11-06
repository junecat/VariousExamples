#include <iostream>
#include <fstream>
#include <vector>
#include <algorithm>
#include <numeric>
#include <conio.h>

using namespace std;

int main(){
    ifstream inp("input.txt");
    ofstream out("output.txt");
    int N;
    inp >> N;
    int r[N];
    for( int i=0; i<N; ++i )
        inp >> r[i];
    
    if ( N==1 ){
        cout << "500" << endl;
        out << "500" << endl;
        return 0;
    }

    if ( N==2 ){
        if (r[0]==r[1]){
            cout << "1000" << endl;
            out << "1000" << endl;
        }
        else {
            cout << "1500" << endl;
            out << "1500" << endl;
        }
        return 0;
    }

    int b[N];
    for( int i=0; i<N; ++i )
        b[i]=0;
    
    // left to right
    for( int i=1; i<N; ++i ){
        if (r[i]>r[i-1])
            b[i]=b[i-1]+1;
    }

    // right to left
    for( int i=N-2; i>=0; --i ){
        if ( r[i]>r[i+1] )
            b[i] = b[i+1]+1;
    }

    cout << "R: ";
    for( int i=0; i<N; ++i )
        cout << r[i] << " ";
    cout << endl;

    cout << "b: ";
    for( int i=0; i<N; ++i )
        cout << b[i] << " ";
    cout << endl;

    int bsum = 0;
    for( auto& x : b )
        bsum+=x;
    cout << "answer: " << 500 * (N+bsum);
    
    getch();
}