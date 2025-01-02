using Iris.Base.Abstractions;

namespace Iris.Base.RenderThreads;

public class IrisDefaultRenderThread : IIrisRenderThread
{
	private Thread? _workingThread = null; // Oggetto rappresentante il thread
	private volatile Boolean _terminateWork = false; // Flag che indica se è necessario interrompere il thread
	private volatile ManualResetEvent _threadStartedResetEvent = new ManualResetEvent(false); // Oggetto che indica se il thread è stato avviato
	private volatile ManualResetEvent _threadTerminatedResetEvent = new ManualResetEvent(false); // Oggetto che indica se il thread è terminato
	private volatile Boolean _threadIsRunning = false; // Flag che indica se il thread è in esecuzione

	public Boolean IsRunning { get { return this._threadIsRunning; } }

	public event EventHandler? NewFrame;

	public IrisDefaultRenderThread()
	{
		this._threadStartedResetEvent.Reset();
		this._workingThread = new Thread(() => this.StartRenderLoop());
		this._workingThread.IsBackground = true;
	}

	public void Start()
	{
		this._workingThread?.Start();
		this._threadStartedResetEvent.WaitOne();
		this._threadIsRunning = true;
	}

	public void Stop()
	{
		this._terminateWork = true;
		this._threadTerminatedResetEvent.WaitOne();
		this._threadIsRunning = false;
	}

	private void StartRenderLoop()
	{
		this._terminateWork = false;
		this._threadTerminatedResetEvent.Reset();
		this._threadStartedResetEvent.Set();

		while (!this._terminateWork)
			this.NewFrame?.Invoke(this, EventArgs.Empty);

		this._threadTerminatedResetEvent.Set();
	}
}
