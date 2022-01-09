using Google.Cloud.Firestore;

namespace FirestoreLINQ
{
    internal partial class QueryProvider
    {
        public class GenericAggregator<T>
        {
            public static T Aggregate(QuerySnapshot snapshot, string Aggregator)
            {
                object result = null;

                if (typeof(T) == typeof(int))
                {
                    List<int> results = snapshot.Select(x => x.ConvertTo<Dictionary<string, int>>().Values.First()).ToList();
                    result = Aggregator == "Sum" ? results.Sum() : results.Average();
                }
                else if (typeof(T) == typeof(long))
                {
                    List<long> results = snapshot.Select(x => x.ConvertTo<Dictionary<string, long>>().Values.First()).ToList();
                    result = Aggregator == "Sum" ? results.Sum() : results.Average();
                }
                else if (typeof(T) == typeof(decimal))
                {
                    List<decimal> results = snapshot.Select(x => x.ConvertTo<Dictionary<string, decimal>>().Values.First()).ToList();
                    result = Aggregator == "Sum" ? results.Sum() : results.Average();
                }
                else if (typeof(T) == typeof(double))
                {
                    List<double> results = snapshot.Select(x => x.ConvertTo<Dictionary<string, double>>().Values.First()).ToList();
                    result = Aggregator == "Sum" ? results.Sum() : results.Average();
                }
                else
                    throw new NotImplementedException($"Operation is not implemented for {typeof(T)}");

                return (T)Convert.ChangeType(result, typeof(T));
            }
        }
    }
}