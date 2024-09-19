namespace LinqToDB.Repository
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DbRepositorySkipWriteAttribute : Attribute
    {
        public DbRepositorySkipWriteAttribute(object value)
        {
            Value = value;
        }

        public object Value { get; set; }
    }
}