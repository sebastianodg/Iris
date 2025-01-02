using Iris.Graphics.Environment.Initialization.Abstractions;
using Iris.Graphics.RenderWindow;
using Khronos;
using OpenGL;
using System.Diagnostics;

namespace Iris.Graphics.Environment.Initialization.Implementations;

/// <summary>
/// Classe che effettua l'inizializzazione dell'ambiente grafico di OpenGL
/// </summary>
public class IrsDefaultInitializer : IIrsEnvironmentInitializer
{
	private IrisRenderWindow? _renderWindow = null;
	private IrsEnvironmentConfig? _requestedConfig = null;

	private DeviceContext? _renderWindowDeviceContext = null;
	private DevicePixelFormatCollection? _availablePixelFormats = null;
	private List<DevicePixelFormat>? _matchingPixelFormats = null;
	private IntPtr _openGLRenderContext = IntPtr.Zero;

	private String? _currentVendor = null;
	private String? _currentRenderer = null;
	private KhronosVersion? _openGLCurrentVersion = null;
	private KhronosVersion? _openGLShadingVersion = null;

	private IrsEnvironmentConfig? _actualConfiguration = null;

	public System.Boolean Initialize(IrisRenderWindow renderWindow, IrsEnvironmentConfig requestedConfig)
	{
		if (renderWindow == null)
			throw new Exception($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: Reference to {nameof(IrisRenderWindow)} control cannot be null.");
		if (requestedConfig == null)
			throw new Exception($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: Reference to {nameof(IrsEnvironmentConfig)} cannot be null.");

		// Memorizzazione dei dati
		this._renderWindow = renderWindow;
		this._requestedConfig = requestedConfig;

		Trace.TraceInformation($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: Initializing OpenGL graphic environment...");
		Trace.TraceInformation($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: OpenGL requested pixel format.");
		Trace.Indent();
		Trace.WriteLine($"RgbaUnsigned:    {requestedConfig.RGBAUnsigned}");
		Trace.WriteLine($"RenderWindow:    {requestedConfig.RenderWindow}");
		Trace.WriteLine($"ColorBits:       {requestedConfig.ColorBits}");
		Trace.WriteLine($"DepthBits:       {requestedConfig.DepthBits}");
		Trace.WriteLine($"StencilBits:     {requestedConfig.StencilBits}");
		Trace.WriteLine($"MultisampleBits: {requestedConfig.MultisampleBits}");
		Trace.WriteLine($"DoubleBuffer:    {requestedConfig.DoubleBuffer}");
		Trace.WriteLine($"VerticalSync:    {requestedConfig.VerticalSync}");
		Trace.Unindent();

		// Creazione del device context della finestra
		this._renderWindowDeviceContext = DeviceContext.Create(IntPtr.Zero, this._renderWindow.Handle);
		this._renderWindowDeviceContext.IncRef();

		// Lettura dell'elenco dei pixel format disponibili
		this._availablePixelFormats = this._renderWindowDeviceContext.PixelsFormats;
		if (this._availablePixelFormats == null)
		{
			Trace.TraceError($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: Unable to get pixel formats list.");
			throw new Exception($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: Unable to get pixel formats list.");
		}
		if (this._availablePixelFormats.Count == 0)
		{
			Trace.TraceError($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: Unable to find a single available pixel format.");
			throw new Exception($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: Unable to find a single available pixel format.");
		}
		Trace.TraceInformation($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: Found {this._availablePixelFormats.Count} available OpenGL pixel formats.");

		// Impostazione della struttura dati contenente le informazioni sul pixel format richiesto
		DevicePixelFormat controlReqFormat = new DevicePixelFormat()
		{
			RgbaUnsigned = requestedConfig.RGBAUnsigned,
			RenderWindow = requestedConfig.RenderWindow,
			ColorBits = requestedConfig.ColorBits,
			DepthBits = requestedConfig.DepthBits,
			StencilBits = requestedConfig.StencilBits,
			MultisampleBits = requestedConfig.MultisampleBits,
			DoubleBuffer = requestedConfig.DoubleBuffer,
		};

		// Scelta del pixel format più adatto. Vengono selezionati diversi pixel format. Il primo dovrebbe essere quello più vicino alle caratteristiche richieste
		this._matchingPixelFormats = this._availablePixelFormats.Choose(controlReqFormat);
		if (this._matchingPixelFormats == null)
		{
			Trace.TraceError($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: Unable to get choosen pixel formats list.");
			throw new Exception($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: Unable to get choosen pixel formats list.");
		}
		if (this._matchingPixelFormats.Count == 0)
		{
			Trace.TraceError($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: Unable to find a single choosen pixel format.");
			throw new Exception($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: Unable to find a single choosen pixel format.");
		}
		Trace.TraceInformation($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: {this._matchingPixelFormats.Count} OpenGL pixel formats were choosen.");

		// Impostazione del primo pixel format per la finestra di output
		this._renderWindowDeviceContext.SetPixelFormat(this._matchingPixelFormats[0]);

		// Trasformazione del pixel format nella configurazione attuale
		IrsEnvironmentConfig atcualConfig = new IrsEnvironmentConfig()
		{
			RGBAUnsigned = this._matchingPixelFormats[0].RgbaUnsigned,
			RenderWindow = this._matchingPixelFormats[0].RenderWindow,
			ColorBits = (UInt16)this._matchingPixelFormats[0].ColorBits,
			DepthBits = (UInt16)this._matchingPixelFormats[0].DepthBits,
			StencilBits = (UInt16)this._matchingPixelFormats[0].StencilBits,
			MultisampleBits = (UInt16)this._matchingPixelFormats[0].MultisampleBits,
			DoubleBuffer = this._matchingPixelFormats[0].DoubleBuffer,
		};

		Trace.TraceInformation($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: OpenGL pixel format set.");
		Trace.Indent();
		Trace.WriteLine($"RgbaUnsigned:    {atcualConfig.RGBAUnsigned}");
		Trace.WriteLine($"RenderWindow:    {atcualConfig.RenderWindow}");
		Trace.WriteLine($"ColorBits:       {atcualConfig.ColorBits}");
		Trace.WriteLine($"DepthBits:       {atcualConfig.DepthBits}");
		Trace.WriteLine($"StencilBits:     {atcualConfig.StencilBits}");
		Trace.WriteLine($"MultisampleBits: {atcualConfig.MultisampleBits}");
		Trace.WriteLine($"DoubleBuffer:    {atcualConfig.DoubleBuffer}");
		Trace.WriteLine($"VerticalSync:    {atcualConfig.VerticalSync}");
		Trace.Unindent();

		// Creazione del render context
		this._openGLRenderContext = this._renderWindowDeviceContext.CreateContext(IntPtr.Zero);
		if (this._openGLRenderContext != IntPtr.Zero)
			Trace.TraceInformation($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: OpenGL render context successfully created.");
		else
			Trace.TraceError($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: Error creating OpenGL remder context.");

		// Recupero delle informazioni sull'ambiente di OpenGL
		this._currentVendor = Gl.CurrentVendor;
		this._currentRenderer = Gl.CurrentRenderer;
		if (Gl.CurrentVersion != null)
			this._openGLCurrentVersion = Gl.CurrentVersion;
		if (Gl.CurrentShadingVersion != null)
			this._openGLShadingVersion = Gl.CurrentShadingVersion;
		Trace.TraceInformation($"{nameof(IrsDefaultInitializer)}.{nameof(Initialize)}: OpenGL environment information:");
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

		return true;
	}

	public IrsEnvironmentConfig? GetActualConfiguration()
	{
		return this._actualConfiguration;
	}
}
