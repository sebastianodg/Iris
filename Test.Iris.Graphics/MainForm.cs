using Iris.Graphics.Environment;
using Iris.Graphics.Environment.Initialization.Abstractions;
using Iris.Graphics.Environment.Initialization.Implementations;

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
			RgbaUnsigned = true,
			RenderWindow = true,
			ColorBits = 32,
			DepthBits = 24,
			StencilBits = 8,
			MultisampleBits = 0,
			DoubleBuffer = true,
			VerticalSync = true,
		};

		this.InitializeComponent();

		//// Creazione dell'inizializzatore di OpenGL e inizializzazione di OpenGL
		//this._environmentInitializer = new IrsDefaultInitializer();
		//if (!this._environmentInitializer.Initialize(this._irisRenderWindow, this._requestedOpenGLConfig))
		//{
		//	MessageBox.Show("Error initializing OpenGL environment!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		//	Application.Exit();
		//	return;
		//}
	}
}
