using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    public partial class Trader
    {
        public abstract class Strategy
        {
            Random rn = new Random(Master.Seed);
            public abstract void StrategyOutcome(IStockOwner trader, Exchange exchange);
        }

        public class SimpleStrategy : Strategy
        {
            public override void StrategyOutcome(IStockOwner trader, Exchange exchange)
            {
                double money = trader.money;
                Company cp = exchange.StocksForSale.Keys.ToArray()[rn.Next(exchange.StocksForSale.Count - 1)]; //invest in this company

                Stock s = exchange.GetCheapestStock(cp);
                if (s.SellPrice <= cp.Value * 0.01 * s.Percentage) //if stock is right price
                {
                    exchange.BuyStock(s, trader); //buy the stock
                }

                //price exploration
                //price discovery
                //breakout fase


                //channel pattern
                //            _
                //        _  / \/
                //    _  / \/            =========> breakout signal
                //   / \/
                //  /
                // /

                //headnshoulders pattern
                //        _  
                //    _  / \  _
                //   / \/   \/ \          ======> Reversal signal
                //  /
                // /

                //flag pattern
                //          Flat top
                //    _    _  _
                //   / \  / \/       ======> powerful break signal
                //  /   \/
                // /

                //wedge pattern
                //          Flat top
                //    /\      
                //   /  \    /\  /\__       ======> direction of break uncertain
                //  /    \  /  \/
                // /      \/
            }
        }
    }
}
