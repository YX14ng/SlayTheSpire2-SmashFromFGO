using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using OkitaSaber.OkitaSaberCode.Cards.Basic;
using OkitaSaber.OkitaSaberCode.Extensions;
using OkitaSaber.OkitaSaberCode.Relics;

namespace OkitaSaber.OkitaSaberCode.Character;

public class Okita : PlaceholderCharacterModel
{
    public const string CharacterId = "Okita";

    // Azul acero del Sanador de Dragones (armadura) con un dejo de sangre de dragón.
    public static readonly Color Color = new("37597a");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;

    // HP = 80 — ancla Ironclad exacta (DESIGN-SIEGFRIED §5, P1). La identidad bruiser la paga la
    // Sangre de Dragón inicial 2 (Hoja de Tilo), NO HP extra.
    public override int StartingHp => 80;

    // Mazo inicial QAABB fiel (§5): 4 Golpe Buster + 4 Defensa + 2 FIRMAS (Tajo Cazadragones =
    // Buster con sinergia SdD; Bautismo de Sangre = construcción de SdD+NP). El "Golpe" vive DENTRO
    // del mazo (P6 Morgan, compat del tag Strike). Las firmas enseñan las dos identidades (armadura
    // y medidor) y el mazo gana el acto 1 sin el motor armado (Balmung NO está en el mazo inicial).
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Strike>(), ModelDb.Card<Strike>(), ModelDb.Card<Strike>(), ModelDb.Card<Strike>(),
        ModelDb.Card<Defend>(), ModelDb.Card<Defend>(), ModelDb.Card<Defend>(), ModelDb.Card<Defend>(),
        ModelDb.Card<DragonbloodCut>(),
        ModelDb.Card<BloodBaptism>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<LindenLeaf>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<OkitaCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<OkitaRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<OkitaPotionPool>();

    // Visuales: placeholder por ahora (el modelo de batalla 102700 espera el export GUI del bundle;
    // DESIGN-SIEGFRIED §11). El render directo (modelo único) vendrá tras el export.
    public override string CustomVisualPath => $"{MainFile.ResPath}/character/okita_visuals.tscn";
    public override string CustomCharacterSelectBg => $"{MainFile.ResPath}/character/okita_select_bg.tscn";

    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    // NOTA: usan los placeholders del scaffold (sufijo _char_name) hasta el pase de arte real (render
    // del modelo 102700 + charagraph). Renombrar a char_icon/char_select/... cuando llegue el arte.
    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}
