using Khronos;
using OpenGL;
using System.Diagnostics;

namespace Test.OpenGL.SandBox;

internal class RenderControl : Panel
{
	private readonly Color _irisLogoBackgroundColor = Color.FromArgb(44, 44, 44);

	private DeviceContext? _windowDeviceContext = null;
	private String _currentVendor = String.Empty;
	private String _currentRenderer = String.Empty;
	private KhronosVersion? _openGLCurrentVersion = null;
	private KhronosVersion? _openGLShadingVersion = null;
	private IntPtr _renderContext = IntPtr.Zero;

	public RenderControl()
	{
		// Impostazione dello stile della finestra. Necessario per OpenGL
		base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		base.SetStyle(ControlStyles.Opaque, true);
		base.SetStyle(ControlStyles.DoubleBuffer, false);
		base.SetStyle(ControlStyles.ResizeRedraw, true);
		base.SetStyle(ControlStyles.UserPaint, true);
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		if (!base.DesignMode)
			this.CreateDeviceContext();
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

		this.RenderContents();
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

		// Impostazione del primo pixel format per la finestra di output
		this._windowDeviceContext.SetPixelFormat(matchingPixelFormats[0]);

		if (Gl.PlatformExtensions.SwapControl)
			this._windowDeviceContext.SwapInterval(true ? 1 : 0);

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

		Gl.Viewport(0, 0, this.ClientSize.Width, this.ClientSize.Height);

		Gl.ClearColor(0.239f, 0.239f, 0.239f, 1.000f);
		Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

		Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
		Gl.Begin(PrimitiveType.Triangles);
		Gl.Color3(1.000f, 0.627f, 0.157f);
		Gl.Vertex2(0.0f, 0.5f);
		Gl.Color3(0.000f, 0.000f, 0.000f);
		Gl.Vertex2(-0.5f, -0.5f);
		Gl.Vertex2(+0.5f, -0.5f);
		Gl.End();


		Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Point);
		Gl.PointSize(3.000f);
		Gl.Begin(PrimitiveType.Points);
		Gl.Color3(1.000f, 1.000f, 1.000f);
		Gl.Vertex2(0.0f, 0.5f);
		Gl.Color3(0.000f, 0.000f, 0.000f);
		Gl.Vertex2(-0.5f, -0.5f);
		Gl.Vertex2(+0.5f, -0.5f);
		Gl.End();

		this._windowDeviceContext.SwapBuffers();
	}
}
