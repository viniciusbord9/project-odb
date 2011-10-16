#include <iostream>
#include <fstream>
#include <iostream>
#include <string>

using std::fstream;
using std::ofstream;
using std::ifstream;
using std::ios;
using std::string;

using namespace std;

int main()
{
    string line;
    ifstream arq;
    arq.open("C:/Users/vinicius/Documents/Visual Studio 2010/Projects/Laskera_Object_Database/project-odb/trunk laskeyra-project-odb/ODB/ODB/bin/Debug/test.txt",ios::in);
    if(arq.is_open())
    {
        while(!arq.eof())
        {
            getline(arq,line);
            cout << "\n" << line;
        }
    }
    else
        cout << "não abriu";

    return 0;
}


