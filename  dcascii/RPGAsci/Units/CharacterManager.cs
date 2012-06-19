using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
	struct LevelUpGrowthRate
	{
		public float hp;
		public float attack;
		public float defense;
		public float speed;
		public float talent;
		public LevelUpGrowthRate(float hp,float attack,float defense,float speed,float talent)
		{
			this.hp = hp;
			this.attack = attack;
			this.defense = defense;
			this.speed = speed;
			this.talent = talent;
		}
	}
	class CharacterManager
	{
		private static Dictionary<string, List<List<LevelChoice>>> choices = new Dictionary<string, List<List<LevelChoice>>>();
		private static Dictionary<string, Skill> skills = new Dictionary<string, Skill>();
		private static Dictionary<string, LevelUpGrowthRate> growthRates = new Dictionary<string, LevelUpGrowthRate>();
		static public void Init()
		{
			InitSkills();
			InitChoices();
			InitGrowthRates();
		}
		static public List<string> GetLevelChoices(Character character)
		{
			List<string> currentChoices = new List<string>();
			List<LevelChoice> classChoices = choices[character.classType][character.level-1];
			for (int i = 0; i < classChoices.Count(); i++)
			{
				currentChoices.Add(classChoices[i].text);
			}
			return currentChoices;
		}
		static public void SetLevelChanges(Character character)
		{
			int newHP = (int)Math.Round(character.hp * Math.Pow(1 + growthRates[character.classType].hp, character.level));
			int newAttack = (int)Math.Round(character.attack * Math.Pow(1 + growthRates[character.classType].attack, character.level));
			int newDefense = (int)Math.Round(character.defense * Math.Pow(1 + growthRates[character.classType].defense, character.level));
			int newSpeed = (int)Math.Round(character.speed * Math.Pow(1 + growthRates[character.classType].speed, character.level));
			int newTalent = (int)Math.Round(character.talent * Math.Pow(1 + growthRates[character.classType].talent, character.level));
			Console.BackgroundColor = ConsoleColor.Blue;
			Console.WriteLine("Hp:+{0} Attack:+{1} Defense:+{2} Speed:+{3} Talent:+{4} ",newHP-character.hp,newAttack-character.attack, newDefense-character.defense,newSpeed-character.speed, newTalent-character.talent);
			Console.ResetColor();
			character.hp = newHP;
			character.attack = newAttack;
			character.defense = newDefense;
			character.speed = newSpeed;
			character.talent = newTalent;
		}
		static public void SetChoice(Character character, string choice)
		{
			int skillAmount = 1;
			if (int.TryParse(choice, out skillAmount))
			{
				choice.Remove(0, 2);
			}
			else
			{
				skillAmount = 1;
			}
			LevelChoice c = choices[character.classType][character.level].Find(x => x.text == choice);
			if (c.skill != null)
			{
				for (int i = 0; i < skillAmount; i++)
				{
					character.skills.Add(c.skill);
				}				
			}
			if (c.unitChanges != null)
			{
				character.hp += c.unitChanges.hp;
				character.attack += c.unitChanges.attack;
				character.defense += c.unitChanges.defense;
				character.speed += c.unitChanges.speed;
				character.talent += c.unitChanges.talent;
			}
		}
		static private void InitSkills()
		{
			//Wizard
			skills["Heal"] = new Skill(false, "Single", "Heal", new Effect(5,false));
			skills["Shock"] = new Skill(true, "Single", "Shock", new Effect(5));
			skills["HealAll"] = new Skill(false, "All", "HealAll", new Effect(3,false));
			skills["FireBlast"] = new Skill(true, "All", "FireBlast", new Effect(7));
			skills["HealBeam"] = new Skill(false, "Single", "HealBeam", new Effect(32,false));
			skills["LightningBolt"] = new Skill(false, "Single", "LightningBolt", new Effect(50));

			//Fighter
			skills["Meditate"] = new Skill(false, "Self", "Meditate", new Effect(3,false));
			skills["PowerAttack"] = new Skill(true, "Single", "PowerAttack", new Effect(6));
			skills["TornadoSlash"] = new Skill(true, "All", "TornadoSlash", new Effect(10));
			skills["WarRoar"] = new Skill(false, "Self", "WarRoar", new Effect(0,0,5,0,0,0,StatusEffect.None,false,3));
			skills["StunningBlow"] = new Skill(true, "Single", "StunningBlow", new Effect(0, 20, 0, -5, 0, 0, StatusEffect.None, false, 3));
			skills["OverKill"] = new Skill(true, "All", "OverKill", new Effect(30));

			//Ranger
			skills["SleepShot"] = new Skill(true, "Single", "SleepShot", new Effect(0,3,0,0,0,0,StatusEffect.Sleep,false,0));
			skills["DefensiveBarrier"] = new Skill(false, "All", "DefensiveBarrier", new Effect(0, 0, 0, 3, 0, 0, StatusEffect.None, false, 3));
			skills["MultiShot"] = new Skill(true, "All", "MultiShot", new Effect(10));
			skills["SleepyTime"] = new Skill(true, "All", "SleepyTime", new Effect(0,5,0,0,0,0,StatusEffect.Sleep,false,0));
			skills["TalentedBunch"] = new Skill(false, "All", "TalentedBunch", new Effect(0, 0, 0, 0, 0, 10, StatusEffect.None, false, 5));
			skills["MassConfusion"] = new Skill(true, "All", "MassConfusion", new Effect(0, 20, 0, 0, 0, 10, StatusEffect.Confusion, false, 5));
		}
		static private void InitChoices()
		{
			#region Wizard
			List<LevelChoice> level1Wizard = new List<LevelChoice>();
			level1Wizard.Add(new LevelChoice("Heal", null, skills["Heal"]));
			level1Wizard.Add(new LevelChoice("Shock", null, skills["Shock"]));
			List<LevelChoice> level2Wizard = new List<LevelChoice>();
			level2Wizard.Add(new LevelChoice("Hp+10", new Unit(10, 0, 0, "", "", 0,0), null));
			level2Wizard.Add(new LevelChoice("Heal", null, skills["Heal"]));
			level2Wizard.Add(new LevelChoice("Shock", null, skills["Shock"]));
			List<LevelChoice> level3Wizard = new List<LevelChoice>();
			List<LevelChoice> level4Wizard = new List<LevelChoice>();
			level4Wizard.Add(new LevelChoice("Defense+5", new Unit(0, 0, 5, "", "", 0, 0), null));
			level4Wizard.Add(new LevelChoice("Shock", null, skills["Shock"]));
			level4Wizard.Add(new LevelChoice("Heal", null, skills["Heal"]));
			List<LevelChoice> level5Wizard = new List<LevelChoice>();
			List<LevelChoice> level6Wizard = new List<LevelChoice>();
			level6Wizard.Add(new LevelChoice("FireBlast", null, skills["FireBlast"]));
			level6Wizard.Add(new LevelChoice("3xShock", null, skills["Shock"]));
			level6Wizard.Add(new LevelChoice("HealAll", null, skills["HealAll"]));
			List<LevelChoice> level7Wizard = new List<LevelChoice>();
			List<LevelChoice> level8Wizard = new List<LevelChoice>();
			level8Wizard.Add(new LevelChoice("Defense+10", new Unit(0, 0, 10, "", "", 0, 0), null));
			level8Wizard.Add(new LevelChoice("2xFireBlast", null, skills["FireBlast"]));
			level8Wizard.Add(new LevelChoice("2xHealAll", null, skills["HealAll"]));
			List<LevelChoice> level9Wizard = new List<LevelChoice>();
			List<LevelChoice> level10Wizard = new List<LevelChoice>();
			level10Wizard.Add(new LevelChoice("HealBeam", null, skills["HealBeam"]));
			level10Wizard.Add(new LevelChoice("Hp+50", new Unit(50, 0, 0, "", "", 0, 0), null));
			level10Wizard.Add(new LevelChoice("LightningBolt", null, skills["LightningBolt"]));
			choices["Wizard"] = new List<List<LevelChoice>>();
			choices["Wizard"].Add(level1Wizard);
			choices["Wizard"].Add(level2Wizard);
			choices["Wizard"].Add(level3Wizard);
			choices["Wizard"].Add(level4Wizard);
			choices["Wizard"].Add(level5Wizard);
			choices["Wizard"].Add(level6Wizard);
			choices["Wizard"].Add(level7Wizard);
			choices["Wizard"].Add(level8Wizard);
			choices["Wizard"].Add(level9Wizard);
			choices["Wizard"].Add(level10Wizard);
			#endregion
			#region Fighter
			List<LevelChoice> level1Fighter = new List<LevelChoice>();
			level1Fighter.Add(new LevelChoice("Attack+2", new Unit(0, 2, 0, "", "", 0, 0), null));
			level1Fighter.Add(new LevelChoice("Defense+2", new Unit(0, 0, 2, "", "", 0, 0), null));
			List<LevelChoice> level2Fighter = new List<LevelChoice>();
			level2Fighter.Add(new LevelChoice("Attack+3", new Unit(0, 3, 0, "", "", 0, 0), null));
			level2Fighter.Add(new LevelChoice("Meditate", null, skills["Meditate"]));
			level2Fighter.Add(new LevelChoice("PowerAttack", null, skills["PowerAttack"]));
			List<LevelChoice> level3Fighter = new List<LevelChoice>();
			List<LevelChoice> level4Fighter = new List<LevelChoice>();
			level4Fighter.Add(new LevelChoice("Attack+5", new Unit(0, 5, 0, "", "", 0, 0), null));
			level4Fighter.Add(new LevelChoice("2xPowerAttack", null, skills["PowerAttack"]));
			level4Fighter.Add(new LevelChoice("Hp+5", null, skills["Heal"]));
			List<LevelChoice> level5Fighter = new List<LevelChoice>();
			List<LevelChoice> level6Fighter = new List<LevelChoice>();
			level6Fighter.Add(new LevelChoice("3xMeditation", null, skills["Meditate"]));
			level6Fighter.Add(new LevelChoice("TornadoSlash", null, skills["TornadoSlash"]));
			level6Fighter.Add(new LevelChoice("Defense+10", new Unit(0, 0, 10, "", "", 0, 0), null));
			List<LevelChoice> level7Fighter = new List<LevelChoice>();
			List<LevelChoice> level8Fighter = new List<LevelChoice>();
			level8Fighter.Add(new LevelChoice("2xTornadoSlash", null, skills["TornadoSlash"]));
			level8Fighter.Add(new LevelChoice("3xWarRoar", null, skills["WarRoar"]));
			level8Fighter.Add(new LevelChoice("StunningBlow", null, skills["StunningBlow"]));
			List<LevelChoice> level9Fighter = new List<LevelChoice>();
			List<LevelChoice> level10Fighter = new List<LevelChoice>();
			level10Fighter.Add(new LevelChoice("OverKill", null, skills["OverKill"]));
			level10Fighter.Add(new LevelChoice("Attack+20", new Unit(0, 20, 0, "", "", 0, 0), null));
			level10Fighter.Add(new LevelChoice("5xTornadoSlash", null, skills["TornadoSlash"]));
			choices["Fighter"] = new List<List<LevelChoice>>();
			choices["Fighter"].Add(level1Fighter);
			choices["Fighter"].Add(level2Fighter);
			choices["Fighter"].Add(level3Fighter);
			choices["Fighter"].Add(level4Fighter);
			choices["Fighter"].Add(level5Fighter);
			choices["Fighter"].Add(level6Fighter);
			choices["Fighter"].Add(level7Fighter);
			choices["Fighter"].Add(level8Fighter);
			choices["Fighter"].Add(level9Fighter);
			choices["Fighter"].Add(level10Fighter);
			#endregion
			#region Ranger
			List<LevelChoice> level1Ranger = new List<LevelChoice>();
			level1Ranger.Add(new LevelChoice("Defense+2 ", new Unit(0, 0, 2, "", "", 2, 0), null));
			level1Ranger.Add(new LevelChoice("Speed+2", new Unit(0, 0, 0, "", "", 2, 0), null));
			List<LevelChoice> level2Ranger = new List<LevelChoice>();
			level2Ranger.Add(new LevelChoice("Attack+3 Speed+2", new Unit(0, 3, 0, "", "", 2, 0), null));
			level2Ranger.Add(new LevelChoice("SleepShot", null, skills["SleepShot"]));
			level2Ranger.Add(new LevelChoice("DefensiveBarrier", null, skills["DefensiveBarrier"]));
			List<LevelChoice> level3Ranger = new List<LevelChoice>();
			List<LevelChoice> level4Ranger = new List<LevelChoice>();
			level4Ranger.Add(new LevelChoice("Defense+5", new Unit(0, 5, 0, "", "", 0, 0), null));
			level4Ranger.Add(new LevelChoice("2xSleepShot", null, skills["SleepShot"]));
			level4Ranger.Add(new LevelChoice("2xDefensiveBarrier", null, skills["DefensiveBarrier"]));
			List<LevelChoice> level5Ranger = new List<LevelChoice>();
			List<LevelChoice> level6Ranger = new List<LevelChoice>();
			level6Ranger.Add(new LevelChoice("MultiShot", null, skills["MultiShot"]));
			level6Ranger.Add(new LevelChoice("Attack+4 Defense+4", new Unit(0,4, 4, "", "", 0, 0), null));
			level6Ranger.Add(new LevelChoice("Speed+4 Talent+4", new Unit(0, 0, 4, "", "", 0, 4), null));
			List<LevelChoice> level7Ranger = new List<LevelChoice>();
			List<LevelChoice> level8Ranger = new List<LevelChoice>();
			level8Ranger.Add(new LevelChoice("3xMultiShot", null, skills["MultiShot"]));
			level8Ranger.Add(new LevelChoice("SleepyTime", null, skills["SleepyTime"]));
			level8Ranger.Add(new LevelChoice("TalentedBunch", null, skills["TalentedBunch"]));
			List<LevelChoice> level9Ranger = new List<LevelChoice>();
			List<LevelChoice> level10Ranger = new List<LevelChoice>();
			level10Ranger.Add(new LevelChoice("3xSleepyTime", null, skills["SleepyTime"]));
			level10Ranger.Add(new LevelChoice("3xTalentedBunch", null, skills["TalentedBunch"]));
			level10Ranger.Add(new LevelChoice("MassConfusion", null, skills["MassConfusion"]));
			choices["Ranger"] = new List<List<LevelChoice>>();
			choices["Ranger"].Add(level1Ranger);
			choices["Ranger"].Add(level2Ranger);
			choices["Ranger"].Add(level3Ranger);
			choices["Ranger"].Add(level4Ranger);
			choices["Ranger"].Add(level5Ranger);
			choices["Ranger"].Add(level6Ranger);
			choices["Ranger"].Add(level7Ranger);
			choices["Ranger"].Add(level8Ranger);
			choices["Ranger"].Add(level9Ranger);
			choices["Ranger"].Add(level10Ranger);
			#endregion
		}
		static private void InitGrowthRates()
		{
			growthRates["Wizard"] = new LevelUpGrowthRate(0.1f, 0.15f, 0.4f, 0.2f,0.4f);
			growthRates["Fighter"] = new LevelUpGrowthRate(0.25f, 0.2f, 0.5f, 0.1f,0.15f);
			growthRates["Ranger"] = new LevelUpGrowthRate(0.2f, 0.2f, 0.3f, 0.4f,0.3f);
		}
		static public Character CreateCharacter(string type,string name,string image)
		{
			if (type == "Wizard")
			{
				return new Character(11, 3, 3, image, name, 6, 10, type);
			}
			else if (type == "Fighter")
			{
				return new Character(18, 7, 5, image, name, 2, 3, type);
			}
			else if (type == "Ranger")
			{
				return new Character(13, 5, 4, image, name, 10, 5, type);
			}
			return null;
		}
	}
}
