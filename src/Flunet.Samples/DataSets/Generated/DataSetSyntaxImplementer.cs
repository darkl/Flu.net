using System.Data;

namespace Flunet.Samples.DataSets.Generated
{
    public partial class DataSetSyntax
    {
        internal class DataSetSyntaxImplementer<T> : SyntaxImplementer<T>
        {
            #region Members

            private DataSet mDataSet;
            private DataTable mTable;
            private DataColumn mColumn;
            private DataTable mParent;
            private DataColumn mParentKey;
            private DataTable mChild;
            private DataColumn mChildKey;
            private DataRelation mRelation;

            #endregion

            #region Constructor

            public DataSetSyntaxImplementer(DataSet dataSet)
            {
                mDataSet = dataSet;
            }

            private DataSetSyntaxImplementer()
            {
            }

            #endregion

            #region Inner implementation

            protected override SyntaxImplementer<T> InnerWithTable(string name)
            {
                mTable = mDataSet.Tables.Add(name);
                return this;
            }

            protected override SyntaxImplementer<T1> InnerWithPrimaryKey<T1>(string name)
            {
                return ((DataSetSyntaxImplementer<T1>)InnerWithColumn<T1>(name)).InnerMakePrimaryKey();
            }

            protected override SyntaxImplementer<T1> InnerWithColumn<T1>(string name)
            {
                DataColumn column = mTable.Columns.Add(name, typeof(T1));

                return new DataSetSyntaxImplementer<T1>
                {
                    mDataSet = mDataSet,
                    mTable = mTable,
                    mColumn = column
                };
            }

            protected override SyntaxImplementer<T> InnerIsChildOf(string tableName)
            {
                mParent = mDataSet.Tables[tableName];
                mChild = mTable;
                return this;
            }

            protected override SyntaxImplementer<T> InnerIsParentOf(string tableName)
            {
                mParent = mTable;
                mChild = mDataSet.Tables[tableName];
                return this;
            }

            protected override SyntaxImplementer<T> InnerMakeNullable()
            {
                mColumn.AllowDBNull = true;
                return this;
            }

            protected override SyntaxImplementer<T> InnerWithDefaultValue(T value)
            {
                mColumn.DefaultValue = value;
                return this;
            }

            protected override SyntaxImplementer<T> InnerMakePrimaryKey()
            {
                mTable.PrimaryKey = new[] { mColumn };
                return this;
            }

            protected override SyntaxImplementer<T> InnerWithParentKey(string key)
            {
                mParentKey = mParent.Columns[key];
                return this;
            }

            protected override SyntaxImplementer<T> InnerWithChildKey(string key)
            {
                mChildKey = mChild.Columns[key];
                mRelation = mParent.ChildRelations.Add(mParentKey, mChildKey);
                return this;
            }

            protected override SyntaxImplementer<T> InnerNamed(string key)
            {
                mRelation.RelationName = key;
                return this;
            }

            #endregion
        }
         
    }

    public static class DataSetExtensions
    {
        public static DataSetSyntax.IState2 WithTable(this DataSet dataSet, string name)
        {
            DataSetSyntax.IState2 syntax = new DataSetSyntax.DataSetSyntaxImplementer<object>(dataSet);

            return syntax.WithTable(name);
        }
    }
}