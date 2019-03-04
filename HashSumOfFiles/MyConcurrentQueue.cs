using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HashSumOfFiles
{
    public class MyConcurrentQueue<T> where T : class
    {
        private readonly object locker = new object();
        private Queue<T> elems = new Queue<T>();        
        private bool isCompletedEnqueue = false;
        private int producers = 1;
        private int receiveMsgs = 0;

        public MyConcurrentQueue() { }
        
        public MyConcurrentQueue(int producers)
        {
            this.producers = producers;
        }

        public bool IsCompleted()
        {
            lock (locker)
            {
                return (isCompletedEnqueue && elems.Count == 0);          
                
            }
        }

        public void CompleteEnqueue()
        {
            lock (locker)
            {
                receiveMsgs++;
                if (receiveMsgs >= producers)
                {
                    isCompletedEnqueue = true;
                    Monitor.PulseAll(locker);
                }             
            }
        }

        public int Count()
        {
            lock (locker)
            {
                return elems.Count;
            }
        }
        
        public void Enqueue(T elem)
        {
            lock (locker)
            {
                elems.Enqueue(elem);
                Monitor.Pulse(locker);
            }
        }

        public T Dequeue()
        {
            lock (locker)
            {
                while (elems.Count == 0 && !isCompletedEnqueue)
                    Monitor.Wait(locker);
                if (elems.Count == 0)
                    return null;
                return elems.Dequeue();
            }
        }
    }
}
