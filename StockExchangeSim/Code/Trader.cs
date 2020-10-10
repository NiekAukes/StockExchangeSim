using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    
    public class Trader
    {
        public static Random rn = new Random(Master.Seed);
        public List<Company.Stock> stocks = new List<Company.Stock>();
        public double Money = 100;
        public double ReactionTime = 200 + rn.NextDouble() * 200;
        public double ActivityTime
        public void Update()
        {
            
        }
    }
    public class HFTrader : Trader
    {

    }
}
