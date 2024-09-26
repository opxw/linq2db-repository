using LinqToDB.Reflection;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToDB.Repository
{
    internal static class Utils
    {
        public static CustomAttributeData? GetAttribute(this MemberAccessor member, Type attribute)
        {
            return member.MemberInfo.CustomAttributes.Where(x => x.AttributeType == attribute).FirstOrDefault();
        }

        public static bool IsAttributeDefined(this MemberAccessor memberAccessor, Type attribute)
        {
            return memberAccessor.GetAttribute(attribute) != null;
        }

        public static Expression<Func<T, bool>> ModifyStringExpression<T>(
           Expression<Func<T, string>> expression, LinqFilter filter, string text)
        {
            return Expression.Lambda<Func<T, bool>>(
                Expression.Call(
                    expression.Body,
                    filter.ToString(),
                    null,
                    Expression.Constant(text)),
                expression.Parameters);
        }
    }
}