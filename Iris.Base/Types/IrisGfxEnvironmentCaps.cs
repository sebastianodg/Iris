using Khronos;

namespace Iris.Base.Types;

public class IrisGfxEnvironmentCaps
{
	public String AdapterVendor { get; set; }
	public String AdapterModel { get; set; }
	public KhronosVersion GraphicAPIVersion { get; set; }
	public KhronosVersion ShadersLanguageVersion { get; set; }
	public Byte AlphaBits { get; internal set; }
	public Byte DepthBits { get; internal set; }
	public Byte StencilBits { get; internal set; }
	public Boolean VerticalSync { get; set; }

	public IrisGfxEnvironmentCaps()
	{
		this.AdapterVendor = String.Empty;
		this.AdapterModel = String.Empty;
		this.GraphicAPIVersion = new KhronosVersion(0, 0, String.Empty);
		this.ShadersLanguageVersion = new KhronosVersion(0, 0, String.Empty);
		this.AlphaBits = 0;
		this.DepthBits = 0;
		this.StencilBits = 0;
		this.VerticalSync = false;
	}
}