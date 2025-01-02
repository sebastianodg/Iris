using Iris.Base.Abstractions;
using OpenGL;
using System.Diagnostics;
using System.Text;

namespace Iris.Base.Shaders;

public class IrisDefaultShader : IIrisShader
{
	private readonly UInt16 _compileResultLogMaxLength = 0xFFFF;

	private ShaderType _type;
	private String _sourceCode;
	private UInt32 _id;
	private System.Boolean _isCompiled;
	private StringBuilder _compileResultLog;

	public ShaderType Type { get { return this._type; } }

	public UInt32 Id { get { return this._id; } }

	public String SourceCode { get { return this._sourceCode; } }

	public System.Boolean IsCompiled { get { return this._isCompiled; } }

	public StringBuilder CompileResultLog { get { return this._compileResultLog; } }

	public IrisDefaultShader(ShaderType type, String sourceCode)
	{
		if (String.IsNullOrEmpty(sourceCode))
			throw new Exception($"{nameof(IrisDefaultShader)}.{nameof(IrisDefaultShader)}: Shader source code cannot be null or empty.");

		this._type = type;
		this._id = 0;
		this._sourceCode = sourceCode;
		this._isCompiled = false;
		this._compileResultLog = new StringBuilder(this._compileResultLogMaxLength);
	}

	public System.Boolean Compile()
	{
		this._isCompiled = false;
		this._compileResultLog = new StringBuilder(this._compileResultLogMaxLength);

		this._id = Gl.CreateShader(this._type);
		Gl.ShaderSource(this._id, new string[] { this._sourceCode });
		Gl.CompileShader(this._id);

		Gl.GetShader(this._id, ShaderParameterName.CompileStatus, out Int32 compileStatus);
		this._isCompiled = compileStatus == 1;
		if (!this._isCompiled)
		{
			Gl.GetShader(this._id, ShaderParameterName.InfoLogLength, out Int32 infoLogLength);
			Gl.GetShaderInfoLog(this._id, this._compileResultLogMaxLength, out Int32 infoLogActualLength, this._compileResultLog);

			Trace.TraceError($"{nameof(IrisDefaultShader)}.{nameof(Compile)}: Failed to compile {this._type} shader with id {this._id}:");
			Trace.Indent();
			Trace.WriteLine(this._compileResultLog.ToString());
			Trace.Unindent();

			Gl.DeleteShader(this._id);
			this._id = 0;
		}

		return true;
	}

	public void Delete()
	{
		if (!this._isCompiled)
			return;

		Gl.DeleteShader(this._id);
		this._isCompiled = false;
		this._id = 0;
	}
}
