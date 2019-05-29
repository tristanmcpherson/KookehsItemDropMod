using RoR2;
using UnityEngine.Networking;

namespace DropItems
{
	public class DropItemMessage : MessageBase
	{
		public bool IsItem;
		public ItemIndex ItemIndex = ItemIndex.None;
		public EquipmentIndex EquipmentIndex = EquipmentIndex.None;

		public DropItemMessage(ItemIndex itemIndex)
		{
			ItemIndex = itemIndex;
			IsItem = true;
		}

		public DropItemMessage(EquipmentIndex equipmentIndex)
		{
			EquipmentIndex = equipmentIndex;
			IsItem = false;
		}

		public override void Serialize(NetworkWriter writer)
		{
			var isItem = (ItemIndex != ItemIndex.None);
			writer.Write(isItem);
			writer.Write(isItem ? (int)ItemIndex : (int)EquipmentIndex);
		}

		public override void Deserialize(NetworkReader reader)
		{
			IsItem = reader.ReadBoolean();
			var index = reader.ReadInt32();
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
	}
}