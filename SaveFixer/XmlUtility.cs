/* 
	Licensed under the Apache License, Version 2.0

	http://www.apache.org/licenses/LICENSE-2.0
	*/
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace Xml2CSharp
{
	[XmlRoot(ElementName = "stat")]
	public class Stat
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "stats")]
	public class Stats
	{
		[XmlElement(ElementName = "stat")]
		public List<Stat> Stat { get; set; }
		[XmlElement(ElementName = "unlock")]
		public List<string> Unlock { get; set; }
	}

	[XmlRoot(ElementName = "SkillPreference")]
	public class SkillPreference
	{
		[XmlAttribute(AttributeName = "skillFamily")]
		public string SkillFamily { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "BodyLoadout")]
	public class BodyLoadout
	{
		[XmlElement(ElementName = "Skin")]
		public string Skin { get; set; }
		[XmlElement(ElementName = "SkillPreference")]
		public List<SkillPreference> SkillPreference { get; set; }
		[XmlAttribute(AttributeName = "bodyName")]
		public string BodyName { get; set; }
	}

	[XmlRoot(ElementName = "BodyLoadouts")]
	public class BodyLoadouts
	{
		[XmlElement(ElementName = "BodyLoadout")]
		public List<BodyLoadout> BodyLoadout { get; set; }
	}

	[XmlRoot(ElementName = "loadout")]
	public class Loadout
	{
		[XmlElement(ElementName = "BodyLoadouts")]
		public BodyLoadouts BodyLoadouts { get; set; }
	}

	[XmlRoot(ElementName = "UserProfile")]
	public class UserProfile
	{
		[XmlElement(ElementName = "achievementsList")]
		public string AchievementsList { get; set; }
		[XmlElement(ElementName = "coins")]
		public string Coins { get; set; }
		[XmlElement(ElementName = "discoveredPickups")]
		public string DiscoveredPickups { get; set; }
		[XmlElement(ElementName = "gamepadVibrationScale")]
		public string GamepadVibrationScale { get; set; }
		[XmlElement(ElementName = "joystickMap")]
		public string JoystickMap { get; set; }
		[XmlElement(ElementName = "keyboardMap")]
		public string KeyboardMap { get; set; }
		[XmlElement(ElementName = "mouseLookInvertX")]
		public string MouseLookInvertX { get; set; }
		[XmlElement(ElementName = "mouseLookInvertY")]
		public string MouseLookInvertY { get; set; }
		[XmlElement(ElementName = "mouseLookScaleX")]
		public string MouseLookScaleX { get; set; }
		[XmlElement(ElementName = "mouseLookScaleY")]
		public string MouseLookScaleY { get; set; }
		[XmlElement(ElementName = "mouseLookSensitivity")]
		public string MouseLookSensitivity { get; set; }
		[XmlElement(ElementName = "mouseMap")]
		public string MouseMap { get; set; }
		[XmlElement(ElementName = "name")]
		public string Name { get; set; }
		[XmlElement(ElementName = "screenShakeScale")]
		public string ScreenShakeScale { get; set; }
		[XmlElement(ElementName = "stickLookInvertX")]
		public string StickLookInvertX { get; set; }
		[XmlElement(ElementName = "stickLookInvertY")]
		public string StickLookInvertY { get; set; }
		[XmlElement(ElementName = "stickLookScaleX")]
		public string StickLookScaleX { get; set; }
		[XmlElement(ElementName = "stickLookScaleY")]
		public string StickLookScaleY { get; set; }
		[XmlElement(ElementName = "stickLookSensitivity")]
		public string StickLookSensitivity { get; set; }
		[XmlElement(ElementName = "totalAliveSeconds")]
		public string TotalAliveSeconds { get; set; }
		[XmlElement(ElementName = "totalCollectedCoins")]
		public string TotalCollectedCoins { get; set; }
		[XmlElement(ElementName = "totalLoginSeconds")]
		public string TotalLoginSeconds { get; set; }
		[XmlElement(ElementName = "totalRunCount")]
		public string TotalRunCount { get; set; }
		[XmlElement(ElementName = "totalRunSeconds")]
		public string TotalRunSeconds { get; set; }
		[XmlElement(ElementName = "unviewedAchievementsList")]
		public string UnviewedAchievementsList { get; set; }
		[XmlElement(ElementName = "version")]
		public string Version { get; set; }
		[XmlElement(ElementName = "viewedUnlockablesList")]
		public string ViewedUnlockablesList { get; set; }
		[XmlElement(ElementName = "viewedViewables")]
		public string ViewedViewables { get; set; }
		[XmlElement(ElementName = "stats")]
		public Stats Stats { get; set; }
		[XmlElement(ElementName = "tutorialDifficulty")]
		public string TutorialDifficulty { get; set; }
		[XmlElement(ElementName = "tutorialEquipment")]
		public string TutorialEquipment { get; set; }
		[XmlElement(ElementName = "tutorialSprint")]
		public string TutorialSprint { get; set; }
		[XmlElement(ElementName = "loadout")]
		public Loadout Loadout { get; set; }
	}

}