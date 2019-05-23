using Common.Exceptions;
using System;
using System.Collections.Generic;

namespace BW.SM
{
    public class VarTable
    {
        Dictionary<string, object> _varTable;

        public VarTable()
        {
            _varTable = new Dictionary<string, object>();
        }

        public object this[string varName]
        {
            get
            {
                foreach(var elem in _varTable)
                {
                    if (elem.Key.Split(' ')[1] == varName)
                    {
                        return elem.Value;
                    }
                }

                throw new RuntimeException("var undefined");
            }
            set
            {
                if (!_varTable.ContainsKey(varName))
                {
                    _varTable.Add(varName, value);
                    return;
                }
                _varTable[varName] = value;
            }
        }

        public void Print()
        {
            foreach(var elem in _varTable)
            {
                Console.WriteLine($"{elem.Key}:{elem.Value.ToString()}");
            }
        }
    }
}