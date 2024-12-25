namespace Iris.Documents.Abstractions;

internal interface IIrisDoc
{
	UInt32 Id { get; set; }
	String Title { get; set; }
	String Path { get; set; }

	IIrisDocumentManager DocumentManager { get; set; }
}
