using Iris.Graphics.Abstractions;
using System.Diagnostics;

namespace Iris.Graphics.FrameRate;

public class IrisDefaultFrameRateCalculator : IIrisFrameRateCalculator
{
	private Stopwatch _framesStopWatch = new Stopwatch();
	private Single _elapsedSeconds = 0.0f;
	private Single _framesPerSecond = 0.0f;

	public Boolean IsWatching { get { return this._framesStopWatch.IsRunning; } }

	public Single GetFramesPerSecond()
	{
		return this._framesPerSecond;
	}

	public Single GetFrameTime()
	{
		return this._elapsedSeconds;
	}

	public void SignalFrameRendered()
	{
		if (!this._framesStopWatch.IsRunning)
			throw new Exception($"{nameof(IrisDefaultFrameRateCalculator)}.{nameof(SignalFrameRendered)}: Stopwatch in not running");

		this._elapsedSeconds = (Single)this._framesStopWatch.ElapsedMilliseconds / 1000.0f;
		if (this._elapsedSeconds != 0.0f)
			this._framesPerSecond = 1.0f / this._elapsedSeconds;
		this._framesStopWatch.Restart();
	}

	public void StartWatching()
	{
		this._framesStopWatch.Restart();
		this._elapsedSeconds = 0.0f;
		this._framesPerSecond = 0.0f;
	}

	public void StopWatching()
	{
		this._framesStopWatch.Stop();
	}
}
