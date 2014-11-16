using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class BlackSmith : Character
	{
		public BlackSmith(string image, string name)
			: base(20, 3, 3, image, name, 1, 2)
		{
			classType = "BlackSmith";
			growthRate = new CharacterGrowthRate(0.3f, 0.3f, 0.2f, 0.1f, 0.1f);
			skillTree.AddNode(CharacterManager.GetSkill("MightyBlow"),"MASSIVE DAMAGE, MASSIVE MISS CHANCE");
			skillTree.AddNode(CharacterManager.GetSkill("Forgery"),"Lets you forge on equipment in town");
			skillTree.AddNode(CharacterManager.GetSkill("SpecialBooze"),"The blacksmith's favourite special brew, raises attack and miss chance");
			skillTree.AddNode(CharacterManager.GetSkill("Scavenger"),"Finds more material from dead foes");
			skillTree.AddChild(new string[] { "MightyBlow" }, new int[] { 4 }, CharacterManager.GetSkill("ExplosiveAnger"), "ARAAAAAAAAAAAGGGGGGHHHAAHA", 1);
			skillTree.AddChild(new string[] { "SpecialBooze" }, new int[] { 4 }, CharacterManager.GetSkill("PureAlcohol"), "96% pure ethnanol brings out the best and worst of the black smith", 1);
			skillTree.AddChild(new string[] { "Forgery","Scavenger" }, new int[]{3,1}, CharacterManager.GetSkill("MonsterSmithing"), "Makes you able to create items of dead enemies", 1);
			skillTree.AddChild(new string[] { "Forgery","Scavenger" }, new int[] { 1, 3 }, CharacterManager.GetSkill("GreatWall"), "Creates a defensive wall around the party", 1);
			skillTree.FinalizeTree(this);
		}
	}
}
