using System;
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
        private Func<CharacterMaster> getMaster;
        private Func<PickupIndex> getPickupIndex;

        public void SetData(Func<CharacterMaster> getMaster, Func<PickupIndex> getPickupIndex) {
            this.getMaster = getMaster;
            this.getPickupIndex = getPickupIndex;
        }

		public void OnPointerClick(PointerEventData eventData)
		{
			KookehsDropItemMod.Logger.LogDebug("KDI: Pointer click, trying to send network message");
			var master = getMaster();

			if (!master.inventory.hasAuthority)
			{
				return;
			}

			var pickupIndex = getPickupIndex();
			var identity = master.GetBody().gameObject.GetComponent<NetworkIdentity>();
			var pickupDef = PickupCatalog.GetPickupDef(pickupIndex);

			if (pickupDef.itemIndex != ItemIndex.None) {
				if (ItemCatalog.GetItemDef(pickupDef.itemIndex).tier == ItemTier.NoTier) {
					return;
                }
            }

			KookehsDropItemMod.Logger.LogDebug("KDI: Sending network message");

            DropItemMessage itemDropMessage = new DropItemMessage(identity.netId, pickupIndex);
			itemDropMessage.Send(NetworkDestination.Server);
		}

		public static void DropItem(Transform charTransform, Inventory inventory, PickupIndex pickupIndex)
		{
			KookehsDropItemMod.Logger.LogDebug("Transform: " + charTransform.position.ToString());
			KookehsDropItemMod.Logger.LogDebug("Inventory: " + inventory.name);
			KookehsDropItemMod.Logger.LogDebug("Pickup Index: " + pickupIndex);


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
			var notification = character.gameObject.AddComponent<Notification>();
			notification.transform.SetParent(transform);
			notification.SetPosition(new Vector3((float)(Screen.width * 0.8), (float)(Screen.height * 0.25), 0));
			notification.SetIcon(texture);
			notification.GetTitle = () => title;
			notification.GetDescription = () => description;
		}
	}
}