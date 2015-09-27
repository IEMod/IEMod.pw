using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patchwork.Attributes;

namespace IEMod.Mods.Targeting
{
    [ModifiesType]
    public abstract class mod_AIController : AIController
    {
        [ModifiesMember("GetOriginalTeam")]
        public Team mod_GetOriginalTeam()
        {
            if (this.m_teamBeforeConfusion != null)
            {
                return this.m_teamBeforeConfusion;
            }
            if (this.m_stats != null)
            {
                /* MODIFIED CODE */
                mod_CharacterStats modStats = (mod_CharacterStats)this.m_stats;
                Team cachedTeam = modStats.GetSwappedTeam();
                if(cachedTeam != null)
                {
                    return cachedTeam;
                }
                /* END MODIFIED CODE */
            }
            Faction component = this.StateManager.CurrentState.Owner.GetComponent<Faction>();
            if (component == null)
            {
                return null;
            }
            return component.CurrentTeam;
        }
    }
}
