namespace Iris.Base.Types;

/// <summary>
/// Enumerazione rappresentante il livello di anti aliasing da utilizzare nella visualizzazione
/// </summary>
public enum IrisAntiAliasingLevel : UInt16
{
	Off = 0,
	x2 = 2,
	x4 = 4,
	x8 = 8,
	x16 = 16,
	x32 = 32,
}
