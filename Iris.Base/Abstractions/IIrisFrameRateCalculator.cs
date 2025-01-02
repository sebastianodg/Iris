namespace Iris.Base.Abstractions
{
	public interface IIrisFrameRateCalculator
	{
		Boolean IsWatching { get; }

		void SignalFrameRendered();
		void StartWatching();
		void StopWatching();
		Single GetFrameTimeMilliseconds();
		Single GetFramesPerSecond();
	}
}
