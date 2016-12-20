using IEMod.Mods.Options;
using Patchwork.Attributes;

namespace IEMod.Mods.XPTable {
	[ModifiesType]
	public class mod_CharacterStats : CharacterStats {
		//using dropdown box
		[ModifiesMember("ExperienceNeededForLevel")]
		public static int ExperienceNeededForLevelNew(int level)
		{
			switch (IEModOptions.NerfedXPTableSetting)
			{
				case IEModOptions.NerfedXpTable.Disabled:
					return (level - 1) * level * 500;
				case IEModOptions.NerfedXpTable.Increase25:
					return (level - 1) * level * 625;
                case IEModOptions.NerfedXpTable.Increase33:
                    return (level - 1) * level * 665;
                case IEModOptions.NerfedXpTable.Increase50:
					return (level - 1) * level * 750;
				case IEModOptions.NerfedXpTable.Square:
					return (level - 1) * (level - 1) * 1000;
				default:
					goto case IEModOptions.NerfedXpTable.Disabled;
			}
		}

		[ModifiesMember("ExperienceNeededForNextLevel")]
		public static int ExperienceNeededForNextLevelNew(int currentLevel)
		{
			switch (IEModOptions.NerfedXPTableSetting)
			{
				case IEModOptions.NerfedXpTable.Disabled:
					return currentLevel * (currentLevel + 1) * 500;
				case IEModOptions.NerfedXpTable.Increase25:
					return currentLevel * (currentLevel + 1) * 625;
                case IEModOptions.NerfedXpTable.Increase33:
                    return currentLevel * (currentLevel + 1) * 665;
                case IEModOptions.NerfedXpTable.Increase50:
					return currentLevel * (currentLevel + 1) * 750;
				case IEModOptions.NerfedXpTable.Square:
					return currentLevel * currentLevel * 1000;
				default:
					goto case IEModOptions.NerfedXpTable.Disabled;
			}
		}

	}
}
