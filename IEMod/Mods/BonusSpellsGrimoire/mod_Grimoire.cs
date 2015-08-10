using System.Collections.Generic;
using System.Linq;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.BonusSpellsGrimoire {
	[ModifiesType()]
	public class mod_Grimoire : Grimoire
	{
		[ModifiesMember]
		new public const int MaxSpellsPerLevel = 7;

		[NewMember]
		public new List<string> SerializedSpellNames;

		[ModifiesMember("IsLevelFull")]
		public bool IsLevelFullNew(int level)
		{
        
			level--;

			if (level >= this.Spells.Length)
			{
				return true;
			}

			int numBonusSpells = (int)IEModOptions.ExtraWizardSpells;

			if (numBonusSpells == 0 && Spells[level].IsFull())
			{
				return true;
			}
			int preparedCount = Spells[level].SpellData.Count(x => x);
			var isFull = preparedCount == Spells[level].SpellData.Length;
			return isFull;
		}
		[ModifiesMember("Start")]
		private void StartNew()
		{
			if (this.Spells.Length != 6)
			{
				if (this.Spells.Length > 6)
				{
					Debug.LogError("Too many spell levels in grimoire '" + base.name + "': some will be dropped!");
				}
				Grimoire.SpellChapter[] array = new Grimoire.SpellChapter[6];
				this.Spells.CopyTo(array, 0);
				this.Spells = array;
            
			}
			for (int i = 0; i < this.Spells.Length; i++)
			{
				if (this.Spells[i] == null)
				{
					this.Spells[i] = new Grimoire.SpellChapter();
				}
				else if (this.Spells[i].SpellData.Length != (4 + (int)IEModOptions.ExtraWizardSpells))
				{
					if (this.Spells[i].SpellData.Length > (4 + (int)IEModOptions.ExtraWizardSpells))
					{
						Debug.LogError(string.Concat(new object[]
						{
							"Too many spell slots in grimoire '",
							base.name,
							"' for level ",
							i + 1,
							": some will be dropped!"
						}));
					}
					GenericSpell[] array2 = new GenericSpell[(4 + (int)IEModOptions.ExtraWizardSpells)];
					for (int j = 0; j < Mathf.Min(array2.Length, this.Spells[i].SpellData.Length); j++)
					{
						array2[j] = this.Spells[i].SpellData[j];
					}
					this.Spells[i].SpellData = array2;
				}
			}
		}
		[ModifiesMember("HasSpell")]
		public bool HasSpellNew(GenericSpell spell)
		{
			if (spell != null)
			{
				int num = spell.SpellLevel - 1;
				if (num >= 0 && num < 6)
				{
					for (int i = 0; i < 4 + (int)IEModOptions.ExtraWizardSpells; i++)
					{
						if (this.Spells[num].SpellData[i] != null && this.Spells[num].SpellData[i].DisplayName.StringID == spell.DisplayName.StringID)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		[ModifiesMember("FindNewSpell")]
		public GenericSpell FindNewSpellNew(GameObject caster, int max_spell_level)
		{
			if (caster == null)
			{
				return null;
			}
			CharacterStats component = caster.GetComponent<CharacterStats>();
			if (component == null)
			{
				return null;
			}
			List<GenericSpell> list = new List<GenericSpell>();
			int num = 6;
			if (max_spell_level < num)
			{
				num = max_spell_level;
			}
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < 4 + (int)IEModOptions.ExtraWizardSpells; j++)
				{
					if (this.Spells[i].SpellData[j] != null)
					{
						bool flag = false;
						foreach (GenericAbility current in component.ActiveAbilities)
						{
							if (current is GenericSpell && this.Spells[i].SpellData[j].DisplayName.StringID == current.DisplayName.StringID)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							list.Add(this.Spells[i].SpellData[j]);
						}
					}
				}
			}
			GenericSpell result = null;
			if (list.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				result = list[index];
			}
			return result;
		}
	}
}