using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace L1_A
{
    class DataMonitor
    {
        static readonly object _lock = new object();
        const int arrSize = 10;
        private Student[] Students;
        private int n, j;
        const string fv = @"C:\Users\asus\Desktop\Lab1_A\L1_A\result.txt";
        public DataMonitor()
        {
            n = 0;
            j = 0;
            Students = new Student[arrSize];
        }

        public void AddItem(Student item)
        {
            lock (_lock)
            {
                while (n >= arrSize)
                {
                    Monitor.Wait(_lock);
                }
                Students[n++] = item;
                j++;
                Monitor.PulseAll(_lock);
            }
        
        }

        public int Count() { return n; }

        public Student Get(int i) { return Students[i]; }
        public Student RemoveItem()
        {
            lock (_lock)
            {

                while (n == 0)
                {
                    Monitor.Wait(_lock);
                }

                Student d = null;
                if (Students[0] != null)
                {
                    d = Students[0];
                    for (int i = 0; i < n - 1; i++)
                    {
                        Students[i] = Students[i + 1];
                    }
                    Students[arrSize - 1] = null;
                    n--;
                    Monitor.PulseAll(_lock);
                    return d;
                }
                n--;
                Monitor.PulseAll(_lock);
                return d;
            }
            
        }
        
        public void Print(string text = "")
        {
            using (StreamWriter writer = new StreamWriter(fv, true))
            {
                writer.WriteLine(text);
                writer.WriteLine(new string('-', 37));
                writer.WriteLine(" name    \t|    year |   grade |");
                writer.WriteLine(new string('-', 37));
                for (int i = 0; i < Students.Count(); i++)
                {
                    Student student = Get(i);
                    if (student != null)
                        writer.WriteLine(student.ToString());
                }
                writer.WriteLine(new string('-', 37));
                writer.WriteLine();
            }
        }

    }
}
