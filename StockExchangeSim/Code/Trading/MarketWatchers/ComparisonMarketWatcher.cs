using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    public class ComparisonMarketWatcher : MarketWatcher<InvestorStrategy>
    {
        StockPriceComparisonTool SPCTool = new StockPriceComparisonTool();
        StockPriceComparisonToolData SPCData = null;

        public ComparisonMarketWatcher(Company company)
        {
            cp = company;
        }

        public override void RedoInsights()
        {
            //TODO
            SPCData = SPCTool.StrategyOutcome(cp);

        }
        
        public override float UpdateInsights()
        {
            float ret = 0;
            //there is a price update

            //get the new prices
            //List<StockPriceGraph> NewPrices;
            //lock (cp.stockPrices1m)
            //{
            //    NewPrices = new List<StockPriceGraph>(cp.stockPrices1m);
            //}
            //NewPrices.AddRange(cp.stockPrices.getrange); 
            //cp.stockPrices.CopyTo(NewPrices, lastdatapoint);

            RedoInsights();

            // keer -2 om de effectieve range tussen -2 en 2 te maken
            ret = 2 * (SPCData.ExpectedStockPrice - cp.stockprice) / (cp.stockprice + SPCData.ExpectedStockPrice) ;

            
            return MathF.Round(ret) * 2.5f;
        }
    }
}
