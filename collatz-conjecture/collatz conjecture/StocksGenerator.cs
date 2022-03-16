using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace collatz_conjecture
{
    public class StocksGenerator
    {
        static Random random = new Random();
        public static List<double> StockList = new List<double>();
        public static List<double> MA50List = new List<double>();
        public static List<double> MA200List = new List<double>();
        static double NumHolder = 0;
        
        public static void RandNumGen()
        {
            int n = 0;
            NumHolder = random.Next(-2, 3);
            while (n != 2000)
            {
                double newNum;
                newNum = NumHolder + random.Next(-2, 3);
                NumHolder = newNum;
                StockList.Add(newNum);
                n++;
            }
           
        }

        public static void MovingAvg50()
        {
            int skip = 0;
            int count = 0;
            while (count != 40)
            {
                double sum = StockList.Skip(skip).Take(50).Sum()/50;
                MA50List.Add(sum);
                count++;
                skip = skip + 50;
            }

        }

        public static void MovingAvg200()
        {
            int skip = 0;
            int count = 0;
            while (count != 10)
            {
                double sum = StockList.Skip(skip).Take(200).Sum() / 200;
                MA200List.Add(sum);
                count++;
                skip = skip + 200;
            }

        }
    }
}
