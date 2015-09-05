using System;
using System.Collections.Generic;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.BonusSpellsGrimoire {
	
	[ModifiesType]
	public class mod_UIGrimoireSpellsInRow : UIGrimoireInSpellRow
	{

		[ModifiesMember("Init")]
		public void InitNew()
		{
			if (this.m_Spells != null)
			{
				return;
			}

			this.m_Spells = new List<UIGrimoireSpell> {
				this.RootSpell
			};



			int bonusSpellSlots = (int)IEModOptions.ExtraWizardSpells;

			this.Grid.maxPerLine = 4 + bonusSpellSlots;


			for (int i = 1; i < 4 + bonusSpellSlots; i++)
			{
				this.m_Spells.Add(NGUITools.AddChild(this.RootSpell.transform.parent.gameObject, this.RootSpell.gameObject).GetComponent<UIGrimoireSpell>());
			}

			foreach (UIGrimoireSpell current in this.m_Spells)
			{
				UIEventListener expr_98 = UIEventListener.Get(current.Icon.gameObject);
				expr_98.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_98.onClick, new UIEventListener.VoidDelegate(this.OnChildClick));
				UIEventListener expr_C9 = UIEventListener.Get(current.Icon.gameObject);
				expr_C9.onRightClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_C9.onRightClick, new UIEventListener.VoidDelegate(this.OnChildRightClick));
			}
			UIGrimoireLevelButtons expr_111 = UIGrimoireManager.Instance.LevelButtons;
			expr_111.LevelChanged = (UIGrimoireLevelButtons.OnLevelChanged)Delegate.Combine(expr_111.LevelChanged, new UIGrimoireLevelButtons.OnLevelChanged(this.SelectedLevelChanged));
			this.Grid.Reposition();
		}

		[ModifiesMember("Reload")]
		public void ReloadNew(int spellLevel)
		{

			this.m_SpellLevel = spellLevel;
			int i = 0;
			Grimoire loadedGrimoire = UIGrimoireManager.Instance.LoadedGrimoire;
			if (loadedGrimoire && spellLevel - 1 < loadedGrimoire.Spells.Length)
			{

				GenericSpell[] spellData = loadedGrimoire.Spells[spellLevel - 1].SpellData;
				for (int j = 0; j < spellData.Length; j++)
				{
					GenericSpell genericSpell = spellData[j];
					if (!(genericSpell == null))
					{

						if (i >= this.m_Spells.Count)
						{
							Debug.LogWarning(string.Concat(new object[]
							{
								"Grimoire has too many spells for UI (",
								UIGrimoireManager.Instance.LoadedGrimoire.name,
								" S.L.",
								spellLevel,
								")"
							}));
							break;
						}
						this.m_Spells[i].SetSpell(genericSpell);
						this.m_Spells[i].SetVisibility(true);
						this.m_Spells[i].SetSelected(i < (4 + (int)IEModOptions.ExtraWizardSpells) && spellLevel == UIGrimoireManager.Instance.LevelButtons.CurrentLevel);
						this.m_Spells[i].SetDisabled(GameState.InCombat || !UIGrimoireManager.Instance.CanEditGrimoire);
						i++;
					}
				}
			}
			while (i < this.m_Spells.Count)
			{
				this.m_Spells[i].SetVisibility(false);
				i++;
			}
		}
	}

}