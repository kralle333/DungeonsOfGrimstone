using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Adventurer:Character
	{
		public Adventurer(string image,string name):base(15, 7, 5, image, name, 2, 3)
		{
			equipment["Weapon"] = EquipmentManager.GetEquipment("Wooden Sword");
			growthRate = new LevelUpGrowthRate(0.3f, 0.3f, 0.2f, 0.15f, 0.15f);
			skillTree.AddNode(CharacterManager.GetSkill("PowerAttack"));
			skillTree.AddNode(CharacterManager.GetSkill("TornadoSlash"));
			skillTree.AddNode(CharacterManager.GetSkill("BodySlam"));
			skillTree.AddNode(CharacterManager.GetSkill("StunningBlow"));
			skillTree.AddNode(CharacterManager.GetSkill("WarRoar"));
			skillTree.AddNode(CharacterManager.GetSkill("EncourageAll"));
			skillTree.AddChild(new string[] { "WarRoar", "EncourageAll" }, CharacterManager.GetSkill("EncourageWar"));
			skillTree.AddChild(new string[] { "PowerAttack", "TornadoSlash" }, CharacterManager.GetSkill("OverKill"));
		}
	}
}
