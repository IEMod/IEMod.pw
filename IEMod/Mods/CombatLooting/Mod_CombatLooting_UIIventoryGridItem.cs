using System.Collections.Generic;
using IEMod.Helpers;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.CombatLooting {
	[ModifiesType]
	public class Mod_CombatLooting_UIIventoryGridItem : UIInventoryGridItem
	{
		[NewMember]
		static List<string> ForbiddenToMoveItems;

		[MemberAlias(".ctor", typeof(MonoBehaviour))]
		private void MonoBehavior_ctor() {
			
		}

		[ModifiesMember(".ctor")]
		public void CtorNew() {
			MonoBehavior_ctor();
			// Note: this type is marked as 'beforefieldinit'.
			//UIInventoryGridItem.s_TooltipRepeatTime = 0f;
			UIInventoryGridItem.s_TooltipRepeatLast = Vector2.zero;
			UIInventoryGridItem.s_BlockRefresh = false;

			if (ForbiddenToMoveItems == null)
			{
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
		public static bool ItemTransferValidOrig(InventoryItem invitem, UIInventoryGridItem from, UIInventoryItemZone to, out string error) {
			throw new DeadEndException("ItemTransferValidOrig");
		}

		[ModifiesMember("ItemTransferValid")]
		public static bool ItemTransferValidNew(InventoryItem invitem, UIInventoryGridItem from, UIInventoryItemZone to, out string error)
		{
			// this is to disallow items "FROM"...
			// added this code
			if (IEModOptions.UnlockCombatInv && GameState.InCombat && (from.EquipmentSlot == Equippable.EquipmentSlot.Armor || ForbiddenToMoveItems.Contains(invitem.BaseItem.Name)))
			{
				error = GUIUtils.GetText(0xd7);
				return false;
			}
			// end of added code
			return ItemTransferValidOrig(invitem, from, to, out error);
		}

		[ModifiesMember("ItemTransferValid")]
		public static bool ItemTransferValidNew(InventoryItem invitem, UIInventoryGridItem from, UIInventoryGridItem to, out string error)
		{
			error = string.Empty;
			if ((invitem == null) || (invitem.baseItem == null)) { return true; }
			if ((@from == to) && UIGlobalInventory.Instance.DragOwnerIsSame())
			{
				return true;
			}
			if (!ItemTransferValid(invitem, @from, to.Owner, out error))
			{
				return false;
			}

			// this is to disallow items TO
			// added code
			if (IEModOptions.UnlockCombatInv && GameState.InCombat && (to.EquipmentSlot == Equippable.EquipmentSlot.Armor || ForbiddenToMoveItems.Contains(invitem.BaseItem.Name))) // added this line
			{
				error = GUIUtils.GetText(0xd7);
				return false; // added this line // doesn't allow equipping/unequipping armor during combat, as well as summoned magical items such as druid cat claws
			}
			// end of added code

			if ((to.Locked || to.Blocked) || !to.EquipmentModifyValid())
			{
				return false;
			}
			if (to.EquipmentSlot != Equippable.EquipmentSlot.None)
			{
				error = GUIUtils.GetText(0xd9);
				Equippable baseItem = invitem.baseItem as Equippable;
				if (baseItem == null)
				{
					return false;
				}
				if (!baseItem.CanUseSlot(to.EquipmentSlot))
				{
					return false;
				}
				if ((to.WeaponSetBuddy != null) && !to.WeaponSetBuddy.Empty)
				{
					if (baseItem.BothPrimaryAndSecondarySlot)
					{
						error = GUIUtils.GetText(0x6c9);
						return false;
					}
					Equippable equippable2 = to.WeaponSetBuddy.InvItem.baseItem as Equippable;
					if ((equippable2 != null) && equippable2.BothPrimaryAndSecondarySlot)
					{
						error = GUIUtils.GetText(0x6c9);
						return false;
					}
				}
				Equipment selectedEquipment = UIInventoryManager.Instance.Equipment.SelectedEquipment;
				if ((selectedEquipment != null) && selectedEquipment.IsSlotLocked(to.EquipmentSlot))
				{
					return false;
				}
				if (!baseItem.CanEquip(UIInventoryManager.Instance.SelectedCharacter.gameObject))
				{
					CharacterStats selectedCharacter = UIInventoryManager.Instance.SelectedCharacter;
					object[] parameters = new object[] { invitem.baseItem.Name, GUIUtils.GetClassString(selectedCharacter.CharacterClass, selectedCharacter.Gender) };
					error = GUIUtils.Format(0x3eb, parameters);
					return false;
				}
			}
			if (@from.EquipmentSlot != Equippable.EquipmentSlot.None)
			{
				Equipment equipment2 = UIInventoryManager.Instance.Equipment.SelectedEquipment;
				if ((equipment2 != null) && equipment2.IsSlotLocked(to.EquipmentSlot))
				{
					return false;
				}
			}
			if ((to.EquipmentSlot == Equippable.EquipmentSlot.Grimoire) && (UIInventoryManager.Instance.SelectedCharacter.SpellCastingDisabled > 0))
			{
				error = GUIUtils.GetText(0x6ca);
				return false;
			}
			if (((to.RestrictByFilter != UIInventoryFilter.ItemFilterType.NONE) && ((invitem.baseItem.FilterType & to.RestrictByFilter) == UIInventoryFilter.ItemFilterType.NONE)) && (((to.OrAllowEquipment == Equippable.EquipmentSlot.None) || !(invitem.baseItem is Equippable)) || !(invitem.baseItem as Equippable).CanUseSlot(to.OrAllowEquipment)))
			{
				return false;
			}
			return true;
		}
	}
}