using System.Collections.Generic;
using System.Linq;

namespace BW.Common.DataStruct
{
    public class BWHashSet<T>
    {
        private int _groupCount;

        private Dictionary<int, List<T>> _groups;

        public BWHashSet(int groupCount = 20)
        {
            _groups = new Dictionary<int, List<T>>();
            _groupCount = groupCount;

            for (int i = 0; i < groupCount; i++)
            {
                _groups.Add(i, new List<T>());
            }
        }

        public void Add(T elem)
        {
            var hashCode = elem.GetHashCode();
            var groupNum = hashCode % _groupCount;

            var group = _groups[groupNum];

            foreach (var groupElem in group)
            {
                if (groupElem.Equals(elem))
                {
                    return;
                }
            }

            group.Add(elem);
        }

        public bool Contains(T elem)
        {
            var hashCode = elem.GetHashCode();
            var groupNum = hashCode % _groupCount;

            if (_groups.ContainsKey(groupNum))
            {
                var group = _groups[groupNum];
                foreach (var groupElem in group)
                {
                    if (groupElem.Equals(elem))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override string ToString()
        {
            List<T> res = new List<T>();
            
            foreach(var group in _groups)
            {
                foreach(var elem in group.Value)
                {
                    res.Add(elem);
                }
            }

            return "{" + string.Join(",", res) + "}";
        }
    }
}
