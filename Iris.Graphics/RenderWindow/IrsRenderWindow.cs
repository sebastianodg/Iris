using Iris.Graphics.Environment;
using Iris.Graphics.Types;
using OpenGL;
using System.ComponentModel;
using System.Reflection;

namespace Iris.Graphics.RenderWindow;

/// <summary>
/// Controllo utilizzato come output del rendering
/// </summary>
public partial class IrsRenderWindow : UserControl
{
	private readonly Color _irisLogoBackgroundColor = Color.FromArgb(44, 44, 44);
	private readonly Int16 _contentsRectangleBorder = 50;
	private readonly Int16 _informationBorder = 10;

	private Bitmap? _irisLogoBitmap = null;
	private Font? _irisInfoFont = null;
	private Brush? _irisInfoBrush = null;

	private IrsGraphicEnvironment _graphicEnvironment;

	[Browsable(true)]
	[Category("Graphic Environment")]
	[Description("The graphic API to use")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IrsGraphicAPI GraphicAPI { get; set; } = IrsGraphicAPI.OpenGL;

	[Browsable(true)]
	[Category("Graphic Environment")]
	[Description("The requested bit depth for the color buffer")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IrsBufferBitsCount ColorBufferDepth { get; set; } = IrsBufferBitsCount.Bits32;

	[Browsable(true)]
	[Category("Graphic Environment")]
	[Description("The requested bit depth for the depth buffer")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IrsBufferBitsCount DepthBufferDepth { get; set; } = IrsBufferBitsCount.Bits24;

	[Browsable(true)]
	[Category("Graphic Environment")]
	[Description("The requested bit depth for the stencil buffer")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IrsBufferBitsCount StencilBufferDepth { get; set; } = IrsBufferBitsCount.Bits8;

	[Browsable(true)]
	[Category("Graphic Environment")]
	[Description("The requested anti aliasing level")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IrsAntiAliasingLevel AntiAliasingLevel { get; set; } = IrsAntiAliasingLevel.Off;

	[Browsable(true)]
	[Category("Graphic Environment")]
	[Description("Enable or disable double buffering")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public System.Boolean DoubleBuffering { get; set; } = true;

	[Browsable(true)]
	[Category("Graphic Environment")]
	[Description("Enable or disable vertical sync")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public System.Boolean VerticalSync { get; set; } = true;
	public IrsRenderWindow()
	{
		// Impostazione dello stile della finestra. Necessario per OpenGL
		base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		base.SetStyle(ControlStyles.Opaque, true);
		base.SetStyle(ControlStyles.DoubleBuffer, false);
		base.SetStyle(ControlStyles.ResizeRedraw, true);
		base.SetStyle(ControlStyles.UserPaint, true);


		// Caricamento delle risorse
		Assembly irisAssembly = Assembly.GetExecutingAssembly();
		Stream? irisLogoStream = irisAssembly.GetManifestResourceStream("Iris.Graphics.Resources.iris-logo.jpg");
		if (irisLogoStream != null)
			this._irisLogoBitmap = new Bitmap(irisLogoStream);

		// Creazione dei font e di altre risorse
		this._irisInfoFont = new Font("Segoe UI", 10, FontStyle.Regular);
		this._irisInfoBrush = new SolidBrush(Color.FromArgb(255, 255, 255));

		this.InitializeComponent();
		if (base.DesignMode)
			return;

		//// Inizializzazione del creatore del device context
		//this._wndDCCreator = new IrsDefaultWndDCCreator(this);

		//// Creazione del device context per la finestra di output del rendering
		//IrsGraphicSettings requestedWndDCSettings = new IrsGraphicSettings()
		//{
		//	ColorBufferDepth = this.ColorBufferDepth,
		//	DepthBufferDepth = this.DepthBufferDepth,
		//	StencilBufferDepth = this.StencilBufferDepth,
		//	AntiAliasingLevel = this.AntiAliasingLevel,
		//	DoubleBuffer = this.DoubleBuffered,
		//	VerticalSync = this.VerticalSync,
		//	RGBAUnsigned = true,
		//	RenderWindow = true,
		//};
		//this._wndDCCreator.CreateDeviceContext(requestedWndDCSettings);






		//if (!base.DesignMode)
		//	this._graphicEnvironment = new IrsGraphicEnvironment(this);
	}

	protected override void OnPaint(PaintEventArgs args)
	{
		// Disegno degli elementi a design time
		if (base.DesignMode)
		{
			//base.OnPaint(args);
			this.DrawControlBackground(args);
			return;
		}
	}

	/// <summary>
	/// Disegna lo sfondo del controllo a design time
	/// </summary>
	/// <param name="args">Oggetto contenete le informazioni per il disegno</param>
	private void DrawControlBackground(PaintEventArgs args)
	{
		// Disegno dello sfondo
		args.Graphics.SetClip(base.ClientRectangle);
		args.Graphics.Clear(this._irisLogoBackgroundColor);

		// Calcolo del rettangolo utile per il disegno del contenuto
		Rectangle contentsRectangle = new Rectangle()
		{
			X = this._contentsRectangleBorder,
			Y = this._contentsRectangleBorder,
			Width = base.ClientSize.Width - (this._contentsRectangleBorder * 2),
			Height = base.ClientSize.Height - (this._contentsRectangleBorder * 2),
		};

		// Disegno del logo. Se il controllo è troppo piccolo per contenerlo, non viene disegnato
		if (this._irisLogoBitmap != null && contentsRectangle.Width > 0 && contentsRectangle.Height > 0)
		{
			// Calcolo dei rapporti di aspetto per l'immagine del logo e per l'area client del controllo
			Single logoAspectRatio = (Single)this._irisLogoBitmap.Width / (Single)this._irisLogoBitmap.Height;
			Single contentsAspectRatio = (Single)contentsRectangle.Width / (Single)contentsRectangle.Height;

			// Calcolo della posizione e delle dimensioni del rettangolo che conterrà l'immagine del logo
			Int32 logoRectX = 0;
			Int32 logoRectY = 0;
			Int32 logoRectWidth = 0;
			Int32 logoRectHeight = 0;
			if (logoAspectRatio <= contentsAspectRatio)
			{
				logoRectHeight = contentsRectangle.Height;
				logoRectWidth = Convert.ToInt32(contentsRectangle.Height * logoAspectRatio);
				logoRectX = (base.ClientRectangle.Width - logoRectWidth) / 2;
				logoRectY = contentsRectangle.Y;
			}
			else
			{
				logoRectWidth = contentsRectangle.Width;
				logoRectHeight = Convert.ToInt32(contentsRectangle.Width / logoAspectRatio);
				logoRectX = contentsRectangle.X;
				logoRectY = (base.ClientRectangle.Height - logoRectHeight) / 2;
			}
			Rectangle logoRectangle = new Rectangle()
			{
				X = logoRectX,
				Y = logoRectY,
				Width = logoRectWidth,
				Height = logoRectHeight,
			};

			// Disegno del logo
			args.Graphics.DrawImage(this._irisLogoBitmap, logoRectangle);
		}

		// Creazione delle informazioni relative all'ambiente grafico
		String graphicEnvironmentInfo = String.Empty;
		graphicEnvironmentInfo += $"Adapter: {Gl.CurrentVendor} - {Gl.CurrentRenderer}";
		graphicEnvironmentInfo += $"\nOpenGL: {Gl.CurrentVersion.Major}.{Gl.CurrentVersion.Minor}.{Gl.CurrentVersion.Revision}";
		graphicEnvironmentInfo += $" - Shading: {Gl.CurrentShadingVersion.Major}.{Gl.CurrentShadingVersion.Minor}.{Gl.CurrentShadingVersion.Revision}";

		// Disegno delle informazioni sull'ambiente grafico
		if (this._irisInfoFont != null && this._irisInfoBrush != null)
		{
		SizeF textSize = args.Graphics.MeasureString(graphicEnvironmentInfo, this._irisInfoFont);
		args.Graphics.DrawString(graphicEnvironmentInfo, this._irisInfoFont, this._irisInfoBrush, this._informationBorder, this.ClientSize.Height - textSize.Height - this._informationBorder);
		}
	}
}
