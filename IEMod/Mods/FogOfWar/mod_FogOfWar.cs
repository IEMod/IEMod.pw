using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;
using System.Threading;



namespace IEMod.Mods.mod_FogOfWar
{
    /*
    [ModifiesType]
    public class mod_FogOfWar : FogOfWar
    {
        [NewMember]
        public void AdjutFogOpacity(float newOpacity)
        {
            Vector2[] mFogAlphasStaging = this.m_fogAlphasStaging;
            Monitor.Enter(mFogAlphasStaging);
            try
            {
                float[] mFogDesiredAlphas = this.m_fogDesiredAlphas;
                Monitor.Enter(mFogDesiredAlphas);
                try
                {
                    for (int i = 0; i < (int)this.m_fogAlphasStaging.Length; i++)
                    {
                        if (this.m_fogAlphasStaging[i].y > 0.7f)
                        {
                            this.m_fogAlphasStaging[i].y = newOpacity;
                        }
                        if (this.m_fogAlphasStaging[i].x > 0.7f)
                        {
                            this.m_fogAlphasStaging[i].x = newOpacity;
                        }
                        if (this.m_fogDesiredAlphas[i] > 0.7f)
                        {
                            this.m_fogDesiredAlphas[i] = newOpacity;
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(mFogDesiredAlphas);
                }
            }
            finally
            {
                Monitor.Exit(mFogAlphasStaging);
            }
        }

    }// End of mod_FogOfWar
 */
}
