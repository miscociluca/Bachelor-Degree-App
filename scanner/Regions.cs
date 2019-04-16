using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scanner
{
    class Regions
    {
        public static string fisier = "";
        public static List<string[]> seria = new List<string[]>();
        public static List<string[]> numar=new List<string[]>();
        public static List<string[]> cnp = new List<string[]>(); 
        public static List<string[]> nume = new List<string[]>(); 
        public static List<string[]> prenume = new List<string[]>();
        public static List<string[]> cetatenie = new List<string[]>();
        public static List<string[]> nastere = new List<string[]>(); 
        public static List<string[]> domiciliu = new List<string[]>(); 
        public static List<string[]> eliberat = new List<string[]>();
        public static List<string[]> valabilitate = new List<string[]>(); 
        public static List<string[]> oras = new List<string[]>(); 
        public static List<string[]> judet = new List<string[]>(); 
        public static List<string[]> strada = new List<string[]>();
        public static List<string[]> numarul = new List<string[]>();
        public static List<string[]> semnatura = new List<string[]>();
        public static List<string[]> data = new List<string[]>();
        public Regions(string Numefisier)
        {
            fisier = @"C:\Users\misco\source\repos\scanner\Cereri\" + Numefisier + ".txt";
            StreamReader st = new StreamReader(fisier);
            String line;
            while ((line = st.ReadLine()) != null)
            {
                string aux = line;
                if (aux.Contains("semnatura"))
                {
                    string sign = aux.Remove(0, 10);
                    string[] rez=sign.Split(' ').ToArray<string>();
                    semnatura.Add(rez);
                    continue;
                }
                if (aux.Contains("seria"))
                {
                    string sign = aux.Remove(0,6);
                    string[] rez = sign.Split(' ').ToArray<string>();
                    seria.Add(rez);
                    continue;
                }
                if (aux.Contains("numarul"))
                {
                    string sign = aux.Remove(0, 8);
                    string[] rez = sign.Split(' ').ToArray<string>();
                    numarul.Add(rez);
                    continue;
                }
                if (aux.Contains("numar"))
                {
                    string sign = aux.Remove(0, 6);
                    string[] rez = sign.Split(' ').ToArray<string>();
                    numar.Add(rez);
                    continue;
                }
                if (aux.Contains("cnp"))
                {
                    string sign = aux.Remove(0, 4);
                    string[] rez = sign.Split(' ').ToArray<string>();
                    cnp.Add(rez);
                    continue;
                }
                if (aux.Contains("prenume"))
                {
                    string sign = aux.Remove(0, 8);
                    string[] rez = sign.Split(' ').ToArray<string>();
                    prenume.Add(rez);
                    continue;
                }
                if (aux.Contains("nume"))
                {
                    string sign = aux.Remove(0, 5);
                    string[] rez = sign.Split(' ').ToArray<string>();
                    nume.Add(rez);
                    continue;
                }
                if (aux.Contains("cetatenie"))
                {
                    string sign = aux.Remove(0, 10);
                    string[] rez = sign.Split(' ').ToArray<string>();
                    cetatenie.Add(rez);
                    continue;
                }
                if (aux.Contains("nastere"))
                {
                    string sign = aux.Remove(0, 8);
                    string[] rez = sign.Split(' ').ToArray<string>();
                    nastere.Add(rez);
                    continue;
                }
                if (aux.Contains("domiciliu"))
                {
                    string sign = aux.Remove(0, 10);
                    string[] rez = sign.Split(' ').ToArray<string>();
                    domiciliu.Add(rez);
                    continue;
                }
                if (aux.Contains("eliberat"))
                {
                    string sign = aux.Remove(0, 9);
                    string[] rez = sign.Split(' ').ToArray<string>();
                    eliberat.Add(rez);
                    continue;
                }
                if (aux.Contains("valabilitate"))
                {
                    string sign = aux.Remove(0, 13);
                    string[] rez = sign.Split(' ').ToArray<string>();
                    valabilitate.Add(rez);
                    continue;
                }
                if (aux.Contains("oras"))
                {
                    string sign = aux.Remove(0, 5);
                    string[] rez = sign.Split(' ').ToArray<string>();
                    oras.Add(rez);
                    continue;
                }
                if (aux.Contains("judet"))
                {
                    string sign = aux.Remove(0, 6);
                    string[] rez = sign.Split(' ').ToArray<string>();
                    judet.Add(rez);
                    continue;
                }
                if (aux.Contains("strada"))
                {
                    string sign = aux.Remove(0, 7);
                    string[] rez = sign.Split(' ').ToArray<string>();
                    strada.Add(rez);
                    continue;
                }
                if (aux.Contains("data"))
                {
                    string sign = aux.Remove(0, 5);
                    string[] rez = sign.Split(' ').ToArray<string>();
                    data.Add(rez);
                    continue;
                }

            }

        }
        public static void clear_metadata()
        {
            fisier = "";
            seria.Clear();
            numar.Clear();
            cnp.Clear();
            nume.Clear();
            prenume.Clear();
            cetatenie.Clear();
            nastere.Clear();
            domiciliu.Clear();
            eliberat.Clear();
            valabilitate.Clear();
            oras.Clear();
            judet.Clear();
            strada.Clear();
            numarul.Clear();
            semnatura.Clear();
            data.Clear();
    }
    }
}
