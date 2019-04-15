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
			var t1 = new List<ItemIndex> {
				ItemIndex.SecondarySkillMagazine,
				ItemIndex.BossDamageBonus,
				ItemIndex.BleedOnHit,
				ItemIndex.HealWhileSafe,
				ItemIndex.Mushroom,
				ItemIndex.Bear,
				ItemIndex.Hoof,
				ItemIndex.FireRing,
				ItemIndex.CritGlasses,
				ItemIndex.WardOnLevel,
				ItemIndex.Bandolier,
			};

			var t2 = new List<ItemIndex> {
				ItemIndex.Feather,
				ItemIndex.ChainLightning,
				ItemIndex.SlowOnHit,
				ItemIndex.JumpBoost,
				ItemIndex.EquipmentMagazine,
				ItemIndex.Seed
			};

			var t3 = new List<ItemIndex> {
				ItemIndex.AlienHead,
				ItemIndex.IncreaseHealing,
				ItemIndex.UtilitySkillMagazine,
				ItemIndex.ExtraLife,
				ItemIndex.FallBoots,
				ItemIndex.HealOnCrit
			};

			var eq = new List<EquipmentIndex> {
				EquipmentIndex.Lightning,
				EquipmentIndex.CritOnUse,
				EquipmentIndex.Blackhole,
				EquipmentIndex.Fruit
			};

			ItemDropAPI.AddDropInformation(ItemDropLocation.SmallChest, new List<ItemIndex> { ItemIndex.Hoof }.ToSelection(0.3f), t1.ToSelection(0.8f), t2.ToSelection(0.15f), t3.ToSelection(0.05f));
			ItemDropAPI.AddDropInformation(ItemDropLocation.MediumChest, t2.ToSelection(0.7f), t3.ToSelection(0.3f));
			ItemDropAPI.AddDropInformation(ItemDropLocation.LargeChest, t3.ToSelection());

			ItemDropAPI.AddDropInformation(ItemDropLocation.Boss, t3.ToSelection());
			ItemDropAPI.AddDropInformation(ItemDropLocation.Shrine, ItemDropAPI.None.ToSelection(0.5f), t1.ToSelection(0.8f), t2.ToSelection(0.2f), t3.ToSelection(0.03f));

			// if i didn't care about equipment items:
			// DefaultItemDrops.AddEquipmentChestDefaultDrops();
			// but i do so:
			ItemDropAPI.AddDropInformation(ItemDropLocation.EquipmentChest, eq.ToSelection());


			ItemDropAPI.ChestSpawnRate = 1.5f;

			RootObject = new GameObject("DropItemsMod");
			DontDestroyOnLoad(RootObject);
			DropItemHandler = RootObject.AddComponent<DropItemHandler>();

			var equipmentIndex = typeof(EquipmentSlot).GetField("equipmentIndex", BindingFlags.NonPublic | BindingFlags.Instance);
			var itemIndex = typeof(ItemIcon).GetField("itemIndex", BindingFlags.NonPublic | BindingFlags.Instance);
			var inventory = typeof(ItemInventoryDisplay).GetField("inventory", BindingFlags.NonPublic | BindingFlags.Instance);
			var equipmentTargetInventory =
				typeof(EquipmentIcon).GetField("targetInventory", BindingFlags.NonPublic | BindingFlags.Instance);

			InventoryAPI.OnItemIconAdded += (itemIcon) => {
				if (itemIcon.GetComponent<DropItemHandler>() == null) 
				{
					DropItemHandler dropItemHandler = itemIcon.transform.gameObject.AddComponent<DropItemHandler>();
					dropItemHandler.GetItemIndex = () => (ItemIndex)itemIndex.GetValue(itemIcon);
					dropItemHandler.Inventory = (Inventory)inventory.GetValue(itemIcon.rectTransform.parent.GetComponent<ItemInventoryDisplay>());
				}
			};

			InventoryAPI.OnEquipmentIconAdded += (equipmentIcon) => {
				if (equipmentIcon.GetComponent<DropItemHandler>() == null) {
					DropItemHandler dropItemHandler = equipmentIcon.transform.gameObject.AddComponent<DropItemHandler>();
					dropItemHandler.GetEquipmentIndex = () => (EquipmentIndex)equipmentIndex.GetValue(equipmentIcon.targetEquipmentSlot);
					dropItemHandler.Inventory = (Inventory)equipmentTargetInventory.GetValue(equipmentIcon);
				}
			};
		}
	}

	public class DropItemHandler : MonoBehaviour, IPointerClickHandler
	{
		public Func<EquipmentIndex> GetEquipmentIndex { get; set; } = null;
		public Func<ItemIndex> GetItemIndex { get; set; }
		public Inventory Inventory { get; set; }

		private static readonly FieldInfo characterBody_transform = typeof(CharacterBody).GetField("transform", BindingFlags.Instance | BindingFlags.NonPublic);

		public void OnPointerClick(PointerEventData eventData)
		{
			// removed !NetworkServer.active
			if (!NetworkServer.active || Inventory == null) return;


			CharacterBody characterBody = Inventory.GetComponent<CharacterMaster>().GetBody();
			Notification notification = characterBody.gameObject.AddComponent<Notification>();

			Transform transform = (Transform)characterBody_transform.GetValue(characterBody);

			var equipmentIndex = GetEquipmentIndex?.Invoke() ?? null;
			if (equipmentIndex != null && equipmentIndex != EquipmentIndex.None) {
				if (Inventory.GetEquipmentSlotCount() <= 0) {
					return;
				}

				EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef((EquipmentIndex)equipmentIndex);
				notification.SetIcon(Resources.Load<Texture>(equipmentDef.pickupIconPath));
				notification.GetTitle = () => "Equipment dropped";
				notification.GetDescription = () => $"{Language.GetString(equipmentDef.nameToken)}";
				Inventory.SetEquipmentIndex(EquipmentIndex.None);
				PickupDropletController.CreatePickupDroplet(new PickupIndex((EquipmentIndex)equipmentIndex), transform.position, Vector3.up * 20f + transform.forward * 10f);
				return;
			}

			var itemIndex = GetItemIndex();
			var itemCount = Inventory.GetItemCount(itemIndex);
			if (itemCount <= 0) {
				return;
			}


			ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
			notification.SetIcon(Resources.Load<Texture>(itemDef.pickupIconPath));
			notification.GetTitle = () => "Item dropped";
			notification.GetDescription = () => $"{Language.GetString(itemDef.nameToken)}";
			Inventory.RemoveItem(itemIndex, 1);
			PickupDropletController.CreatePickupDroplet(new PickupIndex(itemIndex), transform.position, Vector3.up * 20f + transform.forward * 10f);

			notification.transform.SetParent((Transform)characterBody_transform.GetValue(characterBody));
			notification.SetPosition(new Vector3((float)(Screen.width * 0.8), (float)(Screen.height * 0.25), 0));
		}
	}
}
