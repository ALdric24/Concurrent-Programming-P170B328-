using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L1_A
{
    internal class Student : IComparable<Student>, IEquatable<Student>

    {
        public string Name { get; set; }
        public int Year { get; set; }
        public double Grade { get; set; }
        public string Hash { get; set; }
        private static Random random = new Random();

        public Student()
        {
            Name = "";
            Year = 0;
            Grade = 0.0;
            
        }

        public Student(string name, int year, double grade)
        {
            Name = name;
            Year = year;
            Grade = grade;
            this.Hash = RandomString(25);
        }

        public override string ToString()
        {
            return string.Format(" {0, -14} |{1, 8:d} |{2, 8:f2} |", Name, Year, Grade);
        }

        public int CompareTo(Student other) //we add hash to sort by hash
        {
            int p = String.Compare(Name, other.Name, StringComparison.CurrentCulture);
            if (p > 0)
                return 1;
            else if (p < 0)
                return -1;
            else
                return 0;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Student);
        }

        public bool Equals(Student other)
        {
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
