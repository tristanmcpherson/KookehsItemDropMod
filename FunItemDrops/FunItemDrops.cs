using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace FunItemDrops
{
	[BepInDependency("com.bepis.r2api")]
	[BepInPlugin("FunItemDrops", "Fun Item Drops", "0.1.0")]
	public class FunItemDrops : BaseUnityPlugin
	{
		private Xoroshiro128Plus xoro = new Xoroshiro128Plus(45345354345834234);
		private static SpawnCard[] chests;

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

			chests = new []
			{
				Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscGoldChest"),
				Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscChest1"),
				Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscChest2")
			};
		

			//ItemDropAPI.AddDropInformation(ItemDropLocation.SmallChest, new List<ItemIndex> { ItemIndex.Hoof }.ToSelection(0.3f), t1.ToSelection(0.8f), t2.ToSelection(0.15f), t3.ToSelection(0.05f));
			//ItemDropAPI.AddDropInformation(ItemDropLocation.MediumChest, t2.ToSelection(0.7f), t3.ToSelection(0.3f));
			//ItemDropAPI.AddDropInformation(ItemDropLocation.LargeChest, t3.ToSelection());

			//ItemDropAPI.AddDropInformation(ItemDropLocation.Boss, t3.ToSelection());
			//ItemDropAPI.AddDropInformation(ItemDropLocation.Shrine, ItemDropAPI.None.ToSelection(0.5f), t1.ToSelection(0.8f), t2.ToSelection(0.2f), t3.ToSelection(0.03f));

			//// if i didn't care about equipment items:
			//// DefaultItemDrops.AddEquipmentChestDefaultDrops();
			//// but i do so:
			//ItemDropAPI.AddDropInformation(ItemDropLocation.EquipmentChest, eq.ToSelection());
			On.RoR2.UserProfile.HasAchievement += (orig, self, achievementName) => true;
			On.RoR2.UserProfile.HasDiscoveredPickup += (orig, self, achievementName) => true;
			On.RoR2.UserProfile.HasSurvivorUnlocked += (orig, self, achievementName) => true;
			On.RoR2.UserProfile.HasViewedViewable += (orig, self, achievementName) => true;
			On.RoR2.UserProfile.HasUnlockable_UnlockableDef += (orig, self, achievementName) => true;
			On.RoR2.UserProfile.HasUnlockable_string += (orig, self, achievementName) => true;


			//ItemDropAPI.ChestSpawnRate = 2f;
		}

		private static int i = 0;

		public void Update()
		{
			CheckDropItem(KeyCode.F2, new Lazy<List<PickupIndex>>(() => ItemDropAPI.GetDefaultDropList(ItemTier.Tier1).Select(x => new PickupIndex(x)).ToList()));
			CheckDropItem(KeyCode.F3, new Lazy<List<PickupIndex>>(() => ItemDropAPI.GetDefaultDropList(ItemTier.Tier2).Select(x => new PickupIndex(x)).ToList()));
			CheckDropItem(KeyCode.F4, new Lazy<List<PickupIndex>>(() => ItemDropAPI.GetDefaultDropList(ItemTier.Tier3).Select(x => new PickupIndex(x)).ToList()));
			CheckDropItem(KeyCode.F5, new Lazy<List<PickupIndex>>(() => ItemDropAPI.GetDefaultEquipmentDropList().Select(x => new PickupIndex(x)).ToList()));

			if (Input.GetKeyDown(KeyCode.F6))
			{
				PlayerCharacterMasterController.instances[0].master.GiveMoney(10000);

				var item = ItemIndex.BoostHp;

				var items = new List<PickupIndex>();
				items.Add(new PickupIndex(item)); // item that hasnt an actual prefab (default to exclamation mark) so fairly easy to recognize if it drops
				//ItemDropAPI.AddDrops(ItemDropLocation.LargeChest, items.ToSelection());

				var trans = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

				chests[i++%chests.Length].DoSpawn(trans.position, trans.rotation, null); 
			}
		}

		private static void CheckDropItem(KeyCode keyCode, Lazy<List<PickupIndex>> items) {

			if (!Input.GetKeyDown(keyCode) || !NetworkServer.active)
				return;
			//We grab a list of all available Tier 3 drops:
			var dropList = items.Value;

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
