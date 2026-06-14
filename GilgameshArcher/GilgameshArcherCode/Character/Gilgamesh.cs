using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using GilgameshArcher.GilgameshArcherCode.Cards.Basic;
using GilgameshArcher.GilgameshArcherCode.Extensions;
using GilgameshArcher.GilgameshArcherCode.Relics;

namespace GilgameshArcher.GilgameshArcherCode.Character;

public class Gilgamesh : PlaceholderCharacterModel
{
    public const string CharacterId = "Gilgamesh";

    // Dorado imperial (sobre el rojo vino de los relámpagos de Ea) — DESIGN-GILGAMESH §7.
    public static readonly Color Color = new("d4af37");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;

    // HP = 72 — ancla entre Silent 70 e Ironclad 80 (DESIGN-GILGAMESH §7). Semidiós 2/3 pero END C y
    // demasiado arrogante para defender con su cuerpo: su defensa REAL es la Puerta, no su carne. El
    // kit es explosivo (economía de oro → picos de burst comprado), por eso baja del 75 que proponía B.
    public override int StartingHp => 72;

    // Mazo inicial (fase ESPINA): 4 Golpe + 4 Defensa + 2 FIRMAS de comando que enseñan los dos hilos
    // FGOCore vivos — Arts (Carga NP → Enuma Elish auto a 100) y Quick (Estrellas → Crítico Listo ×2).
    // El "Golpe" vive DENTRO del mazo (compat del tag Strike, patrón Morgan/Siegfried). Las FIRMAS de
    // oro/armas del diseño (Puerta de Babilonia, Regla de Oro) entran cuando aterricen los módulos
    // Arsenal/Gold de FGOCore (checklist §10) — no bloquean el arranque del personaje.
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Strike>(), ModelDb.Card<Strike>(), ModelDb.Card<Strike>(), ModelDb.Card<Strike>(),
        ModelDb.Card<Defend>(), ModelDb.Card<Defend>(), ModelDb.Card<Defend>(), ModelDb.Card<Defend>(),
        ModelDb.Card<Arts>(),
        ModelDb.Card<Quick>()
    ];

    // Starter de fase ESPINA: el Juramento del Rey de Uruk (BondRelic ×1.25 heredado + Nv7 +20 NP inicial
    // + capstone Nv10 = empezás con 1 Crítico Listo). La starter-MOTOR del diseño (Bab-ilu, la Llave del
    // Tesoro) se suma cuando exista el módulo Arsenal de FGOCore — DESIGN-GILGAMESH §6/§7, checklist §10.
    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<OathOfUruk>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<GilgameshCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<GilgameshRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<GilgameshPotionPool>();

    // Visuales: modelo de batalla único (200200, sin formas — §3.5). El swap cosmético opcional con
    // NP≥100 (200210→200220) es no-bloqueante y NO se enchufa en la espina (no requiere FormPower).
    public override string CustomVisualPath => $"{MainFile.ResPath}/character/gilgamesh_visuals.tscn";
    public override string CustomCharacterSelectBg => $"{MainFile.ResPath}/character/gilgamesh_select_bg.tscn";

    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    // NOTA: usan los placeholders del scaffold (sufijo _char_name) hasta el pase de arte real (render del
    // modelo 200200 + charagraph, DESIGN-GILGAMESH §8). Renombrar a char_icon/char_select/... con el arte.
    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}
