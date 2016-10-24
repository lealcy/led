using System;
using System.Collections.Generic;

namespace led
{
    class Program
    {
        static void Main(string[] args)
        {
            int lineIndex = 0;
            var lines = new List<string>();
            string line;
            while(true) {
                printLineNumber(lineIndex + 1);
                line = Console.ReadLine();
                if (line.Length > 1 && line[0] == '/') {
                    switch(line[1]) {
                        case 'c':
                            Console.Clear();
                            break;
                        case 'C':
                            lineIndex = 0;
                            lines.Clear();
                            break;
                        case 'e':
                            lineIndex = lines.Count;
                            break;
                        case 'g':
                            try {
                                lineIndex = Convert.ToInt32(line.Substring(2)) - 1;
                            } catch (OverflowException) {
                                printError("Outside the range of the Int32 type.");
                            } catch (FormatException) {
                                printError("Invalid line number.");
                            }
                            if (lineIndex < 0) {
                                lineIndex = 0;
                            }
                            break;
                        case 'h':
                        case '?':
                            printHelp();
                            break;
                        case 'p':
                            for (int i = 0; i < lines.Count; i++)
                            {
                                printLineNumber(i + 1);
                                Console.WriteLine(lines[i]);
                            }
                            break;
                        case 'q':
                            return;
                        default:
                            printError("Invalid command.");
                            break;
                    }
                } else {
                    if (lineIndex == lines.Count) {
                        lines.Add(line);
                        lineIndex++;
                    } else if (lineIndex < lines.Count) {
                        lines[lineIndex] = line;
                        lineIndex++;
                    }
                    else {
                        for (int i = lines.Count; i < lineIndex; i++) {
                            lines.Add("");
                        }
                        lines.Add(line);
                        lineIndex = lines.Count;
                    }
                }
            }
        }

        static void printLineNumber(int number) {
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("{0,5}", number);
            Console.ResetColor();
            Console.Write(" ");
        }

        static void printError(string message) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static void printHelp() {
            Console.WriteLine("\t{0,-8} - {1}", "/c", "Clear the screen.");
            Console.WriteLine("\t{0,-8} - {1}", "/C", "Empty the file.");
            Console.WriteLine("\t{0,-8} - {1}", "/e", "Go to the end of the file.");
            Console.WriteLine("\t{0,-8} - {1}", "/g<line>", "Go to the specified line of the file.");
            Console.WriteLine("\t{0,-8} - {1}", "/h /?", "Print this help.");
            Console.WriteLine("\t{0,-8} - {1}", "/p", "Print all the lines of the file.");
            Console.WriteLine("\t{0,-8} - {1}", "/q", "Exit led.");
        }
    }
}
