using Iris.Graphics.RenderWindow;
using Khronos;
using OpenGL;
using System.Diagnostics;

namespace Iris.Graphics.Environment;

public class IrsGraphicEnvironment : IDisposable
{
	private IrsRenderWindow _renderWindow;
	private DeviceContext? _renderWindowDeviceContext = null;
	private DevicePixelFormatCollection? _availablePixelFormats = null;
	private List<DevicePixelFormat>? _matchingPixelFormats = null;
	private IntPtr _openGLRenderContext = IntPtr.Zero;

	private String? _currentVendor = null;
	private String? _currentRenderer = null;
	private KhronosVersion? _openGLCurrentVersion = null;
	private KhronosVersion? _openGLShadingVersion = null;

	/// <summary>
	/// Costruttore
	/// </summary>
	/// <param name="renderWindow">Riferimento al controllo destinatario dell'output del rendering</param>
	public IrsGraphicEnvironment(IrsRenderWindow renderWindow)
	{
		if (renderWindow == null)
			throw new Exception($"{nameof(IrsGraphicEnvironment)}.{nameof(IrsGraphicEnvironment)}: Reference to {nameof(IrsRenderWindow)} control cannot be null.");

		Trace.TraceInformation($"{nameof(IrsGraphicEnvironment)}.{nameof(IrsGraphicEnvironment)}: Initializing OpenGL graphic environment...");

		// Memorizzazione dei dati
		this._renderWindow = renderWindow;

		// Creazione del device context della finestra
		this._renderWindowDeviceContext = DeviceContext.Create(IntPtr.Zero, this._renderWindow.Handle);
		this._renderWindowDeviceContext.IncRef();

		// Lettura dell'elenco dei pixel format disponibili
		this._availablePixelFormats = this._renderWindowDeviceContext.PixelsFormats;
		if (this._availablePixelFormats == null)
		{
			Trace.TraceError($"{nameof(IrsGraphicEnvironment)}.{nameof(IrsGraphicEnvironment)}: Unable to get pixel formats list.");
			throw new Exception($"{nameof(IrsGraphicEnvironment)}.{nameof(IrsGraphicEnvironment)}: Unable to get pixel formats list.");
		}
		if (this._availablePixelFormats.Count == 0)
		{
			Trace.TraceError($"{nameof(IrsGraphicEnvironment)}.{nameof(IrsGraphicEnvironment)}: Unable to find a single available pixel format.");
			throw new Exception($"{nameof(IrsGraphicEnvironment)}.{nameof(IrsGraphicEnvironment)}: Unable to find a single available pixel format.");
		}
		Trace.TraceInformation($"{nameof(IrsGraphicEnvironment)}.{nameof(IrsGraphicEnvironment)}: Found {this._availablePixelFormats.Count} available OpenGL pixel formats.");

		// Impostazione della struttura dati contenente le informazioni sul pixel format richiesto
		DevicePixelFormat controlReqFormat = new DevicePixelFormat()
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
		{
			Trace.TraceError($"{nameof(IrsGraphicEnvironment)}.{nameof(IrsGraphicEnvironment)}: Unable to get choosen pixel formats list.");
			throw new Exception($"{nameof(IrsGraphicEnvironment)}.{nameof(IrsGraphicEnvironment)}: Unable to get choosen pixel formats list.");
		}
		if (this._matchingPixelFormats.Count == 0)
		{
			Trace.TraceError($"{nameof(IrsGraphicEnvironment)}.{nameof(IrsGraphicEnvironment)}: Unable to find a single choosen pixel format.");
			throw new Exception($"{nameof(IrsGraphicEnvironment)}.{nameof(IrsGraphicEnvironment)}: Unable to find a single choosen pixel format.");
		}
		Trace.TraceInformation($"{nameof(IrsGraphicEnvironment)}.{nameof(IrsGraphicEnvironment)}: {this._matchingPixelFormats.Count} OpenGL pixel formats were choosen.");

		// Impostazione del pixel format per la finestra di output
		this._renderWindowDeviceContext.SetPixelFormat(this._matchingPixelFormats[0]);
		Trace.TraceInformation($"{nameof(IrsGraphicEnvironment)}.{nameof(IrsGraphicEnvironment)}: OpenGL pixel format set: {this._matchingPixelFormats[0]}.");

		// Creazione del render context
		this._openGLRenderContext = this._renderWindowDeviceContext.CreateContext(IntPtr.Zero);
		if (this._openGLRenderContext != IntPtr.Zero)
			Trace.TraceInformation($"{nameof(IrsGraphicEnvironment)}.{nameof(IrsGraphicEnvironment)}: OpenGL render context successfully created.");
		else
			Trace.TraceError($"{nameof(IrsGraphicEnvironment)}.{nameof(IrsGraphicEnvironment)}: Error creating OpenGL remder context.");

		// Recupero delle informazioni sull'ambiente di OpenGL
		this._currentVendor = Gl.CurrentVendor;
		this._currentRenderer = Gl.CurrentRenderer;
		if (Gl.CurrentVersion != null)
			this._openGLCurrentVersion = Gl.CurrentVersion;
		if (Gl.CurrentShadingVersion != null)
			this._openGLShadingVersion = Gl.CurrentShadingVersion;
		Trace.TraceInformation($"{nameof(IrsGraphicEnvironment)}.{nameof(IrsGraphicEnvironment)}: OpenGL environment information:");
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

		this._renderWindow.Paint += this._renderWindow_Paint;
	}

