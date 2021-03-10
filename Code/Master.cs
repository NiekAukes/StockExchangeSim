using StockExchangeSim.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Eco
{
    public class Master
    {
        public static bool fCustomLiquidityTarget = false;
        public static Int32 CustomLiqTarget = 0;
        public static bool fCustomSeed = false;
        public static Master inst = null;
        public static bool fAsyncFields = false;
        public static bool fAsyncCompanies = false;
        public static Int32 CustomSeed = 0;
        public static Int32 Seed = (new Random()).Next();
        public static Random rn = new Random(Seed);
        Thread thread;
        public void SetSecondsPerTick(float Seconds) //2103840 = 1 year per second
        {
            SecondsPerTick = Seconds;
        }

        public float SecondsPerTick = 15.0f;
        public int FieldAmount;
        public int TraderAmount;
        public int HFTAmount;
        public static bool HFTEnabled = false;
        //public float TPS = 2103840; //ticks per second. 1577880 = 20s per year
        public double Year;
        //list for fields and traders
        public List<Field> Fields = new List<Field>();
        public List<Trader> Traders = new List<Trader>();
        public List<Traderpool> TraderPools = new List<Traderpool>();
        public ECNBroker exchange = new ECNBroker();

        public TableOfNames masterTable = new TableOfNames();
        public TraderNames MasterTraderNames = new TraderNames();

        public static float Conjucture { get; set; }
        public static float TotalShare = 0;
        public static float MoneyScaler = 1000.0f;

        public bool active { get; set; }
        public bool alive { get; set; }
        public Master(int fields, int traders, int hftraders)
        {
            alive = true;
            inst = this;
            if (fCustomSeed)
            {
                Seed = CustomSeed;
            }
            else
            {
                Seed = (new Random()).Next();
            }
            //set vars
            FieldAmount = fields;
            TraderAmount = traders;
            HFTAmount = hftraders;
            Conjucture = 1;

            //construction
            for (int i = 0; i < fields; i++)
            {
                Field field = new Field(i);
                field.Innovation = (float)rn.NextDouble() * 0.5f + 0.75f;
                field.Scandals = (float)rn.NextDouble() * 0.5f + 0.75f;

                Fields.Add(field);
                TotalShare += 100;
            }
            int threadcount = 20;
            for (int i = 0; i < threadcount; i++)
            {
                TraderPools.Add(new Traderpool());
            }
            Traders.Add(new Trader(true));
            TraderPools[0].traders.Add(Traders[0]);
            Traders.Add(new Trader(false));
            TraderPools[1].traders.Add(Traders[1]);

            for (int i = 0; i < traders; i++)
            {
                Trader td = new Trader();
                Traders.Add(td);
                TraderPools[(i + 2) % threadcount].traders.Add(td);
            }

            for (int i = 0; i < threadcount; i++)
            {
                TraderPools[i].StartThread();
            }

            thread = new System.Threading.Thread(Update);
            thread.Name = "Master Thread";
            thread.Priority = ThreadPriority.Highest;
            thread.Start();

            exchange.thread.Start();
        }
        public static long ticks = 0;

        public List<Company> GetAllCompanies()
        {
            List<Company> ret = new List<Company>(Fields[0].companies);
            for (int i = 1; i < Fields.Count; i++)
            {
                ret.AddRange(Fields[i].companies);
            }
            return ret;
        }
        public void Update()
        {


            for (; ticks < 100000000000 && alive; ticks++)
            {

                if (active)
                {
                    //recalculate total market share
                    TotalShare = 0;
                    for (int j = 0; j < FieldAmount; j++)
                    {
                        TotalShare += Fields[j].MarketShare;
                    }

                    Conjucture = (0.05f * MathF.Sin((float)MainPage.master.Year) + 1);


                    for (int j = 0; j < Fields.Count; j++)
                    {
                        Fields[j].Update();
                        for (int k = 0; k < Fields[j].companies.Count; k++)
                        {
                            Fields[j].companies[k].Data(ticks, false);
                        }
                    }




                    //int n = Traders.Count;
                    //while (n > 1)
                    //{
                    //    n--;
                    //    int k = rn.Next(n + 1);
                    //    Trader value = Traders[k];
                    //    Traders[k] = Traders[n];
                    //    Traders[n] = value;
                    //}
                    if (ticks % 10 == 0)
                    {
                        for (int j = 0; j < TraderAmount; j++)
                        {
                            Traders[j].ActionTime += SecondsPerTick * 10;
                        }
                    }

                    Year += SecondsPerTick / (365.25 * 24 * 60 * 60);




                }
                else
                {
                    ticks--;
                    Thread.Sleep(10);
                }
            }
        }
    }
}
