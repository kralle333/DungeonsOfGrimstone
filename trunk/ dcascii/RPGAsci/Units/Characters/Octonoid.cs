using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Octonoid:Character
	{
		public Octonoid(string image, string name):base(16, 4, 8, image, name, 1, 1)
		{
			classType = "Octonoid";
			skillTree.AddNode(CharacterManager.GetSkill("TentacleMassage"), "A delightful massage that heals");
			skillTree.AddNode(CharacterManager.GetSkill("InkySaliva"), "The Octonoid shares his disgusting saliva with the an enemy");
			skillTree.AddNode(CharacterManager.GetSkill("TentacleNursing"), "The Octonoid cleans and nurses his tentacles, Attack up!");
			skillTree.AddNode(CharacterManager.GetSkill("Entangle"), "An enemy is entangled and hurt for some rounds");
			skillTree.AddChild(new string[] { "TentacleMassage" }, new int[] { 3 }, CharacterManager.GetSkill("MultiTentacleMassage"), "Every party member gets a tentacle massage, feels good...",1);
			skillTree.AddChild(new string[] { "InkySaliva" }, new int[] { 2 }, CharacterManager.GetSkill("PoisonSlime"), "An even more disgusting saliva blob is shared with the enemy, may poison", 1);
			skillTree.AddChild(new string[] { "Entangle" }, new int[] { 3 }, CharacterManager.GetSkill("MightyGrip"), "The Octonoid entangles every foe and damages over multiple rounds", 1);
			skillTree.AddChild(new string[] { "MightyGrip" }, new int[] { 3 }, CharacterManager.GetSkill("UncomfortableHug"), "A very very uncomfortable hug is given to an enemy over multiple rounds", 2);
			growthRate = new CharacterGrowthRate(0.3f, 0.2f, 0.2f, 0.1f, 0.2f);
			skillTree.FinalizeTree(this);
		}
	}
}
