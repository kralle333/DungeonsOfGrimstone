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
		public MenuItem childMarked;
		public List<MenuItem> children = new List<MenuItem>();
		public bool currentlyMarked = false;
		public bool currentlySelected = false;
		public string text;
		private int index = 0;
		private int currentLevel = 1;
		private int consoleY = -1;
		private int consoleX = -1;
		public bool locked = false;
		public int width = 0;
		public bool showingDescription = false;
		public string description;

		public MenuItem(string text)
		{
			this.text = text;
			this.description = "No description";
		}
		public MenuItem(string text,string description)
		{
			this.text = text;
			this.description = description;
		}
		public MenuItem(string text, int consoleX, int consoleY)
		{
			this.text = text;
			this.consoleX = consoleX + 1;
			this.consoleY = consoleY + 41;
			currentlySelected = true;
			this.description = "No description";
		}
		public void AddChild(MenuItem menuItem)
		{
			menuItem.level = level + 1;
			menuItem.parent = this;
			menuItem.consoleY = consoleY + 1;
			menuItem.consoleX = consoleX;
			if (consoleY == -1 || consoleX == -1)
			{
				throw new Exception("Parent coordinates not set!");
			}
			children.Add(menuItem);
			if (children.Count == 1)
			{
				menuItem.currentlyMarked = true;
			}
		}
		public void ReadInput(ConsoleKeyInfo kb)
		{
			if (!showingDescription)
			{
				if (currentlySelected && childSelected == null && children.Count >= 0)
				{

					if (kb.Key == ConsoleKey.LeftArrow && index > 0)
					{
						children[index].currentlyMarked = false;
						index--;
						children[index].currentlyMarked = true;
						childMarked = children[index];
						ConsoleHelper.GameClearLine();
						DrawChildren();
					}
					else if (kb.Key == ConsoleKey.RightArrow && index + 1 < children.Count)
					{
						children[index].currentlyMarked = false;
						index++;
						children[index].currentlyMarked = true;
						childMarked = children[index];
						ConsoleHelper.GameClearLine();
						DrawChildren();
					}
					else if (kb.Key == ConsoleKey.Z)
					{
						if (level > 0)
						{
							currentlySelected = false;
							currentlyMarked = true;
							if (children.Count() > 0)
							{
								childMarked.currentlyMarked = false;
								childSelected = null;
							}
							ConsoleHelper.GameClearLine();
							ConsoleHelper.GameGotoLine(level);
							ConsoleHelper.GameClearLine();
							if (parent != null)
							{
								parent.childSelected = null;
								parent.childMarked = parent.children[parent.index];
								parent.DrawChildren();
							}

						}
					}
					else if (kb.Key == ConsoleKey.X && children.Count() > 0 && !children[index].locked)
					{
						currentlySelected = true;
						currentlyMarked = false;
						children[index].currentlySelected = true;
						childSelected = children[index];
						ConsoleHelper.GameClearLine();
						if (parent != null)
						{
							parent.childMarked = null;
						}
						childMarked.currentlyMarked = false;
						childMarked.Draw();
						childMarked = null;
						ConsoleHelper.GameWriteLine("");
						children[index].DrawChildren();
						currentLevel++;
					}
					else if (kb.Key == ConsoleKey.H && childMarked != null)
					{
						showingDescription = true;
						ConsoleHelper.GameWriteLine();
						ConsoleHelper.GameWrite(childMarked.description);
					}
				}
				else if (childSelected != null)
				{
					childSelected.ReadInput(kb);
				}
			}
			else
			{
				showingDescription = false;
				ConsoleHelper.GameClearLine();
				ConsoleHelper.GameGotoLine(Console.CursorTop-42);
			}
		}
		public void Draw()
		{
			Console.SetCursorPosition(consoleX, consoleY);
			if (currentlyMarked)
			{
				ConsoleHelper.GameWrite("->");
			}
			else if (currentlySelected)
			{
				ConsoleHelper.GameWrite("-");
			}
			ConsoleHelper.GameWrite(text + "    ");
			if (level == 0)
			{
				ConsoleHelper.GameWriteLine("");
				DrawChildren();
			}

		}
		public void DrawChildren()
		{
			for (int i = 0; i < children.Count; i++)
			{
				if (i == 0 && childMarked == null)
				{
					children[0].currentlyMarked = true;
					childMarked = children[0];
				}
				if (childSelected != null && children[i] == childSelected)
				{
					ConsoleHelper.GameWrite("-");
				}
				if (children[i].currentlyMarked)
				{
					ConsoleHelper.GameWrite("->");
				}
				ConsoleHelper.GameWrite(children[i].text + "    ");
			}
			if (childSelected != null)
			{
				ConsoleHelper.GameWriteLine("");
				childSelected.DrawChildren();
			}
		}
		public void Update()
		{
			ConsoleHelper.ClearConsole();
			Draw();
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
			childMarked = null;
			currentlySelected = false;
			currentlyMarked = false;
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
