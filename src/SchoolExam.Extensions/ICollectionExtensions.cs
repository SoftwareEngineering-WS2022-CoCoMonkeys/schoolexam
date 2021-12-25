using System.Collections;

namespace SchoolExam.Extensions
{
    public static class CollectionExtensions
    {
        public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> source)
        {
            if (source == null)
                throw new ArgumentException();
            return new ReadOnlyCollectionWrapper<T>(source);
        }

        public class ReadOnlyCollectionWrapper<T> : IReadOnlyCollection<T>
        {
            private readonly ICollection<T> collection;
            
            public int Count => collection.Count;

            public ReadOnlyCollectionWrapper(ICollection<T> collection)
            {
                this.collection = collection;
            }
            public IEnumerator<T> GetEnumerator()
            {
                return collection.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}