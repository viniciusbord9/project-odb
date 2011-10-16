using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ODB
{
    /*
     * Método MyAtribute ainda não implementado
     * 
     * 
     **/ 
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class ListAttribute : Attribute
    {
      
        // This is a positional argument
        public ListAttribute(string name)
        {
            Name = name;
            Length = 41;
        }

        public string Name { get; set; }
        public int Length { get; set; }
   
    }

    /// <summary>
    /// Tipo do campo do banco de dados.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class VarCharAttribute : Attribute
    {
        /// <summary>
        /// Construtor do tipo VarChar
        /// </summary>
        /// <param name="name"></param>
        /// <param name="length"></param>
        public VarCharAttribute(string name, int length = 80)
        {
            Name = name;
            Length = length;    
        }

        public string Name { get; set; }
        public int Length { get; set; }

    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class IntAttribute : Attribute
    {
        /// <summary>
        /// Construtor do tipo Int
        /// </summary>
        /// <param name="name"></param>
        /// <param name="length"></param>
        public IntAttribute(string name, int length = 10)
        {
            Name = name;
            Length = length;
        }

        public string Name { get; set; }
        public int Length { get; set; }

    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class DecimalAttribute : Attribute
    {

        /// <summary>
        /// Construtor do tipo decimal.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="decimalPlaces"></param>
        public DecimalAttribute(string name, int decimalPlaces = 50)
        {
            Name = name;
            DecimalPlaces = decimalPlaces;
        }

        public string Name { get; set; }
        public int DecimalPlaces { get; set; }
    }
    
    /// <summary>
    /// Classe utilizada para retornar o tamanho do campo.
    /// </summary>
    class DBType
    {
        ///gets size of the types
        /// <summary>
        /// Método que retorna o tamanho do tipo do objeto.
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static int GetSizeType(Attribute T)
        {
            if (T is VarCharAttribute)
            {
                VarCharAttribute varchar = (VarCharAttribute)T;
                return varchar.Length;
            }
            if (T is IntAttribute)
            {
                IntAttribute integer = (IntAttribute)T;
                return integer.Length;
            }
            if (T is DecimalAttribute)
            {
                DecimalAttribute dec = (DecimalAttribute)T;
                return dec.DecimalPlaces;
            }
            if (T is ListAttribute)
            {
                ListAttribute list = (ListAttribute)T;
                return list.Length;
            }

            return 0;
        }

        /// <summary>
        /// Método que retorna o tipo do objeto.
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static string GetNameType(Attribute T)
        {
            if (T is VarCharAttribute)
            {
                VarCharAttribute varchar = (VarCharAttribute)T;
                return varchar.Name;
            }
            if (T is IntAttribute)
            {
                IntAttribute integer = (IntAttribute)T;
                return integer.Name;
            }
            if (T is DecimalAttribute)
            {
                DecimalAttribute dec = (DecimalAttribute)T;
                return dec.Name;
            }

            if (T is ListAttribute)
            {
                ListAttribute list = (ListAttribute)T;
                return list.Name;
            }

            return "Unknowm";
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        internal static string Converter(object obj, Attribute j)
        {
            if (j is VarCharAttribute)
            {
                VarCharAttribute varchar = (VarCharAttribute) j;
                return ConverterFormat(obj, varchar.Length);
            }

            if (j is IntAttribute)
            {
                IntAttribute integer = (IntAttribute)j;
                return ConverterFormat(obj, integer.Length);
            }

            if (j is DecimalAttribute)
            {
                DecimalAttribute dec = (DecimalAttribute)j;
                return ConverterFormat(obj, dec.DecimalPlaces);
            }
            if (j is ListAttribute)
            {
                ListAttribute list = (ListAttribute)j;
                return ConverterFormat(obj, list.Length);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ConverterFormat(Object obj, int length)
        {

            char[] c = obj.ToString().ToCharArray();

            char[] aux = new char[length];

            for (int i = 0; i < length; i++)
            {
                if (i < c.Length)
                {
                    aux[i] = c[i];
                }
                else
                {
                    aux[i] = '*';
                }
            }

            return new String(aux);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="T"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static object ConverterPrimitiveType(Attribute T,string s)
        {
            if (T is VarCharAttribute)
            {
                return s;
            }
            if (T is IntAttribute)
            {               
                return int.Parse(s);
            }
            if (T is DecimalAttribute)
            {
                return double.Parse(s);
            }

            return null;
         
        }

    }
}
