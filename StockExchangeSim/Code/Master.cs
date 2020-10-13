using System;
using System.Collections.Generic;
using System.Diagnostics;
using MicroLibrary;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Reflection.Metadata.Ecma335;
using StockExchangeSim.Views;

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

            return seconds / 60.0f / 60/ 24;
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
                if (seconds > 60*60)
                {
                    //hours
                    if (seconds > 60 * 60 * 24)
                    {
                        //days
                        if (seconds > 60 * 60 * 24 * 365.25)
                        {
                            //years
                        }
                    }
                    else
                    {
                        double hours = Math.Floor(seconds / 60.0 / 60.0);
                        return hours.ToString() + " : " + (((seconds / 60 / 60) - hours) * 60).ToString();
                    }
                }
                else
                {
                    double mins = Math.Floor(seconds / 60.0);
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
        public static Int32 CustomSeed = 0;
        public static Int32 Seed = (new Random()).Next();
        Thread thread;
        public void SetSecondsPerTick(double Seconds) //2103840 = 1 year per second
        {
            SecondsPerTick = Seconds;
        }
        public Time time;
        public double SecondsPerTick = 15.0;
        public int FieldAmount;
        public int TraderAmount;
        public int HFTAmount;
        //public double TPS = 2103840; //ticks per second. 1577880 = 20s per year
        public double Year;
        //list for fields and traders
        public List<Field> Fields = new List<Field>();
        public List<Trader> Traders = new List<Trader>();

        public Master(int fields, int traders, int hftraders)
        {
            if (fCustomSeed)
            {
                Seed = CustomSeed;
            }
            //set vars
            FieldAmount = fields;
            TraderAmount = traders;
            HFTAmount = hftraders;

            

            //construction
            for (int i = 0; i < fields; i++)
            {
                Fields.Add(new Field(i));
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
        public void Update()
        {

            MicroStopwatch sw = new MicroStopwatch();
            sw.Start();
            for (long i = 0; i < 100000000000 && alive; i++)
            {

                if (active)
                {

                    for (int j = 0; j < FieldAmount; j++)
                    {
                        Fields[j].Update();
                    }
                    Year += SecondsPerTick / (365.25 * 24 * 60 * 60);
                    if (i % 1000000 == 0)
                    {
                        for (int j = 0; j < FieldAmount; j++)
                        {
                            //System.Diagnostics.Debug.WriteLine(i / (365.25 * 24 * 60 * 4) + " years past");
                            //Fields[j].print();
                        }
                    }
                    //int tickpass = 1000;
                    //if (i % tickpass == 0 && i != 0)
                    //{
                    //    sw.Stop();
                    //    if ((sw.ElapsedMicroseconds * 0.001) < (tickpass * 1000.0 / TPS))
                    //    {
                    //        Thread.Sleep((int)( (tickpass * 1000.0 / TPS) - sw.ElapsedMicroseconds / 1000));
                    //    }
                    //    sw.Reset();
                    //    sw.Start();
                    //}

                }
                else
                {
                    i--;
                    Thread.Sleep(10);
                }
            }
        }
    }
}
