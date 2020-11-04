﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Input;

namespace Eco
{
    public partial class Trader
    {
        public abstract class Strategy
        {
            Random rn = new Random(Master.Seed);
            public abstract void StrategyOutcome(Trader trader, Exchange exchange);
        }

        public class SimpleStrategy : Strategy
        {
            public override void StrategyOutcome(Trader trader, Exchange exchange)
            {
                double money = trader.money;
                Company cp = exchange.StocksForSale.Keys.ToArray()[rn.Next(exchange.StocksForSale.Count - 1)]; //invest in this company

                Stock s = exchange.GetCheapestStock(cp);
                if (s.SellPrice <= cp.Value * 0.01 * s.Percentage) //if stock is right price
                {
                    exchange.BuyStock(s, trader); //buy the stock
                    trader.ActionTime -= 3;
                }
                trader.ActionTime -= 5;

                
            }
        }
        public class PatternStrategy : Strategy
        {
            List<Company> interestedCompanies = null;
            public PatternStrategy()
            {
                interestedCompanies = new List<Company>(Master.exchange.StocksForSale.Keys.ToList());
                while (interestedCompanies.Count > 10)
                {
                    interestedCompanies.RemoveAt(rn.Next(interestedCompanies.Count));
                }
            }
            public override void StrategyOutcome(Trader trader, Exchange exchange)
            {
                //price exploration
                double money = trader.money;
                double scale = 1;
                Company cp = exchange.StocksForSale.Keys.ToArray()[rn.Next(exchange.StocksForSale.Count - 1)]; //invest in this company
                List<StockPriceGraph> stockPrices = new List<StockPriceGraph>();
                if (cp.stockPrices.Count < 50)
                    return;
                for(int i = 0; i < 50; i++)
                {
                    stockPrices.Add(cp.stockPrices[cp.stockPrices.Count - 1 - (int)(i * scale)]);
                }

                //price discovery
                List<StockPriceGraph> highs = new List<StockPriceGraph>();
                List<StockPriceGraph> lows = new List<StockPriceGraph>();
                for (int i = 1; i < 49; i++)
                {
                    if (stockPrices[i].Close > stockPrices[i - 1].Close &&
                            stockPrices[i].Close > stockPrices[i + 1].Close) //this is a high
                    {
                        highs.Add(stockPrices[i]);
                    }
                    if (highs.Count > 0) //first top always high
                    {
                        if (stockPrices[i].Close < stockPrices[i - 1].Close &&
                            stockPrices[i].Close < stockPrices[i + 1].Close) //this is a low
                        {
                            lows.Add(stockPrices[i]);
                        }
                    }
                }

                trader.ActionTime -= 45;

                List<StockPriceGraph> avHighs = new List<StockPriceGraph>();
                List<StockPriceGraph> avLows = new List<StockPriceGraph>();

                //for (int i = 0)

                //channel pattern
                //            _              H  H-h  H-h  H-h
                //        _  / \/              L  L-h  L-h
                //    _  / \/            =========> breakout signal
                //   / \/
                //  /
                // /

                //if ()


                //headnshoulders pattern
                //        _                 H  H-h  H-l
                //    _  / \  _               L   L-s
                //   / \/   \/ \          ======> Reversal signal
                //  /
                // /

                //flag pattern
                //          Flat top        H  H-s  H-s
                //    _    _  _               L  L-h  L-h
                //   / \  / \/       ======> powerful break signal
                //  /   \/
                // /

                //wedge pattern
                //          Flat top
                //    /\      
                //   /  \    /\  /\__       ======> direction of break uncertain
                //  /    \  /  \/
                // /      \/

                //breakout fase
            }
        }
    }
}
