using Iris.Base.Abstractions;
using Iris.Base.FrameRateCalculators;
using Khronos;
using OpenGL;
using System.Diagnostics;

namespace Test.OpenGL.SandBox;

internal class RenderControl : Panel
{
	private readonly Color _irisLogoBackgroundColor = Color.FromArgb(44, 44, 44);

	private System.Boolean _rendering;
	private IIrisFrameRateCalculator _frameRateCalculator;

	private DeviceContext? _windowDeviceContext = null;
	private String _currentVendor = String.Empty;
	private String _currentRenderer = String.Empty;
	private KhronosVersion? _openGLCurrentVersion = null;
	private KhronosVersion? _openGLShadingVersion = null;
	private IntPtr _renderContext = IntPtr.Zero;

	public System.Boolean Rendering { get { return this._rendering; } }

	System.Windows.Forms.Timer _timer;

	public RenderControl()
	{
		// Impostazione dello stile della finestra. Necessario per OpenGL
		base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		base.SetStyle(ControlStyles.Opaque, true);
		base.SetStyle(ControlStyles.DoubleBuffer, false);
		base.SetStyle(ControlStyles.ResizeRedraw, true);
		base.SetStyle(ControlStyles.UserPaint, true);

		this._rendering = false;
		this._frameRateCalculator = new IrisAverageFrameRateCalculator(10);

		this._timer = new System.Windows.Forms.Timer();
		this._timer.Interval = 1;
		this._timer.Tick += this.OnTimerTick;
	}

	private void OnTimerTick(Object? sender, EventArgs e)
	{
		this.RenderTriangle();
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		if (!base.DesignMode)
			this.CreateDeviceContext();

		this._frameRateCalculator.StartWatching();
		this._timer.Start();
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		base.OnHandleDestroyed(e);

		// Distruzione del render context e del device context della finestra
		if (this._windowDeviceContext != null)
		{
			if (this._renderContext != IntPtr.Zero)
				this._windowDeviceContext.DeleteContext(this._renderContext);
			this._renderContext = IntPtr.Zero;

			this._windowDeviceContext.DecRef();
			this._windowDeviceContext = null;
		}
	}

	protected override void OnPaint(PaintEventArgs args)
	{
		if (base.DesignMode)
		{
			base.OnPaint(args);
			args.Graphics.SetClip(base.ClientRectangle);
			args.Graphics.Clear(this._irisLogoBackgroundColor);
			return;
		}

		this._rendering = true;
		this.RenderContents();
		this._rendering = false;

		this._frameRateCalculator.SignalFrameRendered();
		Trace.WriteLine($"Frame time: {this._frameRateCalculator.GetFrameTimeMilliseconds().ToString("000.000")} - FPS: {this._frameRateCalculator.GetFramesPerSecond().ToString("00000.0")}");
	}

