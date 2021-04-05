using R2API.Networking.Interfaces;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace DropItems
{
	public class DropItemMessage : INetMessage
	{
		public bool IsItem;
		private NetworkInstanceId NetId;
		private ItemIndex ItemIndex = ItemIndex.None;
		private EquipmentIndex EquipmentIndex = EquipmentIndex.None;

        public DropItemMessage() {
        }

		public DropItemMessage(NetworkInstanceId netId, ItemIndex itemIndex)
		{
			NetId = netId;
			ItemIndex = itemIndex;
			IsItem = true;
		}

		public DropItemMessage(NetworkInstanceId netId, EquipmentIndex equipmentIndex)
		{
			NetId = netId;
			EquipmentIndex = equipmentIndex;
			IsItem = false;
		}

		public void Serialize(NetworkWriter writer)
		{
			var isItem = (ItemIndex != ItemIndex.None);
			writer.Write(NetId);
			writer.Write(isItem);
			writer.Write(isItem ? (int)ItemIndex : (int)EquipmentIndex);
		}

		public void Deserialize(NetworkReader reader)
		{
			NetId = reader.ReadNetworkId();
			var index = reader.ReadInt32();
			IsItem = reader.ReadBoolean();

			if (IsItem)
			{
				ItemIndex = (ItemIndex) index;
			}
			else
			{
				EquipmentIndex = (EquipmentIndex) index;
			}
		}

		public override string ToString() => $"DropItemMessage: {(IsItem ? ItemIndex.ToString() : EquipmentIndex.ToString())}";

		private void Log(string message) {
			KookehsDropItemMod.Logger.LogDebug(message);
		}

        public void OnReceived() {
			if (!NetworkServer.active) {
				return;
            }

			Log("Received kookeh drop message");
			Log(NetId.ToString());

			
			var bodyObject = Util.FindNetworkObject(NetId);

			var body = bodyObject.GetComponentInParent<CharacterBody>();

			var inventory = body.master.inventory;
			var charTransform = body.GetFieldValue<Transform>("transform");

			var pickupIndex = IsItem
				? PickupCatalog.FindPickupIndex(ItemIndex)
				: PickupCatalog.FindPickupIndex(EquipmentIndex);

			DropItemHandler.DropItem(charTransform, inventory, pickupIndex);
			DropItemHandler.CreateNotification(body, charTransform, pickupIndex);
		}
    }
}