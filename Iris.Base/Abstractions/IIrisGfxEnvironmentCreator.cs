using Iris.Base.Types;

namespace Iris.Base.Abstractions;

public interface IIrisGfxEnvironmentCreator : IDisposable
{
	IrisGfxEnvironmentSettings GfxEnvironmentSettings { get; }

	void Create();

	IrisGfxEnvironmentCaps GetEnvironmentCaps();

	void PresentBackBuffer();
}
