using System;
using System.Collections.Generic;

namespace led
{
    class Program
    {
        static List<string> buffer;
        static string currentLine;
        static int index;
        static Stack<List<string>> undoHistory;
        static string searchString;

        static void Main(string[] args)
        {
            buffer = new List<string>();
            index = 0;
            undoHistory = new Stack<List<string>>();
            searchString = "";
            while (true)
            {
                printLineNumber(index + 1);
                currentLine = Console.ReadLine();
                if (currentLine.Length > 1 && currentLine[0] == '/')
                {
                    switch (currentLine[1])
                    {
                        case '/':
                            writeLine(currentLine.Substring(1));
                            break;
                        case '!':
                            string cmd = "/c " + currentLine.Substring(2);
                            System.Diagnostics.Process proc = new System.Diagnostics.Process();
                            proc.StartInfo.FileName = "cmd.exe";
                            proc.StartInfo.Arguments = cmd;
                            proc.StartInfo.UseShellExecute = false;
                            proc.StartInfo.RedirectStandardOutput = true;
                            proc.StartInfo.RedirectStandardError = true;
                            proc.Start();
                            string[] lines;
                            if (!proc.StandardOutput.EndOfStream)
                            {
                                lines = proc.StandardOutput.ReadToEnd().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                                foreach (string line in lines)
                                {
                                    writeLine(line);
                                    printLine(index - 1);
                                }
                            }
                            if (!proc.StandardError.EndOfStream)
                            {
                                lines = proc.StandardError.ReadToEnd().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                                foreach (string line in lines)
                                {
                                    writeLine(line);
                                    printLine(index - 1);
                                }
                            }
                            break;
                        case 'c':
                            Console.Clear();
                            break;
                        case 'e':
                            index = buffer.Count;
                            break;
                        case 'E':
                            index = 0;
                            buffer.Clear();
                            break;
                        case 'g':
                            try
                            {
                                index = Convert.ToInt32(currentLine.Substring(2)) - 1;
                            }
                            catch (OverflowException)
                            {
                                printError("Outside the range of the Int32 type.");
                            }
                            catch (FormatException)
                            {
                                printError("Invalid line number.");
                            }
                            if (index < 0)
                            {
                                index = 0;
                            }
                            break;
                        case 'h':
                        case '?':
                            printHelp();
                            break;
                        case 'p':
                            for (int i = 0; i < buffer.Count; i++)
                            {
                                printLine(i);
                            }
                            break;
                        case 'q':
                            // Exit
                            return;
                        case 'r':
                            string replaceString = currentLine.Substring(2);
                            for (int i = 0; i < buffer.Count; i++)
                            {
                                string replacedString = buffer[i].Replace(searchString, replaceString);
                                if (replacedString != buffer[i])
                                {
                                    saveUndo();
                                    buffer[i] = replacedString;
                                }
                            }
                            break;
                        case 's':
                            searchString = currentLine.Substring(2);
                            for (int i = 0; i < buffer.Count; i++)
                            {
                                if (buffer[i].Contains(searchString))
                                {
                                    printLine(i);
                                }
                            }
                            break;
                        case 'u':
                            int steps = 0;
                            try
                            {
                                steps = Convert.ToInt32(currentLine.Substring(2)) - 1;
                            }
                            catch (Exception)
                            {
                                undo();
                                break;
                            }
                            for (int i = 0; i < steps; i++)
                            {
                                undo();
                            }
                            break;
                        default:
                            printError("Invalid command.");
                            break;
                    }
                }
                else
                {
                    writeLine(currentLine);
                }
            }
        }

        static void writeLine(string line)
        {
            saveUndo();
            if (index == buffer.Count)
            {
                buffer.Add(line);
                index++;
            }
            else if (index < buffer.Count)
            {
                buffer[index] = line;
                index++;
            }
            else
            {
                for (int i = buffer.Count; i < index; i++)
                {
                    buffer.Add("");
                }
                buffer.Add(line);
                index = buffer.Count;
            }
        }

        static void saveUndo()
        {
            undoHistory.Push(new List<string>(buffer));
        }

        static void undo()
        {
            if (undoHistory.Count > 0)
            {
                buffer = undoHistory.Pop();
                index = buffer.Count;
            }
        }

        static void printLine(int i)
        {
            printLineNumber(i + 1);
            Console.WriteLine(buffer[i]);
        }

        static void printLineNumber(int number)
        {
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("{0,5}", number);
            Console.ResetColor();
            Console.Write(" ");
        }

        static void printError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static void printHelp()
        {
            Console.WriteLine("\t{0,-12} - {1}", "/!<command>", "Run a shell command and copies the output.");
            Console.WriteLine("\t{0,-12} - {1}", "/c", "Clear the screen.");
            Console.WriteLine("\t{0,-12} - {1}", "/e", "Go to the end of the file.");
            Console.WriteLine("\t{0,-12} - {1}", "/E", "Empty the file.");
            Console.WriteLine("\t{0,-12} - {1}", "/g<line>", "Go to the specified line of the file.");
            Console.WriteLine("\t{0,-12} - {1}", "/h /?", "Print this help.");
            Console.WriteLine("\t{0,-12} - {1}", "/p", "Print all the lines of the file.");
            Console.WriteLine("\t{0,-12} - {1}", "/q", "Exit led.");
            Console.WriteLine("\t{0,-12} - {1}", "/r<string>", "Replace every occurence of the string found with /s.");
            Console.WriteLine("\t{0,-12} - {1}", "/s<string>", "Print every line that contains 'string'.");
            Console.WriteLine("\t{0,-12} - {1}", "/u[steps]", "Undo line insertions.");
        }
    }
}
