using IEMod.Mods.Options;
using Patchwork.Attributes;

namespace IEMod.Mods.Religion {
	/// <summary>
	/// Religion. Applies disposition-based bonuses to NPC Paladins (Faith and Conviction) and Priests (Holy Radiance)
	/// </summary>
	[ModifiesType]
	public class mod_Religion : global::Religion
	{
		[ModifiesMember("GetCurrentBonusMultiplier")]
		public float GetCurrentBonusMultiplierNew(global::CharacterStats stats, GenericAbility abilityOrigin)
		{
			if (stats == null)
			{
				return 1f;
			}
			float bonus = 0f;
			if (IEModOptions.NPCDispositionFix || (GameState.s_playerCharacter != null && stats.gameObject == GameState.s_playerCharacter.gameObject)) // added ((Mod_GameOptions_GameMode)GameState.Mode).NPCDispositionFix || 
			{
				float single = 1f;
				if (abilityOrigin != null)
				{
					single = single * abilityOrigin.GatherAbilityModProduct(AbilityMod.AbilityModType.NegativeReligiousTraitMultiplier);
				}
				if (stats.CharacterClass == CharacterStats.Class.Priest)
				{
					global::Religion.DeityData deityDatum = this.FindDeityData(stats.Deity);
					if (deityDatum != null)
					{
						bonus = bonus + this.GetBonus(deityDatum.PositiveTrait[0], this.PositiveTraitBonus);
						bonus = bonus + this.GetBonus(deityDatum.PositiveTrait[1], this.PositiveTraitBonus);
						bonus = bonus + this.GetBonus(deityDatum.NegativeTrait[0], this.NegativeTraitBonus) * single;
						bonus = bonus + this.GetBonus(deityDatum.NegativeTrait[1], this.NegativeTraitBonus) * single;
					}
				}
				else if (stats.CharacterClass == CharacterStats.Class.Paladin)
				{
					global::Religion.PaladinOrderData paladinOrderDatum = this.FindPaladinOrderData(stats.PaladinOrder);
					if (paladinOrderDatum != null)
					{
						/* patching in dis/favored dispositions for Pallegina's order */
						if (stats.PaladinOrder == PaladinOrder.FrermasMesCancSuolias)
						{
							var favored1 = IEModOptions.PalleginaFavored1;
							var favored2 = IEModOptions.PalleginaFavored2;
							var disfavored1 = IEModOptions.PalleginaDisfavored1;
							var disfavored2 = IEModOptions.PalleginaDisfavored2;

							paladinOrderDatum.PositiveTrait = new Disposition.Axis[2];
							paladinOrderDatum.PositiveTrait[0] = favored1;
							paladinOrderDatum.PositiveTrait[1] = favored2;
							paladinOrderDatum.NegativeTrait = new Disposition.Axis[2];
							paladinOrderDatum.NegativeTrait[0] = disfavored1;
							paladinOrderDatum.NegativeTrait[1] = disfavored2;
						}
						/**************************************************************************************/

						bonus = bonus + this.GetBonus(paladinOrderDatum.PositiveTrait[0], this.PositiveTraitBonus);
						bonus = bonus + this.GetBonus(paladinOrderDatum.PositiveTrait[1], this.PositiveTraitBonus);
						bonus = bonus + this.GetBonus(paladinOrderDatum.NegativeTrait[0], this.NegativeTraitBonus) * single;
						bonus = bonus + this.GetBonus(paladinOrderDatum.NegativeTrait[1], this.NegativeTraitBonus) * single;
					}
				}
			}
			bonus = bonus + 1f;
			return bonus;
		}
	}
}
