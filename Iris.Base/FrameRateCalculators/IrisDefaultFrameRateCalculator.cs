using Iris.Base.Abstractions;
using System.Diagnostics;

namespace Iris.Base.FrameRateCalculators;

public class IrisDefaultFrameRateCalculator : IIrisFrameRateCalculator
{
	private Stopwatch _framesStopWatch = new Stopwatch();
	private Single _elapsedMilliseconds = 0.0f;
	private Single _framesPerSecond = 0.0f;

	public Boolean IsWatching { get { return this._framesStopWatch.IsRunning; } }

	public Single GetFramesPerSecond()
	{
		return this._framesPerSecond;
	}

	public Single GetFrameTimeMilliseconds()
	{
		return this._elapsedMilliseconds;
	}

	public void SignalFrameRendered()
	{
		if (!this._framesStopWatch.IsRunning)
			throw new Exception($"{nameof(IrisDefaultFrameRateCalculator)}.{nameof(SignalFrameRendered)}: Stopwatch is not running");

		this._elapsedMilliseconds = (this._framesStopWatch.ElapsedTicks * 1000.0f) / (Single)Stopwatch.Frequency;
		if (this._elapsedMilliseconds != 0)
			this._framesPerSecond = 1000.0f / this._elapsedMilliseconds;
		this._framesStopWatch.Restart();
	}

	public void StartWatching()
	{
		this._framesStopWatch.Restart();
		this._elapsedMilliseconds = 0;
		this._framesPerSecond = 0.0f;
	}

	public void StopWatching()
	{
		this._framesStopWatch.Stop();
	}
}
