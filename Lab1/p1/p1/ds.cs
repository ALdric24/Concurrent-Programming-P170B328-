using System;
using System.Threading;

namespace p1
{
    internal class Ds
    {
        public int a;
        public int b;
        public int aa;
        public int bb;
        private readonly object _locker;

        public Ds()
        {
            _locker = new object();
        }

        public void ChangeA()
        {

            lock (_locker)
            {
                while (Math.Abs(a - b) < 20) { Monitor.Wait(_locker); }
                a -= 10;
                Monitor.PulseAll(_locker);
            }
        }

        public void ChangeB()
        {
            lock (_locker)
            {
                while (Math.Abs(a - b) < 20) { Monitor.Wait(_locker); }
                b += 10;
                Monitor.PulseAll(_locker);
            }
        }
        public void Print()
        {
            lock (_locker)
            {
                while (Math.Abs(a - b) < 20) { Monitor.Wait(_locker); }
                Console.WriteLine("A: {0} B: {1}", a, b);
                Monitor.PulseAll(_locker);
            }
        }
        public void ReadA(int a)
        {
            this.a = a;
            aa = a;
        }

        public void ReadB(int b)
        {
            this.b = b;
            bb = b;
        }
    }
}
