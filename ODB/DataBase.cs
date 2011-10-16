using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ODB
{
    class DataBase
    {
        private string Path { get; set; }
        private HashSet<Guid> Guids { get; set; }
        private string FileObjects{get; set;}
        
        public DataBase()
        {
            

        }

        public void CreateDataBase(string path)
        {
            System.IO.Directory.CreateDirectory(path);
            FileObjects = path + "/Objects";
            Stream stream = System.IO.File.Open(FileObjects,FileMode.Create);
            stream.Close();
        }

        public void UseDataBase(string path)
        {
            Path = path;
            Guids = LoadGuidObjects();
        }

        private HashSet<Guid> LoadGuidObjects()
        {
            List<string> objects = LoadTypeObjects();
            foreach(var obj in objects)
            {
                Console.WriteLine(obj);
            }
            return null;
        }
        
        private List<string> LoadTypeObjects()
        {
            string s = FileObjects;
            StreamReader streamReader = new StreamReader(File.Open(FileObjects,FileMode.Open));
            List<string> objects = new List<string>();
            while (streamReader.Peek() >= 0)
            {
                objects.Add(streamReader.ReadLine());
            }
            streamReader.Close();
            return objects;            
        }

        public void Insert(object obj)
        {
            StreamWriter streamWriter = new StreamWriter(File.Open(FileObjects, FileMode.Append, FileAccess.Write, FileShare.None));
            DBFormatter formatter = new DBFormatter();
            formatter.Serialize(obj);
            streamWriter.Write(obj.GetType().Name);
            streamWriter.Close();
            return;
        }


    }
}
