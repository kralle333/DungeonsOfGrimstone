using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class ConsoleHelper
	{
		static private int consoleIndexX = 0;
		static private int consoleIndexY = 0;
		static public int ConsoleX { get { return 1 + consoleIndexX; } }
		static public int ConsoleY { get { return 41 + consoleIndexY; } }
		static public string SimpleRead(string text, int lengthLimit)
		{
			Console.WriteLine(text + " - Allowed length :" + lengthLimit);
			while (true)
			{
				string read = Console.ReadLine();
				if (read.Count() > 0 && read.Count() < lengthLimit)
				{
					return read;
				}
			}
		}
		static public string SimpleRead(string text, int lengthLimit, ConsoleColor color)
		{
			bool allowedLength = true;
			while (true)
			{
				if (allowedLength)
				{
					WriteLine(text + " - Allowed length :" + lengthLimit, color);
				}
				else
				{
					WriteLine(text + "- string was too long - Allowed length :" + lengthLimit, color);
				}
				string read = Console.ReadLine();
				if (read.Count() > 0 && read.Count() < lengthLimit)
				{
					return read;
				}
				else
				{
					EraseLines(Console.CursorTop, 3);
					allowedLength = false;
				}
			}
		}
		static public void WriteLine(string text, ConsoleColor color)
		{
			Console.BackgroundColor = color;
			Console.WriteLine(text);
			Console.ResetColor();
		}
		static public void Write(string text, ConsoleColor color)
		{
			Console.BackgroundColor = color;
			Console.Write(text);
			Console.ResetColor();
		}
		static public void WriteBlanks(int numberOfBlanks)
		{
			for (int i = 0; i < numberOfBlanks; i++)
			{
				Console.Write(" ");
			}
		}
		static public void WriteLineBlanks(int numberOfLines)
		{
			for (int i = 0; i < numberOfLines; i++)
			{
				Console.WriteLine("                                                              ");
			}
		}
		static public void WriteLineBlanks(int numberOfLines,int from,int to)
		{
			for (int i = 0; i < numberOfLines; i++)
			{
				Console.SetCursorPosition(from,i);
				WriteBlanks(to - from);
			}
		}
		static public void EraseLines(int currentPosition, int amount)
		{
			Console.SetCursorPosition(0, currentPosition - amount);
			WriteLineBlanks(amount);
			Console.SetCursorPosition(0, currentPosition - (amount - 1));
		}

		static public void EraseLines(int upToLine)
		{
			EraseLines(Console.WindowTop - upToLine-1, Console.WindowTop - upToLine);
		}
		static public void PaddedWriteLine(int totalLength, string s, char padding)
		{
			totalLength -= s.Length;
			int notEven = 0;
			if (totalLength % 2 != 0)
			{
				notEven = -1;
				totalLength += 1;
			}
			for (int i = 0; i < totalLength + notEven; i++)
			{
				if (totalLength / 2 == i)
				{
					Console.Write(s);
				}
				Console.Write(padding);
			}
			Console.WriteLine();
		}
		static public void PaddedWriteLine(int totalLength, string s, char padding, char border)
		{
			totalLength -= s.Length;
			int notEven = 0;
			if (totalLength % 2 != 0)
			{
				notEven = -1;
				totalLength += 1;
			}
			for (int i = 0; i < totalLength + notEven; i++)
			{
				if (totalLength / 2 == i)
				{
					Console.Write(s);
				}
				if (i == 0)
				{
					Console.Write(border);
				}
				Console.Write(padding);
				if (i == totalLength + notEven - 1)
				{
					Console.Write(border);
				}
			}
			Console.WriteLine();
		}
		static public void PaddedWrite(int totalLength, string s, char padding)
		{
			totalLength -= s.Length;
			int notEven = 0;
			if (totalLength % 2 != 0)
			{
				notEven = -1;
				totalLength += 1;
			}
			for (int i = 0; i < totalLength + notEven; i++)
			{
				if (totalLength / 2 == i)
				{
					Console.Write(s);
				}
				Console.Write(padding);
			}
		}
		static public void PaddedWrite(int totalLength, string s, char padding, char border)
		{
			totalLength -= s.Length;
			int notEven = 0;
			if (totalLength % 2 != 0)
			{
				notEven = -1;
				totalLength += 1;
			}
			for (int i = 0; i < totalLength + notEven; i++)
			{
				
				if (totalLength / 2 == i)
				{
					Console.Write(s);
				}
				if (i == 0)
				{
					Console.Write(border);
				}
				Console.Write(padding);
				if (i == totalLength + notEven - 1)
				{
					Console.Write(border);
				}
			}
		}
		static public void ClearMainFrame()
		{
			for (int y = 1; y < Border.MainFrameHeight; y++)
			{
				Console.SetCursorPosition(1,y);
				WriteBlanks( Border.GameConsoleWidth);
			}
		}
		static public void ClearConsole()
		{
			consoleIndexX = consoleIndexY = 0;
			for (int y = 0; y < 8; y++)
			{
				Console.SetCursorPosition(1, 41+y);
				WriteBlanks(78);
			}
			consoleIndexX = 0;
			consoleIndexY = 0;
		}
		static public void GameWrite(string text,ConsoleColor color)
		{
			Console.BackgroundColor = color;
			Console.SetCursorPosition(consoleIndexX + 1, 41 + consoleIndexY);
			consoleIndexX += text.Length;
			Console.Write(text);
			if (consoleIndexX > 30)
			{
				consoleIndexX = 0;
				consoleIndexY++;
			}
			Console.ResetColor();
		}
		static public void GameWrite(string text)
		{
			Console.SetCursorPosition(consoleIndexX+1,41+consoleIndexY);
			consoleIndexX += text.Length;
			Console.Write(text);
			if (consoleIndexX > 80)
			{
				consoleIndexX = 0;
				consoleIndexY++;
			}
			if (consoleIndexY > 8)
			{
				consoleIndexY = 0;
			}
		}
		static public void GameWriteLine()
		{
			Console.SetCursorPosition(consoleIndexX + 1, 41 + consoleIndexY);
			consoleIndexX = 0;
			consoleIndexY++;
			if (consoleIndexY > 8)
			{
				consoleIndexY = 0;
			}
		}
		static public void GameWriteLine(string text)
		{
			Console.SetCursorPosition(consoleIndexX+1, 41+consoleIndexY);
			Console.Write(text);
			consoleIndexX = 0;
			consoleIndexY++;
			if (consoleIndexY > 8)
			{
				consoleIndexY = 0;
			}
		}
		static public void GameWriteLine(string text,ConsoleColor color)
		{
			Console.SetCursorPosition(consoleIndexX + 1, 41 + consoleIndexY);
			GameWrite(text, color); 
			consoleIndexX = 0;
			consoleIndexY++;
			if (consoleIndexY > 8)
			{
				consoleIndexY = 0;
			}
		}
		static public string GameSimpleRead(string text, int lengthLimit, ConsoleColor color)
		{
			bool allowedLength = true;
			while (true)
			{
				if (allowedLength)
				{
					GameWriteLine(text + " - Allowed length :" + lengthLimit, color);
				}
				else
				{
					GameWriteLine(text + "- string was too long - Allowed length :" + lengthLimit, color);
				}
				Console.SetCursorPosition(1, 41+consoleIndexY);
				string read = Console.ReadLine();
				if (read.Count() > 0 && read.Count() < lengthLimit)
				{
					return read;
				}
				else
				{
					ClearConsole();
					allowedLength = false;
				}
			}
		}
		static public void GameClearLine(int line)
		{
			Console.SetCursorPosition(1, line);
			WriteBlanks(50);
		}
		/// <summary>
		/// Clears the current line in the game console and sets the position to the same line
		/// </summary>
		static public void GameClearLine()
		{
			Console.SetCursorPosition(1, 41+consoleIndexY);
			WriteBlanks(78);
			Console.SetCursorPosition(1, 41 + consoleIndexY);
			consoleIndexX = 0;
		}
		static public void GameGotoLine(int line)
		{
			consoleIndexY = line;
			consoleIndexX = 0;
			Console.SetCursorPosition(1, line);
		}
	}
}
