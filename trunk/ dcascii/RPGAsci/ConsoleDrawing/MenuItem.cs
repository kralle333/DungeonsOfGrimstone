using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	public class MenuItem
	{
		int level = 0;
		MenuItem parent;
		public MenuItem childSelected;
		public List<MenuItem> children = new List<MenuItem>();
		public string text;
		public int index = 0;
		private int scrollIndex = 0;
		private int lastDrawnIndex = 0;
		private bool firstIndexSeen = true;
		private bool lastIndexSeen = false;
		private int currentLevel = 1;
		private int consoleY = -1;
		private int consoleX = -1;
		public bool locked = false;
		public int width = 0;
		public bool showingDescription = false;
		public bool scrollable = false;
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
			//currentlySelected = true;
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
		}
		public void ReadInput(ConsoleKeyInfo kb)
		{
			if (!showingDescription)
			{
				if (childSelected == null && children.Count >= 0)
				{
					if (kb.Key == ConsoleKey.LeftArrow && index > 0)
					{
						index--;
						if (scrollable && !firstIndexSeen && index == 1)
						{
							scrollIndex--;
						}
						ConsoleHelper.GameClearLine();
						DrawChildren();
					}
					else if (kb.Key == ConsoleKey.RightArrow && index + 1 < children.Count)
					{
						index++;
						ConsoleHelper.GameClearLine();
						if (scrollable && !lastIndexSeen && lastDrawnIndex == index)
						{
							scrollIndex++;
						}
						DrawChildren();
					}
					else if (kb.Key == ConsoleKey.Z)
					{
						if (level > 0)
						{
							ConsoleHelper.GameClearLine();
							ConsoleHelper.GameGotoLine(level);
							ConsoleHelper.GameClearLine();
							if (parent != null)
							{
								parent.childSelected = null;
								parent.DrawChildren();
							}

						}
					}
					else if (kb.Key == ConsoleKey.X && children.Count() > 0 && !children[index].locked)
					{
						childSelected = children[index];
						ConsoleHelper.GameClearLine();
						ConsoleHelper.GameWriteLine("");
						children[index].DrawChildren();
						currentLevel++;
					}
					else if (kb.Key == ConsoleKey.H)
					{
						showingDescription = true;
						ConsoleHelper.GameWriteLine();
						ConsoleHelper.GameWrite("[Description]: "+ GetMarkedItem().description);
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
			ConsoleHelper.GameWrite(text + "    ");
			if (level == 0)
			{
				ConsoleHelper.GameWriteLine("");
				DrawChildren();
			}

		}
		public void DrawChildren()
		{
			if (scrollIndex == 0)
			{
				firstIndexSeen = true;
			}
			else if (scrollIndex >0)
			{
				firstIndexSeen = false;
				ConsoleHelper.GameWrite("...   ");
			}
			for (int i = scrollIndex; i < children.Count; i++)
			{
				if (!scrollable || ConsoleHelper.ConsoleX + (children[i].text + "     ").Length < Border.GameConsoleWidth)
				{
					if (childSelected != null && children[i] == childSelected)
					{
						ConsoleHelper.GameWrite("-");
					}
					else if (index == i)
					{
						ConsoleHelper.GameWrite("->");
					}
					ConsoleHelper.GameWrite(children[i].text + "   ");
				}
				else
				{
					ConsoleHelper.GameWrite("   ...");
					lastDrawnIndex = i - 1;
					lastIndexSeen = false;
					break;
				}
				lastIndexSeen = true;
				if (scrollIndex > 0)
				{
					firstIndexSeen = false;
				}
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
		public int GetIndexOfSelectedChild(string item)
		{
			if(text.Equals(item))
			{
				return children.IndexOf(childSelected);
			}
			else
			{
				if (childSelected != null)
				{
					return childSelected.GetIndexOfSelectedChild(item);
				}
				else
				{
					return -1;
				}
			}
		}
		public bool IsSelected(string item)
		{
			if (childSelected != null && childSelected.text == item)
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
		public string GetSelectedItemText(int level)
		{
			if (this.level + 1 == level)
			{
				if (childSelected != null)
				{
					return childSelected.text;
				}
				else
				{
					return "";
				}
			}
			else
			{
				if (childSelected != null)
				{
					return childSelected.GetSelectedItemText(level);
				}
			}
			return "";
		}
		public string GetMarkedItemText()
		{
			return GetMarkedItem().text;
		}
		public MenuItem GetMarkedItem()
		{
			if (childSelected != null)
			{
				return childSelected.GetMarkedItem();
			}
			else
			{
				return children[index];
			}
		}
		public void Reset()
		{
			index = 0;
			scrollIndex = 0;
			childSelected = null;
			foreach (MenuItem child in children)
			{
				child.Reset();
			}
		}
	}
}
