using Iris.Graphics.Abstractions;
using System.Diagnostics;

namespace Iris.Graphics.FrameRate;

public class IrisAverageFrameRateCalculator : IIrisFrameRateCalculator
{
	private Stopwatch _framesStopWatch = new Stopwatch();
	private Single _elapsedSeconds = 0.0f;
	private Single _framesPerSecond = 0.0f;

	private UInt16 _measurementCount;
	private Single[]? _elapsedSecondsBuffer = null;
	private Single[]? _framesPerSecondBuffer = null;
	private UInt16 _measurementIndex = 0;

	public Boolean IsWatching { get { return this._framesStopWatch.IsRunning; } }

	public IrisAverageFrameRateCalculator(UInt16 measurementCount)
	{
		if (measurementCount <= 0)
			throw new Exception($"{nameof(IrisAverageFrameRateCalculator)}.{nameof(IrisAverageFrameRateCalculator)}: Measurement count must be greater than 0.");

		// Salvataggio delle informazioni
		this._measurementCount = measurementCount;

		// Inizializzazione della classe
		this._elapsedSecondsBuffer = new Single[this._measurementCount];
		this._framesPerSecondBuffer = new Single[this._measurementCount];
		for (UInt16 index = 0; index < this._measurementCount; index++)
		{
			this._elapsedSecondsBuffer[index] = 0.0f;
			this._framesPerSecondBuffer[index] = 0.0f;
		}
	}

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
		if (this._elapsedSecondsBuffer == null || this._framesPerSecondBuffer == null)
			return;

		this._elapsedSecondsBuffer[this._measurementIndex] = (Single)this._framesStopWatch.ElapsedMilliseconds / 1000.0f;
		this._framesPerSecondBuffer[this._measurementIndex] = 1.0f / this._elapsedSeconds;

		this._measurementIndex++;
		if (this._measurementIndex >= this._measurementCount)
			this._measurementIndex = 0;

		this._elapsedSeconds = (Single)(from s in this._elapsedSecondsBuffer select s).Sum() / (Single)this._measurementCount;
		if (this._elapsedSeconds != 0.0f)
			this._framesPerSecond = 1.0f / (Single)Math.Round(this._elapsedSeconds, 3);
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
