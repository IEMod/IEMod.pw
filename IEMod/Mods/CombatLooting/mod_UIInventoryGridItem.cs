using System.Collections.Generic;
using IEMod.Helpers;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.CombatLooting {

	[ModifiesType]
	public class mod_UIInventoryGridItem : UIInventoryGridItem {
		[NewMember]
		private static List<string> ForbiddenToMoveItems;

		[MemberAlias(".ctor", typeof (MonoBehaviour))]
		private void MonoBehavior_ctor() {

		}

		[NewMember]
		[DuplicatesBody(".ctor")]
		private void CtorOrig() {

		}

		[ModifiesMember(".ctor")]
		public void CtorNew() {
			MonoBehavior_ctor();
			CtorOrig();
			if (ForbiddenToMoveItems == null) {
				ForbiddenToMoveItems = new List<string> {
					"Druid Cat Claws",
					"Druid Bear Claws",
					"Druid Boar Tusks",
					"Druid Stag Horns",
					"Druid Wolf Teeth",
					"Concelhaut's Parasitic Quarterstaff",
					"Citzal's Spirit Lance",
					"Firebrand",
					"Kalakoth's Minor Blight - Burn",
					"Kalakoth's Minor Blight - Corrode",
					"Kalakoth's Minor Blight - Freeze",
					"Kalakoth's Minor Blight - Shock",
					"Dragon Sky Claws"
				};
				// a speculation
			}
		}

		[NewMember]
		[DuplicatesBody("ItemTransferValid")]
		public static bool ItemTransferValidOrig(InventoryItem invitem, UIInventoryGridItem from, UIInventoryItemZone to,
			out string error, bool alreadyHeld = false) {
			throw new DeadEndException("ItemTransferValidOrig");
		}

		[ModifiesMember("ItemTransferValid")]
		public static bool ItemTransferValidNew(InventoryItem invitem, UIInventoryGridItem from, UIInventoryItemZone to,
			out string error, bool alreadyHeld = false) {
			// this is to disallow items "FROM"...
			//!+ ADDED CODE
			if (IEModOptions.UnlockCombatInv && GameState.InCombat
				&& (from.EquipmentSlot == Equippable.EquipmentSlot.Armor || ForbiddenToMoveItems.Contains(invitem.BaseItem.Name))) {
				error = GUIUtils.GetText(0xd7);
				return false;
			}
			//!+ END ADD
			return ItemTransferValidOrig(invitem, from, to, out error, alreadyHeld);
		}

		[ModifiesMember("ItemTransferValid")]
		public static bool ItemTransferValidNew(InventoryItem invitem, UIInventoryGridItem from, UIInventoryGridItem to,
			out string error, bool alreadyHeld = false) {
			UIInventoryItemZone owner;
			error = string.Empty;
			if (invitem == null || invitem.baseItem == null) {
				return true;
			}
			if (from == to && UIGlobalInventory.Instance.DragOwnerIsSame()) {
				return true;
			}
			InventoryItem inventoryItem = invitem;
			UIInventoryGridItem uIInventoryGridItem = from;
			if (!to) {
				owner = null;
			} else {
				owner = to.Owner;
			}
			if (!UIInventoryGridItem.ItemTransferValid(inventoryItem, uIInventoryGridItem, owner, out error, alreadyHeld)) {
				return false;
			}
			if (!UIInventoryGridItem.ItemTakeValid(to, out error)) {
				return false;
			}

			//!+ ADDED CODE
			// this is to disallow items TO
			if (IEModOptions.UnlockCombatInv && GameState.InCombat
				&& (to.EquipmentSlot == Equippable.EquipmentSlot.Armor || ForbiddenToMoveItems.Contains(invitem.BaseItem.Name)))
			{
				// doesn't allow equipping/unequipping armor during combat, as well as summoned magical items such as druid cat claws
				error = GUIUtils.GetText(0xd7);
				return false;
			}
			//!+ END ADD

			if (to && (to.Locked || to.Blocked || !to.EquipmentModifyValid())) {
				return false;
			}
			if (to && to.EquipmentSlot != Equippable.EquipmentSlot.None) {
				error = GUIUtils.GetText(217);
				Equippable equippable = invitem.baseItem as Equippable;
				EquipmentSoulbind component = invitem.baseItem.GetComponent<EquipmentSoulbind>();
				if (!equippable) {
					return false;
				}
				if (!equippable.CanUseSlot(to.EquipmentSlot)) {
					return false;
				}
				if (to.WeaponSetBuddy != null && !to.WeaponSetBuddy.Empty) {
					if (equippable.BothPrimaryAndSecondarySlot) {
						error = GUIUtils.GetText(1737);
						return false;
					}
					Equippable invItem = to.WeaponSetBuddy.InvItem.baseItem as Equippable;
					if (invItem && invItem.BothPrimaryAndSecondarySlot) {
						error = GUIUtils.GetText(1737);
						return false;
					}
				}
				Equipment selectedEquipment = UIInventoryManager.Instance.Equipment.SelectedEquipment;
				if (selectedEquipment && selectedEquipment.IsSlotLocked(to.EquipmentSlot)) {
					return false;
				}
				Equippable.CantEquipReason cantEquipReason =
					equippable.WhyCantEquip(UIInventoryManager.Instance.SelectedCharacter.gameObject);
				if (cantEquipReason != Equippable.CantEquipReason.None) {
					CharacterStats selectedCharacter = UIInventoryManager.Instance.SelectedCharacter;
					switch (cantEquipReason) {
						case Equippable.CantEquipReason.EquipmentLocked: {
							object[] name = new object[] {
								invitem.baseItem.Name,
								CharacterStats.Name(selectedCharacter)
							};
							error = GUIUtils.Format(1003, name);
							break;
						}
						case Equippable.CantEquipReason.ClassMismatch: {
							error = GUIUtils.Format(1003, new object[] {
								invitem.baseItem.Name,
								GUIUtils.GetClassString(selectedCharacter.CharacterClass, selectedCharacter.Gender)
							});
							break;
						}
						case Equippable.CantEquipReason.SoulboundToOther: {
							error = GUIUtils.Format(2038, new object[] {
								equippable.Name,
								CharacterStats.Name(component.BoundGuid)
							});
							break;
						}
						default: {
							goto case Equippable.CantEquipReason.EquipmentLocked;
						}
					}
					return false;
				}
			}
			if (from.EquipmentSlot != Equippable.EquipmentSlot.None) {
				Equipment equipment = UIInventoryManager.Instance.Equipment.SelectedEquipment;
				if (equipment && to && equipment.IsSlotLocked(to.EquipmentSlot)) {
					return false;
				}
			}
			if (to && to.EquipmentSlot == Equippable.EquipmentSlot.Grimoire
				&& UIInventoryManager.Instance.SelectedCharacter.SpellCastingDisabled > 0) {
				error = GUIUtils.GetText(1738);
				return false;
			}
			if (to && to.RestrictByFilter != UIInventoryFilter.ItemFilterType.NONE
				&& (invitem.baseItem.FilterType & to.RestrictByFilter) == UIInventoryFilter.ItemFilterType.NONE) {
				if (to.OrAllowEquipment == Equippable.EquipmentSlot.None || !(invitem.baseItem is Equippable)
					|| !(invitem.baseItem as Equippable).CanUseSlot(to.OrAllowEquipment)) {
					return false;
				}
			}
			return true;
		}
	}

}