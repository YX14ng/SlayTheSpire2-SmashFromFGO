using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using OberonPretender.OberonPretenderCode.Cards.Basic;
using OberonPretender.OberonPretenderCode.Extensions;
using OberonPretender.OberonPretenderCode.Relics;

namespace OberonPretender.OberonPretenderCode.Character;

public class Oberon : PlaceholderCharacterModel
{
    public const string CharacterId = "Oberon";

    // Purpura feerico del Rey Hada (alas de mariposa nocturnas con un dejo de oro de cuento).
    public static readonly Color Color = new("6e4a9e");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;

    // HP = 72 (DESIGN-OBERON 8). Estatuilla de soporte: ni el tanque (78-80) ni la squishy (70). La
    // Deuda impaga sangra HP (3/punto), asi que el colchon importa, pero el lift global lo da el x1.25
    // del BondRelic, NO el statline (regla 1.bis.5).
    public override int StartingHp => 72;

    // Mazo inicial QQABB-fiel adaptado a la espina (4 Golpe + 4 Defensa + 2 firmas, instruccion de
    // fase): Palabra del Rey Hada (el prestamo modelo: recurso AHORA + Deuda) y Caer la Noche (el
    // toggle Rey<->Invierno). Las dos firmas codifican las identidades clave -- banco/Deuda y formas --
    // desde el turno 1; el mazo gana el acto 1 sin el motor armado (las Desatadas y los grandes
    // prestamos NO estan en el mazo inicial). Las 6 cartas de comando (Buster/Arts/Quick) llegan con el
    // pase de cartas (FGOCore Cards/Command, pendiente compartido Morgan/Gilgamesh).
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Strike>(), ModelDb.Card<Strike>(), ModelDb.Card<Strike>(), ModelDb.Card<Strike>(),
        ModelDb.Card<Defend>(), ModelDb.Card<Defend>(), ModelDb.Card<Defend>(), ModelDb.Card<Defend>(),
        ModelDb.Card<KingFaesWord>(),
        ModelDb.Card<Nightfall>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<DreamContract>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<OberonCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<OberonRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<OberonPotionPool>();

    // Visuales: render del modelo de batalla 2800100 (Rey del Cuento) como forma base. Las formas
    // swapean SpriteFrames via FormVisuals (oberon_frames_{king,winter,vortigern}.tres, MainFile).
    public override string CustomVisualPath => $"{MainFile.ResPath}/character/oberon_visuals.tscn";

    // Multiplayer/perf: precargar los frames pesados en el set residente de la run; si no, Godot
    // los carga sincrónicamente al entrar a combate (freeze -> timeout/desconexión en MP).
    protected override IEnumerable<string> ExtraAssetPaths =>
    [
        $"{MainFile.ResPath}/character/oberon_frames.tres",
    ];
    public override string CustomCharacterSelectBg => $"{MainFile.ResPath}/character/oberon_select_bg.tscn";

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
    // del modelo 2800100 + charagraph). Renombrar a char_icon/char_select/... cuando llegue el arte.
    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}
