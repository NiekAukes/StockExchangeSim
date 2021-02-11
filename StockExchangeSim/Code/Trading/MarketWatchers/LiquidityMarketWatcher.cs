﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    class LiquidityMarketWatcher : MarketWatcher<MarketMakingStrategy>
    {
        int BufferTarget = 1;
        float Spread = 0.01f;
        float GeneralPrice = 2;

        int LiquidityTarget = 500;
        float liquidity = 0;

        Holder Holder { get; set; }
        StockPriceComparisonTool SPCTool = new StockPriceComparisonTool();

        public LiquidityMarketWatcher(MarketMakingStrategy strat, Company cp, Trader td) : base(strat)
        {
            this.cp = cp;
            Holder = new Holder(cp, td);
            Holder.Stocks = new SynchronizedCollection<Stock>( new bool(), cp.TradeStocks((float)Master.rn.NextDouble() * 0.1f, td, true));
            Holder.bidask.Bid = cp.stockprice;
            Holder.bidask.Ask = cp.stockprice;
            Master.inst.exchange.RegisterHolder(Holder);
            GeneralPrice = cp.stockprice;

            Holder.StockTraded += Holder_StockTraded;
        }

        private void Holder_StockTraded(object sender, EventArgs e)
        {
            liquidity += 500 * (LiquidityTarget / Master.inst.Traders.Count) * (1/Master.inst.SecondsPerTick);
        }

        public override void RedoInsights()
        {
        }

        public override float UpdateInsights()
        {
            int buyorders = 0, sellorders = 0;
            double totalbuyvalue = 0, totalsellvalue = 0;
            BuyOrder lowestbidder = null;
            SellOrder highestorder = null;
            
            #warning this is a bodge too!
            #region Price Discovery
            //kijk naar vraag en aanbod
            for (int i = 0; i < cp.BuyOrders.Count; i++)
            {
                try
                {
                    buyorders += cp.BuyOrders[i].Amount;
                    totalbuyvalue += (double)cp.BuyOrders[i].LimitPrice * cp.BuyOrders[i].Amount;
                    if (i != 0)
                    {
                        if (cp.BuyOrders[i].LimitPrice > lowestbidder.LimitPrice)
                        {
                            lowestbidder = cp.BuyOrders[i];
                        }
                    }
                    else
                        lowestbidder = cp.BuyOrders[i];
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            for (int i = 0; i < cp.SellOrders.Count; i++)
            {
                try
                {
                    sellorders += cp.SellOrders[i].Amount;
                    totalsellvalue += (double)cp.SellOrders[i].LimitPrice * cp.SellOrders[i].Amount;

                    if (i != 0)
                    {
                        if (cp.SellOrders[i].LimitPrice > highestorder.LimitPrice)
                        {
                            highestorder = cp.SellOrders[i];
                        }
                    }
                    else
                        highestorder = cp.SellOrders[i];
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            #endregion
            //kijk naar hoogste aanbod en laagste vraag


            if (highestorder == null || lowestbidder == null)
            {
                //if there is no bid or ask yet, construct one
                var data = SPCTool.StrategyOutcome(cp);
                GeneralPrice = data.ExpectedStockPrice;
            }
            else
            {
                GeneralPrice = (0.8f * highestorder.LimitPrice + 0.2f * lowestbidder.LimitPrice) / 2.0f;
                float demandflow = buyorders - sellorders;

                double DemandElasticity =((totalbuyvalue / buyorders) - highestorder.LimitPrice) /
                    (buyorders / 2.0);
                double SupplyElasticity = ((totalsellvalue / sellorders) - lowestbidder.LimitPrice) /
                    (sellorders / 2.0);

                if (demandflow > (30 * Master.inst.TraderAmount) && (Holder.Stocks.Count < 50 || highestorder.LimitPrice < 1.2 * GeneralPrice))
                {
                    //there needs to be more stocks
                    List<Stock> stocks = cp.TradeStocks(demandflow / 100, Strategy.trader);
                    lock (Holder.Stocks.SyncRoot)
                    {
                        foreach (Stock st in stocks)
                        {
                            Holder.Stocks.Add(st);
                        }
                    }

                    //decrease price
                    GeneralPrice += (float)(1 / DemandElasticity) * 2.5f * MathF.Log(demandflow);
                }
                else if (demandflow > (10 * Master.inst.TraderAmount))
                {
                    //significantly increase prices

                }
                else if (demandflow > 0)
                {
                    //price can be slightly increased
                    GeneralPrice += (float)(1 / -DemandElasticity) * 0.5f * MathF.Log(demandflow);

                }
                else
                {
                    //price should be decreased
                    GeneralPrice += (float)(1 / -SupplyElasticity) * 0.5f * MathF.Log(demandflow);

                }

            }

            Spread = 0.01f * cp.Value * cp.StockPart;

            Holder.bidask.Ask = GeneralPrice -= Spread;
            Holder.bidask.Bid = GeneralPrice += Spread;

            if (Master.rn.NextDouble() < 0.05)
                Debug.WriteLine(Strategy.trader.name + ", liquidity: " + liquidity);
            liquidity = 0;
            return 0;
        }

    }
}
