using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace DropItems
{
	[BepInDependency("com.bepis.r2api")]
	[BepInPlugin("KookehsDropItemMod", "Kookeh's Drop Item Mod", "0.1.0")]
	public class KookehsDropItemMod : BaseUnityPlugin
	{
		public static GameObject RootObject { get; set; }
		public static DropItemHandler DropItemHandler { get; set; }

		public void Awake()
		{
			RootObject = new GameObject("DropItemsMod");
			DontDestroyOnLoad(RootObject);
			DropItemHandler = RootObject.AddComponent<DropItemHandler>();

			var itemIndex = typeof(ItemIcon).GetField("itemIndex", BindingFlags.NonPublic | BindingFlags.Instance);
			var inventory = typeof(ItemInventoryDisplay).GetField("inventory", BindingFlags.NonPublic | BindingFlags.Instance);

			InventoryAPI.OnItemIconAdded += (itemIcon) => {
				if (itemIcon.GetComponent<DropItemHandler>() == null) 
				{
					DropItemHandler dropItemHandler = itemIcon.transform.gameObject.AddComponent<DropItemHandler>();
					dropItemHandler.GetItemIndex = () => (ItemIndex)itemIndex.GetValue(itemIcon);
					dropItemHandler.GetInventory = () => (Inventory)inventory.GetValue(itemIcon.rectTransform.parent.GetComponent<ItemInventoryDisplay>());
				}
			};


			InventoryAPI.OnEquipmentIconAdded += (equipmentIcon) => {
				if (equipmentIcon.GetComponent<DropItemHandler>() == null) {
					DropItemHandler dropItemHandler = equipmentIcon.transform.gameObject.AddComponent<DropItemHandler>();
					dropItemHandler.GetInventory = () => equipmentIcon.targetInventory;
					dropItemHandler.EquipmentIcon = true;
				}
			};
		}
	}

	public class DropItemHandler : MonoBehaviour, IPointerClickHandler
	{
		public Func<ItemIndex> GetItemIndex { get; set; }
		public Func<Inventory> GetInventory { get; set; }
		public bool EquipmentIcon { get; set; }

		private static readonly FieldInfo characterBody_transform = typeof(CharacterBody).GetField("transform", BindingFlags.Instance | BindingFlags.NonPublic);

		public void OnPointerClick(PointerEventData eventData)
		{
			if (GetInventory == null)
			{
				return;
			}

			var inventory = GetInventory?.Invoke();

			if (!NetworkServer.active || inventory == null) return;

			CharacterBody characterBody = inventory.GetComponent<CharacterMaster>().GetBody();
			Notification notification = characterBody.gameObject.AddComponent<Notification>();
			notification.transform.SetParent((Transform)characterBody_transform.GetValue(characterBody));
			notification.SetPosition(new Vector3((float)(Screen.width * 0.8), (float)(Screen.height * 0.25), 0));

			Transform charTransform = (Transform)characterBody_transform.GetValue(characterBody);

			if (EquipmentIcon)
			{
				var equipmentIndex = inventory.GetEquipmentIndex();

				if (equipmentIndex != EquipmentIndex.None)
				{
					EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef((EquipmentIndex) equipmentIndex);
					notification.SetIcon(Resources.Load<Texture>(equipmentDef.pickupIconPath));
					notification.GetTitle = () => "Equipment dropped";
					notification.GetDescription = () => $"{Language.GetString(equipmentDef.nameToken)}";
					inventory.SetEquipmentIndex(EquipmentIndex.None);
					PickupDropletController.CreatePickupDroplet(new PickupIndex((EquipmentIndex) equipmentIndex),
						charTransform.position, Vector3.up * 20f + charTransform.forward * 10f);
					return;
				}
			}

			if (GetItemIndex == null)
				return;

			var itemIndex = GetItemIndex();
			var itemCount = inventory.GetItemCount(itemIndex);
			if (itemCount <= 0) {
				return;
			}


			ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
			notification.SetIcon(Resources.Load<Texture>(itemDef.pickupIconPath));
			notification.GetTitle = () => "Item dropped";
			notification.GetDescription = () => $"{Language.GetString(itemDef.nameToken)}";
			inventory.RemoveItem(itemIndex, 1);
			PickupDropletController.CreatePickupDroplet(new PickupIndex(itemIndex), charTransform.position, Vector3.up * 20f + charTransform.forward * 10f);
		}
	}
}
