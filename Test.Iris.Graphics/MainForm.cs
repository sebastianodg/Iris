using Iris.Graphics.Environment;
using Iris.Graphics.Environment.Initialization.Abstractions;

namespace Test.Iris.Graphics;

public partial class MainForm : Form
{
	private IrsEnvironmentConfig _requestedOpenGLConfig;
	private IIrsEnvironmentInitializer _environmentInitializer;

	public MainForm()
	{
		// Creazione delle opzioni di inizializzazione di OpenGL
		this._requestedOpenGLConfig = new IrsEnvironmentConfig()
		{
			RGBAUnsigned = true,
			RenderWindow = true,
			ColorBits = 32,
			DepthBits = 24,
			StencilBits = 8,
			MultisampleBits = 0,
			DoubleBuffer = true,
			VerticalSync = true,
		};

		this.InitializeComponent();
	}
}
