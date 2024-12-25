namespace Iris.Graphics.Abstractions;

public interface IIrisImageBuffer : IDisposable
{
	UInt16 BufferWidth { get; }
	UInt16 BufferHeight { get; }

	void ResizeBuffer(UInt16 bufferWidth, UInt16 bufferHeight);
	void Clear(Color color);
	void SetPixelColor(UInt16 pixelX, UInt16 pixelY, Color color);
	Color GetPixelColor(UInt16 pixelX, UInt16 pixelY);
	Bitmap GetImage();
}
