using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;


namespace ODB
{
    class Program
    {
        //O ../ significa voltar no diretório ....fiz a função em C++ talvez podemos usar isso para buscas.
        //[DllImport("../../Reader/bin/debug/Reader.dll")]
        //private static extern int main();

        static void Main(string[] args)
        {

           // main();
            test u = new test("joseh", 12, 1.50);
            u.addElement();


            //DBFormatter bformatter = new DBFormatter();
            //bformatter.Serialize(u);




            //Stream streamr = File.Open("test", FileMode.Open, FileAccess.Read, FileShare.None);

            //DBFormatter bformatter = new DBFormatter();
            //Guid guid = new Guid("5ca72477-2a44-4177-8d13-609259a5453b");
            //object k = bformatter.Deserialize(streamr, guid);
            //if (k != null)
            //{
            //    test p = (test)k;
            //    Console.WriteLine("nome: " + p.Nome);
            //    Console.WriteLine("idade: " + p.Idade);
            //    foreach (var s in p.Lista)
            //    {
            //        test2 t = (test2)s;
            //        Console.WriteLine(t.Nome);
            //    }

            //    Console.ReadKey();
            //}


            DataBase db = new DataBase();
            db.CreateDataBase("teste");
            db.UseDataBase("teste");
            db.Insert(u);
        }
    }
}
