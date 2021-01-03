using StockExchangeSim.Views;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Eco
{

    public class Time
    {
        UInt64 seconds = 0;
        public Time(UInt64 Seconds)
        {
            seconds = Seconds;
        }
        public UInt64 GetSeconds()
        {
            return seconds;
        }
        public float GetMinutes()
        {
            return seconds / 60.0f;
        }
        public float GetHours()
        {
            return seconds / 60.0f / 60;

        }
        public float GetDays()
        {

            return seconds / 60.0f / 60 / 24;
        }
        public float GetMonths()
        {
            return seconds / 60.0f / 60 / 365.25f * 12;

        }
        public float GetYears()
        {
            return seconds / 60.0f / 60 / 365.25f;

        }

        public string ParseTime()
        {
            //seconds
            if (seconds > 60)
            {
                //minutes
                if (seconds > 60 * 60)
                {
                    //hours
                    if (seconds > 60 * 60 * 24)
                    {
                        //days
                        if (seconds > 60 * 60 * 24 * 365.25f)
                        {
                            //years
                        }
                    }
                    else
                    {
                        float hours = MathF.Floor(seconds / 60.0f / 60.0f);
                        return hours.ToString() + " : " + (((seconds / 60 / 60) - hours) * 60).ToString();
                    }
                }
                else
                {
                    float mins = MathF.Floor(seconds / 60.0f);
                    return mins.ToString() + " : " + (((seconds / 60) - mins) * 60).ToString();
                }
            }
            else
            {
                return seconds.ToString() + " seconds";
            }
            return "";
        }

    }
    public class Master
    {
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
        public Time time;
        public float SecondsPerTick = 15.0f;
        public int FieldAmount;
        public int TraderAmount;
        public int HFTAmount;
        //public float TPS = 2103840; //ticks per second. 1577880 = 20s per year
        public float Year;
        //list for fields and traders
        public List<Field> Fields = new List<Field>();
        public List<Trader> Traders = new List<Trader>();
        public ECNBroker exchange = new ECNBroker();

        public TableOfNames masterTable = new TableOfNames();
        public TraderNames MasterTraderNames = new TraderNames();

        public static float Conjucture { get; set; }
        public static float TotalShare = 0;

        public Master(int fields, int traders, int hftraders)
        {
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
                Fields.Add(field);
                TotalShare += 100;
            }
            for (int i = 0; i < traders; i++)
            {
                Traders.Add(new Trader());
            }

            thread = new System.Threading.Thread(Update);
            thread.Name = "Master Thread";
            thread.Start();
        }
        public bool active = false;
        public bool alive = true;
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

                    Conjucture = (0.05f * MathF.Sin(MainPage.master.Year) + 1);


                    for (int j = 0; j < Fields.Count; j++)
                    {
                        Fields[j].Update();
                    }




                    int n = Traders.Count;
                    while (n > 1)
                    {
                        n--;
                        int k = rn.Next(n + 1);
                        Trader value = Traders[k];
                        Traders[k] = Traders[n];
                        Traders[n] = value;
                    }

                    for (int j = 0; j < TraderAmount; j++)
                    {
                        Traders[j].Update();
                    }

                    Year += SecondsPerTick / (365.25f * 24 * 60 * 60);




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
