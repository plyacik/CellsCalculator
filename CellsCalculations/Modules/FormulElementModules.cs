using System;

namespace CellsCalculations.Modules
{
    public abstract class ContentElement
    {
        public abstract string Text();
        public abstract double Value();
        public abstract byte GetPriority();
        public virtual bool IsIndex()
        {
            return false;
        }
        public virtual bool IsOperators()
        {
            return false;
        }
        public virtual bool IsDegree()
        {
            return false;
        }
        public virtual bool IsOpenParenthesis()
        {
            return false;
        }
        public virtual bool IsCloseParenthesis()
        {
            return false;
        }
    }
    class DigitElement : ContentElement
    {
        private string StringElement;
        public DigitElement(string element)
        {
            StringElement = element;
        }
        public override double Value()
        {
            return Convert.ToDouble(StringElement);
        }
        public override string Text()
        {
            return Convert.ToDouble(StringElement).ToString();
        }
        public override byte GetPriority()
        {
            throw new Exception("Число не имеет приоритета!");
        }
    }

    class IndexElement : ContentElement
    {
        private string StringElement;
        public override bool IsIndex()
        {
            return true;
        }
        public IndexElement(string element)
        {
            StringElement = element;
        }
        public override double Value()
        {
            throw new Exception("Ссылка на ячейку!");
        }
        public override string Text()
        {
            return StringElement;
        }
        public override byte GetPriority()
        {
            throw new Exception("Ссылка на ячейку не имеет приоритета!");
        }
    }
    class OperationElement : ContentElement
    {
        private string StringElement;
        public override bool IsOperators()
        {
            return true;
        }
        public override bool IsDegree()
        {
            return StringElement.Equals("^");
        }
        public override bool IsOpenParenthesis()
        {
            return StringElement.Equals("(");
        }
        public override bool IsCloseParenthesis()
        {
            return StringElement.Equals(")");
        }
        public OperationElement(string element)
        {
            StringElement = element;
        }
        public override double Value()
        {
            throw new Exception("Операция не переводится в число");
        }
        public override string Text()
        {
            return StringElement;
        }
        public override byte GetPriority()
        {
            switch (StringElement)
            {
                case "(":
                case ")":
                    return 0;
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                case "^":
                    return 3;
                default:
                    return 4;
            }
        }
    }
    class TextElement : ContentElement
    {
        private string StringElement;
        public TextElement(string element)
        {
            StringElement = element;
        }
        public override double Value()
        {
            throw new Exception("Ссылка на текст не возвращает числовое значение!");
        }
        public override string Text()
        {
            return StringElement;
        }
        public override byte GetPriority()
        {
            throw new Exception("Текст не имеет приоритета!");
        }
    }
}
