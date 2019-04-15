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

namespace FunItemDrops
{
	[BepInDependency("com.bepis.r2api")]
	[BepInPlugin("FunItemDrops", "Fun Item Drops", "0.1.0")]
	public class FunItemDrops : BaseUnityPlugin
	{
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
		}

		public void Update()
		{
			CheckDropItem(KeyCode.F2, ItemDropAPI.GetDefaultDropList(ItemTier.Tier1).Select(x => new PickupIndex(x)).ToList());
			CheckDropItem(KeyCode.F3, ItemDropAPI.GetDefaultDropList(ItemTier.Tier2).Select(x => new PickupIndex(x)).ToList());
			CheckDropItem(KeyCode.F4, ItemDropAPI.GetDefaultDropList(ItemTier.Tier3).Select(x => new PickupIndex(x)).ToList());

			CheckDropItem(KeyCode.F5, ItemDropAPI.GetDefaultEquipmentDropList().Select(x => new PickupIndex(x)).ToList());

		}

		private static void CheckDropItem(KeyCode keyCode, List<PickupIndex> items) {
			if (!Input.GetKeyDown(keyCode) || !NetworkServer.active)
				return;
			//We grab a list of all available Tier 3 drops:
			var dropList = items;

			//Randomly get the next item:
			var nextItem = Run.instance.treasureRng.RangeInt(0, dropList.Count);

			//Get the player body to use a position:
			var playerTransform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

			if (playerTransform == null)
			{
				return;
			}

			//And then finally drop it infront of the player.
			PickupDropletController.CreatePickupDroplet(dropList[nextItem], playerTransform.position, playerTransform.forward * 20f);
		}
	}
}
