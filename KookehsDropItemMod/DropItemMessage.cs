using R2API.Networking.Interfaces;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace DropItems
{
	public class DropItemMessage : INetMessage
	{
		private NetworkInstanceId netId;
		private PickupIndex pickupIndex;

        public DropItemMessage() {
        }

		public DropItemMessage(NetworkInstanceId netId, PickupIndex pickupIndex)
		{
			this.netId = netId;
			this.pickupIndex = pickupIndex;
		}

		public void Serialize(NetworkWriter writer)
		{
			writer.Write(netId);
			PickupIndex.WriteToNetworkWriter(writer, pickupIndex);
		}

		public void Deserialize(NetworkReader reader)
		{
			netId = reader.ReadNetworkId();
			pickupIndex = reader.ReadPickupIndex();
		}

		public override string ToString() => $"DropItemMessage: {pickupIndex}";

		private void Log(string message) {
			KookehsDropItemMod.Logger.LogDebug(message);
		}

        public void OnReceived() {
			if (!NetworkServer.active) {
				return;
            }

			Log("Received kookeh drop message");
			Log("NetworkID" + netId.ToString());
            Log("PickupIndex" + this.pickupIndex.ToString());

			var bodyObject = Util.FindNetworkObject(netId);

			var body = bodyObject.GetComponent<CharacterBody>();
			Log("Body is null: " + (body == null).ToString());

			var inventory = body.master.inventory;
			var charTransform = body.GetFieldValue<Transform>("transform");

			DropItemHandler.DropItem(charTransform, inventory, pickupIndex);
			DropItemHandler.CreateNotification(body, charTransform, pickupIndex);
		}
    }
}