using Flunet.Attributes;

namespace Flunet.Samples.DataSets
{
    [Scope("DataColumn")]
    public interface IDataColumnSyntax<T>
    {
        [UniqueInScope("DataColumn")]
        IDataColumnSyntax<T> MakeNullable();

        [UniqueInScope("DataColumn")]
        IDataColumnSyntax<T> WithDefaultValue(T value);

        [UniqueInScope("DataTable")]
        [Alias("PrimaryKey")]
        IDataColumnSyntax<T> MakePrimaryKey();
    }
}
