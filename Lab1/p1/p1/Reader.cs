using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace p1
{
    internal class Reader
    {
        public int A { get; set; }
        public int B { get; set; }
        private readonly object _locker;
        private bool _canContinue;


        public Reader()
        {
            A = 100;
            B = 0;
            _canContinue = true;
            _locker = new object();
        }


        public void Put()
        {
            lock (_locker)
            {
                while (!_canContinue) { Monitor.Wait(_locker); }
                A -= 10;
                B += 10;
                _canContinue = A - B > 20;
                Monitor.PulseAll(_locker);
            }
        }

        public int GetA()
        {
            int num;
            lock (_locker)
            {
                while (!_canContinue) { Monitor.Wait(_locker); }
                num = A;
                _canContinue = A - B > 20;
                return num;
            }
        }

        public int GetB()
        {
            int num;
            lock (_locker)
            {
                while (!_canContinue) { Monitor.Wait(_locker); }
                num = B;
                _canContinue = A - B > 20;
                return num;
            }
        }


    }
}
