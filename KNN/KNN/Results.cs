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

        //convert and save string into corresponding int value
        public int Result(string value)
        {
            if (value == null) throw new ResultCannotBeNullException();
            //return int if already existing
            if (_inverseresults.ContainsKey(value)) return _inverseresults[value];
            else
            {
                //generate new value if not already existing
                _results.Add(index, value);
                _inverseresults.Add(value, index);
                index++;
                return _inverseresults[value];
            }
        }

        //convert int value back to string result
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
