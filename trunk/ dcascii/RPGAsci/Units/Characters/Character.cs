using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	public class Character : Unit
	{
		public int experience = 0;
		public int level = 1;
		public struct LevelUpGrowthRate
		{
			public float hp;
			public float attack;
			public float defense;
			public float speed;
			public float talent;
			public LevelUpGrowthRate(float hp, float attack, float defense, float speed, float talent)
			{
				this.hp = hp;
				this.attack = attack;
				this.defense = defense;
				this.speed = speed;
				this.talent = talent;
			}
		}
		public SkillTree skillTree = new SkillTree();
		public LevelUpGrowthRate growthRate;
		
		public Character(int hp, int attack, int defense, string image, string name, int speed, int talent)
			: base(hp, attack, defense, image, name, speed, talent)
		{
			equipment["Armor1"] = EquipmentManager.GetEquipment("Common Cap");
			equipment["Armor2"] = EquipmentManager.GetEquipment("Common Shirt");
			equipment["Armor3"] = EquipmentManager.GetEquipment("Common Pants");
		}
		public void SetLevelChanges()
		{
			int newHP = (int)Math.Round(hp * Math.Pow(1 + growthRate.hp, level));
			int newAttack = (int)Math.Round(attack * Math.Pow(1 + growthRate.attack, level));
			int newDefense = (int)Math.Round(defense * Math.Pow(1 + growthRate.defense, level));
			int newSpeed = (int)Math.Round(speed * Math.Pow(1 + growthRate.speed, level));
			int newTalent = (int)Math.Round(talent * Math.Pow(1 + growthRate.talent, level));
			Console.BackgroundColor = ConsoleColor.Blue;
			ConsoleHelper.GameWriteLine("Hp:+" + (newHP - hp) + " Attack:+" + (newAttack - attack) + " Defense:+" + (newDefense - defense) + " Talent:+" + (newTalent - talent) + " Speed:+" + (newSpeed - speed));
			Console.ResetColor();
			hp = newHP;
			attack = newAttack;
			defense = newDefense;
			speed = newSpeed;
			talent = newTalent;
		}
	}
}
