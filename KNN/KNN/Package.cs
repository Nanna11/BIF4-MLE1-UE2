﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNN
{
    class Package
    {
        LinkedList<Instance> _instances = new LinkedList<Instance>();
        LinkedList<LinkedListNode<Instance>> _instanceNodes = new LinkedList<LinkedListNode<Instance>>();

        public void AddInstance(Instance i)
        {
            _instances.AddLast(i);
            _instanceNodes.AddLast(_instances.Last);
        }

        public Instance GetInstance(int i)
        {
            if (i < _instances.Count) return _instances.ElementAt(i);
            else throw new InstanceDoesNotExistException();
        }

        public void DeleteInstance(int i)
        {
            if (i < _instances.Count)
            {
                _instances.Remove(_instanceNodes.ElementAt(i));
                _instanceNodes.Remove(_instanceNodes.ElementAt(i));
            }
            else throw new InstanceDoesNotExistException();
        }

        public void Clear()
        {
            _instances.Clear();
            _instanceNodes.Clear();
        }

        public int Count
        {
            get
            {
                return _instances.Count;
            }
        }

        public void Randomize()
        {
            int size = _instances.Count;
            Random Rand = new Random();
            _instances = new LinkedList<Instance>(_instances.OrderBy((o) =>
            {
                return (Rand.Next() % size);
            }));
        }

        static public Package Concat(Package i, Package j)
        {
            Package p = new Package();
            for(int k = 0; k < i.Count; k++)
            {
                p.AddInstance(i.GetInstance(k));
            }

            for (int k = 0; k < j.Count; k++)
            {
                p.AddInstance(j.GetInstance(k));
            }

            return p;
        }
    }
}
