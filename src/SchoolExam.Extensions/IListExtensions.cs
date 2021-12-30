using System.Collections;

namespace SchoolExam.Extensions;

public static class ListExtensions
{
    public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> source)
    {
        if (source == null)
            throw new ArgumentException();
        return new ReadOnlyListWrapper<T>(source);
    }

    private class ReadOnlyListWrapper<T> : IReadOnlyList<T>
    {
        private readonly IList<T> _list;

        public int Count => _list.Count;

        public ReadOnlyListWrapper(IList<T> list)
        {
            this._list = list;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[int index] => _list[index];
    }
}