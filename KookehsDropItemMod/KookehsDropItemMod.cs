using BepInEx;
using BepInEx.Logging;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Networking;
using R2API.Utils;
using RoR2;
using RoR2.UI;
using System;
using UnityEngine;

namespace DropItems
{
    [BepInDependency(R2API.R2API.PluginGUID)]
	[BepInPlugin(ModGuid, ModName, ModVersion)]
	[R2APISubmoduleDependency(nameof(NetworkingAPI), nameof(CommandHelper))]
	public class KookehsDropItemMod : BaseUnityPlugin
	{
		public static GameObject RootObject { get; set; }
		public static DropItemHandler DropItemHandler { get; set; }

		private const string ModGuid = "KookehsDropItemMod";
		private const string ModName = "Kookeh's Drop Item Mod";
		private const string ModVersion = "2.3.4";

		public static event Action<ItemIcon> OnItemIconAdded;
		public static event Action<EquipmentIcon> OnEquipmentIconAdded;

		internal new static ManualLogSource Logger { get; set; }

		public void Awake()
		{
			Logger = base.Logger;
			NetworkingAPI.RegisterMessageType<DropItemMessage>();

			IL.RoR2.UI.ItemInventoryDisplay.AllocateIcons += OnItemIconAddedHook;
			IL.RoR2.UI.ScoreboardStrip.SetMaster += OnEquipmentIconAddedHook;

			RootObject = new GameObject("DropItemsMod");
			DontDestroyOnLoad(RootObject);
			DropItemHandler = RootObject.AddComponent<DropItemHandler>();

			CommandHelper.AddToConsoleWhenReady();

			OnItemIconAdded += (itemIcon) => {
				if (itemIcon.GetComponent<DropItemHandler>() != null) return;

				Func<CharacterMaster> getCharacterMaster = () => itemIcon.rectTransform.parent.GetComponent<ItemInventoryDisplay>().GetFieldValue<Inventory>("inventory").GetComponent<CharacterMaster>();

				var dropItemHandler = itemIcon.transform.gameObject.AddComponent<DropItemHandler>();
				dropItemHandler.SetData(getCharacterMaster, () => PickupCatalog.FindPickupIndex(itemIcon.GetFieldValue<ItemIndex>("itemIndex")));
			};

			OnEquipmentIconAdded += (equipmentIcon) => {
				if (equipmentIcon.GetComponent<DropItemHandler>() != null) return;

				var dropItemHandler = equipmentIcon.transform.gameObject.AddComponent<DropItemHandler>();
				dropItemHandler.SetData(() => equipmentIcon.targetInventory.GetComponent<CharacterMaster>(), () => PickupCatalog.FindPickupIndex(equipmentIcon.targetInventory.GetEquipmentIndex()));
			};
		}

		private static void OnItemIconAddedHook(ILContext il) {
			var cursor = new ILCursor(il).Goto(0);
			cursor.GotoNext(
				x => x.MatchStloc(out _),
				x => x.MatchLdarg(0),
				x => x.MatchLdfld<ItemInventoryDisplay>("itemIcons")
			);
			cursor.Emit(OpCodes.Dup);
			cursor.EmitDelegate<Action<ItemIcon>>(i => OnItemIconAdded?.Invoke(i));
		}

		private static void OnEquipmentIconAddedHook(ILContext il) {
			var cursor = new ILCursor(il).Goto(0);
			var setSubscribedInventory = typeof(ItemInventoryDisplay).GetMethodCached("SetSubscribedInventory");
			cursor.GotoNext(x => x.MatchCallvirt(setSubscribedInventory));
			cursor.Index += 1;

			cursor.Emit(OpCodes.Ldarg_0);

			cursor.EmitDelegate<Action<ScoreboardStrip>>(eq => {
				if (eq != null && eq.equipmentIcon != null) {
					OnEquipmentIconAdded?.Invoke(eq.equipmentIcon);
				}
			});
		}
	}
}
