using Iris.Base.Abstractions;
using Iris.Base.Programs;
using Iris.Base.Shaders;
using Iris.Base.Types;
using OpenGL;
using System.Diagnostics;
using System.Text;

namespace Iris.Graphics.RenderControl;

/// <summary>
/// Controllo utilizzato come output del rendering
/// </summary>
public partial class IrisRenderControl : UserControl
{
	private IrisRenderCtrlLogoPainter _logoPainter;

	private IrisGfxEnvironmentSettings? _gfxEnvironmentSettings = null;
	private IIrisGfxEnvironmentCreator? _gfxEnvironmentCreator = null;

	private DeviceContext? _openGLDeviceContext = null;
	private DevicePixelFormatCollection? _availablePixelFormats = null;
	private List<DevicePixelFormat>? _matchingPixelFormats = null;
	private IntPtr _openGLRenderContext = IntPtr.Zero;

	private IIrisShadersProgram? _shadersProgram = null;

	public IrisRenderControl()
	{
		// Impostazione dello stile della finestra. Necessario per OpenGL
		base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		base.SetStyle(ControlStyles.Opaque, true);
		base.SetStyle(ControlStyles.DoubleBuffer, false);
		base.SetStyle(ControlStyles.ResizeRedraw, true);
		base.SetStyle(ControlStyles.UserPaint, true);

		this._logoPainter = new IrisRenderCtrlLogoPainter(this);

		this.InitializeComponent();
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		if (this.DesignMode)
			return;

		this.InitializeOpenGL();
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		this.DestroyOpenGL();
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
		this.PrepareRendering();
		this.ClearFrameBuffers();
		this.InitVertexAndIndexBuffers();
		this.CreateShaderProgram();
		this.RenderGraphics();
	}

