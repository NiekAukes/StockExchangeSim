using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1;

namespace Eco
{
    class SupportResistanceData
    {
        public List<Line> resistanceLevels = new List<Line>();
        public List<Line> supportLevels = new List<Line>();
    }
    class SupportResistanceTool : MarketTool<SupportResistanceData>
    {
        public int minimumLineLength = 6;
        public int minimumMargin = 3000;
        public override SupportResistanceData StrategyOutcome(Company cp)
        {
            MinimumPriceCount = cp.stockPrices.Count;
            List<StockPriceGraph> highs = new List<StockPriceGraph>();
            List<int> highpos = new List<int>();
            List<StockPriceGraph> lows = new List<StockPriceGraph>();
            List<int> lowpos = new List<int>();
            highs.Add(cp.stockPrices[0]);
            for (int i = 1; i < MinimumPriceCount - 1; i++)
            {
                if (cp.stockPrices[i].High > cp.stockPrices[i - 1].High &&
                        cp.stockPrices[i].High > cp.stockPrices[i + 1].High) //this is a high
                {
                    highs.Add(cp.stockPrices[i]);
                    highpos.Add(i);
                    //MainPage.inst.AddVerticalLine(cp.stockPrices[i].Year, false);
                }
                if (highs.Count > 0) //first top always high
                {
                    if (cp.stockPrices[i].Low < cp.stockPrices[i - 1].Low &&
                        cp.stockPrices[i].Low < cp.stockPrices[i + 1].Low) //this is a low
                    {
                        lows.Add(cp.stockPrices[i]);
                        lowpos.Add(i);
                        //MainPage.inst.AddVerticalLine(cp.stockPrices[i].Year, true);

                    }
                }
            }

            SupportResistanceData ret = new SupportResistanceData();

            //calculate resistance levels
            for (int i = 0; i < highs.Count; i++)
            {
                for (int j = 0; j < highs.Count; j++)
                {
                    if (i == j)
                        continue;
                    if (Math.Abs(highs[i].High - highs[j].High) < highs[i].High / minimumMargin)
                    {
                        //level of resistance
                        Line line = new Line(new Point(highs[i].Year, highs[i].High), new Point(highs[j].Year, highs[j].High));
                        bool linevalid = true;
                        if (highpos[j] - highpos[i] > minimumLineLength)
                        {
                            for (int k = highpos[i]; k < highpos[j]; k++)
                            {
                                if (cp.stockPrices[k].High > line.Multiplier * cp.stockPrices[k].Year + line.Adder) //if line crosses
                                {
                                    linevalid = false;
                                    break;
                                }
                            }
                        }
                        else
                            linevalid = false;

                        if (linevalid)
                            ret.resistanceLevels.Add(line);
                    }
                }
            }

            //calculate support levels
            for (int i = 0; i < lows.Count; i++)
            {
                for (int j = 0; j < lows.Count; j++)
                {
                    if (i == j)
                        continue;
                    if (Math.Abs(lows[i].Low - lows[j].Low) < lows[i].Low / minimumMargin)
                    {
                        //level of support
                        Line line = new Line(new Point(lows[i].Year, lows[i].Low), new Point(lows[j].Year, lows[j].Low));
                        bool linevalid = true;
                        if (lowpos[j] - lowpos[i] > minimumLineLength)
                        {
                            for (int k = lowpos[i]; k < lowpos[j]; k++)
                            {
                                if (cp.stockPrices[k].Low < line.Multiplier * cp.stockPrices[k].Year + line.Adder) //if line crosses
                                {
                                    linevalid = false;
                                    break;
                                }
                            }
                        }
                        else
                            linevalid = false;

                        if (linevalid)
                            ret.supportLevels.Add(line);
                    }
                }
            }

            return ret;
        }
    }
}
