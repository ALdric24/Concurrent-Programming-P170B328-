using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L1_A
{
    
    class SortedResultMonitor
    {
        
        const int arrSize = 25;
        private readonly Student[] Students;
        private int n;
        

        public SortedResultMonitor()
        {
            n = 0;
            Students = new Student[arrSize];
        }

        public Student Get(int i) { return Students[i]; }
        public void AddItemSorted(Student item,char[] A)
        {
            foreach (char a in A)
            {
                if (char.ToUpperInvariant(a) == item.Name[0])
                {
                    Students[n++] = item;
                    Sort();
                }
            }
            
           
            
        }

        public int Count() { return n; }

        public Student GetItems()
        {
            return Students[n];
        }

        public void Sort()
        {
            bool flag = true;
            while (flag)
            {
                flag = false;
                for (int i = 0; i < Count() - 1; i++)
                {
                    Student a = Students[i];
                    Student b = Students[i + 1];
                    if (a.CompareTo(b) > 0)
                    {
                        Students[i] = b;
                        Students[i + 1] = a;
                        flag = true;
                    }
                }
            }
        }

        public void Print( string cfr, string text = "")
        {
            

            using (StreamWriter writer = new StreamWriter(cfr, true))
            {
                writer.WriteLine(text);
                writer.WriteLine(new string('-', 37));
                writer.WriteLine(" name    \t|    year |   grade |");
                writer.WriteLine(new string('-', 37));
                for (int i = 0; i < n; i++)
                {
                    Student student = Get(i);
                    if (student != null)
                        writer.WriteLine("{0} {1} ", student.ToString(), student.Hash);
                }
                writer.WriteLine(new string('-', 37));
                writer.WriteLine();
            }
        }
    }

}
