namespace Iris.Base.Abstractions;

public interface IIrisRenderThread
{
	Boolean IsRunning { get; }

	event EventHandler? NewFrame;

	void Start();
	void Stop();
}
