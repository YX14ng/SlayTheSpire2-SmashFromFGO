using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using MegaCrit.Sts2.Core.Nodes.Combat;
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

    // Mazo inicial de 10 (DESIGN-MORDRED §5.0/§7), QAABB sesgado a Buster — con las cartas de COMANDO
    // ya cableadas (DESIGN-REVIEW-2: el deck genérico sin generación de NP/★ dejaba el acto 1 roto por
    // falta de motor). Modeladas sobre las de Okita: el Buster carga NP al pegar, el Arts es el feeder
    // de NP de peso, el Quick produce las Estrellas del Crítico. Así el mazo GENERA desde el turno 1.
    // Composición: 2 Buster + 2 Arts + 1 Quick (QAABB real) + 3 Defensa + las DOS FIRMAS (Rebelión
    // y Bajar la Visera, los cambios de forma que enseñan la danza del casco desde el combate 1).
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<BusterCommand>(), ModelDb.Card<BusterCommand>(),
        ModelDb.Card<ArtsCommand>(), ModelDb.Card<ArtsCommand>(),
        ModelDb.Card<QuickCommand>(),
        ModelDb.Card<Defend>(), ModelDb.Card<Defend>(), ModelDb.Card<Defend>(),
        ModelDb.Card<Rebellion>(),
        ModelDb.Card<LowerTheVisor>()
    ];

    // Starter: Clarent, la Espada Robada (entra en Enmascarado + motor ★/NP). Las otras starters
    // (Juramento = BondRelic; Sello de Invocación = INpLevelStore) las añade la fase Content.
    // + Oath (Bond) y SummoningSeal (dupes) — audit 2026-07-05 HIGH: sin ellas, el Vinculo, el gacha
    // de dupes (cap NP >100, escalado NP) y el motor Buster/Arts/Quick (CommandBonusPower, que
    // siembra la BondRelic) estaban MUERTOS en Mordred. Espejo de Mash/Morgan.
    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<ClarentTheStolenSword>(),
        ModelDb.Relic<OathOfTheKnightOfTreachery>(),
        ModelDb.Relic<SummoningSealSaberOfRed>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<MordredCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<MordredRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<MordredPotionPool>();

    // Visuales: placeholder por ahora (el modelo de batalla 100900 espera el export GUI del bundle;
    // DESIGN-MORDRED §8). Las 3 formas se renderizan como attach/detach del casco sobre el MISMO rig.
    public override string CustomVisualPath => $"{MainFile.ResPath}/character/mordred_visuals.tscn";

    // Robustez anti-conflicto: construye las visuals directo desde la factory de BaseLib, en vez
    // del Instantiate<NCreatureVisuals>() del juego que depende del patch global de conversion.
    // Inmune al clobber de otra BaseLib forkeada (p. ej. figure_Saya). Sin escena propia => null
    // => comportamiento original. Ver docs/FINDINGS.md.
    public override NCreatureVisuals? CreateCustomVisuals()
        => string.IsNullOrEmpty(CustomVisualPath)
            ? null
            : NodeFactory<NCreatureVisuals>.CreateFromScene(CustomVisualPath);

    // Multiplayer/perf: los frames de combate son pesados. Sin precarga, Godot los carga
    // sincrónicamente al entrar a combate (freeze de segundos -> timeout/desconexión en MP).
    // ExtraAssetPaths los mete en el set residente de la run (precarga en la pantalla de carga).
    protected override IEnumerable<string> ExtraAssetPaths =>
    [
        $"{MainFile.ResPath}/character/mordred_frames.tres",
    ];
    public override string CustomMerchantAnimPath => $"{MainFile.ResPath}/character/mordred_merchant.tscn";
    public override string CustomRestSiteAnimPath => $"{MainFile.ResPath}/character/mordred_rest.tscn";
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
