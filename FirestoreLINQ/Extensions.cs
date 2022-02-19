using Google.Cloud.Firestore;
using System.Reflection;

namespace FirestoreLINQ
{
    public static class Extensions
    {
        public static IQueryable<T> AsQuerable<T>(this CollectionReference collection)
        {
            IQueryable<T> source = new Queryable<T>(collection);
            return source;
        }

        public static CollectionReference Collection<T>(this FirestoreDb db) where T : class
        {
            TypeInfo typeInfo = typeof(T).GetTypeInfo();
            var attrib = (FirestoreCollectionAttribute)Attribute.GetCustomAttributes(typeInfo).SingleOrDefault(x => x is FirestoreCollectionAttribute);
            string collectionName = attrib?.CollectionName ?? typeInfo.Name.ToLower();
            return db.Collection(collectionName);
        }

        public static IQueryable<T> AsQuerable<T>(this FirestoreDb db) where T : class
        {
            return db.Collection<T>().AsQuerable<T>();
        }
    }
}