using Iris.Graphics.Abstractions;
using System.Diagnostics.Metrics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Iris.Graphics.Buffers;

public class Iris32BitsARGBColorBuffer : IIrisImageBuffer
{
	private UInt16 _bufferWidth = 0;
	private UInt16 _bufferHeight = 0;
	private Bitmap? _internalBitmap = null;
	private Byte[]? _internalBuffer = null;
	private Int32 _bitmapImageStride = 0;

	public UInt16 BufferWidth { get { return this._bufferWidth; } }

	public UInt16 BufferHeight { get { return this._bufferHeight; } }

	public Iris32BitsARGBColorBuffer(UInt16 bufferWidth, UInt16 bufferHeight)
	{
		if (bufferWidth <= 0)
			throw new Exception($"{nameof(Iris32BitsARGBColorBuffer)}.{nameof(Iris32BitsARGBColorBuffer)}: Buffer width must be greater than zero.");
		if (bufferHeight <= 0)
			throw new Exception($"{nameof(Iris32BitsARGBColorBuffer)}.{nameof(Iris32BitsARGBColorBuffer)}: Buffer height must be greater than zero.");

		this._bufferWidth = bufferWidth;
		this._bufferHeight = bufferHeight;

		this._internalBitmap = new Bitmap(bufferWidth, bufferHeight, PixelFormat.Format32bppArgb);
		BitmapData unlockedBitmapData = this._internalBitmap.LockBits(new Rectangle(0, 0, this._internalBitmap.Width, this._internalBitmap.Height), ImageLockMode.ReadWrite, this._internalBitmap.PixelFormat);
		IntPtr bitmapFirstRowPtr = unlockedBitmapData.Scan0;
		this._bitmapImageStride = unlockedBitmapData.Stride;
		Int32 bitmapBufferSize = Math.Abs(this._bitmapImageStride) * this._internalBitmap.Height;
		this._internalBuffer = new byte[bitmapBufferSize];
		this._internalBitmap.UnlockBits(unlockedBitmapData);
	}

	public void Clear(Color color)
	{
		if (this._internalBitmap == null || this._internalBuffer == null)
			throw new Exception($"{nameof(Iris32BitsARGBColorBuffer)}.{nameof(Clear)}: Buffer not initialized.");

		for (UInt32 counter = 0; counter < this._internalBuffer.Length; counter += 4)
		{
			this._internalBuffer[counter + 0] = color.B;
			this._internalBuffer[counter + 1] = color.G;
			this._internalBuffer[counter + 2] = color.R;
			this._internalBuffer[counter + 3] = color.A;
		}
	}

	public void Dispose()
	{
		this._bufferWidth = 0;
		this._bufferHeight = 0;
		this._internalBuffer = null;
		this._internalBitmap?.Dispose();
		this._bitmapImageStride = 0;
	}

	public Bitmap GetImage()
	{
		if (this._internalBitmap == null || this._internalBuffer == null)
			throw new Exception($"{nameof(Iris32BitsARGBColorBuffer)}.{nameof(Clear)}: Buffer not initialized.");

		BitmapData unlockedBitmapData = this._internalBitmap.LockBits(new Rectangle(0, 0, this._internalBitmap.Width, this._internalBitmap.Height), ImageLockMode.ReadWrite, this._internalBitmap.PixelFormat);
		IntPtr bitmapFirstRowPtr = unlockedBitmapData.Scan0;
		Marshal.Copy(this._internalBuffer, 0, bitmapFirstRowPtr, this._internalBuffer.Length);
		this._internalBitmap.UnlockBits(unlockedBitmapData);

		return this._internalBitmap;
	}

	public Color GetPixelColor(UInt16 pixelX, UInt16 pixelY)
	{
		throw new NotImplementedException();
	}

	public void ResizeBuffer(UInt16 bufferWidth, UInt16 bufferHeight)
	{
		if (this._internalBitmap == null || this._internalBuffer == null)
			throw new Exception($"{nameof(Iris32BitsARGBColorBuffer)}.{nameof(Clear)}: Buffer not initialized.");

		this._bufferWidth = 0;
		this._bufferHeight = 0;
		this._internalBuffer = null;
		this._internalBitmap?.Dispose();
		this._bitmapImageStride = 0;

		this._bufferWidth = bufferWidth;
		this._bufferHeight = bufferHeight;

		this._internalBitmap = new Bitmap(bufferWidth, bufferHeight, PixelFormat.Format32bppArgb);
		BitmapData unlockedBitmapData = this._internalBitmap.LockBits(new Rectangle(0, 0, this._internalBitmap.Width, this._internalBitmap.Height), ImageLockMode.ReadWrite, this._internalBitmap.PixelFormat);
		IntPtr bitmapFirstRowPtr = unlockedBitmapData.Scan0;
		this._bitmapImageStride = unlockedBitmapData.Stride;
		Int32 bitmapBufferSize = Math.Abs(this._bitmapImageStride) * this._internalBitmap.Height;
		this._internalBuffer = new byte[bitmapBufferSize];
		this._internalBitmap.UnlockBits(unlockedBitmapData);
	}

	public void SetPixelColor(UInt16 pixelX, UInt16 pixelY, Color color)
	{
		throw new NotImplementedException();
	}
}
