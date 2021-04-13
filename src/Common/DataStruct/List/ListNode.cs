namespace BW.Common.DataStruct
{
    public class ListNode<T>
    {
        public T Data { get; set; }

        public ListNode<T> Next { get; set; }

        public ListNode<T> Prev { get; set; }
    }
}