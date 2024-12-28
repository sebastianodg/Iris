namespace Iris.Base.Types;

public class IrisEnvironmentInfo
{
	/// <summary>
	/// Restituisce o imposta il produttore della GPU
	/// </summary>
	public String GPUManufacturer { get; set; }

	/// <summary>
	/// Restituisce o imposta il modello della GPU
	/// </summary>
	public String GPUModel { get; set; }

	/// <summary>
	/// Restituisce o imposta il numero di bit per il buffer del colore
	/// </summary>
	public IrisBufferBitsCount ColorBufferBits { get; set; }

	/// <summary>
	/// Restituisce o imposta il numero di bit per il buffer della profondità
	/// </summary>
	public IrisBufferBitsCount DepthBufferBits { get; set; }

	/// <summary>
	/// Restituisce o imposta il numero di bit per il buffer dello stencil
	/// </summary>
	public IrisBufferBitsCount StencilBufferBits { get; set; }

	/// <summary>
	/// Restituisce o imposta il numero di bit per il multisampling (anti aliasing)
	/// </summary>
	public IrisAntiAliasingLevel AntiAliasingLevel { get; set; }

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
	public IrisEnvironmentInfo()
	{
		this.GPUManufacturer = String.Empty;
		this.GPUModel = String.Empty;
		this.ColorBufferBits = IrisBufferBitsCount.Bits32;
		this.DepthBufferBits = IrisBufferBitsCount.Bits24;
		this.StencilBufferBits = IrisBufferBitsCount.Bits8;
		this.AntiAliasingLevel = IrisAntiAliasingLevel.Off;
		this.DoubleBuffer = true;
		this.VerticalSync = true;
		this.RGBAUnsigned = true;
		this.RenderWindow = true;
	}
}
