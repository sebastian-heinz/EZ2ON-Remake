using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZR
{
    public class ObjectPool
    {
        Queue<GameObject> queue = new Queue<GameObject>();
        public int Count => queue.Count;
        public void Put(GameObject obj)
        {
            queue.Enqueue(obj);
        }
        public GameObject Get()
        {
            return queue.Dequeue();
        }
        public void Clear()
        {
            queue.Clear();
        }
    }
}