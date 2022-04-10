using Google.Cloud.Firestore;
using System.Linq.Expressions;
using System.Reflection;

namespace FirestoreLINQ
{
    internal static class FieldValueHelper
    {
        static readonly string FirestoreDontSupportMsg = "Firestore doesn't support this operation.";

        internal static (MemberInfo memberInfo, string fieldName, object value) GetMemberAndFieldNameAndValue(this MethodCallExpression expression)
        {
            MemberInfo memberInfo = null;
            MemberExpression me = null;
            object value = null;
            if (expression.Object != null)
            {
                if (expression.Object.NodeType == ExpressionType.MemberAccess)
                {
                    me = expression.Object as MemberExpression;
                    memberInfo = me.Member;

                    var fireProperty = memberInfo.GetCustomAttribute<FirestorePropertyAttribute>();
                    if (fireProperty != null)
                    {
                        value = expression.Arguments[0].GetExpressionValue();
                    }
                    else memberInfo = null;
                }
                else if (expression.Object.NodeType == ExpressionType.ListInit)
                {
                    var li = expression.Object as ListInitExpression;
                    value = li.Initializers.Select(x => x.Arguments[0].GetExpressionValue());

                    me = expression.Arguments[0] as MemberExpression;
                    memberInfo = me.Member;

                    var fireProperty = memberInfo.GetCustomAttribute<FirestorePropertyAttribute>();
                    if (fireProperty == null)
                        me = null;
                }
            }
            if (memberInfo == null && expression.Arguments[0] != null)
            {
                me = expression.Arguments[0] as MemberExpression;
                memberInfo = me.Member;
                value = null;

                var fireProperty = memberInfo.GetCustomAttribute<FirestorePropertyAttribute>();
                if (fireProperty != null)
                {
                    value = expression.Object?.GetExpressionValue() ?? expression.Arguments[1].GetExpressionValue();
                }
                else
                {
                    me = expression.Arguments[1] as MemberExpression;
                    memberInfo = me.Member;
                    fireProperty = memberInfo.GetCustomAttribute<FirestorePropertyAttribute>();
                    if (fireProperty != null)
                    {
                        value = expression.Object?.GetExpressionValue() ?? expression.Arguments[0].GetExpressionValue();
                    }
                }
            }
            return (memberInfo, me.GetFirestoreFieldName(), value);
        }

        internal static (string fieldName, object fieldValue) GetFieldNameAndValue(Expression leftExp, Expression rightExp)
        {
            string fieldName;
            object fieldValue;

            if (leftExp.NodeType == ExpressionType.MemberAccess && ((MemberExpression)leftExp).Member is PropertyInfo)
            {
                fieldName = leftExp.GetFirestoreFieldName();

                fieldValue = rightExp.GetExpressionValue();
            }
            else if (rightExp.NodeType == ExpressionType.MemberAccess && ((MemberExpression)rightExp).Member is PropertyInfo)
            {
                fieldName = rightExp.GetFirestoreFieldName();

                fieldValue = leftExp.GetExpressionValue();
            }
            else if (leftExp.NodeType == ExpressionType.Parameter || rightExp.NodeType == ExpressionType.Parameter)
                throw new NotSupportedException("Passing paramater is not supported");
            else throw new NotImplementedException();

            return (fieldName, fieldValue);
        }

        internal static string GetFirestoreFieldName(this MemberExpression exp)
        {
            string path = exp.Member.GetCustomAttribute<FirestorePropertyAttribute>().Name ?? exp.Member.Name;
            while (exp.Expression != null && exp.Expression.NodeType == ExpressionType.MemberAccess)
            {
                exp = exp.Expression as MemberExpression;
                var memberName = exp.Member.GetCustomAttribute<FirestorePropertyAttribute>().Name ?? exp.Member.Name;
                path = memberName + "." + path;
            }
            return path;
        }

        internal static string GetFirestoreFieldName(this Expression exp)
        {
            return (exp as MemberExpression).GetFirestoreFieldName();
        }

        internal static object GetExpressionValue(this Expression expression)
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
