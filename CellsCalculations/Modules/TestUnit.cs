using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellsCalculations.Modules
{
    class TestUnit
    {
        public TestUnit()
        {

        }
        public void TestSheet()
        {
            CellIndex TestCell = new CellIndex("A1");
            CellIndex TestCell2 = new CellIndex("B2");
            if (TestCell.Column() == 0 && TestCell.Row() == 0)
            {
                Console.WriteLine("OK");
                if (TestCell2.Column() == 1 && TestCell2.Row() == 1)
                {
                    Console.WriteLine("OK");
                }
                else Console.WriteLine("Индекс ячейки B2 (1,1):" + TestCell2.Column() + ", " + TestCell2.Row());
            }
            else Console.WriteLine("Индекс ячейки A1 (0,0):" + TestCell.Column() + ", " + TestCell.Row());
        }
    }
}
