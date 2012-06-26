using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	public class SkillTree
	{
		public List<SkillNode> nodes = new List<SkillNode>();

		public void AddNode(Skill skill)
		{
			nodes.Add(new SkillNode(skill));
		}
		public void AddChild(string[] parentsName, Skill skill)
		{
			SkillNode skillNode = new SkillNode(skill);
			foreach (string s in parentsName)
			{
				//skillNode.required.Add(
			}
			
		}
		public void Draw()
		{

		}
	}
}
