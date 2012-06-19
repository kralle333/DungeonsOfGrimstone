using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class MenuItem
	{
		int level = 0;
		MenuItem parent;
		public MenuItem childSelected;
		public List<MenuItem> children = new List<MenuItem>();
		public bool currentlyMarked = false;
		public bool currentlySelected = false;
		public string text;
		private int index = 0;
		private int currentLevel = 1;
		private int consoleY = -1;

		public MenuItem(string text)
		{
			this.text = text;
		}
		public MenuItem(string text, int consoleY)
		{
			this.text = text;
			this.consoleY = consoleY;
		}
		public void AddChild(MenuItem menuItem)
		{
			menuItem.level = level + 1;
			menuItem.parent = this;
			children.Add(menuItem);
			if (children.Count == 1)
			{
				menuItem.currentlyMarked = true;
			}
		}
		public void ReadInput()
		{
			ConsoleKeyInfo kb = Console.ReadKey(true);
			if (currentlySelected && childSelected == null && children.Count > 0)
			{

				if (kb.Key == ConsoleKey.LeftArrow && index > 0)
				{
					children[index].currentlyMarked = false;
					index--;
					children[index].currentlyMarked = true;

				}
				else if (kb.Key == ConsoleKey.RightArrow && index + 1 < children.Count)
				{
					children[index].currentlyMarked = false;
					index++;
					children[index].currentlyMarked = true;

				}
				else if (kb.Key == ConsoleKey.Z)
				{
					if (level > 0)
					{
						currentlySelected = false;
						currentlyMarked = true;
						children[index].currentlyMarked = false;
						childSelected = null;
					}
				}
				else if (kb.Key == ConsoleKey.X)
				{
					currentlySelected = true;
					children[index].currentlyMarked = false;
					children[index].currentlySelected = true;
					childSelected = children[index];
					children[index].DrawChildren();
					currentLevel++;
					if (children[index].children.Count > 0)
					{
						children[index].children[0].currentlyMarked = true;
					}
				}
			}
			else if (childSelected != null)
			{
				childSelected.ReadInput(kb);
			}

		}
		public void ReadInput(ConsoleKeyInfo kb)
		{
			if (currentlySelected && childSelected == null && children.Count >= 0)
			{

				if (kb.Key == ConsoleKey.LeftArrow && index > 0)
				{
					children[index].currentlyMarked = false;
					index--;
					children[index].currentlyMarked = true;

				}
				else if (kb.Key == ConsoleKey.RightArrow && index + 1 < children.Count)
				{
					children[index].currentlyMarked = false;
					index++;
					children[index].currentlyMarked = true;

				}
				else if (kb.Key == ConsoleKey.Z)
				{
					if (level > 0)
					{
						currentlySelected = false;
						currentlyMarked = true;
						if (children.Count() > 0)
						{
							children[index].currentlyMarked = false;
							childSelected = null;

						}
						Console.SetCursorPosition(0, Console.CursorTop);
						Console.WriteLine(" ".PadRight(80, ' '));
						if (parent != null)
						{
							parent.childSelected = null;
						}

					}
				}
				else if (kb.Key == ConsoleKey.X && children.Count() > 0)
				{
					currentlySelected = true;
					children[index].currentlyMarked = false;
					children[index].currentlySelected = true;
					childSelected = children[index];
					children[index].DrawChildren();
					currentLevel++;
					if (children[index].children.Count > 0)
					{
						children[index].children[0].currentlyMarked = true;
					}
				}
			}
			else if (childSelected != null)
			{
				childSelected.ReadInput(kb);
			}

		}
		public void Draw()
		{
			if (consoleY != -1)
			{
				Console.SetCursorPosition(0, consoleY);
			}
			if (currentlyMarked)
			{
				Console.Write("->");
			}
			else if (currentlySelected)
			{
				Console.Write("-");
			}
			Console.Write(text + "    ");
			if (level == 0)
			{
				if (consoleY == -1)
				{
					consoleY = Console.CursorTop;
				}
				DrawChildren();
			}

		}
		public void DrawChildren()
		{
			if (Console.CursorTop != consoleY && consoleY != -1)
			{
				Console.SetCursorPosition(0, consoleY);
			}
			Console.WriteLine();
			for (int i = 0; i < children.Count; i++)
			{
				if (childSelected != null && children[i] == childSelected)
				{
					Console.Write("-");
				}
				if (children[i].currentlyMarked)
				{
					Console.Write("->");
				}
				Console.Write(children[i].text + "    ");
			}
			if (childSelected != null)
			{
				childSelected.DrawChildren();
			}
		}
		public void DrawChildren(int level, int consoleY)
		{
			if (level == this.level)
			{
				consoleY += level;
				DrawChildren();
			}
			else
			{
				childSelected.DrawChildren(level, consoleY);
			}
		}
		public void Clear()
		{
			Console.SetCursorPosition(0, Console.CursorTop - currentLevel);
			for (int i = 0; i < currentLevel; i++)
			{
				Console.WriteLine("                                                                ");
			}
			Console.SetCursorPosition(0, Console.CursorTop - currentLevel);
		}
		public MenuItem GetItem(string item)
		{
			if (text == item)
			{
				return this;
			}
			foreach (MenuItem child in children)
			{
				MenuItem i = child.GetItem(item);
				if (i != null)
				{
					return i;
				}
			}
			return null;
		}
		public int IsChildrenPressed(string item)
		{
			MenuItem i = GetItem(item);
			if (children.Count > 0)
			{
				for (int count = 0; count < i.children.Count; count++)
				{
					if (i.children[count].currentlySelected)
					{
						return count;
					}
				}
			}
			return -1;
		}
		public bool IsSelected(string item)
		{
			MenuItem i = GetItem(item);
			if (i.text == item && i.currentlySelected)
			{
				return true;
			}
			return false;
		}
		public void RemoveChild(string item)
		{
			foreach (MenuItem child in children)
			{
				if (child.text == item)
				{
					children.Remove(child);
					break;
				}
				else
				{
					child.RemoveChild(item);
				}
			}
		}
		public string GetSelectedItem(int level)
		{
			if (this.level == level)
			{
				if (currentlySelected)
				{
					return text;
				}
			}
			foreach (MenuItem child in children)
			{
				string text = child.GetSelectedItem(level);
				if (text != "")
				{
					return text;
				}
			}
			return "";
		}
		public void Reset()
		{
			index = 0;
			childSelected = null;
			currentlySelected = false;
			currentlyMarked = false;
			consoleY = -1;
			foreach (MenuItem child in children)
			{
				child.Reset();
			}
			if (level == 0)
			{
				currentlySelected = true;
				if (children.Count() > 0)
				{
					children[0].currentlyMarked = true;
				}
			}
		}
	}
}
