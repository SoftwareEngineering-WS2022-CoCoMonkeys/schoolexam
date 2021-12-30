using System.Collections;

namespace SchoolExam.Extensions;

public static class CollectionExtensions
{
    public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> source)
    {
        if (source == null)
            throw new ArgumentException();
        return new ReadOnlyCollectionWrapper<T>(source);
    }

    private class ReadOnlyCollectionWrapper<T> : IReadOnlyCollection<T>
    {
        private readonly ICollection<T> _collection;
            
        public int Count => _collection.Count;

        public ReadOnlyCollectionWrapper(ICollection<T> collection)
        {
            this._collection = collection;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}