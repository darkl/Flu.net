using Flunet.Attributes;

namespace Flunet.Samples.DataSets
{
    [Scope("DataRelation")]
    public interface IDataRelationSyntax
    {
        [Mandatory]
        [UniqueInScope("DataRelation")]
        IDataRelationSyntax WithParentKey(string key);

        [Mandatory]
        [UniqueInScope("DataRelation")]
        IDataRelationSyntax WithChildKey(string key);

        [UniqueInScope("DataRelation")]
        IDataRelationSyntax Named(string key);
    }
}
