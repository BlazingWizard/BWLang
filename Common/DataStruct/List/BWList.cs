using Common.Exceptions;

namespace BW.Common.DataStruct
{
    public class BWList<T>
    {
        private ListNode<T> _first;

        private ListNode<T> _last;

        private ListNode<T> _currentItem;

        public BWList()
        {

        }

        public int Count { get; set;}

        public void Add(T elem)
        {
            ListNode<T> node;
            if (Count == 0)
            {
                node = new ListNode<T>()
                {
                    Data = elem,
                    Prev = null,
                    Next = null
                };

                _first = _last = node;
            }
            else
            {
                node = new ListNode<T>()
                {
                    Data = elem,
                    Prev = _last,
                    Next = null
                };

                _last.Next = node;
                _last = node;
            }

            Count++;
        }

        public void Delete(int index)
        {
            Reset();
            for (int i = 0; i < index; i++)
            {
                Next();
            }

            if (_currentItem.Prev != null)
            {
                _currentItem.Prev.Next = _currentItem.Next;
            }
            else
            {
                _first = _currentItem.Next;
            }
           
            if (_currentItem.Next != null)
            {
                _currentItem.Next.Prev = _currentItem.Prev;
            }
            else
            {
                _last = _currentItem.Prev;
            }
            
            Count--;
           
        }

        public T Get(int index)
        {
            if (index < Count)
            {
                Reset();
                for (int i = 0; i < index; i++)
                {
                    Next();
                }
                return _currentItem.Data;
            }
            else
            {
                throw new RuntimeException("List out of range exception");
            }
            
        }

        public override string ToString()
        {
            string[] res = new string[Count];

            Reset();
            for (int i = 0; i < Count; i++)
            {
                res[i] = $" \"{i}\" : \"{_currentItem.Data}\"";
                Next();
            }

            return $"{{ {string.Join(",", res)} }}";
        }

        private void Reset()
        {
            _currentItem = _first;
        }

        private void Next()
        {
            _currentItem = _currentItem.Next;
        }
    }
}
