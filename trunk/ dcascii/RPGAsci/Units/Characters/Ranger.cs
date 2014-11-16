using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Ranger:Character
	{
		public Ranger(string image, string name):base(13, 5, 4, image, name, 10, 5)
		{
			classType = "Ranger";
			growthRate = new CharacterGrowthRate(0.1f, 0.15f, 0.4f, 0.2f, 0.4f);
			skillTree.AddNode(CharacterManager.GetSkill("PowerAttack"), "Powerful attack");
			skillTree.AddNode(CharacterManager.GetSkill("StunningBlow"), "A blow so stunning that it might stun your foe");
			skillTree.AddNode(CharacterManager.GetSkill("WarRoar"), "ROAR ATTACK UP!!");
			skillTree.AddNode(CharacterManager.GetSkill("EncourageAll"), "Encourage your team mates for all the great work");
			skillTree.AddChild(new string[] { "WarRoar", "EncourageAll" }, new int[] { 2, 2 }, CharacterManager.GetSkill("EncourageWar"), "Encourages your party to become war machines", 1);
			skillTree.AddChild(new string[] { "PowerAttack" }, new int[] { 4 }, CharacterManager.GetSkill("BodySlam"), "Use the power of your body weight to inflict damage", 1);
			skillTree.AddChild(new string[] { "StunningBlow" }, new int[] { 4 }, CharacterManager.GetSkill("TornadoSlash"), "You spin as a tornado slashing every foe", 1);
			skillTree.AddChild(new string[] { "BodySlam", "TornadoSlash" }, new int[] { 3, 3 }, CharacterManager.GetSkill("OverKill"), "Deals a lot of damage", 2);

			skillTree.FinalizeTree(this);
		}
	}
}
