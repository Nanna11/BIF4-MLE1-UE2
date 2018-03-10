using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNN
{
    class Results
    {
        Dictionary<int, string> _results = new Dictionary<int, string>();
        Dictionary<string, int> _inverseresults = new Dictionary<string, int>();
        int index = 0;

        public int Result(string value)
        {
            if (value == null) throw new ResultCannotBeNullException();
            if (_inverseresults.ContainsKey(value)) return _inverseresults[value];
            else
            {
                _results.Add(index, value);
                _inverseresults.Add(value, index);
                index++;
                return _inverseresults[value];
            }
        }

        public string Result(int value)
        {
            if (_results.ContainsKey(value)) return _results[value];
            else throw new ResultDoesNotExistException();
        }

        public int Count
        {
            get
            {
                return _results.Count;
            }
        }

    }
}
