using System.Linq.Expressions;

namespace FirestoreLINQ
{
    internal partial class QueryProvider
    {
        public static class GenericAggregator<T>
        {
            static Func<T, T, T> GetAdder()
            {
                var exp1 = Expression.Parameter(typeof(T), "x");
                var exp2 = Expression.Parameter(typeof(T), "y");

                Func<T, T, T> adder;
                adder = (Func<T, T, T>)Expression
                    .Lambda(Expression.Add(exp1, exp2), exp1, exp2)
                    .Compile();

                return adder;
            }

            public static T Sum(List<T> items)
            {
                Func<T, T, T> adder = GetAdder();
                T result = default;

                foreach (var item in items)
                {
                    result = adder(result, item);
                }
                return result;
            }

            public static T Average(List<T> items)
            {
                var sumResult = Sum(items);

                Func<T, T, T> divider;
                var exp1 = Expression.Parameter(typeof(T), "x");
                var exp2 = Expression.Parameter(typeof(T), "y");

                divider = (Func<T, T, T>)Expression
                    .Lambda(Expression.Divide(exp1, exp2), exp1, exp2)
                    .Compile();

                var itemsCount = (T)Convert.ChangeType(items.Count, typeof(T));

                var avgResult = divider(sumResult, itemsCount);
                return avgResult;
            }
        }
    }
}