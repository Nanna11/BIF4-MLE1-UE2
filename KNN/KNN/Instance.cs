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
        //nodes need to be saved so they can be removed from linked list later
        Dictionary<int, LinkedListNode<double>> _invalidattributes = new Dictionary<int, LinkedListNode<double>>();
        Dictionary<int, LinkedListNode<double>> _attributenodes = new Dictionary<int, LinkedListNode<double>>();

        int _result;

        public Instance(int result)
        {
            _result = result;
        }

        //add a new, valid attribute
        public void AddAttribute(double value)
        {
            _attributenodes.Add(_attributes.Count, _attributes.AddLast(value));
        }

        //return attribute if existing and valid
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

        //correct an invalid attribute to make it valid
        public void ReviseAttribute(int index, double value)
        {
            //correct only if invalid
            if (_invalidattributes.ContainsKey(index))
            {
                //insert new and delete old because nodes in linked list cannot simply be swaped
                _attributenodes[index] = _attributes.AddAfter(_invalidattributes[index], value);
                _attributes.Remove(_invalidattributes[index]);
                //remove from invalid list so attribute is valid now
                _invalidattributes.Remove(index);
            }
        }

        //add an invalid attribute to correct later so order of attributes does not get destroyed
        public void AddInvalid()
        {
            _invalidattributes.Add(_attributes.Count, _attributes.AddLast(0));
            _attributenodes.Add(_attributes.Count - 1, _invalidattributes[_attributes.Count - 1]);
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

        //normalize attribute number index to make it compareable
        public void NormalizeAttribute(int index, double med, double stddeviation)
        {
            double value = (_attributes.ElementAt(index) - med) / stddeviation;
            LinkedListNode<double> ToDelete = _attributenodes[index];
            _attributenodes[index] = _attributes.AddAfter(_attributenodes[index], value);
            _attributes.Remove(ToDelete);
        }
    }
}
