/*
 * Not supported for the moment
 * 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patchwork.Attributes;
using IEMod.Mods.Options;


namespace IEMod.Mods.XpForMonsterKills
{
    [ModifiesType]
    public class mod_BestiaryManager : BestiaryManager
    {
        [ModifiesMember("RecordKill")]
        public new void RecordKill(BestiaryReference form)
        {
            this.m_TotalKills++;
            if (!form)
            {
                return;
            }
            float killProportion = this.GetKillProportion(form);
            int num = this.IndexOf(form);
            if (num >= 0)
            {
                // Start of mod
                // Prevent the killcount from increasing if the option is enabled
                if (IEModOptions.XpForMonsterKills) {
                    this.m_KillCounts[num] = 1;
                }
                else {
                    this.m_KillCounts[num] ++;
                }
                // End of mod
            }

        }
    }
}

 */