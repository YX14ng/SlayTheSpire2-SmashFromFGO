using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MashShielder.MashShielderCode.Relics;

/// <summary>Diario de la A-Team — every room you enter, gain 1 Max HP. Memories make her stronger.</summary>
public sealed class ATeamDiary : MashShielderRelic
{
    private int _grantedRooms;

    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(1m)];

    /// <summary>Salas ya premiadas, medidas contra el historial de mapa del run.</summary>
    [SavedProperty]
    public int GrantedRooms
    {
        get => _grantedRooms;
        set
        {
            AssertMutable();
            _grantedRooms = value;
        }
    }

    public override Task AfterObtained()
    {
        // La reliquia es de rareza Event, o sea que la vía natural de conseguirla es DENTRO de un
        // evento — justo la sala que después re-dispara AfterRoomEntered al continuar el save. Sin
        // esta línea el contador arrancaría en 0 y esa sala, ya pisada, contaría como nueva.
        GrantedRooms = CountRoomsEntered();
        return base.AfterObtained();
    }

    public override async Task AfterRoomEntered(AbstractRoom _)
    {
        // El motor RE-DISPARA este hook sobre un EventRoom ya jugado al continuar un save:
        // EventRoom.EnterInternal llama a Hook.AfterRoomEntered sin mirar IsPreFinished ni
        // isRestoringRoomStackBase (CombatRoom sí se protege: solo lo dispara desde StartCombat).
        // El Max HP se persiste crudo, así que el +1 original ya está en el save y el replay
        // regalaba otro. El historial de mapa NO se re-apendea al restaurar, así que sirve de marca
        // idempotente: solo premiamos salas nuevas. Único hueco: el evento final del Arquitecto se
        // entra sin apendear historial (sala terminal, sin impacto en el run).
        int roomsEntered = CountRoomsEntered();
        if (roomsEntered <= GrantedRooms) return;

        GrantedRooms = roomsEntered;
        Flash();
        await CreatureCmd.GainMaxHp(Owner.Creature, 1m);
    }

    private int CountRoomsEntered()
    {
        int total = 0;
        foreach (var act in Owner.RunState.MapPointHistory)
        {
            foreach (var mapPoint in act)
            {
                total += mapPoint.Rooms.Count;
            }
        }

        return total;
    }
}
