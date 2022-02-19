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
    }
}
