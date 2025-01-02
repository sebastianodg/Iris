using Iris.Base.Abstractions;
using Iris.Base.Types;
using OpenGL;
using System.Reflection;

namespace Iris.Graphics.RenderWindow;

/// <summary>
/// Controllo utilizzato come output del rendering
/// </summary>
public partial class IrisRenderWindow : UserControl
{
	private IrisRenderWndLogoPainter _logoPainter;

	private IrisGfxEnvironmentSettings _gfxEnvironmentSettings;
	private IIrisGfxEnvironmentCreator _gfxEnvironmentCreator;

	public event EventHandler<PaintEventArgs>? OnRenderFrame = null;

	public IrisRenderWindow()
	{
		// Impostazione dello stile della finestra. Necessario per OpenGL
		base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		base.SetStyle(ControlStyles.Opaque, true);
		base.SetStyle(ControlStyles.DoubleBuffer, false);
		base.SetStyle(ControlStyles.ResizeRedraw, true);
		base.SetStyle(ControlStyles.UserPaint, true);

		this._logoPainter = new IrisRenderWndLogoPainter(this);

		this.InitializeComponent();
		if (base.DesignMode)
			return;
	}

	public void CreateEnvironment(IrisGfxEnvironmentSettings gfxEnvironmentSettings, IIrisGfxEnvironmentCreator gfxEnvironmentCreator)
	{
		if (gfxEnvironmentSettings == null)
			throw new Exception($"{nameof(IrisRenderWindow)}.{nameof(CreateEnvironment)}: Graphic environment settings cannot be null.");
		if (gfxEnvironmentCreator == null)
			throw new Exception($"{nameof(IrisRenderWindow)}.{nameof(IrisRenderWindow)}: Graphic environment cannot be null.");

		// Salvataggio delle informazioni
		this._gfxEnvironmentSettings = gfxEnvironmentSettings;
		this._gfxEnvironmentCreator = gfxEnvironmentCreator;

		// Inizializzazione dell'ambiente grafico
		this._gfxEnvironmentCreator.Create();
	}


	protected override void OnPaint(PaintEventArgs args)
	{
		// Disegno degli elementi a design time
		this._logoPainter.OnPaint(args);
		if (!base.DesignMode)
			return;

		this.OnRenderFrame?.Invoke(this, args);
	}
}
