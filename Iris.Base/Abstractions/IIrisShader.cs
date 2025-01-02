using OpenGL;
using System.Text;

namespace Iris.Base.Abstractions;

public interface IIrisShader
{
	ShaderType Type { get; }

	UInt32 Id { get; }

	String SourceCode { get; }

	System.Boolean IsCompiled { get; }

	StringBuilder CompileResultLog { get; }

	System.Boolean Compile();

	void Delete();
}
