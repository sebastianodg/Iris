using Iris.Base.Abstractions;
using System.Diagnostics;

namespace Iris.Base.FrameRateCalculators;

public class IrisAverageFrameRateCalculator : IIrisFrameRateCalculator
{
	private Stopwatch _framesStopWatch = new Stopwatch();
	private Single _elapsedMilliseconds = 0.0f;
	private Single _framesPerSecond = 0.0f;

	private UInt16 _measurementCount;
	private Single[]? _elapsedMillisecondsBuffer = null;
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
		this._elapsedMillisecondsBuffer = new Single[this._measurementCount];
		this._framesPerSecondBuffer = new Single[this._measurementCount];
		for (UInt16 index = 0; index < this._measurementCount; index++)
		{
			this._elapsedMillisecondsBuffer[index] = 0.0f;
			this._framesPerSecondBuffer[index] = 0.0f;
		}
	}

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
			throw new Exception($"{nameof(IrisAverageFrameRateCalculator)}.{nameof(SignalFrameRendered)}: Stopwatch is not running");
		if (this._elapsedMillisecondsBuffer == null || this._framesPerSecondBuffer == null)
			return;

		this._elapsedMillisecondsBuffer[this._measurementIndex] = (this._framesStopWatch.ElapsedTicks * 1000.0f) / (Single)Stopwatch.Frequency;
		this._framesPerSecondBuffer[this._measurementIndex] = 1000.0f / this._elapsedMilliseconds;

		Single elapsedMillisecondsSum = 0.0f;
		for (UInt16 measurementIndex = 0; measurementIndex < this._measurementCount; measurementIndex++)
			elapsedMillisecondsSum += this._elapsedMillisecondsBuffer[measurementIndex];
		this._elapsedMilliseconds = elapsedMillisecondsSum / (Single)this._measurementCount;

		if (this._elapsedMilliseconds != 0.0f)
			this._framesPerSecond = 1000.0f / this._elapsedMilliseconds;
		this._framesStopWatch.Restart();

		this._measurementIndex++;
		if (this._measurementIndex >= this._measurementCount)
			this._measurementIndex = 0;
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
