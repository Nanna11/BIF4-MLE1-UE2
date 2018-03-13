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
        Dictionary<int, LinkedListNode<double>> _invalidattributes = new Dictionary<int, LinkedListNode<double>>();
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
            if (index < _attributes.Count)
            {
                if (!_invalidattributes.ContainsKey(index)) return _attributes.ElementAt(index);
                else throw new AttributeInvalidException();
            }
            else throw new AttributeDoesNotExistException();
        }

        public int Count
        {
            get
            {
                return _attributes.Count;
            }
        }

        public void ReviseAttribute(int index, double value)
        {
            if (_invalidattributes.ContainsKey(index))
            {
                _attributes.AddAfter(_invalidattributes[index], value);
                _attributes.Remove(_invalidattributes[index]);
                _invalidattributes.Remove(index);
            }
        }

        public void AddInvalid()
        {
            _invalidattributes.Add(_attributes.Count, _attributes.AddLast(0));
        }

        public int Result
        {
            get
            {
                return _result;
            }
        }

        public List<int> InvalidAttributes
        {
            get
            {
                return new List<int>(_invalidattributes.Keys);
            }
        }

        public bool IsValid
        {
            get
            {
                if (_invalidattributes.Count == 0) return true;
                else return false;
            }
        }
    }
}
