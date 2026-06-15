using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using MashShielder.MashShielderCode.Cards.Basic;
using MashShielder.MashShielderCode.Extensions;
using MashShielder.MashShielderCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace MashShielder.MashShielderCode.Character;

public class MashShielder : PlaceholderCharacterModel
{
    public const string CharacterId = "MashShielder";
    
    public static readonly Color Color = new("a883e0");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    // HP de ecosistema (revertido el +10 del re-stat de JeanneAlter): la tanque va al
    // tope del rango ~80. Valor puente; el rediseño desde 0 lo confirma.
    public override int StartingHp => 80;
    
    // Rediseño v2, parche P7 del juez: 2 Golpe / 2 Defender / 1 Buster / 2 Arts /
    // 1 Quick / ShieldBash / ProtectSenpai = 10 cartas. Enseña las 4 economías el
    // turno 1 (daño de ciclo ~49, paridad Jeanne) sin perder la identidad defensiva
    // (la retención de 10 de la starter hace el trabajo del 3er Defender).
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<StrikeMash>(),
        ModelDb.Card<StrikeMash>(),
        ModelDb.Card<DefendMash>(),
        ModelDb.Card<DefendMash>(),
        ModelDb.Card<BusterMash>(),
        ModelDb.Card<ArtsMash>(),
        ModelDb.Card<ArtsMash>(),
        ModelDb.Card<QuickMash>(),
        ModelDb.Card<ShieldBash>(),
        ModelDb.Card<ProtectSenpai>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<RoundTableFragment>(),
        ModelDb.Relic<MashBond>(),
        ModelDb.Relic<SummonTicket>()
    ];
    
    /// <summary>
    /// Combat visual: the original FGO model rendered to frame sequences (idle/attack/
    /// cast/hurt/die) played by an AnimatedSprite2D — pipeline in docs/ANIMATIONS.md.
    /// BaseLib converts the scene to NCreatureVisuals and routes animation signals.
    /// </summary>
    public override string CustomVisualPath => $"{MainFile.ResPath}/character/mash_visuals.tscn";

    // Multiplayer/perf: precargar los frames pesados en el set residente de la run; si no, Godot
    // los carga sincrónicamente al entrar a combate (freeze -> timeout/desconexión en MP).
    protected override IEnumerable<string> ExtraAssetPaths =>
    [
        $"{MainFile.ResPath}/character/mash_frames_base.tres",
        $"{MainFile.ResPath}/character/mash_frames_ortinax.tres",
        $"{MainFile.ResPath}/character/mash_frames_paladin.tres",
    ];

    /// <summary>Character select background: the golden Castle of the Distant Utopia, dimmed.</summary>
    public override string CustomCharacterSelectBg => $"{MainFile.ResPath}/character/mash_select_bg.tscn";

    /// <summary>Merchant/rest-site visuals: the animated idle, replacing the Ironclad placeholders.</summary>
    public override string CustomMerchantAnimPath => $"{MainFile.ResPath}/character/mash_merchant.tscn";
    public override string CustomRestSiteAnimPath => $"{MainFile.ResPath}/character/mash_rest.tscn";

    public override CardPoolModel CardPool => ModelDb.CardPool<MashShielderCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<MashShielderRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<MashShielderPotionPool>();
    
    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets. 
        These are just some of the simplest assets, given some placeholders to differentiate your character with. 
        You don't have to, but you're suggested to rename these images. */
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }
    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}