using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1;

namespace Eco
{

    public class TrendData
    {
        public Line Uptrend, DownTrend;


    }
    //no touchy
    public class TrendTool : MarketTool<TrendData>
    {
        public override TrendData StrategyOutcome(Company cp)
        {

            MinimumPriceCount = cp.stockPrices.Count;
            List<StockPriceGraph> highs = new List<StockPriceGraph>();
            List<StockPriceGraph> lows = new List<StockPriceGraph>();
            highs.Add(cp.stockPrices[0]);
            for (int i = 1; i < MinimumPriceCount - 1; i++)
            {
                if (cp.stockPrices[i].High > cp.stockPrices[i - 1].High &&
                        cp.stockPrices[i].High > cp.stockPrices[i + 1].High) //this is a high
                {
                    highs.Add(cp.stockPrices[i]);
                    //MainPage.inst.AddVerticalLine(cp.stockPrices[i].Year, false);
                }
                if (highs.Count > 0) //first top always high
                {
                    if (cp.stockPrices[i].Low < cp.stockPrices[i - 1].Low &&
                        cp.stockPrices[i].Low < cp.stockPrices[i + 1].Low) //this is a low
                    {
                        lows.Add(cp.stockPrices[i]);
                        //MainPage.inst.AddVerticalLine(cp.stockPrices[i].Year, true);

                    }
                }
            }
            Point High = new Point(), Low = new Point();

            foreach (var sp in highs)
            {
                if (sp.High > High.y || High.y == 0)
                {
                    High.y = (float)sp.High;
                    High.x = (float)sp.Year;
                }
            }
            foreach (var sp in lows)
            {
                if (sp.Low < Low.y || Low.y == 0)
                {
                    Low.y = (float)sp.Low;
                    Low.x = (float)sp.Year;
                }
            }

            //MainPage.inst.AddHorizontalLine(High.y);
            //MainPage.inst.AddHorizontalLine(Low.y);

            //Lijnen worden gebouwd en worden getest of ze de prijs niet kruisen
            TrendData ret = new TrendData();

            Line line = null;
            foreach (var sp in lows)
            {
                if (sp.Year <= Low.x)
                    continue;
                //build line
                line = new Line(Low, new Point(sp.Year, sp.Low));

                bool LineValid = true;
                //check line
                foreach (var comp in lows)
                {
                    if (comp.Close < line.Multiplier * comp.Year + line.Adder)
                    {
                        LineValid = false;
                        break;
                    }
                }
                if (LineValid)
                    break;
            }
            if (line != null)
            {
                MainPage.inst.AddContinuousline(line);
                ret.Uptrend = line;
            }
            line = null;
            foreach (var sp in highs)
            {
                if (sp.Year <= High.x)
                    continue;


                //build line
                line = new Line(High, new Point(sp.Year, sp.High));


                bool LineValid = true;
                //check line
                foreach (var comp in lows)
                {
                    if (comp.Close > line.Multiplier * comp.Year + line.Adder) //check if it crosses
                    {
                        LineValid = false;
                        break;
                    }
                }
                if (LineValid)
                    break;
            }
            if (line != null)
            {
                MainPage.inst.AddContinuousline(line);
                ret.DownTrend = line;
            }
            return ret;
        }
    }

}
