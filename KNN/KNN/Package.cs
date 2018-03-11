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

        public int Count
        {
            get
            {
                return _instances.Count;
            }
        }

        static public Package Concat(Package i, Package j)
        {
            Package p = new Package();
            return null;
        }
    }
}
