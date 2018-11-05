using System;
using System.Collections.Generic;
using System.Linq;

namespace CellsCalculations.Modules
{
    public abstract class Cells
    {
        public abstract IEnumerable<ContentElement> getContent();
        public abstract string getResult();
        public abstract double getDigitResult();
        public virtual bool IsFormul()
        {
            return false;
        }
        public virtual bool IsText()
        {
            return false;
        }
    }
    public class StringCell : Cells
    {
        ContentElement CellContent;        
        public override IEnumerable<ContentElement> getContent()
        {
            yield return CellContent;
        }
        public StringCell(string CellContent)
        {
            this.CellContent = new TextElement(CellContent);
        }
        public override string getResult()
        {
            return CellContent.Text();
        }
        public override double getDigitResult()
        {
            return CellContent.Value();
        }
        public override bool IsText()
        {
            return true;
        }
    }
    public class ExpressionCell : Cells
    {
        IEnumerable<ContentElement> CellContent;
        string TextResult;
        public override IEnumerable<ContentElement> getContent()
        {
            return CellContent;
        }
        public ExpressionCell(string CellContent)
        {
            this.CellContent = Separate(CellContent);
        }
        public override string getResult()
        {
            foreach (ContentElement element in this.CellContent)
            {
                TextResult = TextResult + element.Text();
            }
            return TextResult;
        }
        public override double getDigitResult()
        {
            throw new Exception("Ссылка не чмсло");
        }
        private IEnumerable<ContentElement> Separate(string input)
        {
            List<string> standart_operators = new List<string>(new string[] { "(", ")", "+", "-", "*", "/", "^" });
            int pos = 0;
            ContentElement elementOfString;
            while (pos < input.Length)
            {
                string s = string.Empty + input[pos];
                bool isOpenParenthesisDivideMultiply;
                if (pos == 0)
                {
                    isOpenParenthesisDivideMultiply = true;
                }
                else
                {
                    isOpenParenthesisDivideMultiply = input[pos - 1] == '(' | input[pos - 1] == '*' | input[pos - 1] == '/';
                }
                if (!standart_operators.Contains(input[pos].ToString()) || (input[pos] == '-' && isOpenParenthesisDivideMultiply))
                {
                    if (Char.IsDigit(input[pos]) || input[pos] == '-')
                    {
                        for (int i = pos + 1; i < input.Length &&
                            (Char.IsDigit(input[i]) || input[i] == ',' || input[i] == '.'); i++)
                            s += input[i];
                        elementOfString = new DigitElement(s);

                    }
                    else
                    {
                        if (Char.IsLetter(input[pos]) || input[pos] == '-')
                        {
                            for (int i = pos + 1; i < input.Length &&
                                (Char.IsLetter(input[i]) || Char.IsDigit(input[i])); i++)

                                s += input[i];
                            elementOfString = new IndexElement(s);

                        }
                        else
                        {
                            throw new Exception("Эллемент не цифра и не ссылка на ячейку!");
                        }
                    }
                }
                else
                {
                    elementOfString = new OperationElement(s);
                }
                yield return elementOfString;
                pos += s.Length;
            }
        }
        public override bool IsFormul()
        {
            return true;
        }
    }
    public class DigitCell : Cells
    {
        ContentElement CellContent;
        public override IEnumerable<ContentElement> getContent()
        {
            yield return CellContent;
        }
        public DigitCell(string CellContent)
        {
            this.CellContent = new DigitElement(CellContent);
        }
        public override string getResult()
        {
            return CellContent.Text();
        }
        public override double getDigitResult()
        {
            return CellContent.Value();
        }
    }
    public class NullCell : Cells
    {
        ContentElement CellContent;
        public override IEnumerable<ContentElement> getContent()
        {
            yield return CellContent;
        }
        public NullCell()
        {
            this.CellContent = new DigitElement("0");
        }
        public override string getResult()
        {
            return "";
        }
        public override double getDigitResult()
        {
            return 0;
        }
    }
    public class CellIndex
    {
        int column;
        int row;
        public CellIndex(string index)
        {
            char[] charIndex = index.ToCharArray();
            string columnIndx = "";
            int rowIndx;
            for (int i = 0; (i < charIndex.Length && Char.IsLetter(charIndex[i])); i++)
            {
                columnIndx = columnIndx + charIndex[i];
            }
            if (Int32.TryParse(index.Substring(columnIndx.Length), out rowIndx))
            {
                column = ColumnToDigitIndex(columnIndx);
                row = rowIndx - 1;
            }
            else
            {
                throw new Exception("Ссылка на ячейку указана неверно!");
            }

        }
        public CellIndex(int column, int row)
        {
            this.column = column;
            this.row = row;
        }
        public int Column()
        {
            return column;
        }
        public int Row()
        {
            return row;
        }

        private int ColumnToDigitIndex(string column)
        {
            int columnIndex = 0;
            if (column.Length > 0 && column.All(a => (a >= 'A' && a <= 'Z')))
                try
                {
                    for (int i = column.Length; i > 0; i--)
                        columnIndex += (int)checked(Math.Pow(26, i - 1) + (column[i - 1] - 'A') *
                            Math.Pow(26, column.Length - i));
                }
                catch (OverflowException)
                {
                    columnIndex = -1;
                }
            else
                columnIndex = -1;
            return columnIndex - 1;
        }
    }
}
