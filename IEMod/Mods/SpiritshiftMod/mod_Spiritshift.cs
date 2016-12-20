using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;
using AI.Player;


namespace IEMod.Mods.SpiritshiftMod
{
    [ModifiesType]
    public class mod_Spiritshift : Spiritshift
    {
        [ModifiesMember("ActivateHelper")]
        public void ActivateHelperNew()
        {
            AIController aIController = GameUtilities.FindActiveAIController(this.Owner);
            if (aIController != null)
            {
                if (aIController.StateManager.CurrentState is ReloadWeapon)
                {
                    aIController.StateManager.PopCurrentState();
                }
                if (aIController.StateManager.CurrentState is ReloadWeapon)
                {
                    aIController.StateManager.PopCurrentState();
                }
            }
            Equipment component = this.Owner.GetComponent<Equipment>();
            if (component == null)
            {
                return;
            }
            component.ClearSummonEffectInSlot(Equippable.EquipmentSlot.PrimaryWeapon);
            component.ClearSummonEffectInSlot(Equippable.EquipmentSlot.SecondaryWeapon);
            Stealth.SetInStealthMode(this.Owner, false);
            if (component.CurrentItems.GetItemInSlot(Equippable.EquipmentSlot.Neck) != null)
            {
                ShroudInstance componentInChildren = this.Owner.GetComponentInChildren<ShroudInstance>();
                if (componentInChildren != null)
                {
                    componentInChildren.gameObject.SetActive(false);
                    this.hasCape = true;
                    this.capeShroud = componentInChildren;
                }
            }
            for (Equippable.EquipmentSlot i = Equippable.EquipmentSlot.Head; i < Equippable.EquipmentSlot.Count; i = (Equippable.EquipmentSlot)((int)i + (int)Equippable.EquipmentSlot.Neck))
            {
                if (!this.LeaveSlotEquipped(i))
                {
                    Equippable itemInSlot = component.CurrentItems.GetItemInSlot(i);
                    if (itemInSlot != null)
                    {
                        itemInSlot.HandleItemModsOnSpiritshift(this.Owner);
                        Equippable equippable = component.UnEquip(itemInSlot, i);
                        if (equippable != null)
                        {
                            this.m_ItemsToRestore.Add(equippable);
                        }
                    }
                }
            }
            this.SwitchForm();
            Equippable[] tempEquipment = this.TempEquipment;
            for (int j = 0; j < (int)tempEquipment.Length; j++)
            {
                Equippable equippable1 = tempEquipment[j];
                Equippable equippable2 = GameResources.Instantiate<Equippable>(equippable1);
                equippable2.Prefab = equippable1;
                this.m_ItemsToDestroy.Add(equippable2);
                if (component.Equip(equippable2))
                {
                    UIDebug.Instance.LogOnScreenWarning(string.Concat(new object[] { "Spiritshift '", base.name, "' tried to overwrite equipment at '", equippable1.GetPreferredSlot(), "' but there was already something there." }), UIDebug.Department.Design, 10f);
                }
            }
            foreach (GenericAbility mAbility in this.m_abilities)
            {
                mAbility.IsVisibleOnUI = true;
            }
            this.m_ownerStats.LockEquipment();
            if (base.OwnerAI != null)
            {
                base.OwnerAI.StateManager.ClearQueuedStates();
                Attack currentState = base.OwnerAI.StateManager.CurrentState as Attack;
                if (currentState != null)
                {
                    base.OwnerAI.StateManager.PopAllStates();
                    Attack currentTarget = AIStateManager.StatePool.Allocate<Attack>();
                    base.OwnerAI.StateManager.PushState(currentTarget);
                    currentTarget.IsAutoAttack = false;
                    currentTarget.Target = currentState.CurrentTarget;
                }
            }
            this.EnableSkinnedMeshes(false);
            this.m_ownerStats.RecoveryTimer = 0f;
            this.m_commentDelay = 2f;
            if (BigHeads.Enabled)
            {
                BigHeads.Apply(this.m_ownerStats.gameObject, null);
            }
            this.m_activeTimer = this.DurationOverride;
            if (this.m_ownerStats != null)
            {
                Spiritshift mActiveTimer = this;
                if (IEModOptions.SpiritshiftToggleable)
                {
                    mActiveTimer.m_activeTimer = 99999999999;
                }
                else {
                    mActiveTimer.m_activeTimer = mActiveTimer.m_activeTimer * this.m_ownerStats.StatEffectDurationMultiplier;
                }
            }
        }
    }
}
