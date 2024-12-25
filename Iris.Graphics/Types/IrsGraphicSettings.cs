namespace Iris.Graphics.Types;

/// <summary>
/// Classe contenente le informazioni sulla configurazione dell'ambiente di OpenGL
/// </summary>
public class IrsGraphicSettings
{
	/// <summary>
	/// Restituisce o imposta il numero di bit per il buffer del colore
	/// </summary>
	public IrsBufferBitsCount ColorBufferDepth { get; set; }

	/// <summary>
	/// Restituisce o imposta il numero di bit per il buffer della profondità
	/// </summary>
	public IrsBufferBitsCount DepthBufferDepth { get; set; }

	/// <summary>
	/// Restituisce o imposta il numero di bit per il buffer dello stencil
	/// </summary>
	public IrsBufferBitsCount StencilBufferDepth { get; set; }

	/// <summary>
	/// Restituisce o imposta il numero di bit per il multisampling (anti aliasing)
	/// </summary>
	public IrsAntiAliasingLevel AntiAliasingLevel { get; set; }

	/// <summary>
	/// Restituisce o imposta il flag che indica se si utilizza la tecnica del double duffering
	/// </summary>
	public Boolean DoubleBuffer { get; set; }

	/// <summary>
	/// Restituisce o imposta il flag che indica se si sincronizza lo swap dei buffer con la scansione verticale del monitor
	/// </summary>
	public Boolean VerticalSync { get; set; }

	/// <summary>
	/// Restituisce o imposta il flag che indica se il formato del buffer del colore è RGBA senza segno
	/// </summary>
	public Boolean RGBAUnsigned { get; set; }

	/// <summary>
	/// Restituisce o imposta il flag che indica se si effettua il rendering all'interno di una finestra
	/// </summary>
	public Boolean RenderWindow { get; set; }

	/// <summary>
	/// Costruttore
	/// </summary>
	public IrsGraphicSettings()
	{
		ColorBufferDepth = IrsBufferBitsCount.Bits32;
		DepthBufferDepth = IrsBufferBitsCount.Bits24;
		StencilBufferDepth = IrsBufferBitsCount.Bits8;
		AntiAliasingLevel = IrsAntiAliasingLevel.Off;
		DoubleBuffer = true;
		VerticalSync = true;
		RGBAUnsigned = true;
		RenderWindow = true;
	}
}
