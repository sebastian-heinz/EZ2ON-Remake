using System.Collections.Concurrent;

namespace EZR
{
    public class Queue<T> : ConcurrentQueue<T>
    {
        public T Dequeue()
        {
            TryDequeue(out T t);
            return t;
        }
        public T Peek()
        {
            TryPeek(out T t);
            return t;
        }
        public void Clear()
        {
            while (TryDequeue(out T t)) ;
        }
    }
}