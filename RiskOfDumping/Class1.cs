using BepInEx;
using Newtonsoft.Json;
using R2API;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RiskOfDumping
{
	[BepInDependency("com.bepis.r2api")]
	[BepInPlugin("com.github.tristanmcpherson.riskofdumping", "RiskOfDumping", "0.1.0")]
	public class Dumper : BaseUnityPlugin {
		void Awake() {
			var dump = new Dump();
			DumpBodyNames(dump);

			File.WriteAllText("Dump.json", JsonConvert.SerializeObject(dump));
		}

		void DumpBodyNames(Dump dump) {
			var validDefinitions = SurvivorAPI.SurvivorDefinitions;
			var validBodyNames = validDefinitions.Select(def => def.bodyPrefab.name);

			dump.BodyNames = validBodyNames.ToList();
		}
	}

	public class Dump
	{
		public List<string> BodyNames { get; set; }
	}
}