	public void Dispose()
	{
		// Distruzione del render context
		if (this._renderWindowDeviceContext != null && this._openGLRenderContext != IntPtr.Zero)
		{
			this._renderWindowDeviceContext.DeleteContext(this._openGLRenderContext);
			this._openGLRenderContext = IntPtr.Zero;
		}

		// Distruzione del device context
		if (this._renderWindowDeviceContext != null)
		{
			this._renderWindowDeviceContext.DecRef();
			this._renderWindowDeviceContext = null;
		}
	}

	private void _renderWindow_Paint(Object? sender, PaintEventArgs e)
	{
		if (this._renderWindowDeviceContext == null || this._openGLRenderContext == IntPtr.Zero)
			return;

		// Make context current
		if (this._renderWindowDeviceContext.MakeCurrent(this._openGLRenderContext) == false)
			throw new InvalidOperationException("unable to make context current");

		Gl.ClearColor(0.239f, 0.239f, 0.239f, 1.000f);
		Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

		// Creazione del vertex buffer
		Single[] vertices = {
			 0.0f,  0.5f,
			-0.5f, -0.5f,
			+0.5f, -0.5f,
		};
		UInt32 bufferId = Gl.GenBuffer();
		Gl.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
		Gl.BufferData(BufferTarget.ArrayBuffer, (UInt32)(sizeof(Single) * vertices.Length), vertices, BufferUsage.StaticDraw);

		// Definizione degli attributi del vertice
		Gl.EnableVertexArrayAttrib(bufferId, 0);
		Gl.VertexAttribPointer(0, 2, VertexAttribType.Float, false, sizeof(Single) * 2, 0);

		// Rendering delle primitive
		Gl.DrawArrays(PrimitiveType.Triangles, 0, 3);


		//Gl.Viewport(0, 0, this._renderWindow.ClientSize.Width, this._renderWindow.ClientSize.Height);


		//Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
		//Gl.Begin(PrimitiveType.Triangles);
		//Gl.Color3(1.000f, 0.627f, 0.157f);
		//Gl.Vertex2(0.0f, 0.5f);
		//Gl.Color3(0.000f, 0.000f, 0.000f);
		//Gl.Vertex2(-0.5f, -0.5f);
		//Gl.Vertex2(+0.5f, -0.5f);
		//Gl.End();

		//Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Point);
		//Gl.PointSize(3.000f);
		//Gl.Begin(PrimitiveType.Points);
		//Gl.Color3(1.000f, 1.000f, 1.000f);
		//Gl.Vertex2(0.0f, 0.5f);
		//Gl.Color3(0.000f, 0.000f, 0.000f);
		//Gl.Vertex2(-0.5f, -0.5f);
		//Gl.Vertex2(+0.5f, -0.5f);
		//Gl.End();

		this._renderWindowDeviceContext.SwapBuffers();
	}
}
