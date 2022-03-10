using System;
using KookehsDropItemMod;
using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace DropItems
{
	public class DropItemHandler : MonoBehaviour, IPointerClickHandler
	{
		public Func<ItemIndex> GetItemIndex { get; set; }
		public Func<Inventory> GetInventory { get; set; }
		public bool EquipmentIcon { get; set; }

		public void OnPointerClick(PointerEventData eventData)
		{
			var inventory = GetInventory();

			KookehsDropItemMod.Logger.LogDebug("KDI: Pointer click, trying to send network message");

			if (!inventory.hasAuthority)
			{
				return;
			}

			if (!NetworkServer.active)
			{
				// Client, send command
				var body = inventory.GetComponent<CharacterMaster>().GetBody();
				var identity = body.gameObject.GetComponent<NetworkIdentity>();


				DropItemMessage itemDropMessage;
				if (EquipmentIcon)
				{	
					var equipmentIndex = inventory.GetEquipmentIndex();
					itemDropMessage = new DropItemMessage(identity.netId, equipmentIndex);
				}
				else
				{
					var itemIndex = GetItemIndex();
					if (ItemCatalog.GetItemDef(itemIndex).tier == ItemTier.NoTier) {
						return;
					}

					itemDropMessage = new DropItemMessage(identity.netId, itemIndex);
				}

				KookehsDropItemMod.Logger.LogDebug("KDI Sending network message");

				itemDropMessage.Send(NetworkDestination.Server);
			} else
			{
				// Server, execute command
				var characterBody = inventory.GetComponent<CharacterMaster>().GetBody();
				var charTransform = characterBody.GetFieldValue<Transform>("transform");

				var pickupIndex = EquipmentIcon 
					? PickupCatalog.FindPickupIndex(inventory.GetEquipmentIndex()) 
					: PickupCatalog.FindPickupIndex(GetItemIndex());

				DropItem(charTransform, inventory, pickupIndex);
				CreateNotification(characterBody, charTransform, pickupIndex);
			}
		}

		public static void DropItem(Transform charTransform, Inventory inventory, PickupIndex pickupIndex)
		{
			if (PickupCatalog.GetPickupDef(pickupIndex).equipmentIndex != EquipmentIndex.None)
			{
				if (inventory.GetEquipmentIndex() != PickupCatalog.GetPickupDef(pickupIndex).equipmentIndex)
				{
					return;
				}

				inventory.SetEquipmentIndex(EquipmentIndex.None);
			}
			else
			{
				if (inventory.GetItemCount(PickupCatalog.GetPickupDef(pickupIndex).itemIndex) <= 0) 
				{
					return;
				}

				inventory.RemoveItem(PickupCatalog.GetPickupDef(pickupIndex).itemIndex, 1);
			}

			PickupDropletController.CreatePickupDroplet(pickupIndex,
				charTransform.position, Vector3.up * 20f + charTransform.forward * 10f);
		}

		public static void CreateNotification(CharacterBody character, Transform transform, PickupIndex pickupIndex)
		{
			if (PickupCatalog.GetPickupDef(pickupIndex).equipmentIndex != EquipmentIndex.None)
			{
				CreateNotification(character, transform, PickupCatalog.GetPickupDef(pickupIndex).equipmentIndex);
			} 
			else
			{
				CreateNotification(character, transform, PickupCatalog.GetPickupDef(pickupIndex).itemIndex);
			}
		}

		private static void CreateNotification(CharacterBody character, Transform transform, EquipmentIndex equipmentIndex)
		{
			var equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
			const string title = "Equipment dropped";
			var description = Language.GetString(equipmentDef.nameToken);
			var texture = equipmentDef.pickupIconTexture;

			CreateNotification(character, transform, title, description, texture);
		}

		private static void CreateNotification(CharacterBody character, Transform transform, ItemIndex itemIndex)
		{
			var itemDef = ItemCatalog.GetItemDef(itemIndex);
			const string title = "Item dropped";
			var description = Language.GetString(itemDef.nameToken);
			var texture = itemDef.pickupIconTexture;

			CreateNotification(character, transform, title, description, texture);
		}

		private static void CreateNotification(CharacterBody character, Transform transform, string title, string description, Texture texture)
        {
            var notification = character.gameObject.GetComponent<DropItemNotification>();
            if (notification == null)
            {
                notification = character.gameObject.AddComponent<DropItemNotification>();
                notification.transform.SetParent(transform);
			}
            notification.SetNotification(title, description, texture);
        }
	}
}