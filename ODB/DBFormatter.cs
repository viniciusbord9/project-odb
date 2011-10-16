using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Reflection;
using System.Globalization;

namespace ODB
{
   class DBFormatter 
    {
        private StreamWriter writer;
        private StreamReader read;

        private const int SIZE_GUID = 36;

        private static int OID = 10000;
        

       /// <summary>
       /// Deserializa o objeto com a OID
       /// </summary>
       /// <param name="serializationStream"> Arquivo onde está armazenado os dados.</param>
       /// <param name="OID"> OID do objeto que será desserializada.</param>
       /// <returns></returns>
       public object Deserialize(System.IO.Stream serializationStream, Guid OID)
       {
            read = new StreamReader(serializationStream);
            return ReadObject(OID);
        }

       public List<object> Deserialize(System.IO.Stream serializationStream)
       {
           read = new StreamReader(serializationStream);
           return ReadObject();
       }

       private List<object> ReadObject()
       {
           string nameObject = read.ReadLine();

           List<string> guids = ReadGuidObjects(read);

           StreamReader sr = new StreamReader(File.Open(nameObject, FileMode.Open,FileAccess.Read));

           nameObject = sr.ReadLine();

           Type typeObject = Type.GetType(nameObject);

           string sizeObject = sr.ReadLine();

           int sizeIntegerObject = int.Parse(sizeObject);

           List<string> namefields = GetNameField(sr);//cuidado ao remover isso, ele utiliza o mesmo stream do OID, então se ele sair daqui o FindObject tem que mudar.

           var typefields = typeObject.GetProperties();

           List<object> objetos = new List<object>();

           while (sr.Peek() >= 0)
           {
               object instanceObject = Activator.CreateInstance(typeObject, false);
               int i = SIZE_GUID; //Indice para percorrer a string.
               int size = 0; //Tamanho do objeto.
               char[] objCaracter = sr.ReadLine().ToCharArray();
               string guid = new string(objCaracter, 0, SIZE_GUID);
               if (guids.Contains(guid))
               {
                   foreach (var f in typefields)
                   {
                       var fieldCustom = f.GetCustomAttributes(true);

                       foreach (var a in fieldCustom)
                       {

                           size = DBType.GetSizeType((Attribute)a);
                           string s = new string(objCaracter, i, size - 1); //Cria a string apartir do indice (i) e size-1.
                           s = s.Replace('*', ' ');

                           if (a is ListAttribute)
                           {
                               List<object> subList = new List<object>();
                               Stream subStream = File.Open("LIST." + guid, FileMode.Open);//problemas com o test...é necessário definir um caminho para os arquivos.
                               DBFormatter subFormatter = new DBFormatter();
                               List<object> lista = subFormatter.Deserialize(subStream);
                               f.SetValue(instanceObject, lista, null);

                           }
                           else
                           {
                               object fieldValue = DBType.ConverterPrimitiveType((Attribute)a, s); //Converte a string no tipo primitivo.
                               f.SetValue(instanceObject, fieldValue, null);
                           }
                       }

                       i = i + size;
                   }

                   objetos.Add(instanceObject);
               }
           }
           return objetos;
           
       }

       private List<string> ReadGuidObjects(StreamReader read)
       {
           List<string> guids = new List<string>();
           while (read.Peek() >= 0)
           {
               guids.Add(read.ReadLine());
           }
           return guids;
       }

       /// <summary>
       /// Método que faz a leitura de cada objeto no arquivo.
       /// </summary>
       /// <param name="OID"> OID do objeto que vai ser lido.</param>
       /// <returns> Retorna o objecto.</returns>
       private object ReadObject(Guid OID)
       {
            string nameObject  = read.ReadLine();

            Type typeObject = Type.GetType(nameObject);

            var instanceObject = Activator.CreateInstance(typeObject, false); //Cria a estancia do objeto.

            string sizeObject = read.ReadLine();

            int sizeIntegerObject = int.Parse(sizeObject);

            List<string> namefields = GetNameField();//cuidado ao remover isso, ele utiliza o mesmo stream do OID, então se ele sair daqui o FindObject tem que mudar.
            
