using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using MegaCrit.Sts2.Core.Nodes.Combat;
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

    // Mazo inicial QAABB de 10 cartas (DESIGN-GILGAMESH §5.1): 2 Buster, 2 Arts, 1 Quick,
    // 2 defensas, 2 Puertas de Babilonia y 1 Regla de Oro.
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Strike>(), ModelDb.Card<Strike>(),
        ModelDb.Card<Arts>(), ModelDb.Card<Arts>(),
        ModelDb.Card<Quick>(),
        ModelDb.Card<Defend>(), ModelDb.Card<Defend>(),
        ModelDb.Card<GateOfBabylon>(), ModelDb.Card<GateOfBabylon>(),
        ModelDb.Card<GoldenRule>()
    ];

    // Starters: vínculo, motor del Tesoro y almacén de nivel de NP/dupes.
    public override IReadOnlyList<RelicModel> StartingRelics =>
        [ModelDb.Relic<BabIlu>(), ModelDb.Relic<OathOfUruk>(), ModelDb.Relic<CatalogOfTheRoyalTreasury>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<GilgameshCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<GilgameshRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<GilgameshPotionPool>();

    // Visuales: modelo de batalla único (200200, sin formas — §3.5). El swap cosmético opcional con
    // NP≥100 (200210→200220) es no-bloqueante y NO se enchufa en la espina (no requiere FormPower).
    public override string CustomVisualPath => $"{MainFile.ResPath}/character/gilgamesh_visuals.tscn";

    // Robustez anti-conflicto: construye las visuals directo desde la factory de BaseLib, en vez
    // del Instantiate<NCreatureVisuals>() del juego que depende del patch global de conversion.
    // Inmune al clobber de otra BaseLib forkeada (p. ej. figure_Saya). Sin escena propia => null
    // => comportamiento original. Ver docs/FINDINGS.md.
    public override NCreatureVisuals? CreateCustomVisuals()
        => string.IsNullOrEmpty(CustomVisualPath)
            ? null
            : NodeFactory<NCreatureVisuals>.CreateFromScene(CustomVisualPath);

    // Multiplayer/perf: precargar los frames pesados en el set residente de la run; si no, Godot
    // los carga sincrónicamente al entrar a combate (freeze -> timeout/desconexión en MP).
    protected override IEnumerable<string> ExtraAssetPaths =>
    [
        $"{MainFile.ResPath}/character/gilgamesh_frames.tres",
    ];
    public override string CustomMerchantAnimPath => $"{MainFile.ResPath}/character/gilgamesh_merchant.tscn";
    public override string CustomRestSiteAnimPath => $"{MainFile.ResPath}/character/gilgamesh_rest.tscn";
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
