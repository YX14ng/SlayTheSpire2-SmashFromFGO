using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using Tiamat.TiamatCode.Powers;
using Tiamat.TiamatCode.Powers.Forms;

namespace Tiamat.TiamatCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "Tiamat";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // Precarga de frames de las 2 formas (humano / Bestia). Los .tres se generan en el
        // pase de arte (ver DESIGN-TIAMAT.md / WORKFLOW-FGO §3).
        FormVisuals.RegisterFrames(
            $"{ResPath}/character/tiamat_frames_human.tres",
            $"{ResPath}/character/tiamat_frames_beast.tres");

        // Ventana de NP "Génesis": a 100, el Mar desborda — estallido de cría + recursos y
        // te volvés Bestia. NO genera ulti gratis (modelo nuevo). Se re-arma al bajar < 100.
        NpCharge.GaugeFilled += TryOpenGenesis;
        NpCharge.GaugeDropped += DisarmGenesis;
    }

    private static async Task TryOpenGenesis(Creature creature)
    {
        if (creature.Player?.Character is not Character.Tiamat) return;
        if (creature.HasPower<GenesisSpentPower>()) return;

        await PowerCmd.Apply<GenesisSpentPower>(creature, 1m, creature, null);

        await PowerCmd.Apply<OverchargeBlessingPower>(creature, 1m, creature, null);
        await Lahmu.Spawn(creature, 2, null);
        await Lahmu.Feed(creature, 1, null);
        if (!creature.HasPower<TiamatBeastPower>())
        {
            await FormSwitch.Enter<TiamatBeastPower>(null, creature, null);
        }

        if (creature.Player != null)
        {
            await PlayerCmd.GainEnergy(1, creature.Player);
            await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), 2, creature.Player);
        }
    }

    private static async Task DisarmGenesis(Creature creature)
    {
        if (creature.HasPower<GenesisSpentPower>())
        {
            await PowerCmd.Remove<GenesisSpentPower>(creature);
        }
    }
}
