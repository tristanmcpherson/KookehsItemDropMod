using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.ConVar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ItemSelect
{
	[BepInDependency("com.bepis.r2api")]
	[BepInPlugin("ItemSelect", "Item Select", "0.1.0")]
	public class ItemSelect : BaseUnityPlugin
	{
		public void Awake()
		{
			//On.RoR2.RuleCatalog.HiddenTestItemsConvar += (orig) => false; 

			//On.RoR2.UI.MainMenu.MainMenuController.Start += (orig, self) => {

			//	orig(self);


			//	var flags = BindingFlags.NonPublic | BindingFlags.Static;
			//	var allChoicesDefs = (List < RuleChoiceDef > )typeof(RuleCatalog).GetField("allChoicesDefs", flags).GetValue(null);

			//	//allChoicesDefs = ((RuleCatalog)null).CGetFieldValue<List<RuleChoiceDef>>("allChoicesDefs", flags);
			//	var allRuleDefs = (List<RuleDef>)typeof(RuleCatalog).GetField("allRuleDefs", flags).GetValue(null);


			//	var addCategory = typeof(RuleCatalog).GetMethod("AddCategory", flags, null, new Type[] { typeof(string), typeof(Color), typeof(string), typeof(Func<bool>) }, null);
			//	var addRule = typeof(RuleCatalog).GetMethod("AddRule", flags);
			//	//addCategory.Invoke(null, new object[] { "RULE_HEADER_MISC", new Color(192, 192, 192, byte.MaxValue), null, false });

			//	addCategory.Invoke(null, new object[] { "RULE_HEADER_ITEMS", new Color(147, 225, 128, byte.MaxValue), null, new Func<bool>(() => false) });
			//	List<ItemIndex> list = new List<ItemIndex>();
			//	for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++) {
			//		list.Add(itemIndex);
			//	}
			//	foreach (ItemIndex itemIndex2 in from i in list
			//									 where ItemCatalog.GetItemDef(i).inDroppableTier
			//									 orderby ItemCatalog.GetItemDef(i).tier
			//									 select i) {
			//		var ruleDef = RuleDef.FromItem(itemIndex2);



			//		addRule.Invoke(null, new object[] { ruleDef });
			//	}
			//	addCategory.Invoke(null, new object[] { "RULE_HEADER_EQUIPMENT", new Color(byte.MaxValue, 128, 0, byte.MaxValue), null, new Func<bool>(() => false) });
			//	List<EquipmentIndex> list2 = new List<EquipmentIndex>();
			//	for (EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile; equipmentIndex < EquipmentIndex.Count; equipmentIndex++) {
			//		list2.Add(equipmentIndex);
			//	}
			//	foreach (EquipmentIndex equipmentIndex2 in from i in list2
			//											   where EquipmentCatalog.GetEquipmentDef(i).canDrop
			//											   select i) {
			//		var ruleDef = RuleDef.FromEquipment(equipmentIndex2);


			//		addRule.Invoke(null, new object[] { ruleDef });
			//	}



			//	for (int k = 0; k < allRuleDefs.Count; k++) {
			//		RuleDef ruleDef4 = allRuleDefs[k];
			//		ruleDef4.globalIndex = k;
			//		for (int j = 0; j < ruleDef4.choices.Count; j++) {
			//			RuleChoiceDef ruleChoiceDef6 = ruleDef4.choices[j];
			//			ruleChoiceDef6.localIndex = j;
			//			ruleChoiceDef6.globalIndex = allChoicesDefs.Count;
			//			allChoicesDefs.Add(ruleChoiceDef6);
			//		}
			//	}

			//	RuleCatalog.availability.MakeAvailable();

			//};

			//for (int k = 0; k < RuleCatalog.allRuleDefs.Count; k++) {
			//	RuleDef ruleDef4 = RuleCatalog.allRuleDefs[k];
			//	ruleDef4.globalIndex = k;
			//	for (int j = 0; j < ruleDef4.choices.Count; j++) {
			//		RuleChoiceDef ruleChoiceDef6 = ruleDef4.choices[j];
			//		ruleChoiceDef6.localIndex = j;
			//		ruleChoiceDef6.globalIndex = RuleCatalog.allChoicesDefs.Count;
			//		RuleCatalog.allChoicesDefs.Add(ruleChoiceDef6);
			//	}
			//}
			//RuleCatalog.availability.MakeAvailable();
		}
	}
}
