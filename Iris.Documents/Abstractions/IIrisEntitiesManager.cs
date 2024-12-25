using Iris.Documents.Types;
using System.Drawing;

namespace Iris.Documents.Abstractions;

internal interface IIrisEntitiesManager
{
	IIrisDoc Doc { get; }

	UInt32 AddEntity(IIrisDocEntity docEntity, String layerName);
	List<UInt32> AddEntities(String layerName, params IIrisDocEntity[] docEntities);
	List<UInt32> AddEntities(List<IIrisDocEntity> docEntities, String layerName);

	void RemoveEntity(UInt32 entityId);
	void RemoveEntity(IIrisDocEntity entity);
	void RemoveEntities(params IIrisDocEntity[] entities);
	void RemoveEntities(List<IIrisDocEntity> entities);
	void RemoveAllEntities();
	void RemoveAllLayerEntities(String layerName);

	IIrisDocEntity GetEntity(UInt32 entityId);
	List<IIrisDocEntity> GetLayerEntities(String layerName);

	void ChangeEntityLayer(UInt32 docEntityId, String layerName);
	void ChangeEntityLayer(IIrisDocEntity docEntity, String layerName);

	void SetEntityName(UInt32 docEntityId, String name);
	void SetEntityColorSource(UInt32 docEntityId, IrisColorSource colorSource);
	void SetEntityColor(UInt32 docEntityId, Color color);
	void SetEntityHidden(UInt32 docEntityId, Boolean hidden);
	void SetEntityLocked(UInt32 docEntityId, Boolean locked);
	void SetEntitySelected(UInt32 docEntityId, Boolean hidden);
}
