using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ODB
{
    [Serializable()]
    class test2
    {
        [VarChar("Nome",20)]
        public string Nome { get; set; }
    
        

        public test2() { }
        public test2(string nome)
        {
            Nome = nome;
        }

       

    }
}
