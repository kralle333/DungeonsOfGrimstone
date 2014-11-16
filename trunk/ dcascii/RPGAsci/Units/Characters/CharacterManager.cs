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
		public LevelChoice(string text, Unit unitChanges, Skill skill)
		{
			this.text = text;
			this.unitChanges = unitChanges;
			this.skill = skill;
		}
	}

	class CharacterManager
	{
		private static Dictionary<string, Skill> skills = new Dictionary<string, Skill>();

		static public void Init()
		{
			InitSkills();
		}

		static private void InitSkills()
		{
			//Retiree
			skills["Screech"] = new Skill(true, "All", "Screech", new Effect(0, 0, 0, -5, 0, 0, StatusEffect.None, false, 2), 1f, new Skill.SkillGrowthRate(1, 1, 1, 1.5f, 1, 1, 1.4f, 1, 1));
			skills["CheatDeath"] = new Skill("CheatDeath");
			skills["Nap"] = new Skill(false, "Self", "Nap", new Effect(3, StatusEffect.Sleep), 1f, new Skill.SkillGrowthRate(1.5f, 1, 1, 1, 1, 1, 1, 1, 1));
			skills["OldWarStories"] = new Skill(true, "All", "OldWarStories", new Effect(0, StatusEffect.Sleep), 1f, new Skill.SkillGrowthRate());
			skills["FoxtrotSlash"] = new Skill(true, "All", "FoxtrotSlash", new Effect(3), 0.9f, new Skill.SkillGrowthRate(0, 1.2f,1,1,1,1,1,1, 1));
			skills["GrumpyBlow"] = new Skill(true, "Single", "GrumpyBlow", new Effect(5), 0.9f, new Skill.SkillGrowthRate(0, 1.5f, 1, 1, 1, 1, 1, 1, 1));
			skills["StrayCats"] = new Skill(true, "All", "StrayCats", new Effect(10), 1f, new Skill.SkillGrowthRate(0, 1.4f, 1, 1, 1, 1, 1, 1, 1));
			skills["EyePoke"] = new Skill(true, "Single", "EyePoke", new Effect(30), 0.9f, new Skill.SkillGrowthRate(0, 1.7f, 1, 1, 1, 1, 1, 1, 1));

			//Octonoid
			skills["TentacleNursing"] = new Skill(false, "Self", "TentacleNursing", new Effect(0, 0, 3, 0, 0, 0, StatusEffect.None, false, 3), 1f, new Skill.SkillGrowthRate(0, 0, 1.4f, 1, 1, 1, 1, 1, 1));
			skills["Entangle"] = new Skill(true, "Single", "Entangle", new Effect(0, 3, 0, 0, 0, 0, StatusEffect.None, false, 5), 0.9f, new Skill.SkillGrowthRate(0, 1.1f, 1, 1, 1, 1, 1.1f, 0, 2));
			skills["MightyGrip"] = new Skill(true, "All", "MightyGrip", new Effect(0, 6, 0, 0, 0, 0, StatusEffect.None, false, 5), 0.8f, new Skill.SkillGrowthRate(0, 1.2f, 1, 1, 1, 1, 1.2f, 0, 2));
			skills["UncomfortableHug"] = new Skill(true, "Single", "UncomfortableHug", new Effect(0, 20, 0, 0, 0, 0, StatusEffect.None, false, 10), 1f, new Skill.SkillGrowthRate(0, 1.2f, 1, 1, 1, 1, 1.2f, 0, 1));
			skills["TentacleMassage"] = new Skill(false, "Single", "TentacleMassage", new Effect(5, false), 1, new Skill.SkillGrowthRate(2, 1, 1, 1, 1, 1, 1, 1, 2));
			skills["InkySaliva"] = new Skill(true, "Single", "InkySaliva", new Effect(3, 0, -5, 0, 0, 0, StatusEffect.None, false, 2), 0.9f, new Skill.SkillGrowthRate());
			skills["PoisonSlime"] = new Skill(true, "Single", "PoisonSlime", null, 0.7f, new Skill.SkillGrowthRate());
			skills["MultiTentacleMassage"] = new Skill(false, "All", "MultiTentacleMassage", new Effect(15, false), 1f, new Skill.SkillGrowthRate());

			//BlackSmith
			skills["MightyBlow"] = new Skill(true, "Single", "MightyBlow", new Effect(10), 0.5f, new Skill.SkillGrowthRate());
			skills["ExplosiveAnger"] = new Skill(true, "All", "ExplosiveAnger", new Effect(30), 0.3f, new Skill.SkillGrowthRate());
			skills["SpecialBooze"] = new Skill(false, "Self", "SpecialBooze", new Effect(0, 0, 5, 0, -3, 0, StatusEffect.None, false, 3), 1f, new Skill.SkillGrowthRate());
			skills["PureAlcohol"] = new Skill(false, "Self", "PureAlcohol", new Effect(0, 0, 10, 0, -6, 0, StatusEffect.None, false, 3), 1f, new Skill.SkillGrowthRate());
			skills["Forgery"] = new Skill("Forgery");
			skills["Scavenger"] = new Skill("Scavenger");
			skills["GreatWall"] = new Skill(true, "All", "GreatWall", new Effect(0, 0, 0, 6, 0, 0, StatusEffect.None, false, 0), 0.7f, new Skill.SkillGrowthRate());
			skills["MonsterSmithing"] = new Skill(false, "All", "MonsterSmithing", new Effect(15, false), 1f, new Skill.SkillGrowthRate());

			//Nurse
			skills["Patting"] = new Skill(true, "Single", "Patting", new Effect(10), 0.5f, new Skill.SkillGrowthRate());
			skills["SuperiorNursing"] = new Skill(true, "All", "SuperiorNursing", new Effect(30), 0.3f, new Skill.SkillGrowthRate());
			skills["Revive"] = new Skill(false, "Single", "Revive", new Effect(10), 1f, new Skill.SkillGrowthRate());
			skills["MedicalEducation"] = new Skill(false, "All", "MedicalEducation", new Effect(5, 0, 0, 0, 0, 0, StatusEffect.None, false, 5), 1f, new Skill.SkillGrowthRate());
			skills["FirstAidPacks"] = new Skill(false, "All", "FirstAidPacks", new Effect(10, false), 1f, new Skill.SkillGrowthRate());
			skills["NurseBrew"] = new Skill(false, "Single", "NurseBrew", new Effect(0, StatusEffect.CureAll), 0.7f, new Skill.SkillGrowthRate());
			skills["CuteSmile"] = new Skill(false, "Self", "CuteSmile", null, 0.0f, new Skill.SkillGrowthRate());
			skills["OverprotectiveBehavior"] = new Skill("OverprotectiveBehavior");

			//Thief
			skills["LockPicking"] = new Skill("Lockpicking");
			skills["FleshPicking"] = new Skill(true, "All", "Fleshpicking", new Effect(10), 0.95f, new Skill.SkillGrowthRate());
			skills["Swifty"] = new Skill(false, "Self", "Swifty", new Effect(5), 0.8f, new Skill.SkillGrowthRate());
			skills["Borrow"] = new Skill(false, "Self", "Borrow", null, 0.3f, new Skill.SkillGrowthRate());
			skills["Cloak"] = new Skill(false, "Single", "Cloak", new Effect(0, 0, 0, 0, 5, 0, StatusEffect.None, false, 5), 1f, new Skill.SkillGrowthRate());
			skills["DollarSigns"] = new Skill(false, "Single", "DollarSigns", null, 0.9f, new Skill.SkillGrowthRate());
			skills["BootyHunter"] = new Skill(false, "Single", "BootyHunter", null, 0.9f, new Skill.SkillGrowthRate());
			skills["Arm"] = new Skill(false, "Single", "Arm", new Effect(15, false), 1f, new Skill.SkillGrowthRate());

			//Adventurer
			skills["PowerAttack"] = new Skill(true, "Single", "PowerAttack", new Effect(10), 1f, new Skill.SkillGrowthRate(1, 1.1f, 1, 1, 1, 1, 1,1, 1));
			skills["TornadoSlash"] = new Skill(true, "All", "TornadoSlash", new Effect(13), 0.95f, new Skill.SkillGrowthRate());
			skills["BodySlam"] = new Skill(true, "Single", "BodySlam", new Effect(15), 0.9f, new Skill.SkillGrowthRate());
			skills["WarRoar"] = new Skill(false, "All", "WarRoar", new Effect(0, 0, 5, 0, 0, 0, StatusEffect.None, false, 3), 1f, new Skill.SkillGrowthRate());
			skills["EncourageAll"] = new Skill(false, "All", "EncourageAll", new Effect(0, 0, 0, 5, 0, 0, StatusEffect.None, false, 3), 1f, new Skill.SkillGrowthRate());
			skills["EncourageWar"] = new Skill(false, "All", "EncourageWar", new Effect(0, 0, 6, 6, 0, 0, StatusEffect.None, false, 3), 1f, new Skill.SkillGrowthRate());
			skills["StunningBlow"] = new Skill(true, "Single", "StunningBlow", new Effect(0, 20, 0, -5, 0, 0, StatusEffect.None, false, 3), 0.6f, new Skill.SkillGrowthRate(1, 1.1f, 1, 1, 1, 1, 1, 1, 1));
			skills["OverKill"] = new Skill(true, "All", "OverKill", new Effect(30), 0.8f, new Skill.SkillGrowthRate());

			//Ranger
			skills["DungeonExpert"] = new Skill("DungeonExpert");
			skills["DefensiveHedge"] = new Skill(false, "All", "DefensiveBarrier", new Effect(0, 0, 0, 3, 0, 0, StatusEffect.None, false, 3), 1, new Skill.SkillGrowthRate());
			skills["TalentedBunch"] = new Skill(false, "All", "TalentedBunch", new Effect(0, 0, 0, 0, 0, 10, StatusEffect.None, false, 5), 1, new Skill.SkillGrowthRate());
			skills["StrangeShrooms"] = new Skill(true, "Self", "StrangeShrooms", new Effect(0, 0, 5, -3, 5, -3, StatusEffect.None, false, 3), 1f, new Skill.SkillGrowthRate());
			skills["SniperScope"] = new Skill(true, "Single", "SniperScope", new Effect(5), 1, new Skill.SkillGrowthRate());
			skills["FungalArrow"] = new Skill(true, "Single", "FungalArrow", new Effect(0, 10, 0, 0, 0, 0, StatusEffect.Poison, false, 0), 0.9f, new Skill.SkillGrowthRate());
			skills["SleepyTime"] = new Skill(true, "All", "SleepyTime", new Effect(0, 5, 0, 0, 0, 0, StatusEffect.Sleep, false, 0), 1, new Skill.SkillGrowthRate());
			skills["MassConfusion"] = new Skill(true, "All", "MassConfusion", new Effect(0, 20, 0, 0, 0, 10, StatusEffect.Confusion, false, 5), 1, new Skill.SkillGrowthRate());
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
