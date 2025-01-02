using Iris.Base.Abstractions;
using Iris.Base.Types;
using OpenGL;
using System.Diagnostics;

namespace Iris.Graphics.RenderWindow;

/// <summary>
/// Controllo utilizzato come output del rendering
/// </summary>
public partial class IrisRenderWindow : UserControl
{
	private IrisRenderWndLogoPainter _logoPainter;

	private IrisGfxEnvironmentSettings _gfxEnvironmentSettings;
	private IIrisGfxEnvironmentCreator? _gfxEnvironmentCreator = null;

	private DeviceContext? _openGLDeviceContext = null;
	private DevicePixelFormatCollection? _availablePixelFormats = null;
	private List<DevicePixelFormat>? _matchingPixelFormats = null;
	private IntPtr _openGLRenderContext = IntPtr.Zero;

	public event EventHandler<PaintEventArgs>? OnRenderFrame = null;

	public IrisRenderWindow()
	{
		// Impostazione dello stile della finestra. Necessario per OpenGL
		base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		base.SetStyle(ControlStyles.Opaque, true);
		base.SetStyle(ControlStyles.DoubleBuffer, false);
		base.SetStyle(ControlStyles.ResizeRedraw, true);
		base.SetStyle(ControlStyles.UserPaint, true);

		this._logoPainter = new IrisRenderWndLogoPainter(this);

		this._gfxEnvironmentSettings = new()
		{
			ColorBufferBits = IrisBufferBitsCount.Bits32,
			DepthBufferBits = IrisBufferBitsCount.Bits24,
			StencilBufferBits = IrisBufferBitsCount.Bits8,
			AntiAliasingLevel = IrisAntiAliasingLevel.Off,
			DoubleBuffer = true,
			VerticalSync = true,
			RGBAUnsigned = true,
			RenderWindow = true,
		};

		this.InitializeComponent();
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		if (this.DesignMode)
			return;

		// Creazione del device context della finestra
		this._openGLDeviceContext = DeviceContext.Create(IntPtr.Zero, base.Handle);
		this._openGLDeviceContext.IncRef();

		// Lettura dell'elenco dei pixel format disponibili
		this._availablePixelFormats = this._openGLDeviceContext.PixelsFormats;
		if (this._availablePixelFormats == null)
			throw new Exception($"{nameof(IrisRenderWindow)}.{nameof(OnHandleCreated)}: Unable to get pixel formats list.");
		if (this._availablePixelFormats.Count == 0)
			throw new Exception($"{nameof(IrisRenderWindow)}.{nameof(OnHandleCreated)}: Unable to find a single available pixel format.");
		Trace.TraceInformation($"{nameof(IrisRenderWindow)}.{nameof(OnHandleCreated)}: Found {this._availablePixelFormats.Count} available OpenGL pixel formats.");

		// Impostazione della struttura dati contenente le informazioni sul pixel format richiesto
		DevicePixelFormat controlReqFormat = new()
		{
			RgbaUnsigned = true,
			RenderWindow = true,
			ColorBits = 32,
			DepthBits = 24,
			StencilBits = 8,
			MultisampleBits = 8,
			DoubleBuffer = true,
		};

		// Scelta del pixel format più adatto. Vengono selezionati diversi pixel format. Il primo dovrebbe essere quello più vicino alle caratteristiche richieste
		this._matchingPixelFormats = this._availablePixelFormats.Choose(controlReqFormat);
		if (this._matchingPixelFormats == null)
			throw new Exception($"{nameof(IrisRenderWindow)}.{nameof(OnHandleCreated)}: Unable to get choosen pixel formats list.");
		if (this._matchingPixelFormats.Count == 0)
			throw new Exception($"{nameof(IrisRenderWindow)}.{nameof(OnHandleCreated)}: Unable to find a single choosen pixel format.");
		Trace.TraceInformation($"{nameof(IrisRenderWindow)}.{nameof(OnHandleCreated)}: {this._matchingPixelFormats.Count} OpenGL pixel formats were choosen.");

		// Impostazione del pixel format per la finestra di output
		this._openGLDeviceContext.SetPixelFormat(this._matchingPixelFormats[0]);
		Trace.TraceInformation($"{nameof(IrisRenderWindow)}.{nameof(OnHandleCreated)}: OpenGL pixel format set: {this._matchingPixelFormats[0]}.");

		// Creazione del render context
		this._openGLRenderContext = this._openGLDeviceContext.CreateContext(IntPtr.Zero);
		if (this._openGLRenderContext != IntPtr.Zero)
			Trace.TraceInformation($"{nameof(IrisRenderWindow)}.{nameof(OnHandleCreated)}: OpenGL render context successfully created.");
		else
			throw new Exception($"{nameof(IrisRenderWindow)}.{nameof(OnHandleCreated)}: Error creating OpenGL render context.");

		// Impostazione del vertical sync
		if (!this._gfxEnvironmentSettings.VerticalSync)
		{
			System.Boolean verticalSyncSet = Wgl.SwapIntervalEXT(this._gfxEnvironmentSettings.VerticalSync ? 1 : 0);
			if (!verticalSyncSet)
				throw new Exception($"{nameof(IrisRenderWindow)}.{nameof(OnHandleCreated)}: Unable to set vertical sync.");
		}
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		// Distruzione del render context e del device context della finestra
		if (this._openGLDeviceContext != null)
		{
			if (this._openGLRenderContext != IntPtr.Zero)
				this._openGLDeviceContext.DeleteContext(this._openGLRenderContext);
			this._openGLRenderContext = IntPtr.Zero;

			this._openGLDeviceContext.DecRef();
			this._openGLDeviceContext = null;
		}

		base.OnHandleDestroyed(e);
	}

	protected override void OnPaint(PaintEventArgs args)
	{
		if (base.DesignMode)
			this._logoPainter.OnPaint(args);
		else
			this.OnRender();
	}

	private void OnRender()
	{
		if (this._openGLDeviceContext == null || this._openGLRenderContext == IntPtr.Zero)
			return;

		if (this._openGLDeviceContext.MakeCurrent(this._openGLRenderContext) == false)
			throw new Exception($"{nameof(IrisRenderWindow)}.{nameof(OnPaint)}: unable to set render context as current.");

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

		this._openGLDeviceContext?.SwapBuffers();
	}
}
