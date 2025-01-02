using Iris.Graphics.RenderWindow;

namespace Iris.Graphics.Environment.Initialization.Abstractions;

/// <summary>
/// Interfaccia che definisce il comportamento degli oggetti che inizializzano l'ambiente di OpenGL
/// </summary>
public interface IIrsEnvironmentInitializer
{
	/// <summary>
	/// Inizializza l'ambiente di OpenGL
	/// </summary>
	/// <param name="renderWindow">Riferimento alla finestra destinazione dell'output di OpenGL</param>
	/// <param name="requestedConfig">Opzioni di configurazione dell'ambiente di OpenGL richieste</param>
	/// <returns>Opzioni di configurazione effettivamente selezionate per l'ambiente di OpenGL</returns>
	Boolean Initialize(IrisRenderWindow renderWindow, IrsEnvironmentConfig requestedConfig);

	/// <summary>
	/// Restituisce la configurazione attuale, selezionata in fase di inizializzazione dell'ambiente grafico
	/// </summary>
	/// <returns>Configurazione attuale, selezionata in fase di inizializzazione dell'ambiente grafico. Null in caso di errore o di mancata inizializzazione</returns>
	IrsEnvironmentConfig? GetActualConfiguration();
}