using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MordredSaber.MordredSaberCode.Cards.Basic;
using MordredSaber.MordredSaberCode.Extensions;
using MordredSaber.MordredSaberCode.Relics;

namespace MordredSaber.MordredSaberCode.Character;

public class Mordred : PlaceholderCharacterModel
{
    public const string CharacterId = "Mordred";

    // Carmesí de la Saber of Red (la armadura roja-sangre / el relámpago de Clarent).
    public static readonly Color Color = new("b03030");

    public override Color NameColor => Color;
    // Saberface: meme-lore aparte, su género de trato es "no la trates como mujer ni de hombre
    // obviamente" (perfil oficial). Femenino a efectos del motor; las reglas de trato viven en las cartas.
    public override CharacterGender Gender => CharacterGender.Feminine;

    // HP = 75 (DESIGN-MORDRED §7): entre Ironclad 80 / Morgan 78 y Silent 70 / Artoria 70 — END A y
    // armadura completa, pero su plan ofensivo (Rebelión recibe +2/golpe) la deja sin armadura.
    public override int StartingHp => 75;

    // Mazo inicial de 10 (DESIGN-MORDRED §5.0/§7), QAABB sesgado a Buster. En esta ESPINA las cartas
    // de comando Buster/Arts/Quick (fase Content) aún no existen, así que el starter usa las básicas
    // genéricas + las DOS FIRMAS (los cambios de forma): 4 Golpe + 4 Defensa + Rebelión + Bajar la
    // Visera. La danza del casco ya está en el mazo inicial (arrancás Enmascarada vía la starter).
    // Cuando Content añada Buster/Arts/Quick, este deck se ajusta a 3B/2A/1Q/2D/2 firmas.
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Strike>(), ModelDb.Card<Strike>(), ModelDb.Card<Strike>(), ModelDb.Card<Strike>(),
        ModelDb.Card<Defend>(), ModelDb.Card<Defend>(), ModelDb.Card<Defend>(), ModelDb.Card<Defend>(),
        ModelDb.Card<Rebellion>(),
        ModelDb.Card<LowerTheVisor>()
    ];

    // Starter: Clarent, la Espada Robada (entra en Enmascarado + motor ★/NP). Las otras starters
    // (Juramento = BondRelic; Sello de Invocación = INpLevelStore) las añade la fase Content.
    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<ClarentTheStolenSword>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<MordredCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<MordredRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<MordredPotionPool>();

    // Visuales: placeholder por ahora (el modelo de batalla 100900 espera el export GUI del bundle;
    // DESIGN-MORDRED §8). Las 3 formas se renderizan como attach/detach del casco sobre el MISMO rig.
    public override string CustomVisualPath => $"{MainFile.ResPath}/character/mordred_visuals.tscn";

    // Multiplayer/perf: los frames de combate son pesados. Sin precarga, Godot los carga
    // sincrónicamente al entrar a combate (freeze de segundos -> timeout/desconexión en MP).
    // ExtraAssetPaths los mete en el set residente de la run (precarga en la pantalla de carga).
    protected override IEnumerable<string> ExtraAssetPaths =>
    [
        $"{MainFile.ResPath}/character/mordred_frames.tres",
    ];
    public override string CustomCharacterSelectBg => $"{MainFile.ResPath}/character/mordred_select_bg.tscn";

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
    // del modelo 100900 + charagraph). Renombrar a char_icon/char_select/... cuando llegue el arte.
    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}
