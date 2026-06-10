using BaseLib.Abstracts;
using BaseLib.Utils;
using MashShielder.MashShielderCode.Character;

namespace MashShielder.MashShielderCode.Potions;

[Pool(typeof(MashShielderPotionPool))]
public abstract class MashShielderPotion : CustomPotionModel;