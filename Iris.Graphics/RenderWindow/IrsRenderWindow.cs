using Iris.Graphics.Environment;
using System.Reflection;

namespace Iris.Graphics.RenderWindow;

/// <summary>
/// Controllo utilizzato come output del rendering
/// </summary>
public partial class IrsRenderWindow : UserControl
{
	private readonly Color _backgroundColor = Color.FromArgb(44, 44, 44);
	private readonly Int16 _contentsRectangleBorder = 50;

	private Bitmap? _irisLogoBitmap = null;
	private IrsGraphicEnvironment _graphicEnvironment;

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

		this.InitializeComponent();

		this._graphicEnvironment = new IrsGraphicEnvironment(this);
	}

	protected override void OnPaint(PaintEventArgs args)
	{
		base.OnPaint(args);

		// Disegno dello sfondo a design time
		if (base.DesignMode)
			this.DrawControlBackground(args);
	}

	/// <summary>
	/// Disegna lo sfondo del controllo a design time
	/// </summary>
	/// <param name="args">Oggetto contenete le informazioni per il disegno</param>
	private void DrawControlBackground(PaintEventArgs args)
	{
		// Disegno dello sfondo
		args.Graphics.SetClip(base.ClientRectangle);
		args.Graphics.Clear(this._backgroundColor);

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
	}
}
