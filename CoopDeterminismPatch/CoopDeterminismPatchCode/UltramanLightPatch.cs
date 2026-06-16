using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;
using Ultraman.UltramanCode.Factions;

namespace CoopDeterminismPatch;

/// <summary>
/// Fix de co-op para Ultraman. <c>Lineage.GrantRandomSecondaryLight</c> elige el Light Relic con
/// <c>player.PlayerRng.Rewards</c> — la RNG de RECOMPENSAS, que NO es parte de la simulacion de combate
/// sincronizada, asi que host y cliente avanzan ese RNG distinto y eligen Ultraman distinto. El relic
/// resultante entra al estado de combate (que SI se checksumea) -> divergencia -> desconexion.
///
/// En multijugador reemplazamos la seleccion por una DETERMINISTA derivada de datos sincronizados
/// (semilla inmutable del jugador + NetId + nº de luces que ya tiene) — identica en todos los clientes.
/// En 1 jugador devolvemos true (corre el original random, sin cambios).
/// </summary>
[HarmonyPatch(typeof(Lineage), "GrantRandomSecondaryLight")]
internal static class GrantRandomSecondaryLightDeterministic
{
    private static bool Prefix(Player player, ref Task __result)
    {
        if (!IsCoOp())
        {
            return true; // 1 jugador: comportamiento original (random)
        }

        __result = GrantDeterministic(player);
        return false; // co-op: salteamos el original no-determinista
    }

    private static bool IsCoOp()
    {
        var type = RunManager.Instance?.NetService?.Type;
        return type == NetGameType.Host || type == NetGameType.Client;
    }

    private static async Task GrantDeterministic(Player player)
    {
        var held = Lineage.HeldByPlayer(player);
        var available = Lineage.AllUltramen.Where(id => !held.Contains(id)).ToList();
        if (available.Count == 0)
        {
            return;
        }

        // Indice determinista a partir de estado SINCRONIZADO (la semilla del PlayerRng es inmutable y
        // derivada del seed de la run; NetId identifica al jugador; held.Count varia por concesion).
        uint mixed = player.PlayerRng.Seed ^ (uint)player.NetId ^ (uint)available.Count;
        int idx = (int)(mixed % (uint)available.Count);

        await Lineage.GrantSecondary[available[idx]](player);
    }
}
