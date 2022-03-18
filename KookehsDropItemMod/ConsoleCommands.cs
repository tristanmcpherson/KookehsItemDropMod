using R2API.Utils;
using RoR2;
using System;
using UnityEngine;
using Console = RoR2.Console;

namespace DropItems
{
    public class ConsoleCommands
    {
		[ConCommand(commandName = "drop_item", flags = ConVarFlags.ExecuteOnServer, helpText = "Drops an item from your inventory")]
		private static void DropItemCommand(ConCommandArgs args) {
			var itemName = args.GetArgString(0);
			var itemIndex = ItemNameToIndex(itemName);
			if (itemIndex == ItemIndex.None) {
				Console.print("Can't find item specified");
				return;
            }

			var count = args.TryGetArgInt(1) ?? 1;
			KookehsDropItemMod.Logger.LogDebug("Item index: " + itemIndex);

			var master = args.GetSenderMaster();

			var inventory = master.inventory;
			var charTransform = master.GetBody().GetFieldValue<Transform>("transform");

			for (int i = 0; i < count; i++)
				DropItemHandler.DropItem(charTransform, inventory, PickupCatalog.FindPickupIndex(itemIndex));
		}

		[ConCommand(commandName = "drop_equip", flags = ConVarFlags.ExecuteOnServer, helpText = "Drops an item from your inventory")]
		private static void DropEquipCommand(ConCommandArgs args) {
			//Actual code here 
			var master = args.GetSenderMaster();

			var inventory = master.inventory;
			var charTransform = master.GetBody().GetFieldValue<Transform>("transform");

			DropItemHandler.DropItem(charTransform, inventory, PickupCatalog.FindPickupIndex(inventory.GetEquipmentIndex()));
		}

		public static ItemIndex ItemNameToIndex(string name) {
			if (Enum.TryParse(name, true, out ItemIndex foundItem) && ItemCatalog.IsIndexValid(foundItem)) {
				return foundItem;
			}

			foreach (var itemIndex in ItemCatalog.allItems) {
				var item = ItemCatalog.GetItemDef(itemIndex);
				if (item.name.ToUpper().Contains(name.ToUpper())) {
					return item.itemIndex;
				}
			}
			return ItemIndex.None;
		}
	}
}
