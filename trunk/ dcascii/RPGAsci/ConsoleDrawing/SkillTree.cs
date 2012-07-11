using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	public class SkillTree
	{
		public Character character;
		public Dictionary<int, List<SkillNode>> skills = new Dictionary<int, List<SkillNode>>();
		private List<SkillNode> drawnChildren = new List<SkillNode>();
		public MenuItem treeItem = new MenuItem("Skill tree", 0, 0);

		public void AddNode(Skill skill, string description)
		{
			SkillNode node = new SkillNode(skill, description);
			if (!skills.ContainsKey(0))
			{
				skills[0] = new List<SkillNode>();
			}
			skills[0].Add(node);
			node.available = true;
			node.locked = true;
		}
		//Use after all the nodes have been added
		public void FinalizeTree(Character character)
		{
			this.character = character;
			for (int i = 0; i < skills[0].Count(); i++)
			{
				treeItem.AddChild(skills[0][i]);
			}
			MenuItem goLevelDownItem = new MenuItem("Go Level Down");
			goLevelDownItem.scrollable = true;
			treeItem.AddChild(goLevelDownItem);
			int level = 1;
			while (skills.ContainsKey(level))
			{
				for (int i = 0; i < skills[level].Count(); i++)
				{
					SkillNode node = skills[level][i];
					foreach(KeyValuePair<Skill,int> pair in node.requiredSkills)
					{
						node.description = "Needed :";
						if (pair.Value > 0)
						{
							node.description += pair.Key.name + "-" + pair.Value;
						}
					}
					goLevelDownItem.AddChild(node);
				}
				if(skills.ContainsKey(level+1))
				{
					goLevelDownItem.AddChild(new MenuItem("Go Level Down"));
					goLevelDownItem = goLevelDownItem.children[goLevelDownItem.children.Count() - 1];
					goLevelDownItem.scrollable = true;
				}
				level++;
			}
			treeItem.scrollable = true;
		}
		public void AddChild(string[] parentsName, int[] requiredNumber, Skill skill, string description, int treeLevel)
		{
			SkillNode skillNode = new SkillNode(skill,description);
			if (!skills.ContainsKey(treeLevel))
			{
				skills[treeLevel] = new List<SkillNode>();
			}
			skills[treeLevel].Add(skillNode);
			for (int i = 0; i < parentsName.Count(); i++)
			{
				skillNode.requiredSkills[CharacterManager.GetSkill(parentsName[i])] = requiredNumber[i];
			}
			skillNode.locked = true;

		}
		public void Draw()
		{
			treeItem.text = "Skills of " + character.name + " - Skill points left :" + character.skillPoints;
			treeItem.Draw();
		}
		public bool HandleInput(ConsoleKeyInfo input)
		{
			bool skillPointUsed = false;
			if (input.Key == ConsoleKey.X && character.skillPoints > 0)
			{
				MenuItem markedItem = treeItem.GetMarkedItem();
				if (markedItem.text != "Go Level Down")
				{
					if (((SkillNode)markedItem).available)
					{
						SkillNode skillItem = (SkillNode)markedItem;
						skillItem.level++;
						skillItem.text = skillItem.skill.name + " " + skillItem.level;
						character.skillPoints--;
						skillPointUsed = true;
						character.LevelUpSkill(skillItem.skill);
					}
					else
					{
						ConsoleHelper.ClearConsole();
						ConsoleHelper.GameWrite("Skill is locked! " + ((SkillNode)markedItem).description);
						Console.ReadKey(true);
						skillPointUsed = true;
					}
				}
			}
			else if(input.Key == ConsoleKey.X && character.skillPoints ==0)
			{
				ConsoleHelper.ClearConsole();
				ConsoleHelper.GameWrite("Not enough skill points left!");
				Console.ReadKey(true);
				skillPointUsed = true;
			}
			treeItem.ReadInput(input);
			return skillPointUsed;
		}

	}
}