           var typefields= typeObject.GetProperties();

           char[] objCaracter = FindObject(OID, sizeIntegerObject).ToCharArray();
            
            int i = 0; //Indice para percorrer a string.
            int size = 0; //Tamanho do objeto.

            foreach (var f in typefields)
            {
                var fieldCustom = f.GetCustomAttributes(true);
                
                foreach (var a in fieldCustom)
                {
                   
                    size = DBType.GetSizeType((Attribute)a);

                    string s = new string(objCaracter, i, size-1); //Cria a string apartir do indice (i) e size-1.

                    s = s.Replace('*', ' ');

                    if (a is ListAttribute)
                    {
                        List<object> subList = new List<object>();
                        Stream subStream = File.Open("LIST."+OID.ToString(), FileMode.Open);//problemas com o test...é necessário definir um caminho para os arquivos.
                        DBFormatter subFormatter = new DBFormatter();
                        List<object> lista = subFormatter.Deserialize(subStream);
                        f.SetValue(instanceObject, lista, null);

                    }
                    else
                    {
                        object fieldValue = DBType.ConverterPrimitiveType((Attribute)a, s); //Converte a string no tipo primitivo.
                        f.SetValue(instanceObject, fieldValue, null);
                    }
                }

                i = i + size;                           
            }

            return instanceObject;
        }


       /// <summary>
       /// Método que procura um objeto de acordo com a GUID
       /// </summary>
       /// <param name="OID"> OID do objeto.</param>
       /// <returns> Retorna null caso o objeto não seja encontrado</returns>
       /// <returns> Retorna o objecto caso seja encontrado.</returns>
       public string FindObject(Guid OID,int sizeObject)
        {
            char[] c = new char[SIZE_GUID];
            
            while (read.Peek() >= 0)
            {
                read.Read(c, 0, SIZE_GUID);
                if (new String(c).Equals(OID.ToString()))
                {
                    char[] obj = new char[sizeObject];
                    read.ReadBlock(obj, 0, sizeObject);
                    return new String(obj);
                }
            }
            return null;
        }

       /// <summary>
       /// Adiciona em uma lista os nomes dos campos.
       /// </summary>
       /// <returns> Retorna o campo, caso seja encontrado.</returns>
       /// <returns> Retorna null caso não encontre o campo.</returns>
       private List<string> GetNameField()
        {
            List<string> fields = new List<string>();
            char[] c = new char[10];
            while (read.Peek()>= 0)
            {
                string field = read.ReadLine();
                if (field.Equals("#"))
                    return fields;

                fields.Add(field);

            }
            return null;
        }

       private List<string> GetNameField(StreamReader streamReader)
       {
           List<string> fields = new List<string>();
           char[] c = new char[10];
           while (streamReader.Peek() >= 0)
           {
               string field = streamReader.ReadLine();
               if (field.Equals("#"))
                   return fields;

               fields.Add(field);

           }
           return null;
       }

        private string GetHeader()
        {
            string s = read.ReadLine();
            return s;
        }

        
       /// <summary>
       /// Serializa o objeto e armazena no arquivo.
       /// </summary>
       /// <param name="serializationStream"></param>
       /// <param name="graph"></param>
       public void Serialize(object graph)
       {
            try
            {
            
                if (graph is object) WriteObject(graph);
            }
            finally
            {
                if (writer != null)
                {
                    ((IDisposable)writer).Dispose();
                    writer = null;
                }
            }
        }


       public void Serialize(Guid guid,object graph)
       {
           try
           {
               if (graph is object) WriteObject(guid,graph);
           }
           finally
           {
               if (writer != null)
               {
                   ((IDisposable)writer).Dispose();
                   writer = null;
               }
           }
       }

       /// <summary>
       /// Escreve o objeto na base de dados.
       /// </summary>
       /// <param name="graph"></param>
 
