using Flunet.Attributes;

namespace Flunet.Samples.DataSets
{
    [Inherited]
    [Scope("DataSet")]
    public interface IDataSetSyntax
    {
        IDataTableSyntax WithTable(string name);
    }
}
