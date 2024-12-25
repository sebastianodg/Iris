namespace Iris.Graphics.Abstractions
{
	public interface IIrisFrameRateCalculator
	{
		Boolean IsWatching { get; }

		void SignalFrameRendered();
		void StartWatching();
		void StopWatching();
		Single GetFrameTime();
		Single GetFramesPerSecond();
	}
}
