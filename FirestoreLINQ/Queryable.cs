using Google.Cloud.Firestore;
using System.Collections;
using System.Linq.Expressions;

namespace FirestoreLINQ
{
    public class Queryable<T> : IOrderedQueryable<T>
    {
        public Queryable(CollectionReference collection)
        {
            this.Provider = new QueryProvider(collection);
            this.Expression = Expression.Constant(this);
        }

        public Queryable(IQueryProvider provider, Expression expression)
        {
            Provider = provider;
            Expression = expression;
        }

        public Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public Expression Expression
        {
            get;
            private set;
        }

        public IQueryProvider Provider
        {
            get;
            private set;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}