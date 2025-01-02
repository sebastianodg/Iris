using Iris.Base.Abstractions;
using OpenGL;

namespace Iris.Base.Programs;

public class IrisDefaultShadersProgram : IIrisShadersProgram
{
	private IIrisShader _vertexShader;
	private IIrisShader _fragmentShader;
	private UInt32 _id;
	private System.Boolean _isCreated;

	public IIrisShader? VertexShader { get { return this._vertexShader; } }

	public IIrisShader? FragmentShader { get { return this._fragmentShader; } }

	public UInt32 Id { get { return this._id; } }

	public System.Boolean IsCreated { get { return this._isCreated; } }

	public IrisDefaultShadersProgram(IIrisShader vertexShader, IIrisShader fragmentShader)
	{
		if (vertexShader == null)
			throw new Exception($"{nameof(IrisDefaultShadersProgram)}.{nameof(IrisDefaultShadersProgram)}: Vertex shader cannot be null.");
		if (fragmentShader == null)
			throw new Exception($"{nameof(IrisDefaultShadersProgram)}.{nameof(IrisDefaultShadersProgram)}: Fragment shader cannot be null.");

		this._vertexShader = vertexShader;
		this._fragmentShader = fragmentShader;
		this._id = 0;
		this._isCreated = false;
	}

	public void Create()
	{
		this._id = Gl.CreateProgram();

		if (!this._vertexShader.IsCompiled)
			this._vertexShader.Compile();
		if (!this._fragmentShader.IsCompiled)
			this._fragmentShader.Compile();

		Gl.AttachShader(this._id, this._vertexShader.Id);
		Gl.AttachShader(this._id, this._fragmentShader.Id);
		Gl.LinkProgram(this._id);
		Gl.ValidateProgram(this._id);

		this._vertexShader.Delete();
		Gl.DetachShader(this._id, this._vertexShader.Id);
		this._fragmentShader.Delete();
		Gl.DetachShader(this._id, this._fragmentShader.Id);

		this._isCreated = true;
	}

	public void Use()
	{
		if (!this._isCreated)
			throw new Exception($"{nameof(IrisDefaultShadersProgram)}.{nameof(Use)}: Program is not created.");

		Gl.UseProgram(this._id);
	}

	public void Delete()
	{
		if (!this._isCreated)
			throw new Exception($"{nameof(IrisDefaultShadersProgram)}.{nameof(Delete)}: Program is not created.");

		Gl.DeleteProgram(this._id);
		this._id = 0;
		this._isCreated = false;
	}
}