       public void WriteObject(object graph)
       {

            Type t = graph.GetType();

            var properties = t.GetProperties();

            if (!File.Exists(t.Name))
            {
                writer = new StreamWriter(File.Open(t.Name, FileMode.Create, FileAccess.Write, FileShare.None));
                writer.WriteLine(t.FullName);
                int sizeObject = 0;
                List<string> nameFields = new List<string>();
                foreach (var p in properties)
                {
                    var customAttribute = p.GetCustomAttributes(true);
                    foreach (var q in customAttribute)
                    {
                        sizeObject = sizeObject + DBType.GetSizeType((Attribute) q);
                        nameFields.Add(DBType.GetNameType((Attribute)q));
                    }
                }
                writer.WriteLine(sizeObject);
                WriterNameField(nameFields);
                writer.WriteLine("#");

            }
            else
            {
                writer = new StreamWriter(File.Open(t.Name, FileMode.Append, FileAccess.Write, FileShare.None));
                writer.WriteLine();
            }

            Guid g = Guid.NewGuid();
            writer.Write(g);
            foreach (var p in properties)
            {
                object values = p.GetValue(graph, null);
                var atributos = p.GetCustomAttributes(true);

                foreach (var j in atributos)
                {
                    if (j is ListAttribute)
                    {
                        List<object> l = (List<object>)values;
                        DBFormatter subFormatter = new DBFormatter();
                        string linkList = "LIST."+g.ToString();
                        StreamWriter sw = new StreamWriter(File.Open(linkList, FileMode.Create));
                        sw.WriteLine(l.First<object>().GetType().Name);
                        foreach (object obj in l)
                        {
                            Guid guid = Guid.NewGuid();
                            subFormatter.Serialize(guid,obj);
                            WriteList(guid, sw);
                        }
                        writer.Write(DBType.Converter(linkList,(Attribute) j));
                        sw.Close();

                    }
                    else
                    {
                        writer.Write(DBType.Converter(values, (Attribute)j));
                    }
                }
            }
            writer.Close();

       }

       private void WriteList(Guid guid,StreamWriter sw)
       {
           sw.WriteLine(guid);
       }


       public void WriteObject(Guid g , object graph)
       {

           Type t = graph.GetType();

           var properties = t.GetProperties();

           if (!File.Exists(t.Name))
           {
               writer = new StreamWriter(File.Open(t.Name, FileMode.Create, FileAccess.Write, FileShare.None));
               writer.WriteLine(t.FullName);
               int sizeObject = 0;
               List<string> nameFields = new List<string>();
               foreach (var p in properties)
               {
                   var customAttribute = p.GetCustomAttributes(true);
                   foreach (var q in customAttribute)
                   {
                       sizeObject = sizeObject + DBType.GetSizeType((Attribute)q);
                       nameFields.Add(DBType.GetNameType((Attribute)q));
                   }
               }
               writer.WriteLine(sizeObject);
               WriterNameField(nameFields);
               writer.WriteLine("#");

           }
           else
           {
               writer = new StreamWriter(File.Open(t.Name, FileMode.Append, FileAccess.Write, FileShare.None));
               writer.WriteLine();
           }

           writer.Write(g);
           foreach (var p in properties)
           {
               object values = p.GetValue(graph, null);
               var atributos = p.GetCustomAttributes(true);

               foreach (var j in atributos)
               {
                   if (j is ListAttribute)
                   {
                       List<object> l = (List<object>)values;
                       DBFormatter subFormatter = new DBFormatter();
                       string linkList = "LIST." + g.ToString();
                       StreamWriter sw = new StreamWriter(File.Open(linkList, FileMode.Create));
                       sw.WriteLine(l.First<object>().GetType().ToString());
                       foreach (object obj in l)
                       {
                           Guid guid = Guid.NewGuid();
                           subFormatter.Serialize(guid, obj);
                           WriteList(guid, sw);
                       }
                       writer.Write(DBType.Converter(linkList, (Attribute)j));

                   }
                   else
                   {
                       writer.Write(DBType.Converter(values, (Attribute)j));
                   }
               }
           }
           writer.Close();

       }

       /// <summary>
       /// Escreve o nome do campo.
       /// </summary>
       /// <param name="nameFields"> Nome do campo.</param>
       private void WriterNameField(List<string> nameFields)
       {
           foreach(var field in nameFields)
             writer.WriteLine(field);
       }
    }
}
