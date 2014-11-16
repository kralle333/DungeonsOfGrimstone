using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RPGAsci.ConsoleDrawing
{
	class AsciiArtConverter
	{
		public static List<ConsoleColor> colorList = new List<ConsoleColor>(new ConsoleColor[] {ConsoleColor.Black, ConsoleColor.DarkGreen, ConsoleColor.DarkBlue, ConsoleColor.DarkRed,ConsoleColor.Red, ConsoleColor.Blue,ConsoleColor.DarkGray,ConsoleColor.DarkYellow,ConsoleColor.White,ConsoleColor.Green});
		public static void ConvertAsciiMonster(string inputAsciiPath, string outputFileName)
		{
			string file = "Art/Monsters/" + inputAsciiPath + ".txt";
			if (File.Exists(file))
			{
				StringBuilder sb = new StringBuilder();
				string[] input = System.IO.File.ReadAllLines(file);
				string[] output = new string[input.Length];
				Dictionary<char, int> assignedColors = new Dictionary<char, int>();
				int usedColorIndex = 0;
				assignedColors[' '] = 0;

				for (int line = 0; line < input.Length; line++)
				{
					for (int c = 0; c < input[line].Length; c++)
					{
						char currentChar = input[line][c];
						if (!assignedColors.ContainsKey(currentChar))
						{
							assignedColors.Add(currentChar, usedColorIndex);
							usedColorIndex++;
							if (usedColorIndex > 9)
							{
								usedColorIndex = 1;
							}
						}
						sb.Append(assignedColors[currentChar]);
					}
					output[line] = sb.ToString();
					sb.Clear();
				}
				System.IO.File.WriteAllLines("Art/Monsters/" + outputFileName+".ara",output);
			}
			else
			{
				Console.WriteLine("No monster found");
			}
		}
	}
}
