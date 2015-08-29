using System;
using System.Linq;
using IEMod.Helpers;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.NoEngagement {
	[ModifiesType]
	public abstract class Mod_NoEngagement_AIController : AIController
	{
		[ModifiesMember("EngageEnemy")]
		public void EngageEnemyNew(GameObject enemy)
		{
			AIController component = enemy.GetComponent<AIController>();
			if (!component || component == this)
			{
				return;
			}
			if (!this.EngagedEnemies.Contains(enemy))
			{
				this.EngagedEnemies.Add(enemy);
			}
			GameObject owner = this.StateManager.CurrentState.Owner;
			CharacterStats component2 = owner.GetComponent<CharacterStats>();
			if (component2 != null)
			{
				component2.NotifyEngagement(enemy);
			}
			GameEventArgs gameEventArgs = new GameEventArgs();
			gameEventArgs.Type = GameEventType.MeleeEngaged;
			gameEventArgs.GameObjectData = new GameObject[1];
			gameEventArgs.GameObjectData[0] = owner;
			component.OnEvent(gameEventArgs);
			if (FogOfWar.Instance.PointVisible(owner.transform.position) && (!IEModOptions.DisableEngagement)) // added && ((Mod_GameOptions_GameMode)GameState.Mode).DisableEngagement == 0
			{
				Console.AddMessage(Console.Format(GUIUtils.GetTextWithLinks(100), new object[]
				{
					CharacterStats.NameColored(owner),
					CharacterStats.NameColored(enemy)
				}));
			}
			GameState.AutoPause(AutoPauseOptions.PauseEvent.CharacterAttacked, enemy, owner, null);
			component.AddEngagedBy(owner);
		}

		[ModifiesMember("DisengageEnemy")]
		public void DisengageEnemyNew(GameObject enemy, AttackBase attack)
		{
			CharacterStats component = enemy.GetComponent<CharacterStats>();
			if ((component != null) && !component.ImmuneToEngagement && !IEModOptions.DisableEngagement) // added && (Mod_GameOptions_GameMode)GameState.Mode).DisableEngagement == 0
			{
				attack.IsDisengagementAttack = true;
				attack.Launch(enemy, -1);
			}
			GameObject owner = this.StateManager.CurrentState.Owner;
			this.m_disengagementTrackers.Add(new DisengagementTracker(enemy, false));
			this.StopEnagagement(enemy);
			if (this.m_stats != null)
			{
				this.m_stats.NotifyEngagementBreak(enemy);
			}
			this.EnemyBreaksEngagement(enemy);
			AIController controller = enemy.GetComponent<AIController>();
			if (controller != null)
			{
				GameEventArgs args = new GameEventArgs
				{
					Type = GameEventType.MeleeEngageBroken,
					GameObjectData = new GameObject[] { owner }
				};
				controller.OnEvent(args);
				controller.EnemyBreaksEngagement(owner);
			}
		}
	}

}