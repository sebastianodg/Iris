using Iris.Graphics.Abstractions;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Iris.Graphics.Buffers;

public class Iris32BitsARGBColorBuffer2 : IIrisImageBuffer
{
	private UInt16 _bufferWidth = 0;
	private UInt16 _bufferHeight = 0;
	private Bitmap? _internalBitmap = null;
	private System.Drawing.Graphics? _gfx = null;

	public UInt16 BufferWidth { get { return this._bufferWidth; } }

	public UInt16 BufferHeight { get { return this._bufferHeight; } }

	public Iris32BitsARGBColorBuffer2(UInt16 bufferWidth, UInt16 bufferHeight)
	{
		if (bufferWidth <= 0)
			throw new Exception($"{nameof(Iris32BitsARGBColorBuffer2)}.{nameof(Iris32BitsARGBColorBuffer2)}: Buffer width must be greater than zero.");
		if (bufferHeight <= 0)
			throw new Exception($"{nameof(Iris32BitsARGBColorBuffer2)}.{nameof(Iris32BitsARGBColorBuffer2)}: Buffer height must be greater than zero.");

		this._bufferWidth = bufferWidth;
		this._bufferHeight = bufferHeight;

		this._internalBitmap = new Bitmap(bufferWidth, bufferHeight, PixelFormat.Format32bppArgb);
		this._gfx = System.Drawing.Graphics.FromImage(this._internalBitmap);
	}

	public void Clear(Color color)
	{
		if (this._internalBitmap == null || this._gfx == null)
			throw new Exception($"{nameof(Iris32BitsARGBColorBuffer2)}.{nameof(Clear)}: Buffer not initialized.");

		this._gfx.Clear(color);
	}

	public void Dispose()
	{
		this._bufferWidth = 0;
		this._bufferHeight = 0;
		this._gfx?.Dispose();
		this._internalBitmap?.Dispose();
	}

	public Bitmap GetImage()
	{
		if (this._internalBitmap == null)
			throw new Exception($"{nameof(Iris32BitsARGBColorBuffer2)}.{nameof(Clear)}: Buffer not initialized.");

		return this._internalBitmap;
	}

	public Color GetPixelColor(UInt16 pixelX, UInt16 pixelY)
	{
		if (this._internalBitmap == null || this._gfx == null)
			throw new Exception($"{nameof(Iris32BitsARGBColorBuffer2)}.{nameof(GetPixelColor)}: Buffer not initialized.");

		return this._internalBitmap.GetPixel(pixelX, pixelY);
	}

	public void ResizeBuffer(UInt16 bufferWidth, UInt16 bufferHeight)
	{
		if (this._internalBitmap == null)
			throw new Exception($"{nameof(Iris32BitsARGBColorBuffer2)}.{nameof(Clear)}: Buffer not initialized.");

		this._bufferWidth = 0;
		this._bufferHeight = 0;
		this._gfx?.Dispose();
		this._internalBitmap?.Dispose();

		this._bufferWidth = bufferWidth;
		this._bufferHeight = bufferHeight;

		this._internalBitmap = new Bitmap(bufferWidth, bufferHeight, PixelFormat.Format32bppArgb);
		this._gfx = System.Drawing.Graphics.FromImage(this._internalBitmap);
	}

	public void SetPixelColor(UInt16 pixelX, UInt16 pixelY, Color color)
	{
		if (this._internalBitmap == null || this._gfx == null)
			throw new Exception($"{nameof(Iris32BitsARGBColorBuffer2)}.{nameof(SetPixelColor)}: Buffer not initialized.");

		this._internalBitmap.SetPixel(pixelX, pixelY, color);
	}
}
