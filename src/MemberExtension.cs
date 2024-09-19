using LinqToDB.Reflection;
using System.Reflection;

namespace LinqToDB.Repository
{
    public static class MemberExtension
    {
        public static CustomAttributeData? GetAttribute(this MemberAccessor member, Type attribute)
        {
            return member.MemberInfo.CustomAttributes.Where(x => x.AttributeType == attribute).FirstOrDefault();
        }

        public static bool IsAttributeDefined(this MemberAccessor memberAccessor, Type attribute)
        {
            return memberAccessor.GetAttribute(attribute) != null;
        }
    }
}