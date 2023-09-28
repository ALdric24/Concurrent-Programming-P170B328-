using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace L1_A
{
    class Program
    {
        const string cfd = "Students_textfile.txt";
        const string cfr1 = @"C:\Users\foxtr\OneDrive\Desktop\Lab1_A\L1_A\result.txt";
        const string cfr2 = @"C:\Users\foxtr\OneDrive\Desktop\Lab1_A\L1_A\result2.txt";
        const string cfr3 = @"C:\Users\foxtr\OneDrive\Desktop\Lab1_A\L1_A\result3.txt";
        static object _lock = new object();
        
        static void Main(string[] args)
        {
            Console.WriteLine("enter year of student:");
            char x = Convert.ToChar(Console.ReadLine());
            DataMonitor students = new DataMonitor();
            SortedResultMonitor s = new SortedResultMonitor();
            SortedResultMonitor s2 = new SortedResultMonitor();
            SortedResultMonitor s3 = new SortedResultMonitor();
            List<Thread> threads = new List<Thread>();
           

            for (int i = 0; i < 6; i++)
            {
                var thread = new Thread(
                () => Worker(students, s,s2,s3,x));
                threads.Add(thread);
            }

            foreach (var item in threads)
            {
                item.Start();
            }
            s.Print(cfr1,"initial");
            Read(cfd, students);
            s.Print(cfr1, "initial");

            s2.Print(cfr2,"Results");
            s3.Print(cfr3, "Results");

            foreach (var item in threads)
            {
                item.Join();
            }

        }
        static void Read(string fv, DataMonitor studs)
        {
            string name, line;
            int year, n;
            double grade;
            using (StreamReader reader = new StreamReader(fv))
            {
                n = File.ReadAllLines(fv).Count();                
                for (int i = 0; i < n; i++)
                {
                    line = reader.ReadLine();
                    string[] parts = line.Split(';');
                    name = parts[0];
                    year = int.Parse(parts[1]);
                    grade = double.Parse(parts[2]);
                    Student s = new Student(name, year, grade);
                    studs.AddItem(s);
                }
            }
        }

        static void Worker(DataMonitor studs, SortedResultMonitor s, SortedResultMonitor s2, SortedResultMonitor s3, char x)
        {
            
            try
            {
                while (true)
                {
                    //studs.Print();
                    var stud = studs.RemoveItem();
                    if (stud == null)
                    {
                        break;
                    }
                    char[] a = { x };
                    char[] b = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
                    char[] c = { 'z' };
                    s.AddItemSorted(stud,a);
                    s2.AddItemSorted(stud,b);
                    s3.AddItemSorted(stud,c);
                }
            }
            finally
            {
                Monitor.Exit(_lock);    
            }
        }

    }
}

    


