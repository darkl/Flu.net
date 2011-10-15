Flu.net
===========

A tool that helps you creating your own fluent syntax for .NET Framework applications in a declarative fashion.

It is aimed for infrastructures and other open-source projects use.

Example
-------

To create a simple fluent syntax for DataTables, all we need to do is declare the following interfaces:

	[Inherited]
	[Scope("DataTable")]
	public interface IDataTableSyntax
	{
		[UniqueInScope("DataTable")]
		[Alias("PrimaryKey")]
		IDataColumnSyntax<T> WithPrimaryKey<T>(string name);

		IDataColumnSyntax<T> WithColumn<T>(string name);
	}

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

We generate a fluent syntax using Flu.net. The generated fluent syntax ensures we can't call WithPrimaryKey/MakePrimaryKey more than once, and that we that we can't call MakeNullable/WithDefaultValue more than once per a declared DataColumn.