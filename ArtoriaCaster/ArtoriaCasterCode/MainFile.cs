using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using FGOCore.FGOCoreCode.Combat;
using ArtoriaCaster.ArtoriaCasterCode.Powers;
using ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

namespace ArtoriaCaster.ArtoriaCasterCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "ArtoriaCaster"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // FGOCore preloads every registered form's frames in background threads.
        FormVisuals.RegisterFrames(
            $"{ResPath}/character/artoria_frames_caster.tres",
            $"{ResPath}/character/artoria_frames_berserker.tres",
            $"{ResPath}/character/artoria_frames_avalon.tres");

        // Modelo de NP nuevo (2026-06-12): cruzar 100 NP NO genera una carta-ulti gratis
        // (eclipsaba a las cartas NP drafteadas — feedback del usuario). Abre la VENTANA
        // "Around Caliburn" 1 turno: este turno podés CRITICAR en cualquier forma (cobrás
        // las estrellas acumuladas en Caster) + soporte (estrellas, Anti-Purga) + devuelve
        // recursos. Las cartas NP drafteadas son el clímax que elegís jugar dentro.
        // La apertura es UNA VEZ POR COMBATE (NpManifestedPower persiste todo el combate):
        // las cartas-NP vacían el medidor y recargarlo no debe repartir el paquete otra vez.
        NpCharge.GaugeFilled += TryOpenNpWindow;
    }

    private static async Task TryOpenNpWindow(Creature creature)
    {
        if (creature.Player?.Character is not Character.ArtoriaCaster) return;
        if (creature.HasPower<NpManifestedPower>()) return;

        // Marker: la ventana ya se abrió este COMBATE. No se remueve al bajar < 100,
        // así que recargar el medidor el mismo combate no vuelve a repartir el paquete.
        await PowerCmd.Apply<NpManifestedPower>(creature, 1m, creature, null);
        await PowerCmd.Apply<AroundCaliburnWindowPower>(creature, 1m, creature, null);

        // Around Caliburn (soporte de Castoria): estrellas para cobrar + protección.
        await PowerCmd.Apply<AntiPurgePower>(creature, 1m, creature, null);
        await Stars.Gain(creature, 6, null);

        // Devuelve recursos: arranca el turno grande, no lo reemplaza (modelo Phrolova).
        // Cierre común factorizado en FGOCore (mismo +1⚡ / robar 1 que Mash y Tiamat).
        await NpWindow.ReturnResources(creature, 1, 1);
    }
}
