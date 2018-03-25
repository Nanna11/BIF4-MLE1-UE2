using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNN
{
    class Package
    {
        LinkedList<Instance> _instances = new LinkedList<Instance>();
        //nodes need to be saved because otherwise they cannot be removed from linked lsit
        LinkedList<LinkedListNode<Instance>> _instanceNodes = new LinkedList<LinkedListNode<Instance>>();

        //add a new instance
        public void AddInstance(Instance i)
        {
            _instances.AddLast(i);
            _instanceNodes.AddLast(_instances.Last);
        }

        //return instance at index i
        public Instance GetInstance(int i)
        {
            if (i < _instances.Count) return _instances.ElementAt(i);
            else throw new InstanceDoesNotExistException();
        }

        //delete instance at index i
        public void DeleteInstance(int i)
        {
            if (i < _instances.Count)
            {
                _instances.Remove(_instanceNodes.ElementAt(i));
                _instanceNodes.Remove(_instanceNodes.ElementAt(i));
            }
            else throw new InstanceDoesNotExistException();
        }

        //clear list of instances
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

        //randomize the order of instances in this package
        public void Randomize()
        {
            int size = _instances.Count;
            Random Rand = new Random();
            _instances = new LinkedList<Instance>(_instances.OrderBy((o) =>
            {
                return (Rand.Next() % size);
            }));
        }

        //mere instances of two packages into a new one
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

        //mere instances of multiple packages into a new one
        public static Package Concat(IEnumerable<Package> tl)
        {
            Package p = new Package();
            for (int i = 0; i < tl.Count(); i++)
            {
                p = Package.Concat(p, tl.ElementAt(i));
            }
            return p;
        }

        //normalize attribute number index in every instance of package
        public void NormalizeAttribute(int index, double med, double stddeviation)
        {
            foreach(Instance i in _instances)
            {
                i.NormalizeAttribute(index, med, stddeviation);
            }
        }
    }
}
