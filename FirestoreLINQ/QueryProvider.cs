using Google.Cloud.Firestore;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace FirestoreLINQ
{
    internal partial class QueryProvider : IQueryProvider
    {
        static readonly string FirestoreDontSupportMsg = "Firestore doesn't support this operation.";
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
                    case "Average" or "Sum":
                        {
                            var snapResult = GetResult();
                            return GenericAggregator<TResult>.Aggregate(snapResult, me.Name);
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
                    default:
                        break;
                }
            }

            var snapshot = GetResult();

            if (!snapshot.Any())
                return default;

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
                                var fieldName = GetFirestoreFieldName((le.Body as MemberExpression).Member);
                                string[] selectedFields = { fieldName };
                                query = query.Select(selectedFields);
                                break;
                            }
                        case "Min" or "Max":
                            {
                                var fieldName = GetFirestoreFieldName((le.Body as MemberExpression).Member);
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
                                var fieldName = GetFirestoreFieldName((le.Body as MemberExpression).Member);

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
                                    var ma = le.Body as MemberExpression;
                                    var fieldName = GetFirestoreFieldName(ma.Member);
                                    SelectedFields = new string[] { fieldName };
                                    query = query.Select(fieldName);
                                }
                                else if (le.Body.NodeType == ExpressionType.New)
                                {
                                    var ne = le.Body as NewExpression;
                                    SelectedFields = ne.Arguments.Select(x => x as MemberExpression).Select(x => GetFirestoreFieldName(x.Member)).ToArray();
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
                                var itemsCount = (int)GetExpressionValue((expression as MethodCallExpression).Arguments.Last());
                                query = query.Offset(itemsCount);
                                break;
                            }
                        case "Take" or "TakeLast":
                            {
                                var itemsCount = (int)GetExpressionValue((expression as MethodCallExpression).Arguments.Last());
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
            var me = expression.Body as MemberExpression;
            var fieldName = GetFirestoreFieldName(me.Member);

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
            var clause = GetFieldNameAndValue(expression);
            var fieldName = GetFirestoreFieldName(clause.memberInfo);
            var methodName = expression.Method.Name;

            if (methodName == "Contains")
            {
                var isGeneric = (clause.memberInfo as PropertyInfo).PropertyType.IsGenericType;
                if (isGeneric)
                    query = query.WhereArrayContains(fieldName, clause.value);
                else
                {
                    if (notAlsoApplied)
                        query = query.WhereNotIn(fieldName, clause.value as IEnumerable);
                    else
                        query = query.WhereIn(fieldName, clause.value as IEnumerable);
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
                        values = GetExpressionValue(me.Object as MemberExpression) as IEnumerable;
                    else if (me.Object.NodeType == ExpressionType.ListInit)
                    {
                        var li = me.Object as ListInitExpression;
                        values = li.Initializers.Select(x => GetExpressionValue(x.Arguments
                               [0]));
                    }
                }
                else
                {
                    values = GetExpressionValue(me.Arguments.First()) as IEnumerable;
                }
                query = query.WhereArrayContainsAny(fieldName, values);
            }
        }

        void ParseBinaryExpression(BinaryExpression expression)
        {
            var clause = GetFieldNameAndValue(expression.Left, expression.Right);
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

        static (MemberInfo memberInfo, object value) GetFieldNameAndValue(MethodCallExpression expression)
        {
            MemberInfo memberInfo = null;
            object value = null;
            if (expression.Object != null)
            {
                if (expression.Object.NodeType == ExpressionType.MemberAccess)
                {
                    var me = expression.Object as MemberExpression;
                    memberInfo = me.Member;

                    var fireProperty = memberInfo.GetCustomAttribute<FirestorePropertyAttribute>();
                    if (fireProperty != null)
                    {
                        value = GetExpressionValue(expression.Arguments[0]);
                    }
                    else memberInfo = null;
                }
                else if (expression.Object.NodeType == ExpressionType.ListInit)
                {
                    var li = expression.Object as ListInitExpression;
                    value = li.Initializers.Select(x => GetExpressionValue(x.Arguments
                           [0]));

                    var me = expression.Arguments[0] as MemberExpression;
                    memberInfo = me.Member;

                    var fireProperty = memberInfo.GetCustomAttribute<FirestorePropertyAttribute>();
                    if (fireProperty == null)
                        me = null;
                }
            }
            if (memberInfo == null && expression.Arguments[0] != null)
            {
                return GetFieldNameAndValue(expression.Arguments, expression.Object);
            }
            return (memberInfo, value);
        }

        static (MemberInfo memberInfo, object value) GetFieldNameAndValue(IList<Expression> args, Expression objectExp)
        {
            var me = args[0] as MemberExpression;
            var memberInfo = me.Member;
            object value = null;

            var fireProperty = memberInfo.GetCustomAttribute<FirestorePropertyAttribute>();
            if (fireProperty != null)
            {
                value = GetExpressionValue(objectExp ?? args[1]);
            }
            else
            {
                me = args[1] as MemberExpression;
                memberInfo = me.Member;
                fireProperty = memberInfo.GetCustomAttribute<FirestorePropertyAttribute>();
                if (fireProperty != null)
                {
                    value = GetExpressionValue(objectExp ?? args[0]);
                }
            }
            return (memberInfo, value);
        }

        static (string fieldName, object fieldValue) GetFieldNameAndValue(Expression leftExp, Expression rightExp)
        {
            string fieldName;
            object fieldValue;

            if (leftExp.NodeType == ExpressionType.MemberAccess && ((MemberExpression)leftExp).Member is PropertyInfo)
            {
                fieldName = GetFirestoreFieldName((leftExp as MemberExpression).Member);

                fieldValue = GetExpressionValue(rightExp);
            }
            else if (rightExp.NodeType == ExpressionType.MemberAccess && ((MemberExpression)rightExp).Member is PropertyInfo)
            {
                fieldName = GetFirestoreFieldName((rightExp as MemberExpression).Member);

                fieldValue = GetExpressionValue(leftExp);
            }
            else if (leftExp.NodeType == ExpressionType.Parameter || rightExp.NodeType == ExpressionType.Parameter)
                throw new NotSupportedException("Passing paramater is not supported");
            else throw new NotImplementedException();

            return (fieldName, fieldValue);
        }

        static string GetFirestoreFieldName(MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttribute<FirestorePropertyAttribute>().Name ?? memberInfo.Name;
        }

        static object GetExpressionValue(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Constant)
            {
                return (expression as ConstantExpression).Value;
            }
            else if (expression.NodeType == ExpressionType.MemberAccess && ((MemberExpression)expression).Member is FieldInfo)
            {
                var memberAccessExpr = expression as MemberExpression;
                if (memberAccessExpr.Expression is ConstantExpression constantAccessExpr)
                {
                    var innerMemberName = memberAccessExpr.Member.Name;
                    var compiledLambdaScopeField = constantAccessExpr.Value.GetType().GetField(innerMemberName);
                    return compiledLambdaScopeField.GetValue(constantAccessExpr.Value);
                }
            }
            else if (expression.NodeType == ExpressionType.MemberAccess)
                throw new NotSupportedException(FirestoreDontSupportMsg);

            return null;
        }
    }
}