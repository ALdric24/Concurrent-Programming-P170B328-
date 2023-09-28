using System;
using System.Collections.Generic;
using System.Threading;

namespace p1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ds ds = new Ds();
            Thread t1 = new Thread(() => ds.ReadA(100));
            Thread t2 = new Thread(() => ds.ReadB(0));
            t1.Start();
            t2.Start();
            while (Math.Abs(ds.a-ds.b) > 20)
            {
                Thread t5 = new Thread(() => ds.Print());
                t5.Start();
                Thread t3 = new Thread(() => ds.ChangeA());
                t3.Start();
                Thread t4 = new Thread(() => ds.ChangeB());
                t4.Start();
                t3.Join();
                t4.Join();
                t5.Join();
            }
            t1.Join();
            t2.Join();

        }

    }
}
