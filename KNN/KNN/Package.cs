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

        public void AddInstance(Instance i)
        {
            _instances.AddLast(i);
        }

        public Instance GetInstance(int i)
        {
            if (i < _instances.Count) return _instances.ElementAt(i);
            else throw new InstanceDoesNotExistException();
        }

        public int Count
        {
            get
            {
                return _instances.Count;
            }
        }
    }
}
