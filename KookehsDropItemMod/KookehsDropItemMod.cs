using BepInEx;
using MiniRpcLib;
using MiniRpcLib.Action;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.UI;
using UnityEngine;

namespace DropItems
{
	[BepInDependency("com.bepis.r2api")]
	[BepInPlugin(ModGuid, ModName, ModVersion)]
	[R2APISubmoduleDependency("InventoryAPI")]
	public class KookehsDropItemMod : BaseUnityPlugin
	{
		public static GameObject RootObject { get; set; }
		public static DropItemHandler DropItemHandler { get; set; }

		private const string ModGuid = "KookehsDropItemMod";
		private const string ModName = "Kookeh's Drop Item Mod";
		private const string ModVersion = "2.0.0";

		public static IRpcAction<DropItemMessage> DropItemCommand { get; set; }

		public KookehsDropItemMod()
		{
			var miniRpc = MiniRpc.CreateInstance(ModGuid);

			DropItemCommand = miniRpc.RegisterAction(Target.Server, (NetworkUser user, DropItemMessage dropItemMessage) => {
				var master = user.master;
				if (master == null)
				{
					return;
				}

				var body = master.GetBody();
				var inventory = master.inventory;
				var charTransform = body.GetFieldValue<Transform>("transform");

				var pickupIndex = dropItemMessage.IsItem
					? new PickupIndex(dropItemMessage.ItemIndex) 
					: new PickupIndex(dropItemMessage.EquipmentIndex);

				DropItemHandler.DropItem(charTransform, inventory, pickupIndex);
				DropItemHandler.CreateNotification(body, charTransform, pickupIndex);
			});
		}

		public void Awake()
		{
			RootObject = new GameObject("DropItemsMod");
			DontDestroyOnLoad(RootObject);
			DropItemHandler = RootObject.AddComponent<DropItemHandler>();

			InventoryAPI.OnItemIconAdded += (itemIcon) => {
				if (itemIcon.GetComponent<DropItemHandler>() != null) return;
				var dropItemHandler = itemIcon.transform.gameObject.AddComponent<DropItemHandler>();
				dropItemHandler.GetItemIndex = () => itemIcon.GetFieldValue<ItemIndex>("itemIndex");
				dropItemHandler.GetInventory = () => itemIcon.rectTransform.parent.GetComponent<ItemInventoryDisplay>().GetFieldValue<Inventory>("inventory");
			};

			InventoryAPI.OnEquipmentIconAdded += (equipmentIcon) => {
				if (equipmentIcon.GetComponent<DropItemHandler>() != null) return;
				var dropItemHandler = equipmentIcon.transform.gameObject.AddComponent<DropItemHandler>();
				dropItemHandler.GetInventory = () => equipmentIcon.targetInventory;
				dropItemHandler.EquipmentIcon = true;
			};
		}
	}
}
