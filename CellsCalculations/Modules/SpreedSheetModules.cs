using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CellsCalculations.Modules
{
    public class SpreedSheet
    {
        private Dictionary<CellIndex,Cells> sheet = new Dictionary<CellIndex, Cells> { };
        private int lastRow;
        private int lastColumn;
        public SpreedSheet(string fileName)
        {
            try
            {
                FileStream fstream = File.OpenRead(fileName);
                using (fstream)
                {
                    byte[] array = new byte[fstream.Length];
                    fstream.Read(array, 0, array.Length);
                    string textFromFile = Encoding.Default.GetString(array);
                    pasteToSheet(textFromFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void pasteToSheet(string dataFromFile)
        {
            string dataString = dataFromFile;
            lastRow = 0;
            lastColumn = 0;
            for (int i = 0; i < dataString.Length; i++) //визначення к-сті рядків та стовбців
            {
                if (dataString.ElementAt(i).Equals('\n')) lastRow++;
                if (dataString.ElementAt(i).Equals('\t') && lastRow == 0) lastColumn++;
            }
            if (lastRow > 0 || lastColumn > 0)
            { lastRow++; lastColumn++; }
            else { throw new Exception("Відсутні данні"); }
            string[] arrayDataFromFile = dataString.Split('\t', '\n');
            int aDi = 0;
            for (int r = 0; r < lastRow; r++)
            {
                for (int c = 0; c < lastColumn; c++)
                {
                    if (arrayDataFromFile[aDi].StartsWith("'")) { sheet.Add(new CellIndex(c, r), new StringCell(arrayDataFromFile[aDi].Substring(1))); }
                    else
                    {
                        if (arrayDataFromFile[aDi].StartsWith("=")) { sheet.Add(new CellIndex(c, r), new ExpressionCell(arrayDataFromFile[aDi].Substring(1))); }
                        else
                        {
                            if (Char.IsDigit(arrayDataFromFile[aDi], 0)) { sheet.Add(new CellIndex(c, r), new DigitCell(arrayDataFromFile[aDi])); }
                            else { sheet.Add(new CellIndex(c, r), new NullCell()); }
                        }
                    }
                    aDi++;
                }
            }
            printResult();
        }
        private void printResult() //вивести результат на екран
        {                        
            Console.WriteLine("Таблиця результатiв:");
            for (int r = 0; r < lastRow; r++)
            {
                for (int c = 0; c < lastColumn; c++)
                {
                    Console.Write("\t");
                    Console.Write(Value(new CellIndex(c, r)));                    
                }
                Console.WriteLine("\n");
            }
        }
        private Cells Cell(CellIndex cell)
        {
            Cells returnCell = new NullCell();
            foreach (CellIndex key in sheet.Keys)
            {
                if (key.Column() == cell.Column() && key.Row() == cell.Row())
                {
                    returnCell = sheet[key];
                    break;
                }
            }
            return returnCell;
        }
        public string Value(CellIndex cell)
        {
            string result;
            if (Cell(cell).IsText())
            {
                result = Cell(cell).getResult();
            }
            else
            {
                result = DigitValue(cell).ToString();
            }            
            return result;
        }
        public double DigitValue(CellIndex cell)
        {
            double result;
            if (Cell(cell).IsFormul())
            {
                result = Calculate(Cell(cell).getContent());
            }
            else
            {
                result = Cell(cell).getDigitResult();
            }
            return result;
        }
        private ContentElement[] ConvertToPostfixNotation(IEnumerable<ContentElement> input)
        {
            List<ContentElement> outputSeparated = new List<ContentElement>();
            Stack<ContentElement> stack = new Stack<ContentElement>();
            foreach (ContentElement c in input)
            {
                if (c.IsOperators())
                {
                    if (stack.Count > 0 && !c.IsOpenParenthesis())
                    {
                        if (c.IsCloseParenthesis())
                        {
                            ContentElement s = stack.Pop();
                            while (!s.IsOpenParenthesis())
                            {
                                outputSeparated.Add(s);
                                s = stack.Pop();
                            }
                        }
                        else if (!c.IsDegree())
                        {
                            if (c.GetPriority() > stack.Peek().GetPriority())
                                stack.Push(c);
                            else
                            {
                                while (stack.Count > 0 && c.GetPriority() <= stack.Peek().GetPriority())
                                    outputSeparated.Add(stack.Pop());
                                stack.Push(c);
                            }
                        }
                        else
                        {
                            stack.Push(c);
                        }
                    }
                    else
                        stack.Push(c);
                }
                else
                    outputSeparated.Add(c);
            }
            if (stack.Count > 0)
                foreach (ContentElement c in stack)
                    outputSeparated.Add(c);

            return outputSeparated.ToArray();
        }
        private double Calculate(IEnumerable<ContentElement> input)
        {
            Stack<ContentElement> stack = new Stack<ContentElement>();
            Queue<ContentElement> queue = new Queue<ContentElement>(ConvertToPostfixNotation(input));
            ContentElement firstInQuene = queue.Dequeue();
            double returnValue = 0;
            double summ;
            if (queue.Count == 0)
            {
                if (firstInQuene.IsIndex())
                {
                    returnValue = DigitValue(new CellIndex(firstInQuene.Text()));
                }
                else
                {
                    returnValue = firstInQuene.Value();
                }
            }
            else
            {
                while (queue.Count >= 0)
                {
                    if (!firstInQuene.IsOperators())
                    {
                        stack.Push(firstInQuene);
                        firstInQuene = queue.Dequeue();
                    }
                    else
                    {
                        ContentElement firstEl = stack.Pop();
                        ContentElement secondEl = stack.Pop();
                        summ = MathOp(firstEl, secondEl, firstInQuene);

                        stack.Push(new DigitElement(summ.ToString()));
                        if (queue.Count > 0)
                            firstInQuene = queue.Dequeue();
                        else
                            break;
                    }
                }
                returnValue = stack.Pop().Value();
            }            
            return returnValue;
        }
        private double MathOp(ContentElement firstDigit, ContentElement secondDigit, ContentElement operation)
        {
            double summ = 0;
            double firstDigitEl;
            double secondDigitEl;
            if (firstDigit.IsIndex())
            {
                firstDigitEl = DigitValue(new CellIndex(firstDigit.Text()));
            }
            else
            {
                firstDigitEl = firstDigit.Value();
            }
            if (secondDigit.IsIndex())
            {
                secondDigitEl = DigitValue(new CellIndex(secondDigit.Text()));
            }
            else
            {
                secondDigitEl = secondDigit.Value();
            }
            try
            {
                switch (operation.Text())
                {

                    case "+":
                        {
                            summ = firstDigitEl + secondDigitEl;
                            break;
                        }
                    case "-":
                        {
                            summ = secondDigitEl - firstDigitEl;
                            break;
                        }
                    case "*":
                        {
                            summ = secondDigitEl * firstDigitEl;
                            break;
                        }
                    case "/":
                        {
                            summ = secondDigitEl / firstDigitEl;
                            break;
                        }
                    case "^":
                        {
                            summ = Math.Pow(secondDigitEl, firstDigitEl);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message + " Математическая опперация не выполнилась!");
            }

            return summ;
        }
    }
}
