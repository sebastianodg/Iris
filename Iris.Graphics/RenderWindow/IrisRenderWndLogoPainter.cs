using OpenGL;
using System.Reflection;

namespace Iris.Graphics.RenderWindow;

internal class IrisRenderWndLogoPainter
{
	private readonly Color _irisLogoBackgroundColor = Color.FromArgb(44, 44, 44);
	private readonly Int16 _contentsRectangleBorder = 50;
	private readonly Int16 _informationBorder = 10;

	private readonly Bitmap? _irisLogoBitmap = null;
	private readonly Font? _irisInfoFont = null;
	private readonly Brush? _irisInfoBrush = null;

	private Control _outputControl;

	public IrisRenderWndLogoPainter(Control outputControl)
	{
		if (outputControl == null)
			throw new Exception($"{nameof(IrisRenderWndLogoPainter)}.{nameof(IrisRenderWndLogoPainter)}: Output control cannot be null.");

		// Caricamento delle risorse
		Assembly irisAssembly = Assembly.GetExecutingAssembly();
		Stream? irisLogoStream = irisAssembly.GetManifestResourceStream("Iris.Graphics.Resources.iris-logo.jpg");
		if (irisLogoStream != null)
			this._irisLogoBitmap = new Bitmap(irisLogoStream);

		// Creazione dei font e di altre risorse
		this._irisInfoFont = new Font("Segoe UI", 10, FontStyle.Regular);
		this._irisInfoBrush = new SolidBrush(Color.FromArgb(255, 255, 255));

		// Salvataggio delle informazioni
		this._outputControl = outputControl;
	}

	/// <summary>
	/// Disegna lo sfondo del controllo a design time
	/// </summary>
	/// <param name="args">Oggetto contenete le informazioni per il disegno</param>
	public void OnPaint(PaintEventArgs args)
	{
		// Disegno dello sfondo
		args.Graphics.SetClip(this._outputControl.ClientRectangle);
		args.Graphics.Clear(this._irisLogoBackgroundColor);

		// Calcolo del rettangolo utile per il disegno del contenuto
		Rectangle contentsRectangle = new()
		{
			X = this._contentsRectangleBorder,
			Y = this._contentsRectangleBorder,
			Width = this._outputControl.ClientRectangle.Width - (this._contentsRectangleBorder * 2),
			Height = this._outputControl.ClientRectangle.Height - (this._contentsRectangleBorder * 2),
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
				logoRectX = (this._outputControl.ClientRectangle.Width - logoRectWidth) / 2;
				logoRectY = contentsRectangle.Y;
			}
			else
			{
				logoRectWidth = contentsRectangle.Width;
				logoRectHeight = Convert.ToInt32(contentsRectangle.Width / logoAspectRatio);
				logoRectX = contentsRectangle.X;
				logoRectY = (this._outputControl.ClientRectangle.Height - logoRectHeight) / 2;
			}
			Rectangle logoRectangle = new()
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
			args.Graphics.DrawString(graphicEnvironmentInfo, this._irisInfoFont, this._irisInfoBrush, this._informationBorder, this._outputControl.ClientRectangle.Height - textSize.Height - this._informationBorder);
		}
	}
}
