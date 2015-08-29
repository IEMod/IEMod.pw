using AI.Player;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;


namespace IEMod.Mods.CombatOnly {
	[ModifiesType()]
	public class Mod_CombatOnly_GenericAbility : GenericAbility
	{
		public GenericAbility.NotReadyValue WhyNotReadyNew
		{
			[ModifiesMember("get_WhyNotReady")]
			get
			{
				return this.m_reason_not_ready;
			}
			[ModifiesMember("set_WhyNotReady")]
			set
			{
				//changed this condition:
				if (value == (GenericAbility.NotReadyValue)0 || (IEModOptions.CombatOnlyMod && value == NotReadyValue.OnlyInCombat))
				{
					this.m_reason_not_ready = (GenericAbility.NotReadyValue)0;
				}
				else
				{
					this.m_reason_not_ready |= value;
				}
			}
		}
		[ModifiesMember("Update")]
		protected virtual void UpdateNew()
		{
			if (this.CooldownType == GenericAbility.CooldownMode.PerEncounter && !GameState.InCombat && this.m_perEncounterResetTimer > 0f)
			{
				this.m_perEncounterResetTimer -= Time.deltaTime;
				if (this.m_perEncounterResetTimer <= 0f)
				{
					this.m_cooldownCounter = 0;
					this.m_perEncounterResetTimer = 0f;
				}
			}
			if (this.m_activated)
			{
				if (this.ClearsOnMovement && this.IsMoving)
				{
					this.HideFromCombatLog = true;
					this.Deactivate(this.m_owner);
					return;
				}
				//MOD (Remove combat-only restrictions) - changed this condition:
				if (this.CombatOnly && !GameState.InCombat && !(IEModOptions.CombatOnlyMod))
				{
					this.Deactivate(this.m_owner);
					return;
				}
				if (this.NonCombatOnly && GameState.InCombat)
				{
					this.Deactivate(this.m_owner);
					return;
				}
			}
			if (!GameState.Paused)
			{
				/* 
				if (this.m_statusEffectsNeeded && !this.m_statusEffectsActivated)
				{
					this.ActivateStatusEffects();
				}
				else if (!this.m_statusEffectsNeeded && this.m_statusEffectsActivated)
				{
					this.DeactivateStatusEffects();
				}
				This code was changed to the following in 2.0:
				*/
				this.UpdateStatusEffectActivation();
			}
			if (this.m_activated && !this.m_applied)
			{
				if (this.CanApply())
				{
					if (this.m_target != null)
					{
						this.Apply(this.m_target);
					}
					else
					{
						this.Apply(this.m_targetPoint);
					}
				}
				return;
			}
			if (!this.Passive && this.Modal)
			{
				bool flag = this.m_ownerHealth && (this.m_ownerHealth.Dead || this.m_ownerHealth.Unconscious);
				if (this.m_activatedLaunching)
				{
					if (flag || !this.m_UITriggered)
					{
						this.m_activatedLaunching = false;
						this.m_rez_modal_cooldown = 5f;
					}
					else if (this.m_UITriggered && this.CombatOnly && GameState.InCombat && !this.m_activated && this.m_rez_modal_cooldown <= 0f)
					{
						PartyMemberAI component = this.m_owner.GetComponent<PartyMemberAI>();
						if (component)
						{
							Ability ability = component.StateManager.FindState(typeof(Ability)) as Ability;
							if (ability == null || ability.QueuedAbility != this)
							{
								this.m_activatedLaunching = false;
							}
						}
						else
						{
							this.m_activatedLaunching = false;
						}
					}
					else if (this.m_UITriggered && this.NonCombatOnly && !GameState.InCombat && !this.m_activated && this.m_rez_modal_cooldown <= 0f)
					{
						PartyMemberAI component2 = this.m_owner.GetComponent<PartyMemberAI>();
						if (component2)
						{
							Ability ability2 = component2.StateManager.FindState(typeof(Ability)) as Ability;
							if (ability2 == null || ability2.QueuedAbility != this)
							{
								this.m_activatedLaunching = false;
							}
						}
						else
						{
							this.m_activatedLaunching = false;
						}
					}
					else if (this.m_rez_modal_cooldown > 0f)
					{
						this.m_rez_modal_cooldown -= Time.deltaTime;
					}
				}
				if (this.m_activated && this.CombatOnly && flag)
				{
					this.Deactivate(this.m_owner);
					this.m_activatedLaunching = false;
				}
				if (!GameState.Paused && this.m_ownerPartyAI != null && this.m_ownerPartyAI.gameObject.activeInHierarchy && this.m_UITriggered != (this.m_activated || this.m_activatedLaunching))
				{
					if (this.m_activated)
					{
						this.Deactivate(this.m_owner);
					}
					else if (this.m_ownerPartyAI != null && this.m_ownerPartyAI.QueuedAbility == this)
					{
						this.m_ownerPartyAI.QueuedAbility = null;
					}
					else if (this.m_UITriggered && !flag)
					{
						if (this.Ready)
						{
							this.LaunchAttack(base.gameObject, false, null, null, null);
						}
					}
					else
					{
						this.m_rez_modal_cooldown = 5f;
						this.m_activatedLaunching = false;
					}
				}
			}
			else if (!this.Passive && this.m_UITriggered)
			{
				this.m_UITriggered = false;
				if (this.m_ownerPartyAI != null && this.m_ownerPartyAI.Selected)
				{
					if (this.TriggerOnHit)
					{
						this.m_activated = true;
						return;
					}
					if (this.UsePrimaryAttack || this.UseFullAttack)
					{
						Equipment component3 = this.m_owner.GetComponent<Equipment>();
						if (component3 != null)
						{
							AttackBase primaryAttack = component3.PrimaryAttack;
							if (primaryAttack != null && this.m_ownerStats != null)
							{
								GenericAbility component4 = base.gameObject.GetComponent<GenericAbility>();
								AttackBase component5 = base.gameObject.GetComponent<AttackBase>();
								if (this.m_attackBase != null)
								{
									StatusEffect[] array = new StatusEffect[1];
									StatusEffectParams statusEffectParams = new StatusEffectParams();
									statusEffectParams.AffectsStat = StatusEffect.ModifiedStat.ApplyAttackEffects;
									statusEffectParams.AttackPrefab = this.m_attackBase;
									statusEffectParams.OneHitUse = true;
									GenericAbility.AbilityType abType = GenericAbility.AbilityType.Ability;
									if (this is GenericSpell)
									{
										abType = GenericAbility.AbilityType.Spell;
									}
									array[0] = StatusEffect.Create(this.m_owner, this, statusEffectParams, abType, null, true);
									if (this.m_attackBase.UseAttackVariationOnFullAttack && this.UseFullAttack)
									{
										this.LaunchAttack(primaryAttack.gameObject, this.UseFullAttack, component4, array, component5, this.m_attackBase.AttackVariation);
									}
									else
									{
										this.LaunchAttack(primaryAttack.gameObject, this.UseFullAttack, component4, array, component5);
									}
								}
								else
								{
									this.LaunchAttack(primaryAttack.gameObject, this.UseFullAttack, component4, null, component5);
								}
							}
						}
					}
					else
					{
						this.LaunchAttack(base.gameObject, false, null, null, null);
					}
				}
			}
			else if (this.m_activated && this.m_applied && !this.Passive && !this.Modal && !this.m_permanent && !this.UseFullAttack && !this.UsePrimaryAttack && (this.m_attackBase == null || this.AttackComplete))
			{
				this.m_activated = false;
				this.m_activatedLaunching = false;
				this.m_applied = false;
				this.OnInactive();
			}
			if (this.IsTriggeredPassive)
			{
				if (this.EffectTriggeredThisFrame)
				{
					if (this.m_owner && FogOfWar.PointVisibleInFog(this.m_owner.transform.position))
					{
						this.ReportActivation();
					}
					this.EffectTriggeredThisFrame = false;
				}
				if (this.EffectUntriggeredThisFrame)
				{
					if (this.m_owner && FogOfWar.PointVisibleInFog(this.m_owner.transform.position))
					{
						this.ReportDeactivation();
					}
					this.EffectUntriggeredThisFrame = false;
				}
			}
		}

	}
}

