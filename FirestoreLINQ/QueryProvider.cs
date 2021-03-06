using Google.Cloud.Firestore;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace FirestoreLINQ
{
    internal partial class QueryProvider : IQueryProvider
    {
        string[] SelectedFields;
        CollectionReference _collection;
        Query query;

        public QueryProvider(CollectionReference collection)
        {
            _collection = collection;
            query = _collection;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new Queryable<TElement>(this, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        QuerySnapshot GetResult()
        {
            return query.GetSnapshotAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            QueryResolver(expression);

            if (expression is MethodCallExpression)
            {
                var me = (expression as MethodCallExpression).Method;
                switch (me.Name)
                {
                    case "Any":
                        {
                            query = query.Limit(1);
                            var snapResult = GetResult();
                            return (TResult)(object)snapResult.Any();
                        }
                    case "Average":
                        {
                            var snapResult = GetResult();
                            var items = snapResult.Select(x => x.ConvertTo<Dictionary<string, TResult>>().Values.First()).ToList();
                            return GenericAggregator<TResult>.Average(items);
                        }
                    case "Count":
                        {
                            var snapResult = GetResult();
                            return (TResult)(object)snapResult.Count;
                        }
                    case "First":
                        {
                            query = query.Limit(1);
                            var snapResult = GetResult();
                            return snapResult.First().ConvertTo<TResult>();
                        }
                    case "FirstOrDefault":
                        {
                            query = query.Limit(1);
                            var snapResult = GetResult();
                            return snapResult.Any() ? snapResult.First().ConvertTo<TResult>() : default;
                        }
                    case "Last":
                        {
                            query = query.LimitToLast(1);
                            var snapResult = GetResult();
                            return snapResult.Last().ConvertTo<TResult>();
                        }
                    case "LastOrDefault":
                        {
                            query = query.LimitToLast(1);
                            var snapResult = GetResult();
                            return snapResult.Any() ? snapResult.First().ConvertTo<TResult>() : default;
                        }
                    case "Max" or "Min":
                        {
                            var result = GetResult().First().ConvertTo<Dictionary<string, TResult>>().Values.First();
                            return result;
                        }
                    case "MaxBy" or "MinBy":
                        {
                            var results = GetResult();
                            return results.Any() ? results[0].ConvertTo<TResult>() : default;
                        }
                    case "Single":
                        {
                            query = query.Limit(2);
                            var snapResult = GetResult();
                            return snapResult.Single().ConvertTo<TResult>();
                        }
                    case "SingleOrDefault":
                        {
                            query = query.Limit(2);
                            var snapResult = GetResult();
                            return snapResult.Any() ? snapResult.SingleOrDefault().ConvertTo<TResult>() : default;
                        }
                    case "Sum":
                        {
                            var snapResult = GetResult();
                            var items = snapResult.Select(x => x.ConvertTo<Dictionary<string, TResult>>().Values.First()).ToList();
                            return GenericAggregator<TResult>.Sum(items);
                        }
                    default:
                        break;
                }
            }

            var snapshot = GetResult();

            if (!snapshot.Any())
            {
                if (typeof(TResult) == typeof(TResult))
                {
                    Type t = typeof(List<>).MakeGenericType(typeof(TResult).GenericTypeArguments.First());
                    IList res = (IList)Activator.CreateInstance(t);
                    return (TResult)res;
                }
                else
                {
                    return default;
                }
            }

            return ConvertResults<TResult>(snapshot);
        }

        T ConvertResults<T>(QuerySnapshot snapshot)
        {
            var resultType = typeof(T);
            var resultGenericType = resultType.GenericTypeArguments;
            IEnumerable<object> results = null;
            if (SelectedFields == null || SelectedFields.Length == 0)
            {
                var docConverter = snapshot[0].GetType().GetMethod("ConvertTo").MakeGenericMethod(resultGenericType);

                results = snapshot.Select(x => docConverter.Invoke(x, null));
            }
            else if (SelectedFields.Length == 1)
            {
                results = snapshot.Select(x => x.GetValue<object>(SelectedFields[0]));
            }
            else if (SelectedFields.Length > 1)
            {
                results = snapshot.Select(x =>
                  {
                      int i = 0;
                      var objValues = new List<object>();
                      foreach (var field in SelectedFields)
                      {
                          var fieldType = resultGenericType[0].GenericTypeArguments[i++];
                          var fieldValue = x.GetValue<object>(field);
                          objValues.Add(fieldValue);
                      }
                      var outObj = resultGenericType[0].GetConstructors().First().Invoke(objValues.ToArray());
                      return outObj;
                  });
            }

            if (results == null)
                return default;

            Type t = typeof(List<>).MakeGenericType(resultGenericType);
            IList res = (IList)Activator.CreateInstance(t);
            foreach (var item in results)
            {
                res.Add(item);
            }
            return (T)res;
        }

        void QueryResolver(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Call)
            {
                var mce = expression as MethodCallExpression;
                foreach (var arg in mce.Arguments)
                {
                    if (arg.NodeType == ExpressionType.Call)
                        QueryResolver(mce.Arguments[0]);
                }

                if (mce.Arguments.Any(x => x.NodeType == ExpressionType.Quote))
                {
                    var le = (mce.Arguments.SingleOrDefault(x => x.NodeType == ExpressionType.Quote) as UnaryExpression).Operand as LambdaExpression;
                    Console.WriteLine("Resolving Member: " + mce.Method.Name);
                    switch (mce.Method.Name)
                    {
                        case "Average" or "Sum":
                            {
                                var fieldName = le.Body.GetFirestoreFieldName();
                                string[] selectedFields = { fieldName };
                                query = query.Select(selectedFields);
                                break;
                            }
                        case "Min" or "Max":
                            {
                                var fieldName = le.Body.GetFirestoreFieldName();
                                string[] selectedFields = { fieldName };

                                if (mce.Method.Name == "Min")
                                    query = query.OrderBy(fieldName);
                                else
                                    query = query.OrderByDescending(fieldName);

                                query = query.Limit(1).Select(selectedFields);
                                break;
                            }
                        case "MinBy" or "MaxBy":
                            {
                                var fieldName = le.Body.GetFirestoreFieldName();

                                if (mce.Method.Name == "MinBy")
                                    query = query.OrderBy(fieldName);
                                else
                                    query = query.OrderByDescending(fieldName);

                                query = query.Limit(1);
                                break;
                            }
                        case "OrderBy" or "OrderByDescending":
                            OrderResolver(le, mce.Method.Name == "OrderByDescending");
                            break;
                        case "ThenBy" or "ThenByDescending":
                            OrderResolver(le, mce.Method.Name == "ThenByDescending");
                            break;
                        case "Select":
                            {
                                if (le.Body.NodeType == ExpressionType.MemberAccess)
                                {
                                    var fieldName = le.Body.GetFirestoreFieldName();
                                    SelectedFields = new string[] { fieldName };
                                    query = query.Select(fieldName);
                                }
                                else if (le.Body.NodeType == ExpressionType.New)
                                {
                                    var ne = le.Body as NewExpression;
                                    SelectedFields = ne.Arguments.Select(x => x.GetFirestoreFieldName()).ToArray();
                                    query = query.Select(SelectedFields);
                                }
                            }
                            break;
                        case "Any":
                        case "Count":
                        case "First":
                        case "FirstOrDefault":
                        case "Last":
                        case "LastOrDefault":
                        case "Single":
                        case "SingleOrDefault":
                        case "Where":
                            FilterResolver(le);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (mce.Method.Name)
                    {
                        case "Skip":
                            {
                                var itemsCount = (int)(expression as MethodCallExpression).Arguments.Last().GetExpressionValue();
                                query = query.Offset(itemsCount);
                                break;
                            }
                        case "Take" or "TakeLast":
                            {
                                var itemsCount = (int)(expression as MethodCallExpression).Arguments.Last().GetExpressionValue();
                                query = mce.Method.Name == "Take" ? query.Limit(itemsCount) : query.LimitToLast(itemsCount);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
        }

        void OrderResolver(LambdaExpression expression, bool isDesc)
        {
            var fieldName = expression.Body.GetFirestoreFieldName();

            if (fieldName == null)
                throw new InvalidOperationException();

            if (string.IsNullOrEmpty(fieldName))
                throw new InvalidOperationException();

            if (isDesc)
                query = query.OrderByDescending(fieldName);
            else
                query = query.OrderBy(fieldName);
        }

        void FilterResolver(LambdaExpression expression)
        {
            if (expression.NodeType != ExpressionType.Lambda)
                throw new InvalidOperationException();

            switch (expression.Body.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    ParseBinaryExpression(expression.Body as BinaryExpression);
                    break;
                case ExpressionType.Call:
                    ParseCallExpression(expression.Body as MethodCallExpression);
                    break;
                case ExpressionType.Not when (expression.Body as UnaryExpression).Operand.NodeType is ExpressionType.Call:
                    ParseCallExpression((expression.Body as UnaryExpression).Operand as MethodCallExpression, true);
                    break;
                case ExpressionType.AndAlso:
                    var be = expression.Body as BinaryExpression;
                    ParseBinaryExpression(be.Left as BinaryExpression);
                    ParseBinaryExpression(be.Right as BinaryExpression);
                    break;
                default:
                    if (query == null) throw new NotSupportedException();
                    break;
            }
        }

        void ParseCallExpression(MethodCallExpression expression, bool notAlsoApplied = false)
        {
            var clause = expression.GetMemberAndFieldNameAndValue();
            var methodName = expression.Method.Name;

            if (methodName == "Contains")
            {
                var isGeneric = (clause.memberInfo as PropertyInfo).PropertyType.IsGenericType;
                if (isGeneric)
                    query = query.WhereArrayContains(clause.fieldName, clause.value);
                else
                {
                    if (notAlsoApplied)
                        query = query.WhereNotIn(clause.fieldName, clause.value as IEnumerable);
                    else
                        query = query.WhereIn(clause.fieldName, clause.value as IEnumerable);
                }
            }
            else if (methodName == "Any")
            {
                IEnumerable values = null;
                var le = expression.Arguments[1] as LambdaExpression;
                var me = le.Body as MethodCallExpression;

                if (me.Object != null)
                {
                    if (me.Object.NodeType == ExpressionType.MemberAccess)
                        values = (me.Object as MemberExpression).GetExpressionValue() as IEnumerable;
                    else if (me.Object.NodeType == ExpressionType.ListInit)
                    {
                        var li = me.Object as ListInitExpression;
                        values = li.Initializers.Select(x => x.Arguments[0].GetExpressionValue());
                    }
                }
                else
                {
                    values = me.Arguments.First().GetExpressionValue() as IEnumerable;
                }
                query = query.WhereArrayContainsAny(clause.fieldName, values);
            }
            else if (methodName == "StartsWith")
            {
                var startCode = clause.value.ToString();
                string endCode;
                if (startCode.Length == 1)
                {
                    endCode = ((char)(startCode.Last() + 1)).ToString();
                }
                else
                {
                    endCode = startCode.Substring(0, startCode.Length - 1) + ((char)(startCode.Last() + 1)).ToString();
                }
                query = query.WhereGreaterThanOrEqualTo(clause.fieldName, clause.value)
                    .WhereLessThan(clause.fieldName, endCode);
            }
        }

        void ParseBinaryExpression(BinaryExpression expression)
        {
            var clause = FieldValueHelper.GetFieldNameAndValue(expression.Left, expression.Right);
            switch (expression.NodeType)
            {
                case ExpressionType.Equal:
                    query = query.WhereEqualTo(clause.fieldName, clause.fieldValue);
                    break;
                case ExpressionType.NotEqual:
                    query = query.WhereNotEqualTo(clause.fieldName, clause.fieldValue);
                    break;
                case ExpressionType.GreaterThan:
                    query = query.WhereGreaterThan(clause.fieldName, clause.fieldValue);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    query = query.WhereGreaterThanOrEqualTo(clause.fieldName, clause.fieldValue);
                    break;
                case ExpressionType.LessThan:
                    query = query.WhereLessThan(clause.fieldName, clause.fieldValue);
                    break;
                case ExpressionType.LessThanOrEqual:
                    query = query.WhereLessThanOrEqualTo(clause.fieldName, clause.fieldValue);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}