	public System.Boolean CreateDeviceContext()
	{
		// Eventuale distruzione del device context della finestra creato precedentemente
		if (this._windowDeviceContext != null)
		{
			this._windowDeviceContext.DecRef();
			this._windowDeviceContext = null;
		}

		// Creazione del device context della finestra
		this._windowDeviceContext = DeviceContext.Create(IntPtr.Zero, this.Handle);
		this._windowDeviceContext.IncRef();

		// Lettura dell'elenco dei pixel format disponibili
		if (this._windowDeviceContext.PixelsFormats == null)
		{
			Trace.TraceError($"{nameof(RenderControl)}.{nameof(CreateDeviceContext)}: Unable to get pixel formats list.");
			throw new Exception($"{nameof(RenderControl)}.{nameof(CreateDeviceContext)}: Unable to get pixel formats list.");
		}
		if (this._windowDeviceContext.PixelsFormats.Count == 0)
		{
			Trace.TraceError($"{nameof(RenderControl)}.{nameof(CreateDeviceContext)}: Unable to find a single available pixel format.");
			throw new Exception($"{nameof(RenderControl)}.{nameof(CreateDeviceContext)}: Unable to find a single available pixel format.");
		}
		Trace.TraceInformation($"{nameof(RenderControl)}.{nameof(CreateDeviceContext)}: Found {this._windowDeviceContext.PixelsFormats.Count} available pixel formats.");

		// Impostazione della struttura dati contenente le informazioni sul pixel format richiesto
		DevicePixelFormat requestedPixelFormat = new DevicePixelFormat()
		{
			ColorBits = 32,
			DepthBits = 24,
			StencilBits = 8,
			MultisampleBits = 0,
			DoubleBuffer = true,
			RgbaUnsigned = true,
			RenderWindow = true,
		};

		// Scelta del pixel format più adatto. Vengono selezionati diversi pixel format. Il primo dovrebbe essere quello più vicino alle caratteristiche richieste
		List<DevicePixelFormat> matchingPixelFormats = this._windowDeviceContext.PixelsFormats.Choose(requestedPixelFormat);
		if (matchingPixelFormats == null)
		{
			Trace.TraceError($"{nameof(RenderControl)}.{nameof(CreateDeviceContext)}: Unable to get choosen pixel formats list.");
			throw new Exception($"{nameof(RenderControl)}.{nameof(CreateDeviceContext)}: Unable to get choosen pixel formats list.");
		}
		if (matchingPixelFormats.Count == 0)
		{
			Trace.TraceError($"{nameof(RenderControl)}.{nameof(CreateDeviceContext)}: Unable to find a single choosen pixel format.");
			throw new Exception($"{nameof(RenderControl)}.{nameof(CreateDeviceContext)}: Unable to find a single choosen pixel format.");
		}
		Trace.TraceInformation($"{nameof(RenderControl)}.{nameof(CreateDeviceContext)}: {matchingPixelFormats.Count} pixel formats were selected.");

		if (Gl.PlatformExtensions.SwapControl)
			this._windowDeviceContext.SwapInterval(0);

		// Impostazione del primo pixel format per la finestra di output
		this._windowDeviceContext.SetPixelFormat(matchingPixelFormats[0]);

		// Recupero delle informazioni sull'ambiente grafico
		this._currentVendor = Gl.CurrentVendor;
		this._currentRenderer = Gl.CurrentRenderer;
		if (Gl.CurrentVersion != null)
			this._openGLCurrentVersion = Gl.CurrentVersion;
		if (Gl.CurrentShadingVersion != null)
			this._openGLShadingVersion = Gl.CurrentShadingVersion;
		Trace.WriteLine("");
		Trace.TraceInformation($"{nameof(RenderControl)}.{nameof(CreateDeviceContext)}: Graphic environment information:");
		Trace.Indent();
		Trace.Write("Current vendor:                          ");
		Trace.WriteLine(!String.IsNullOrEmpty(this._currentVendor) ? this._currentVendor : "-");
		Trace.Write("Current renderer:                        ");
		Trace.WriteLine(!String.IsNullOrEmpty(this._currentRenderer) ? this._currentRenderer : "-");
		Trace.Write("Current OpenGL version:                  ");
		Trace.WriteLine(this._openGLCurrentVersion != null ? this._openGLCurrentVersion.Major + "." + this._openGLCurrentVersion.Minor + " - API:" + this._openGLCurrentVersion.Api : "-");
		Trace.Write("Current OpenGL shading language version: ");
		Trace.WriteLine(this._openGLShadingVersion != null ? this._openGLShadingVersion.Major + "." + this._openGLShadingVersion.Minor + " - API:" + this._openGLShadingVersion.Api : "-");
		Trace.Unindent();

		// Creazione del render context
		this._renderContext = this._windowDeviceContext.CreateContext(IntPtr.Zero);
		Trace.WriteLine("");
		if (this._renderContext != IntPtr.Zero)
			Trace.TraceInformation($"{nameof(RenderControl)}.{nameof(CreateDeviceContext)}: Graphic render context successfully created.");
		else
			Trace.TraceError($"{nameof(RenderControl)}.{nameof(CreateDeviceContext)}: Error creating graphic render context.");

		return true;
	}

	private void RenderContents()
	{
		if (this._windowDeviceContext == null || this._renderContext == IntPtr.Zero)
			return;

		if (this._windowDeviceContext.MakeCurrent(this._renderContext) == false)
		{
			Trace.TraceError($"{nameof(RenderControl)}.{nameof(RenderContents)}: unable to set render context as current.");
			throw new Exception($"{nameof(RenderControl)}.{nameof(RenderContents)}: unable to set render context as current.");
		}

		this.RenderTriangle();
	}

	public void RenderTriangle()
	{
		Random rand = new Random((Int32)DateTime.Now.Ticks);

		Single redComp = rand.NextSingle();
		Single greenComp = rand.NextSingle();
		Single blueComp = rand.NextSingle();

		Gl.Viewport(0, 0, this.ClientSize.Width, this.ClientSize.Height);

		Gl.ClearColor(0.239f, 0.239f, 0.239f, 1.000f);
		Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

		Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
		Gl.Begin(PrimitiveType.Triangles);
		Gl.Color3(redComp, greenComp, blueComp);
		Gl.Vertex2(0.0f, 0.5f);
		Gl.Vertex2(-0.5f, -0.5f);
		Gl.Vertex2(+0.5f, -0.5f);
		Gl.End();


		Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Point);
		Gl.PointSize(3.000f);
		Gl.Begin(PrimitiveType.Points);
		Gl.Color3(redComp, greenComp, blueComp);
		Gl.Vertex2(0.0f, 0.5f);
		Gl.Vertex2(-0.5f, -0.5f);
		Gl.Vertex2(+0.5f, -0.5f);
		Gl.End();

		this._windowDeviceContext?.SwapBuffers();

		this._frameRateCalculator.SignalFrameRendered();
		//MainForm? mainForm = (MainForm?)this.Parent;
		//if (mainForm != null)
		//	mainForm.Text = $"Frame time: {this._frameRateCalculator.GetFrameTimeMilliseconds().ToString("000.000")} - FPS: {this._frameRateCalculator.GetFramesPerSecond().ToString("00000.0")}";
	}
}
