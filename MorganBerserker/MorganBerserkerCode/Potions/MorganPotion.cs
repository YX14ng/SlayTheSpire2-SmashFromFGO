using BaseLib.Abstracts;
using BaseLib.Utils;
using MorganBerserker.MorganBerserkerCode.Character;

namespace MorganBerserker.MorganBerserkerCode.Potions;

[Pool(typeof(MorganPotionPool))]
public abstract class MorganPotion : CustomPotionModel;
