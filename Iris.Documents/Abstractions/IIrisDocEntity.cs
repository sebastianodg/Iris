using Iris.Documents.Types;
using System.Drawing;
using System.Numerics;

namespace Iris.Documents.Abstractions;

internal interface IIrisDocEntity
{
	Guid Id { get; set; }
	String Name { get; set; }
	Boolean IsHidden { get; set; }
	Boolean IsLocked { get; set; }
	Boolean IsSelected { get; set; }
	Vector3 Position { get; set; }
	Vector3 Rotation { get; set; }
	Vector3 Scale { get; set; }
	IrisColorSource ColorSource { get; set; }
	Color Color { get; set; }

	IIrisGeometry Geometry { get; set; }
}
