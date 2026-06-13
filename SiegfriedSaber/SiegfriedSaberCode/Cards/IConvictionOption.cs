namespace SiegfriedSaber.SiegfriedSaberCode.Cards;

/// <summary>
/// Una opción de "Por Convicción Propia": una option-card (rareza Token, no drafteable) que se elige en la
/// pantalla de selección y aplica su efecto sobre su propio dueño. Patrón WineFox (IDirectApply).
/// </summary>
public interface IConvictionOption
{
    Task ApplyConviction();
}
