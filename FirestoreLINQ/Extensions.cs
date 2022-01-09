using Google.Cloud.Firestore;

namespace FirestoreLINQ
{
    public static class Extensions
    {
        public static IQueryable<T> AsQuerable<T>(this CollectionReference collection)
        {
            IQueryable<T> source = new Queryable<T>(collection);
            return source;
        }
    }
}