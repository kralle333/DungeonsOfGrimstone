using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	public class SkillTree
	{
		public List<SkillNode> nodes = new List<SkillNode>();
		private List<SkillNode> drawnChildren = new List<SkillNode>();
		 
		public void AddNode(Skill skill)
		{
			nodes.Add(new SkillNode(skill));
		}
		public void AddChild(string[] parentsName, int[] requiredNumber,Skill skill)
		{
			SkillNode skillNode = new SkillNode(skill);
			for(int i = 0;i<parentsName.Count();i++)
			{
				skillNode.requiredSkills[CharacterManager.GetSkill(parentsName[i])] = requiredNumber[i];	
				nodes.Find(x=>x.skill.name == parentsName[i]).children.add(skillNode);
			}
			
		}
		public void Draw()
		{
				int row = 1;
				int column = 1
				int rowGap = Border.MainFrameHeight/nodes.Count();
				for(int row = 0;row<nodes.Count();row++)
				{
					if(nodes[row].children != null)
					{
						DrawChildren(nodes[row].children,2);
					}
					
					Console.SetCursorPosition(5*column,rowGap*row);
					if(nodes[i].available)
					{
						Console.BackgroundColor = ConsoleColor.Red;		
					}
					else
					{
						Console.BackgroundColor = ConsoleColor.DarkGreen;
					}
					 Console.Write(nodes[i].skill.name+" "+nodes[i].level);
				}
		}
		private void DrawChildren(List<SkillNode> children,int column)
		{
			 for(int i = 0;i<node[i].children.Count();i++)
	 		{
					if(drawnChildren.Contains(node[i].children[i]
			} 		
		}
	}
}
