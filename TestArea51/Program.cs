using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eco;

namespace Eco
{
    public class Program
    {
        public static Exchange exchange = new Exchange();
        static void Main(string[] args)
        {
            Company cp = new Company();
            cp.stockPrices = new List<StockPriceGraph>
            {
                new StockPriceGraph(0, 3120.3, 3119.5, 3121, 3119),
                new StockPriceGraph(0.01, 3119.5, 3116.5, 3120.5, 3115.5),
                new StockPriceGraph(0.02, 3116.5, 3113.8, 3118.0, 3112.3),
                new StockPriceGraph(0.03, 3113.8, 3114.8, 3115.3, 3113.3),
                new StockPriceGraph(0.04, 3114.8, 3111.6, 3115.0, 3109.6),
                new StockPriceGraph(0.05, 3111.6, 3112.9, 3114.6, 3111.3),
                new StockPriceGraph(0.06, 3112.9, 3113.5, 3115.1, 3111.8),
                new StockPriceGraph(0.07, 3113.5, 3113.1, 3114.1, 3111.6),
                new StockPriceGraph(0.08, 3113.1, 3115.0, 3115.1, 3113.1),
                new StockPriceGraph(0.09, 3115.0, 3118.0, 3118.1, 3113.5),
                new StockPriceGraph(0.10, 3118.0, 3116.6, 3118.1, 3116.1),
                new StockPriceGraph(0.11, 3116.6, 3115.2, 3117.7, 3114.5),
                new StockPriceGraph(0.11, 3116.6, 3115.2, 3117.7, 3114.5),
            };


        }
    }
}
