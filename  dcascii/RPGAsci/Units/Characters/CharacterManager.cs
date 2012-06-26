using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RPGAsci
{
	struct LevelChoice
	{
		public string text;
		public Unit unitChanges;
		public Skill skill;
		public LevelChoice(string text,Unit unitChanges,Skill skill)
		{
			this.text = text;
			this.unitChanges = unitChanges;
			this.skill = skill;
		}
	}
	
	class CharacterManager
	{
		private static Dictionary<string, List<List<LevelChoice>>> choices = new Dictionary<string, List<List<LevelChoice>>>();
		private static Dictionary<string, Skill> skills = new Dictionary<string, Skill>();

		static public void Init()
		{
			InitSkills();
		}
		
		static private void InitSkills()
		{
			//Retiree
			skills["Screech"] = new Skill(true, "All", "Screech", new Effect(0, 0, 0, -5, 0, 0, StatusEffect.None, false, 2),1f);
			skills["CheatDeath"] = new Skill("CheatDeath");
			skills["Nap"] = new Skill(false, "Self", "Nap", new Effect(3, StatusEffect.Sleep), 1f);
			skills["OldWarStories"] = new Skill(true,"All","OldWarStories",new Effect(0,StatusEffect.Sleep),1f);
			skills["FoxtrotSlash"] = new Skill(true, "All", "FoxtrotSlash", new Effect(3),0.9f);
			skills["GrumpyBlow"] = new Skill(true, "Single", "GrumpyBlow", new Effect(5),0.9f);
			skills["StrayCats"] = new Skill(true,"All","StrayCats",new Effect(10),1f);
			skills["EyePoke"] = new Skill(true,"Single","EyePoke",new Effect(30),0.9f);
			
			//Octonoid
			skills["TentacleNursing"] = new Skill(false, "Self", "TentacleNursing", new Effect(0, 0, 3, 0, 0, 0, StatusEffect.None, false, 3),1f);
			skills["Entangle"] = new Skill(true,"Single","Entangle",new Effect(0,3,0,0,0,0,StatusEffect.None,false,5),0.9f);
			skills["MightyGrip"] = new Skill(true,"All","MightyGrip",new Effect(0,6,0,0,0,0,StatusEffect.None,false,5),0.8f);
			skills["UncomfortableHug"] = new Skill(true,"Single","UncomfortableHug", new Effect(0,20,0,0,0,0,StatusEffect.None,false,10),1f);
			skills["TentacleMassage"] = new Skill(false,"Single","TentacleMassage",new Effect(5,false),1);
			skills["InkySaliva"] = new Skill(true,"Single","InkySaliva",new Effect(3,0,-5,0,0,0,StatusEffect.None,false,2),0.9f);
			skills["PoisonSlime"] = new Skill(true,"Single","PoisonSlime",null,0.7f);
			skills["MultiTentacleMassage"] = new Skill(false,"All","MultenticaleMassage",new Effect(15,false),1f);
			
			//BlackSmithstring image,string name
			skills["MightyBlow"] = new Skill(true, "Single", "MightyBlow", new Effect(10),0.5f);
			skills["ExplosiveAnger"] = new Skill(true, "All", "ExplosiveAnger", new Effect(30), 0.3f);
			skills["SpecialBooze"] = new Skill(false, "Self", "SpecialBooze", new Effect(0,0, 5, 0, -3, 0, StatusEffect.None, false, 3),1f);
			skills["PureAlcohol"] = new Skill(false, "Self", "PureAlcohol", new Effect(0, 0, 10, 0, -6, 0, StatusEffect.None, false,3),1f);
			skills["Forgery"] = new Skill("Forgery");
			skills["Scavenger"] = new Skill("Scavenger");
			skills["GreatWall"] = new Skill(true, "All", "GreatWall", new Effect(0, 0,0, 6, 0, 0, StatusEffect.None, false, 0), 0.7f);
			skills["MonsterSmithing"] = new Skill(false, "All", "MonsterSmithing", new Effect(15, false),  1f);

			//Nurse
			skills["Patting"] = new Skill(true, "Single", "Patting", new Effect(10), 0.5f);
			skills["SuperiorNursing"] = new Skill(true, "All", "SuperiorNursing", new Effect(30), 0.3f);
			skills["Revive"] = new Skill(false, "Single", "Revive", new Effect(10), 1f);
			skills["MedicalEducation"] = new Skill(false, "All", "MedicalEducation", new Effect(5, 0, 0, 0, 0, 0, StatusEffect.None, false, 5), 1f);
			skills["FirstAidPacks"] = new Skill(false, "All", "FirstAidPacks", new Effect(10,false),1f);
			skills["NurseBrew"] = new Skill(false, "Single", "NurseBrew", new Effect(0,StatusEffect.CureAll),0.7f);
			skills["CuteSmile"] = new Skill(false, "Self", "CuteSmile", null,0.0f);
			skills["OverprotectiveBehavior"] = new Skill("OverprotectiveBehavior");

			//Thief
			skills["Lockpicking"] = new Skill("Lockpicking");
			skills["Fleshpicking"] = new Skill(true, "All", "Fleshpicking", new Effect(10), 0.95f);
			skills["Swifty"] = new Skill(false, "Self", "Swifty", new Effect(5), 0.8f);
			skills["Borrow"] = new Skill(false, "Self", "Borrow", null, 0.3f);
			skills["Cloak"] = new Skill(false, "Single", "Cloak", new Effect(0, 0, 0, 0, 5, 0, StatusEffect.None, false, 5), 1f);
			skills["DollarSigns"] = new Skill(false, "Single", "DollarSigns", null,0.9f);
			skills["BootyHunter"] = new Skill(false, "Single", "BootyHunter", null,0.9f);
			skills["Arm"] = new Skill(false, "Single", "Arm", new Effect(15, false), 1f);

			//Adventurer
			skills["PowerAttack"] = new Skill(true, "Single", "PowerAttack", new Effect(6),0.8f);
			skills["TornadoSlash"] = new Skill(true, "All", "TornadoSlash", new Effect(10),0.95f);
			skills["BodySlam"] = new Skill(true, "Single", "BodySlam", new Effect(15),  0.9f);
			skills["WarRoar"] = new Skill(false, "All", "WarRoar", new Effect(0,0,5,0,0,0,StatusEffect.None,false,3),1f);
			skills["EncourageAll"] = new Skill(false, "All", "EncourageAll", new Effect(0, 0, 0,5, 0, 0, StatusEffect.None, false, 3),1f);
			skills["EncourageWar"] = new Skill(false, "All", "EncourageWar", new Effect(0, 0, 6, 6, 0, 0, StatusEffect.None, false, 3),1f);
			skills["StunningBlow"] = new Skill(true, "Single", "StunningBlow", new Effect(0, 20, 0, -5, 0, 0, StatusEffect.None, false, 3),0.6f);
			skills["OverKill"] = new Skill(true, "All", "OverKill", new Effect(30),0.8f);

			//Ranger
			skills["DungeonExpert"] = new Skill("DungeonExpert");
			skills["DefensiveHedge"] = new Skill(false, "All", "DefensiveBarrier", new Effect(0, 0, 0, 3, 0, 0, StatusEffect.None, false, 3),1);
			skills["TalentedBunch"] = new Skill(false, "All", "TalentedBunch", new Effect(0, 0, 0, 0, 0, 10, StatusEffect.None, false, 5),1);
			skills["StrangeShrooms"] = new Skill(true, "Self", "StrangeShrooms", new Effect(0,0,5,-3,5,-3,StatusEffect.None,false,3),1f);
			skills["SniperScope"] = new Skill(true, "Single", "SniperScope", new Effect(5),1);
			skills["FungalArrow"] = new Skill(true, "Single", "FungalArrow", new Effect(0,10,0,0,0,0,StatusEffect.Poison,false,0),0.9f);
			skills["SleepyTime"] = new Skill(true, "All", "SleepyTime", new Effect(0,5,0,0,0,0,StatusEffect.Sleep,false,0),1);
			skills["MassConfusion"] = new Skill(true, "All", "MassConfusion", new Effect(0, 20, 0, 0, 0, 10, StatusEffect.Confusion, false, 5),1);
		}

		static public Skill GetSkill(string name)
		{
			Regex r = new Regex("[0-9]");
			Match match = r.Match(name);
			if (match.Success)
			{
				return skills[name.Remove(0, 3)];
			}
			else
			{
				return skills[name];
			}
		}

	}
}
