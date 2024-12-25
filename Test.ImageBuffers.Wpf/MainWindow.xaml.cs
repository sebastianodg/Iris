using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Test.ImageBuffers.Wpf;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		this.InitializeComponent();

		this.Loaded += this.OnMainWindowLoaded;
		this._image.Loaded += this.OnImageLoaded;
	}

	private void OnImageLoaded(Object sender, RoutedEventArgs e)
	{
		// Define parameters used to create the BitmapSource.
		PixelFormat pf = PixelFormats.Bgr32;
		int width = (Int32)Math.Round(this._image.ActualWidth);
		int height = (Int32)Math.Round(this._image.ActualHeight);

		width = 512;
		height = 512;

		int rawStride = (width * pf.BitsPerPixel + 7) / 8;
		byte[] rawImage = new byte[rawStride * height];

		// Initialize the image with data.
		Random value = new Random();
		value.NextBytes(rawImage);

		// Create a BitmapSource.
		BitmapSource bitmap = BitmapSource.Create(width, height, 96, 96, pf, null, rawImage, rawStride);

		//// Create an image element;
		//Image myImage = new Image();
		//myImage.Width = 512;
		//// Set image source.
		//myImage.Source = bitmap;

		this._image.Source = bitmap;

		width = (Int32)Math.Round(this._image.ActualWidth);
		height = (Int32)Math.Round(this._image.ActualHeight);
	}

	private void OnMainWindowLoaded(Object sender, RoutedEventArgs e)
	{
	}
}