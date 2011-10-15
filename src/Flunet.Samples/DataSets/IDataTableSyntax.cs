using Flunet.Attributes;

namespace Flunet.Samples.DataSets
{
    [Inherited]
    [Scope("DataTable")]
    public interface IDataTableSyntax
    {
        [UniqueInScope("DataTable")]
        [Alias("PrimaryKey")]
        IDataColumnSyntax<T> WithPrimaryKey<T>(string name);

        IDataColumnSyntax<T> WithColumn<T>(string name);

        IDataRelationSyntax IsChildOf(string tableName);

        IDataRelationSyntax IsParentOf(string tableName);
    }
}
