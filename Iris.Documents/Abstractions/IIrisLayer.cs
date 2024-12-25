using System.Drawing;

namespace Iris.Documents.Abstractions;

internal interface IIrisLayer
{
	String Name { get; set; }
	String Description { get; set; }
	UInt16 Ordinal { get; set; }
	Color Color { get; set; }
	Boolean IsHidden { get; set; }
	Boolean IsLocked { get; set; }
	Dictionary<UInt32, IIrisDocEntity> Entities { get; set; }
}
