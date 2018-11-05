using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CellsCalculations.Modules;

namespace CellsCalculations
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestUnit Test = new TestUnit();
            //Test.TestSheet();
            //Test.TestMath();
            //Console.ReadLine();
            try
            {
                SpreedSheet sheet = new SpreedSheet("input.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }

    }
    
}
