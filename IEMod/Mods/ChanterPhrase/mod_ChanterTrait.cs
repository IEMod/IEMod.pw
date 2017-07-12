using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEMod.Mods.Options;
using Patchwork.Attributes;

namespace IEMod.Mods.ChanterPhrase
{
    [ModifiesType("ChanterTrait")]
    class mod_ChanterTrait : ChanterTrait
    {
        // Gives the chanter some base Phrases Count at combat start
        [ModifiesMember("OnCombatStart")]
        public new void OnCombatStart(object sender, EventArgs e)
        {
            if (!GameState.IsInTrapTriggeredCombat)
            {
                this.StartLastChant();
            }
            this.PhraseCountMax = this.FindMaxRoarCost();
            // Modded code
            if (IEModOptions.ChanterPhraseCount) {
                this.PhraseCount = this.PhraseCountMax;
            }
        }
    }
}
