using Iris.Base.Abstractions;
using Iris.Base.Types;
using OpenGL;
using System.Diagnostics;

namespace Iris.Base.GraphicEnvironment;

public class IrisGfxEnvCreatorOpenGL : IIrisGfxEnvironmentCreator
{
	private readonly IrisGfxEnvironmentSettings _gfxEnvironmentSettings;

	private DeviceContext? _openGLDeviceContext = null;
	private DevicePixelFormatCollection? _availablePixelFormats = null;
	private List<DevicePixelFormat>? _matchingPixelFormats = null;
	private IntPtr _openGLRenderContext = IntPtr.Zero;

	public IrisGfxEnvironmentSettings GfxEnvironmentSettings {  get { return this._gfxEnvironmentSettings; } }

	public IrisGfxEnvCreatorOpenGL(IrisGfxEnvironmentSettings gfxEnvironmentSettings)
	{
		if (gfxEnvironmentSettings == null)
			throw new Exception($"{nameof(IrisGfxEnvCreatorOpenGL)}.{nameof(IrisGfxEnvCreatorOpenGL)}: Graphic environment settings cannot be null.");

		// Salvataggio delle informazioni
		this._gfxEnvironmentSettings = gfxEnvironmentSettings;
	}

	public void Create()
	{
		if (this._gfxEnvironmentSettings.WindowHandle == IntPtr.Zero)
			throw new Exception($"{nameof(IrisGfxEnvCreatorOpenGL)}.{nameof(IrisGfxEnvCreatorOpenGL)}: Invalid render window handle.");

		// Creazione del device context della finestra
		this._openGLDeviceContext = DeviceContext.Create(IntPtr.Zero, this._gfxEnvironmentSettings.WindowHandle);
		this._openGLDeviceContext.IncRef();

		// Lettura dell'elenco dei pixel format disponibili
		this._availablePixelFormats = this._openGLDeviceContext.PixelsFormats;
		if (this._availablePixelFormats == null)
			throw new Exception($"{nameof(IrisGfxEnvCreatorOpenGL)}.{nameof(Create)}: Unable to get pixel formats list.");
		if (this._availablePixelFormats.Count == 0)
			throw new Exception($"{nameof(IrisGfxEnvCreatorOpenGL)}.{nameof(Create)}: Unable to find a single available pixel format.");
		Trace.TraceInformation($"{nameof(IrisGfxEnvCreatorOpenGL)}.{nameof(Create)}: Found {this._availablePixelFormats.Count} available OpenGL pixel formats.");

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
			throw new Exception($"{nameof(IrisGfxEnvCreatorOpenGL)}.{nameof(Create)}: Unable to get choosen pixel formats list.");
		if (this._matchingPixelFormats.Count == 0)
			throw new Exception($"{nameof(IrisGfxEnvCreatorOpenGL)}.{nameof(Create)}: Unable to find a single choosen pixel format.");
		Trace.TraceInformation($"{nameof(IrisGfxEnvCreatorOpenGL)}.{nameof(Create)}: {this._matchingPixelFormats.Count} OpenGL pixel formats were choosen.");

		// Impostazione del pixel format per la finestra di output
		this._openGLDeviceContext.SetPixelFormat(this._matchingPixelFormats[0]);
		Trace.TraceInformation($"{nameof(IrisGfxEnvCreatorOpenGL)}.{nameof(Create)}: OpenGL pixel format set: {this._matchingPixelFormats[0]}.");

		// Creazione del render context
		this._openGLRenderContext = this._openGLDeviceContext.CreateContext(IntPtr.Zero);
		if (this._openGLRenderContext != IntPtr.Zero)
			Trace.TraceInformation($"{nameof(IrisGfxEnvCreatorOpenGL)}.{nameof(Create)}: OpenGL render context successfully created.");
		else
			throw new Exception($"{nameof(IrisGfxEnvCreatorOpenGL)}.{nameof(Create)}: Error creating OpenGL render context.");

		// Impostazione del vertical sync
		if (!this._gfxEnvironmentSettings.VerticalSync)
		{
			System.Boolean verticalSyncSet = Wgl.SwapIntervalEXT(this._gfxEnvironmentSettings.VerticalSync ? 1 : 0);
			if (!verticalSyncSet)
				throw new Exception($"{nameof(IrisGfxEnvCreatorOpenGL)}.{nameof(Create)}: Unable to set vertical sync.");
		}
	}

	public IrisGfxEnvironmentCaps GetEnvironmentCaps()
	{
		Gl.GetInteger<Int32>(GetPName.AlphaBits, out Int32 alphaBits);
		Gl.GetInteger<Int32>(GetPName.DepthBits, out Int32 depthBits);
		Gl.GetInteger<Int32>(GetPName.StencilBits, out Int32 stencilBits);

		IrisGfxEnvironmentCaps gfxEnvCaps = new()
		{
			AdapterVendor = Gl.CurrentVendor,
			AdapterModel = Gl.CurrentRenderer,
			GraphicAPIVersion = Gl.CurrentVersion,
			ShadersLanguageVersion = Gl.CurrentShadingVersion,
			AlphaBits = (Byte)alphaBits,
			DepthBits = (Byte)depthBits,
			StencilBits = (Byte)stencilBits,
			VerticalSync = Wgl.GetSwapIntervalEXT() != 0,
		};

		return gfxEnvCaps;
	}

	public void Dispose()
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
	}

	public void PresentBackBuffer()
	{
		this._openGLDeviceContext?.SwapBuffers();
	}
}
