using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Thief:Character
	{
		public Thief(string image, string name):base(11, 3, 3, image, name, 6, 10)
		{
			classType = "Thief";
			growthRate = new CharacterGrowthRate(0.2f, 0.2f, 0.1f, 0.4f, 0.3f);
			skillTree.AddNode(CharacterManager.GetSkill("LockPicking"),"Lets you pick locks in the dungeon");
			skillTree.AddNode(CharacterManager.GetSkill("Borrow"), "Borrows something from an enemy");
			skillTree.AddNode(CharacterManager.GetSkill("DollarSigns"), "Gold earned after battle increased!");
			skillTree.AddNode(CharacterManager.GetSkill("Arm"), "Arms your opponent with something silly");
			skillTree.AddChild(new string[] { "Lockpicking"}, new int[] { 3 }, CharacterManager.GetSkill("FleshPicking"), "The precision gotten from lockpicking is used to hit enemy weak points", 1);
			skillTree.AddChild(new string[] { "Borrow" }, new int[] { 2 }, CharacterManager.GetSkill("Cloak"), "Cloaks the thief", 1);
			skillTree.AddChild(new string[] { "DollarSigns" }, new int[] { 2 }, CharacterManager.GetSkill("BootyHunter"), "Increases chances of loot after battle", 1);
			skillTree.AddChild(new string[] { "FleshPicking" }, new int[] { 4 }, CharacterManager.GetSkill("Swifty"), "Increases speed, which makes the thief harder to hit", 2);
			skillTree.FinalizeTree(this);
		}
	}
}
