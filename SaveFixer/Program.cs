using Newtonsoft.Json;
using RiskOfDumping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using UserProfile = Xml2CSharp.UserProfile;

namespace SaveFixer
{
	class Program
	{
		static void Main(string[] args) {
			var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml");

			foreach (var file in files) {
				UserProfile userProfile;
				Dump dump = JsonConvert.DeserializeObject<Dump>("Dump.json");

				var serializer = new XmlSerializer(typeof(UserProfile));

				using (var stream = new StringReader(file))
				using (var reader = XmlReader.Create(stream)) {
					userProfile = (UserProfile)serializer.Deserialize(reader);

					FixLoadouts(userProfile, dump);

				}
			}
		}

		static void FixLoadouts(UserProfile userProfile, Dump dump) {
			var bodiesToRemove = new List<string>();

			foreach (var body in userProfile.Loadout.BodyLoadouts.BodyLoadout) {
				if (!dump.BodyNames.Contains(body.BodyName)) {
					bodiesToRemove.Add(body.BodyName);
				}
			}

			userProfile.Loadout.BodyLoadouts.BodyLoadout.RemoveAll(b => bodiesToRemove.Contains(b.BodyName));
		}
	}
}
