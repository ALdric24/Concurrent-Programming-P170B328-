using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace p1
{
    internal class Data
    {
        private List<int> NumsA;
        private readonly Reader _reader;


        public Data(Reader reader)
        {
            NumsA = new List<int>();
            _reader = reader;
        }


        public void Read()
        {
            while (_reader.A - _reader.B > 20)
                NumsA.Add(_reader.GetA());
        }
    }
}
