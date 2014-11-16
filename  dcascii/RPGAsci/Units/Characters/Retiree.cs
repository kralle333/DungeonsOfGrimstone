 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Retiree:Character
	{
		public Retiree(string image, string name)
			: base(12, 8, 5, image, name, 1, 1)
		{
			classType = "Retiree";
			growthRate = new CharacterGrowthRate(0.25f, 0.5f, 0.1f, 0.1f, 0.15f);
			skillTree.AddNode(CharacterManager.GetSkill("Screech"),"A horrible screech causes your enemies to let their guard down");
			skillTree.AddNode(CharacterManager.GetSkill("Nap"), "The Retiree takes a nap, he is so sleepy. Heals hp");
			skillTree.AddNode(CharacterManager.GetSkill("FoxtrotSlash"), "A revamped version of the good olde Foxtrot");
			skillTree.AddNode(CharacterManager.GetSkill("GrumpyBlow"), "Everything thats bothering the Retiree is focused in on grumpy blow");
			skillTree.AddChild(new string[] { "Nap" }, new int[] { 2 }, CharacterManager.GetSkill("CheatDeath"), "The Retiree decides that its not yet his time", 1);
			skillTree.AddChild(new string[] { "FoxtrotSlash", "GrumpyBlow" }, new int[]{3, 3}, CharacterManager.GetSkill("StrayCats"), "Stray cats, which the Retiree have befriended over the years attacks the enemies", 1);
			skillTree.AddChild(new string[] { "StrayCats" }, new int[] { 4 }, CharacterManager.GetSkill("EyePoke"), "A painful eyepoke, full of hatred", 2);
			skillTree.FinalizeTree(this);
		}
	}
}
