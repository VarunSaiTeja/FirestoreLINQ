using System.Reflection;
using FirestoreLINQ.Internals;

// ReSharper disable once CheckNamespace
namespace FirestoreLINQ
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class FirestoreCollectionAttribute : Attribute
    {
        public string CollectionName;

        public FirestoreCollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
        internal static FirestoreCollectionAttribute GetAttributes(TypeInfo typeInfo)
        {
            return typeInfo.CustomAttributes
                .Where(x => x.AttributeType == typeof(FirestoreCollectionAttribute))
                .Select(x => (FirestoreCollectionAttribute)x.InflateAttribute())
                .FirstOrDefault();
        }
    }
}
