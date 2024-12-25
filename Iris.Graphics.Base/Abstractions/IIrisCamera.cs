using Iris.Graphics.Base.Types;
using System.Numerics;

namespace Iris.Graphics.Base.Abstractions;

internal interface IIrisCamera
{
	UInt32 Id { get; }
	String Name { get; }
	Vector3 Position { get; }
	Vector3 Rotation { get; }
	Single FieldOfView { get; }
	IrisCameraProjection ProjectionType { get; }

	Matrix4x4 GetViewMatrix();
	Matrix4x4 GetProjectionMatrix();
}
