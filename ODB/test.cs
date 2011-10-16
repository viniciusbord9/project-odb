using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

namespace ODB
{

    [Serializable()]
    public class test
    {
        public test(String nome, int idade,double altura)
        {
            Nome = nome;
            Idade = idade;
            Altura = altura;
            Lista = new List<object>();
            GUID = Guid.NewGuid();

        }

        public void addElement()
        {
            Lista.Add(new test2("Vinicius"));
            Lista.Add(new test2("Afonso"));
            Lista.Add(new test2("Felipe"));
            Lista.Add(new test2("Ana"));
            Lista.Add(new test2("Vinicius Zattara"));

        }

        public test() { }

        [VarChar("Nome",50)]
        public string Nome { get; set; }

        [Int("Idade",10)]
        public int Idade { get; set; }
        
        [Decimal("altura",30)]
        public double Altura { get; set; }

        [List("Listatest2")]
        public List<object> Lista { get; set; }

        public Guid GUID { get; set; }
    }
}
