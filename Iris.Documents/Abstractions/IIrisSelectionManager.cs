namespace Iris.Documents.Abstractions;

internal interface IIrisSelectionManager
{
	IIrisDoc Doc { get; }

	void AddToSelection(UInt32 docEntityId);
	void AddToSelection(params UInt32[] docEntityIds);
	void AddToSelection(IEnumerable<UInt32> docEntityIds);
	void RemoveFromSelection(UInt32 docEntityId);
	void RemoveFromSelection(params UInt32[] docEntityIds);
	void RemoveFromSelection(IEnumerable<UInt32> docEntityIds);
	List<UInt32> GetAllSelected();
}
