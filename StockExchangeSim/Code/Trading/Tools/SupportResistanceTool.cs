using System;
using System.Collections.Generic;

namespace Eco
{
    public class SupportResistanceData
    {
        public List<Line> resistanceLevels = new List<Line>(), supportLevels = new List<Line>();
        public Line MainResistance, MainSupport;
    }
    //needs optimalization
    public class SupportResistanceTool : MarketTool<SupportResistanceData>
    {
        public int minimumLineLength = 3;
        public int minimumMargin = 25000;
        public override SupportResistanceData StrategyOutcome(Company cp)
        {
            MinimumPriceCount = cp.stockPrices1m.Count;
            SynchronizedCollection<StockPriceGraph> highs = new SynchronizedCollection<StockPriceGraph>(cp.stockPrices1m);
            List<int> highpos = new List<int>();
            SynchronizedCollection<StockPriceGraph> lows = new SynchronizedCollection<StockPriceGraph>(cp.stockPrices1m);
            List<int> lowpos = new List<int>();
            //highs.Add(cp.stockPrices[0]);
            highpos.Add(0);
            lowpos.Add(0);
            for (int i = 1; i < MinimumPriceCount; i++)
            {
                highpos.Add(i);
                lowpos.Add(i);
                //if (cp.stockPrices[i].High > cp.stockPrices[i - 1].High &&
                //        cp.stockPrices[i].High > cp.stockPrices[i + 1].High) //this is a high
                //{
                //    highs.Add(cp.stockPrices[i]);
                //    highpos.Add(i);
                //    //MainPage.inst.AddVerticalLine(cp.stockPrices[i].Year, false);
                //}
                //if (highs.Count > 0) //first top always high
                //{
                //    if (cp.stockPrices[i].Low < cp.stockPrices[i - 1].Low &&
                //        cp.stockPrices[i].Low < cp.stockPrices[i + 1].Low) //this is a low
                //    {
                //        lows.Add(cp.stockPrices[i]);
                //        lowpos.Add(i);
                //        //MainPage.inst.AddVerticalLine(cp.stockPrices[i].Year, true);

                //    }
                //}
            }

            SupportResistanceData ret = new SupportResistanceData();

            //calculate resistance levels
            for (int i = 0; i < highs.Count; i++)
            {
                for (int j = 0; j < highs.Count; j++)
                {
                    if (i == j)
                        continue;
                    if (Math.Abs(highs[i].High - highs[j].High) < highs[i].High * Math.Abs(highpos[j] - highpos[i]) / minimumMargin)
                    {
                        //level of resistance
                        Line line = new Line(new Point(highs[i].Year, highs[i].High), new Point(highs[j].Year, highs[j].High));
                        bool linevalid = true;
                        if (highpos[j] - highpos[i] > minimumLineLength)
                        {
                            for (int k = highpos[i]; k < cp.stockPrices1m.Count; k++)
                            {
                                if (cp.stockPrices1m[k].Close > line.Multiplier * cp.stockPrices1m[k].Year + line.Adder) //if line crosses
                                {
                                    linevalid = false;
                                    break;
                                }
                            }
                        }
                        else
                            linevalid = false;

                        if (linevalid)
                        {
                            float addval = highs[i].High / minimumMargin * 3;
                            line.Adder += addval;
                            line.Begin.y += addval;
                            line.End.y += addval;
                            //line = new Line(new Point(highs[i].Year, highs[i].High + addval), new Point(highs[j].Year, highs[j].High + addval * (1 + (highs[j].Year - highs[i].Year) * 10)));
                            ret.resistanceLevels.Add(line);
                        }
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
                    if (Math.Abs(lows[i].Low - lows[j].Low) < lows[i].Low * Math.Abs(lowpos[j] - lowpos[i]) / minimumMargin)
                    {
                        //level of support
                        Line line = new Line(new Point(lows[i].Year, lows[i].Low), new Point(lows[j].Year, lows[j].Low));
                        bool linevalid = true;
                        if (lowpos[j] - lowpos[i] > minimumLineLength)
                        {
                            for (int k = lowpos[i]; k < cp.stockPrices1m.Count; k++)
                            {
                                if (cp.stockPrices1m[k].Low < line.Multiplier * cp.stockPrices1m[k].Year + line.Adder) //if line crosses
                                {
                                    linevalid = false;
                                    break;
                                }
                            }
                        }
                        else
                            linevalid = false;

                        if (linevalid)
                        {
                            float addval = -lows[i].Low / minimumMargin * 3;
                            //line = new Line(new Point(lows[i].Year, lows[i].Low - addval), new Point(lows[j].Year, lows[j].Low - addval * (1 + (lows[j].Year - lows[i].Year) * 10)));
                            line.Adder += addval;
                            line.Begin.y += addval;
                            line.End.y += addval;
                            ret.supportLevels.Add(line);
                        }
                    }
                }
            }

            //calculate main support & resistance
            foreach (Line ln in ret.resistanceLevels)
            {
                if (ret.MainResistance == null)
                {
                    ret.MainResistance = ln;
                    continue;
                }
                if (ret.MainResistance.Length / ret.MainResistance.Multiplier < ln.Length / ln.Multiplier)
                    ret.MainResistance = ln;
            }

            foreach (Line ln in ret.supportLevels)
            {
                if (ret.MainSupport == null)
                {
                    ret.MainSupport = ln;
                    continue;
                }
                if (ret.MainSupport.Length / ret.MainSupport.Multiplier < ln.Length / ln.Multiplier)
                    ret.MainSupport = ln;
            }

            return ret;
        }
    }
}