	private void RenderGraphics()
	{
		Gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);
		this._openGLDeviceContext?.SwapBuffers();
	}

	private void ClearFrameBuffers()
	{
		Gl.ClearColor(0.239f, 0.239f, 0.239f, 1.000f);
		Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
	}

	private void PrepareRendering()
	{
		if (this._openGLDeviceContext == null || this._openGLRenderContext == IntPtr.Zero)
			return;

		if (this._openGLDeviceContext.MakeCurrent(this._openGLRenderContext) == false)
			throw new Exception($"{nameof(IrisRenderControl)}.{nameof(OnPaint)}: unable to set render context as current.");

		Gl.Viewport(0, 0, this.ClientSize.Width, this.ClientSize.Height);
	}

	private void InitializeOpenGL()
	{
		// Impostazioni di creazione di OpenGL
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

		// Creazione del device context della finestra
		this._openGLDeviceContext = DeviceContext.Create(IntPtr.Zero, base.Handle);
		this._openGLDeviceContext.IncRef();

		// Lettura dell'elenco dei pixel format disponibili
		this._availablePixelFormats = this._openGLDeviceContext.PixelsFormats;
		if (this._availablePixelFormats == null)
			throw new Exception($"{nameof(IrisRenderControl)}.{nameof(OnHandleCreated)}: Unable to get pixel formats list.");
		if (this._availablePixelFormats.Count == 0)
			throw new Exception($"{nameof(IrisRenderControl)}.{nameof(OnHandleCreated)}: Unable to find a single available pixel format.");
		Trace.TraceInformation($"{nameof(IrisRenderControl)}.{nameof(OnHandleCreated)}: Found {this._availablePixelFormats.Count} available OpenGL pixel formats.");

		// Impostazione della struttura dati contenente le informazioni sul pixel format richiesto
		DevicePixelFormat controlReqFormat = new()
		{
			RgbaUnsigned = true,
			RenderWindow = true,
			ColorBits = 32,
			DepthBits = 24,
			StencilBits = 8,
			MultisampleBits = 16,
			DoubleBuffer = true,
		};

		// Scelta del pixel format più adatto. Vengono selezionati diversi pixel format. Il primo dovrebbe essere quello più vicino alle caratteristiche richieste
		this._matchingPixelFormats = this._availablePixelFormats.Choose(controlReqFormat);
		if (this._matchingPixelFormats == null)
			throw new Exception($"{nameof(IrisRenderControl)}.{nameof(OnHandleCreated)}: Unable to get choosen pixel formats list.");
		if (this._matchingPixelFormats.Count == 0)
			throw new Exception($"{nameof(IrisRenderControl)}.{nameof(OnHandleCreated)}: Unable to find a single choosen pixel format.");
		Trace.TraceInformation($"{nameof(IrisRenderControl)}.{nameof(OnHandleCreated)}: {this._matchingPixelFormats.Count} OpenGL pixel formats were choosen.");

		// Impostazione del pixel format per la finestra di output
		this._openGLDeviceContext.SetPixelFormat(this._matchingPixelFormats[0]);
		Trace.TraceInformation($"{nameof(IrisRenderControl)}.{nameof(OnHandleCreated)}: OpenGL pixel format set: {this._matchingPixelFormats[0]}.");

		// Creazione del render context
		this._openGLRenderContext = this._openGLDeviceContext.CreateContext(IntPtr.Zero);
		if (this._openGLRenderContext != IntPtr.Zero)
			Trace.TraceInformation($"{nameof(IrisRenderControl)}.{nameof(OnHandleCreated)}: OpenGL render context successfully created.");
		else
			throw new Exception($"{nameof(IrisRenderControl)}.{nameof(OnHandleCreated)}: Error creating OpenGL render context.");

		// Impostazione del vertical sync
		if (!this._gfxEnvironmentSettings.VerticalSync)
		{
			System.Boolean verticalSyncSet = Wgl.SwapIntervalEXT(this._gfxEnvironmentSettings.VerticalSync ? 1 : 0);
			if (!verticalSyncSet)
				throw new Exception($"{nameof(IrisRenderControl)}.{nameof(OnHandleCreated)}: Unable to set vertical sync.");
		}
	}

	private void DestroyOpenGL()
	{
		// Distruzione del programma contenente gli shader
		this._shadersProgram?.Delete();

		// Distruzione del render context e del device context della finestra
		if (this._openGLDeviceContext != null)
		{
			if (this._openGLRenderContext != IntPtr.Zero)
				this._openGLDeviceContext.DeleteContext(this._openGLRenderContext);
			this._openGLRenderContext = IntPtr.Zero;

			this._openGLDeviceContext.DecRef();
			this._openGLDeviceContext = null;
		}
	}

	private void InitVertexAndIndexBuffers()
	{
		Single[] verticesCoords =
		{
			-0.75f, -0.75f,
			 0.75f, -0.75f,
			 0.75f,  0.75f,
			-0.75f,  0.75f,
		};

		UInt32 vertexBufferId = Gl.GenBuffer();
		Gl.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);
		Gl.BufferData(BufferTarget.ArrayBuffer, (UInt32)(verticesCoords.Length * sizeof(Single)), verticesCoords, BufferUsage.StaticDraw);

		Gl.VertexAttribPointer(0, 2, VertexAttribType.Float, true, 2 * sizeof(Single), 0);
		Gl.EnableVertexAttribArray(0);

		UInt32[] vericesIndices =
		{
			0, 1, 2,
			2, 3, 0,
		};

		UInt32 indexBufferId = Gl.GenBuffer();
		Gl.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferId);
		Gl.BufferData(BufferTarget.ElementArrayBuffer, (UInt32)(vericesIndices.Length * sizeof(UInt32)), vericesIndices, BufferUsage.StaticDraw);
	}

	private void CreateShaderProgram()
	{
		StringBuilder vertexShaderSourceCode = new StringBuilder();
		vertexShaderSourceCode.AppendLine("#version 330 core");
		vertexShaderSourceCode.AppendLine("layout(location = 0) in vec4 position;");
		vertexShaderSourceCode.AppendLine("void main()");
		vertexShaderSourceCode.AppendLine("{");
		vertexShaderSourceCode.AppendLine("gl_Position = position;");
		vertexShaderSourceCode.AppendLine("}");

		IIrisShader vertexShader = new IrisDefaultShader(ShaderType.VertexShader, vertexShaderSourceCode.ToString());
		if (!vertexShader.Compile())
			return;

		StringBuilder fragmentShaderSourceCode = new StringBuilder();
		fragmentShaderSourceCode.AppendLine("#version 330 core");
		fragmentShaderSourceCode.AppendLine("layout(location = 0) out vec4 color;");
		fragmentShaderSourceCode.AppendLine("void main()");
		fragmentShaderSourceCode.AppendLine("{");
		fragmentShaderSourceCode.AppendLine("color = vec4(0.51f, 0.0f, 1.0f, 1.0f);");
		fragmentShaderSourceCode.AppendLine("}");

		IIrisShader fragmentShader = new IrisDefaultShader(ShaderType.FragmentShader, fragmentShaderSourceCode.ToString());
		if (!fragmentShader.Compile())
			return;

		this._shadersProgram = new IrisDefaultShadersProgram(vertexShader, fragmentShader);
		this._shadersProgram.Create();
		this._shadersProgram.Use();
	}
}
