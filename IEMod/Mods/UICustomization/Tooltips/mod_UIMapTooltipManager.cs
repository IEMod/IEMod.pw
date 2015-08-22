using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Tooltips {

	/// <summary>
	/// Tooltip appears where it should for party members, instead of always appearing above portraits. Tooltip appears only for hovered character.
	/// Tooltip's position for party portraits depends on the party bar's position on the screen.
	/// TAB makes abilities tooltip appear instantly. Same was supposed to work for inventory, but it was buggy, and I didn't know how to fix it.
	/// </summary>
	[ModifiesType]
	public class mod_UIMapTooltipManager : UIMapTooltipManager
	{
		[ModifiesMember("Show")]
		public UIMapTooltip ShowNew(GameObject target, bool byMouse, bool byAttack)
		{
			// this mod makes it that only the tooltip for a hovered character is shown when TAB is pressed
			bool showOnlyOneTooltip = IEModOptions.OneTooltip;
			bool npcUnderCursor = GameCursor.ObjectUnderCursor == target; // added this line

			if ((npcUnderCursor && showOnlyOneTooltip) || !showOnlyOneTooltip) // added this line
			{
				if ((target == null) || this.m_LevelUnloading)
				{
					return null;
				}
				if (UIBarkstringManager.Instance.IsBarking(target))
				{
					return null;
				}
				if (!this.m_ActiveTips.ContainsKey(target))
				{
					if (this.m_TipPool.Count <= 0)
					{
						this.NewToPool();
					}
					UIMapTooltip tooltip = this.m_TipPool[this.m_TipPool.Count - 1];
					this.m_TipPool.RemoveAt(this.m_TipPool.Count - 1);
					tooltip.gameObject.SetActive(true);
					tooltip.Set(target);
					tooltip.RevealedByMouse = byMouse;
					tooltip.RevealedByAttackCursor = byAttack;
					this.m_ActiveTips.Add(target, tooltip);
					tooltip.Panel.alpha = 0f;
					return tooltip;
				}
				this.m_ActiveTips[target].RevealedByMouse = byMouse || this.m_ActiveTips[target].RevealedByMouse;
				this.m_ActiveTips[target].RevealedByAttackCursor = byAttack || this.m_ActiveTips[target].RevealedByAttackCursor;
				return this.m_ActiveTips[target];
			} 
			else
				return null;
		}
	}


// this mod removes delay on TAB for all windows

}