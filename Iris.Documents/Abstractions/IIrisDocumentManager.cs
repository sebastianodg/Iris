using Iris.Base.Abstractions;

namespace Iris.Documents.Abstractions;

internal interface IIrisDocumentManager
{
	IIrisDoc Doc { get; }

	IIrisIdsManager IdsManager { get; set; }
	IIrisEntitiesManager EntitiesManager { get; set; }
	IIrisLayersManager LayersManager { get; set; }
	IIrisSelectionManager SelectionManager { get; set; }

	Dictionary<String, IIrisLayer> Layers { get; }
	Dictionary<UInt32, IIrisDocEntity> Entities { get; }
}
