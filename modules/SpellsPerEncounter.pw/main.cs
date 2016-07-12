using Patchwork.Attributes;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace SpellsPerEncounter.pw
{
    [ModifiesType]
    public class mod_CharacterStats : CharacterStats
    {
        [ModifiesMember("HandleGameUtilitiesOnCombatEnd")]
        private void HandleGameUtilitiesOnCombatEnd(object sender, EventArgs e)
    {
        try
        {
            this.m_MarkersAppliedThisCombat.Clear();
            for (int i = this.m_statusEffects.Count - 1; i >= 0; i--)
            {
                if (this.m_statusEffects[i].LastsUntilCombatEnds)
                {
                    this.ClearEffect(this.m_statusEffects[i]);
                }
                else if (this.m_statusEffects[i].AbilityOrigin && this.m_statusEffects[i].AbilityOrigin.CombatOnly)
                {
                    this.ClearEffect(this.m_statusEffects[i]);
                }
                else if (this.m_statusEffects[i].IsDOT)
                {
                    this.ClearEffect(this.m_statusEffects[i]);
                }
            }
            IEnumerator<GenericAbility> enumerator = this.ActiveAbilities.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    GenericAbility current = enumerator.Current;
                    if (current == null)
                    {
                        continue;
                    }
                    current.HandleGameUtilitiesOnCombatEnd(sender, e);
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            if (base.gameObject != null)
            {
                int num = 0;
                while (num < 8)
                {
                    num++;
                }
            }
            this.PlayPartyFatigueSoundIfAble();
			
			//Start of mod
			for (int i = 0; i < (int)this.SpellCastCount.Length; i++)
			{
				this.SpellCastCount[i] = 0;
			}
			//End of mod
        }
        catch (Exception exception)
        {
            Debug.LogException(exception, this);
            if (UIDebug.Instance)
            {
                UIDebug.Instance.LogOnScreenWarning("Exception in CharacterStats.HandleGameUtilitiesOnCombatEnd! Please Fix!", UIDebug.Department.Programming, 10f);
            }
        }
    }

    }
}
