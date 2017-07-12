using Patchwork.Attributes;

namespace LootShuffler.pw
{
    [ModifiesType]
    public class mod_Loot : Loot
    {
        [ModifiesMember("SetSeed", ModificationScope.Body)]
        private void SetSeedNew()
        {
            ResetSeed();
        }
    }
}