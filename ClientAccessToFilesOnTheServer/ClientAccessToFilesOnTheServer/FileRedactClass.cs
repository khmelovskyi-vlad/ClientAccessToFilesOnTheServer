using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAccessToFilesOnTheServer
{
    class FileRedactClass
    {
        public FileRedactClass(string stringArrayBytes)
        {
            this.StringArrayBytes = stringArrayBytes;
            SavingFileFlag = false;
        }
        public string AdressName { get; set; }
        private StringBuilder textBuilderEnd;
        private StringBuilder allTextBuilder;
        private StringBuilder textBuilderFirst = new StringBuilder();
        private int topPosition;
        private int leftPosition;
        private List<string> fileLines = new List<string>();
        private string StringArrayBytes;
        private bool SavingFileFlag;
        public (bool savingFileFlag, string file) RedactFile()
        {
            SavingFileFlag = EditingFile();
            return (SavingFileFlag, StringArrayBytes);

        }
        private bool EditingFile()
        {
            var undestand = UnderstandingQuestions();
            if (undestand.Key == ConsoleKey.Enter)
            {
                InitializationOfVariables();
                if (LeftOrTopPositionIsOrNotTooBig())
                {
                    return false;
                }
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.LeftArrow)
                    {
                        LeftArrow();
                    }
                    else if (key.Key == ConsoleKey.RightArrow)
                    {
                        RightArrow();
                    }
                    else if (key.Key == ConsoleKey.UpArrow)
                    {
                        topPosition--;
                    }
                    else if (key.Key == ConsoleKey.DownArrow)
                    {
                        topPosition++;
                    }
                    else if (key.Key == ConsoleKey.Backspace)
                    {
                        Backspace();
                    }
                    else if (key.Key == ConsoleKey.Delete)
                    {
                        Delete();
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        Enter();
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                    else
                    {
                        ElseClick(key);
                    }
                    if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.DownArrow)
                    {
                        UpOrDownArrow();
                    }
                    Cursor();
                }
                return SavingFile();
            }
            return false;
        }
        private bool LeftOrTopPositionIsOrNotTooBig()
        {
            try
            {
                Console.SetCursorPosition(leftPosition, topPosition);
                return false;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("This text is too big, can`t redact it");
                return true;
            }
        }
        private void Cursor()
        {
            try
            {
                Console.SetCursorPosition(leftPosition, topPosition);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.Clear();
                Console.WriteLine("Console window is too small, expand it and any key except Escape, or if you want exit the program click Escape");
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                {
                    throw new OperationCanceledException();
                }
                else
                {
                    InitializationOfVariables();
                    Cursor();
                }
            }
        }
        private void InitializationOfVariables()
        {
            Console.Clear();
            for (int i = 0; i < StringArrayBytes.Length; i++)
            {
                if (StringArrayBytes[i] == '\n')
                {
                    continue;
                }
                if (StringArrayBytes[i] == '\r' && StringArrayBytes[i + 1] == '\n')
                {
                    fileLines.Add(textBuilderFirst.ToString());
                    textBuilderFirst = new StringBuilder();
                    continue;
                }
                textBuilderFirst.Append(StringArrayBytes[i]);
            }
            fileLines.Add(textBuilderFirst.ToString());
            textBuilderEnd = new StringBuilder();
            allTextBuilder = new StringBuilder();
            topPosition = fileLines.Count - 1;
            leftPosition = textBuilderFirst.Length;
            WriteLines(fileLines);
        }
        private ConsoleKeyInfo UnderstandingQuestions()
        {
            Console.WriteLine("If you want to edit the text click Enter, else click another key");
            Console.WriteLine("If you press enter, only the text of your file will be displayed on the console, at the end of editing press Escape");
            var key = Console.ReadKey(true);
            return key;
        }
        private bool SavingFile()
        {
            Console.Clear();
            Console.WriteLine("If you want to save this file, click Enter, else, enouther click");
            var key = Console.ReadKey(true);
            Console.Clear();
            if (key.Key == ConsoleKey.Enter)
            {
                ListToString();
                Console.WriteLine("File was saving");
                return true;
            }
            return false;
        }
        private void ListToString()
        {
            StringBuilder allTextFile = new StringBuilder();
            allTextFile.Append(fileLines[0]);
            for (int i = 1; i < fileLines.Count; i++)
            {
                allTextFile.Append($"\r\n{fileLines[i]}");
            }
            StringArrayBytes = allTextFile.ToString();
        }
        private static void WriteLines(List<string> fileStrings)
        {
            foreach (var fileString in fileStrings)
            {
                Console.WriteLine(fileString);
            }
        }
        private void LeftArrow()
        {
            if (leftPosition != 0)
            {
                textBuilderEnd.Insert(0, textBuilderFirst[textBuilderFirst.Length - 1]);
                textBuilderFirst.Remove(textBuilderFirst.Length - 1, 1);
                leftPosition--;
            }
            else
            {
                if (leftPosition == 0 && topPosition == 0)
                {
                    leftPosition = 0;
                    return;
                }
                topPosition--;
                textBuilderFirst = new StringBuilder(fileLines[topPosition]);
                leftPosition = textBuilderFirst.Length;
                textBuilderEnd.Clear();
            }
            return;
        }
        private void RightArrow()
        {
            var lengthFiletLine = fileLines[topPosition].Length;
            if (leftPosition != lengthFiletLine)
            {
                textBuilderFirst.Append(textBuilderEnd[0]);
                textBuilderEnd.Remove(0, 1);
                leftPosition++;
            }
            else
            {
                if (leftPosition == lengthFiletLine && topPosition == fileLines.Count - 1)
                {
                    leftPosition = textBuilderFirst.Length;
                    return;
                }
                topPosition++;
                textBuilderFirst.Clear();
                leftPosition = 0;
                textBuilderEnd = new StringBuilder(fileLines[topPosition]);
            }
            return;
        }
        private void UpOrDownArrow()
        {
            if (topPosition == fileLines.Count)
            {
                topPosition = fileLines.Count - 1;
                return;
            }
            if (topPosition < 0)
            {
                topPosition = 0;
                return;
            }
            textBuilderFirst = new StringBuilder(fileLines[topPosition]);
            var countEndBuilder = textBuilderFirst.Length - leftPosition;
            textBuilderEnd.Clear();
            if (countEndBuilder >= 0)
            {
                textBuilderEnd.Append(Convert.ToString(textBuilderFirst), leftPosition, countEndBuilder);
                textBuilderFirst.Remove(leftPosition, countEndBuilder);
            }
            else
            {
                leftPosition = textBuilderFirst.Length;
            }
            return;
        }
        private void Backspace()
        {
            allTextBuilder.Clear();
            Console.Clear();
            if (textBuilderFirst.Length != 0)
            {
                textBuilderFirst.Remove(textBuilderFirst.Length - 1, 1);
            }
            else
            {
                if (topPosition > 0)
                {
                    topPosition--;
                    textBuilderFirst = new StringBuilder(fileLines[topPosition]);
                }
                else
                {
                    WriteLines(fileLines);
                    return;
                }
                SmallArrayString();
                WriteLines(fileLines);
                leftPosition = textBuilderFirst.Length;
                return;
            }
            DeleteBackspaceWriter();
            leftPosition--;
            return;
        }
        private void DeleteBackspaceWriter()
        {
            allTextBuilder.Append(textBuilderFirst);
            allTextBuilder.Append(textBuilderEnd);
            fileLines[topPosition] = Convert.ToString(allTextBuilder);
            WriteLines(fileLines);
        }
        private void Delete()
        {
            allTextBuilder.Clear();
            Console.Clear();
            if (textBuilderEnd.Length != 0)
            {
                textBuilderEnd.Remove(0, 1);
            }
            else
            {
                if (topPosition < fileLines.Count - 1)
                {
                    textBuilderEnd = new StringBuilder(fileLines[topPosition + 1]);
                }
                else
                {
                    WriteLines(fileLines);
                    return;
                }
                SmallArrayString();
                WriteLines(fileLines);
                return;
            }
            DeleteBackspaceWriter();
            return;
        }
        private void Enter()
        {
            Console.Clear();
            fileLines[topPosition] = Convert.ToString(textBuilderFirst);
            textBuilderFirst.Clear();
            topPosition++;
            leftPosition = 0;
            BigArrayString();
            fileLines[topPosition] = Convert.ToString(textBuilderEnd);
            WriteLines(fileLines);
            return;
        }
        private void ElseClick(ConsoleKeyInfo key)
        {
            allTextBuilder.Clear();
            Console.Clear();
            textBuilderFirst.Append(key.KeyChar);
            DeleteBackspaceWriter();
            leftPosition++;
            return;
        }
        private void BigArrayString()
        {
            var smallList = new List<string>();
            var flag = false;
            for (int i = 0; i < fileLines.Count; i++)
            {
                if (i == topPosition)
                {
                    flag = true;
                }
                if (flag)
                {
                    smallList.Add(fileLines[i]);
                    fileLines.RemoveAt(topPosition);
                    i--;
                }
            }
            fileLines.Add(textBuilderEnd.ToString());
            fileLines.AddRange(smallList);
        }
        private void SmallArrayString()
        {
            var someString = fileLines[topPosition + 1];
            fileLines.RemoveAt(topPosition + 1);
            fileLines[topPosition] = $"{fileLines[topPosition]}{someString}";
        }
    }
}
