using System.Linq.Expressions;

namespace FirestoreLINQ
{
    internal partial class QueryProvider
    {
        public class GenericAggregator<T>
        {
            public static T Sum(List<T> items)
            {
                var expType1 = Expression.Parameter(typeof(T), "x");
                var expType2 = Expression.Parameter(typeof(T), "y");
                T result = default;

                Func<T, T, T> adder;
                adder = (Func<T, T, T>)Expression
                    .Lambda(Expression.Add(expType1, expType2), expType1, expType2)
                    .Compile();

                foreach (var item in items)
                {
                    result = adder(result, item);
                }
                return result;
            }

            public static T Average(List<T> items)
            {
                var expType1 = Expression.Parameter(typeof(T), "x");
                var expType2 = Expression.Parameter(typeof(T), "y");

                var sumResult = Sum(items);
                Func<T, T, T> divider;

                divider = (Func<T, T, T>)Expression
                    .Lambda(Expression.Divide(expType1, expType2), expType1, expType2)
                    .Compile();

                var itemsCount = (T)Convert.ChangeType(items.Count, typeof(T));

                var avgResult = divider(sumResult, itemsCount);
                return avgResult;
            }
        }
    }
}