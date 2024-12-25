namespace Iris.Graphics.Environment;

/// <summary>
/// Classe contenente le informazioni sulla configurazione dell'ambiente di OpenGL
/// </summary>
public class IrsEnvironmentConfig
{
	/// <summary>
	/// Restituisce o imposta il flag che indica se il formato del buffer del colore è RGBA senza segno
	/// </summary>
	public Boolean RGBAUnsigned { get; set; }

	/// <summary>
	/// Restituisce o imposta il flag che indica se si effettua il rendering all'interno di una finestra
	/// </summary>
	public Boolean RenderWindow { get; set; }

	/// <summary>
	/// Restituisce o imposta il numero di bit per il buffer del colore
	/// </summary>
	public UInt16 ColorBits { get; set; }

	/// <summary>
	/// Restituisce o imposta il numero di bit per il buffer della profondità
	/// </summary>
	public UInt16 DepthBits { get; set; }

	/// <summary>
	/// Restituisce o imposta il numero di bit per il buffer dello stencil
	/// </summary>
	public UInt16 StencilBits { get; set; }

	/// <summary>
	/// Restituisce o imposta il numero di bit per il multisampling (anti aliasing)
	/// </summary>
	public UInt16 MultisampleBits { get; set; }

	/// <summary>
	/// Restituisce o imposta il flag che indica se si utilizza la tecnica del double duffering
	/// </summary>
	public Boolean DoubleBuffer { get; set; }

	/// <summary>
	/// Restituisce o imposta il flag che indica se si sincronizza lo swap dei buffer con la scansione verticale del monitor
	/// </summary>
	public Boolean VerticalSync { get; set; }

	/// <summary>
	/// Costruttore
	/// </summary>
	public IrsEnvironmentConfig()
	{
		this.RGBAUnsigned = true;
		this.RenderWindow = true;
		this.ColorBits = 32;
		this.DepthBits = 24;
		this.StencilBits = 8;
		this.MultisampleBits = 0;
		this.DoubleBuffer = true;
		this.VerticalSync = true;
	}
}
