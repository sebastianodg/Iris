using Iris.Base.Abstractions;
using Iris.Base.GraphicEnvironment;
using Iris.Base.Types;
using OpenGL;

namespace Test.OpenGL.SandBox;

public partial class MainForm : Form
{
	private IrisGfxEnvironmentSettings _graphicsSettings;
	private IIrisGfxEnvironmentCreator _gfxEnvironment;
	private IrisGfxEnvironmentCaps _gfxEnvironmentCaps;

	public MainForm()
	{
		this.InitializeComponent();

		//try
		//{
		//	this._gfxEnvironment = new IrisGfxEnvCreatorOpenGL(this._graphicsSettings);
		//	this._gfxEnvironment.Create();
		//}
		//catch (Exception exc)
		//{
		//	MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		//}
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		//this._gfxEnvironmentCaps = this._gfxEnvironment.GetEnvironmentCaps();

		//Trace.TraceInformation($"Environment caps:");
		//Trace.Indent();
		//Trace.WriteLine($"AdapterVendor: {this._gfxEnvironmentCaps.AdapterVendor}");
		//Trace.WriteLine($"AdapterModel: {this._gfxEnvironmentCaps.AdapterModel}");
		//Trace.WriteLine($"GraphicAPIVersion: {this._gfxEnvironmentCaps.GraphicAPIVersion}");
		//Trace.WriteLine($"ShadersLanguageVersion: {this._gfxEnvironmentCaps.ShadersLanguageVersion}");
		//Trace.WriteLine($"AlphaBits: {this._gfxEnvironmentCaps.AlphaBits}");
		//Trace.WriteLine($"DepthBits: {this._gfxEnvironmentCaps.DepthBits}");
		//Trace.WriteLine($"StencilBits: {this._gfxEnvironmentCaps.StencilBits}");
		//Trace.WriteLine($"VerticalSync: {this._gfxEnvironmentCaps.VerticalSync}");
		//Trace.Unindent();

		//this._irisRenderWindow.OnRenderFrame += this.OnRenderFrame;
	}

	private void OnRenderFrame(Object? sender, PaintEventArgs e)
	{
		Random rand = new Random((Int32)DateTime.Now.Ticks);

		Single redComp = rand.NextSingle();
		Single greenComp = rand.NextSingle();
		Single blueComp = rand.NextSingle();

		Gl.Viewport(0, 0, this._irisRenderWindow.ClientSize.Width, this._irisRenderWindow.ClientSize.Height);

		Gl.ClearColor(0.239f, 0.239f, 0.239f, 1.000f);
		Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

		Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
		Gl.Begin(PrimitiveType.Triangles);
		Gl.Color3(redComp, greenComp, blueComp);
		Gl.Vertex2(0.0f, 0.5f);
		Gl.Vertex2(-0.5f, -0.5f);
		Gl.Vertex2(+0.5f, -0.5f);
		Gl.End();

		Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Point);
		Gl.PointSize(3.000f);
		Gl.Begin(PrimitiveType.Points);
		Gl.Color3(redComp, greenComp, blueComp);
		Gl.Vertex2(0.0f, 0.5f);
		Gl.Vertex2(-0.5f, -0.5f);
		Gl.Vertex2(+0.5f, -0.5f);
		Gl.End();

		this._gfxEnvironment.PresentBackBuffer();
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (e.KeyCode == Keys.Escape)
			Application.Exit();
	}
}
