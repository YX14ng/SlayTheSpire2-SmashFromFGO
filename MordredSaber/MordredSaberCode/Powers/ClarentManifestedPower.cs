using MegaCrit.Sts2.Core.Entities.Powers;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Marcador OCULTO «la carta-ulti ya se manifestó este pico» (espejo de CamelotManifestedPower
/// de Mash). Lo aplica el <c>GaugeFilled</c> de MainFile al cruzar 100 NP para no spamear la
/// carta «Clarent Blood Arthur: Desatado» mientras seguís sobre 100; el <c>GaugeDropped</c> lo
/// remueve al bajar por debajo de 100, re-armando la ulti para el próximo pico (DESIGN-MORDRED §3.1).
/// </summary>
public sealed class ClarentManifestedPower : MordredPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    protected override bool IsVisibleInternal => false;
}
