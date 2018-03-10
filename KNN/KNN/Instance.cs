using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNN
{
    class Instance
    {
        LinkedList<double> _attributes = new LinkedList<double>();
        int _result;

        public Instance(int result)
        {
            _result = result;
        }

        public void AddAttribute(double value)
        {
            _attributes.AddLast(value);
        }

        public double GetAttribute(int index)
        {
            if (index < _attributes.Count) return _attributes.ElementAt(index);
            else throw new AttributeDoesNotExistException();
        }

        public int Count
        {
            get
            {
                return _attributes.Count;
            }
        }

        public int Result
        {
            get
            {
                return _result;
            }
        }
    }
}
