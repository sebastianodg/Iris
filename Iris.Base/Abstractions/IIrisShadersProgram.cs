namespace Iris.Base.Abstractions;

public interface IIrisShadersProgram
{
	IIrisShader? VertexShader { get; }

	IIrisShader? FragmentShader { get; }

	UInt32 Id { get; }

	Boolean IsCreated { get; }

	void Create();

	void Use();

	void Delete();
}
