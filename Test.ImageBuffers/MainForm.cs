using Iris.Base.Abstractions;
using Iris.Base.FrameRateCalculators;
using Iris.Graphics.Abstractions;
using Iris.Graphics.Buffers;

namespace Test.ImageBuffers;

public partial class MainForm : Form
{
	private Random _rand = new Random((Int32)DateTime.Now.Ticks);

	private Thread? _workingThread = null; // Oggetto rappresentante il thread
	private volatile Boolean _terminateWork = false; // Flag che indica se è necessario interrompere il thread
	private volatile ManualResetEvent _threadStartedResetEvent = new ManualResetEvent(false); // Oggetto che indica se il thread è stato avviato
	private volatile ManualResetEvent _threadTerminatedResetEvent = new ManualResetEvent(false); // Oggetto che indica se il thread è terminato
	private volatile Boolean _threadIsRunning = false; // Flag che indica se il thread è in esecuzione

	private IIrisImageBuffer? _imageBuffer;
	private IIrisFrameRateCalculator _irisFrameRateCalculator = new IrisAverageFrameRateCalculator(20);

	public MainForm()
	{
		this.InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		this._imageBuffer = new Iris32BitsARGBColorBuffer2((UInt16)this.ClientSize.Width, (UInt16)this.ClientSize.Height);

		this._irisFrameRateCalculator.StartWatching();

		this._threadStartedResetEvent.Reset();
		this._workingThread = new Thread(() => this.StartRenderLoop());
		this._workingThread.IsBackground = true;
		this._workingThread.Start();
		this._threadStartedResetEvent.WaitOne();
		this._threadIsRunning = true;
	}

	private void StartRenderLoop()
	{
		this._terminateWork = false;
		this._threadTerminatedResetEvent.Reset();
		this._threadStartedResetEvent.Set();

		while (!this._terminateWork)
			this.Invalidate();

		this._threadTerminatedResetEvent.Set();
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);

		this._imageBuffer?.ResizeBuffer((UInt16)this.ClientSize.Width, (UInt16)this.ClientSize.Height);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);

		this.ShowBufferContents(e.Graphics);
		this._irisFrameRateCalculator.SignalFrameRendered();
		this.Text = $"Test Buffers - Frame Time: {this._irisFrameRateCalculator.GetFrameTimeMilliseconds().ToString("N3")} seconds - Frames per second: {this._irisFrameRateCalculator.GetFramesPerSecond().ToString("N1")}";
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);

		if (e.KeyCode == Keys.Escape)
		{
			this._terminateWork = true;
			this._threadTerminatedResetEvent.WaitOne();
			this._threadIsRunning = false;
			Application.Exit();
		}
	}

	private void ShowBufferContents(Graphics gfx)
	{
		if (this._imageBuffer == null)
			return;

		Byte grayColorComponent = (Byte)this._rand.Next(0, 32);
		Color clearColor = Color.FromArgb(grayColorComponent, grayColorComponent, grayColorComponent);

		for (UInt16 row = 0; row < this._imageBuffer.BufferHeight; row++)
			for (UInt16 col = 0; col < this._imageBuffer.BufferWidth; col++)
				this._imageBuffer.SetPixelColor(col, row, clearColor);

		//this._imageBuffer.Clear(clearColor);

		gfx.DrawImageUnscaledAndClipped(this._imageBuffer.GetImage(), this.ClientRectangle);
	}
}
