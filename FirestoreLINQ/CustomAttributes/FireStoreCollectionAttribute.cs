using System.Reflection;
using FirestoreLINQ.Internals;

// ReSharper disable once CheckNamespace
namespace FirestoreLINQ
{
    public class FireStoreCollectionAttribute : Attribute
    {
        public string CollectionPath { get; set; }

        public FireStoreCollectionAttribute(string collectionPath)
        {
            CollectionPath = collectionPath;
        }
        internal static FireStoreCollectionAttribute GetAttributes(TypeInfo typeInfo)
        {
            return typeInfo.CustomAttributes
                .Where(x => x.AttributeType == typeof(FireStoreCollectionAttribute))
                .Select(x => (FireStoreCollectionAttribute)x.InflateAttribute())
                .FirstOrDefault();
        }
    }
}
