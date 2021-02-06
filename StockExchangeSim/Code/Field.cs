using StockExchangeSim.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Eco
{
    public class Field
    {
        Random rn = new Random();


        public FieldNameInfo NameInfo = null;

        public string fieldName;

        public int id;
        public int companyAmount;
        public int maxCompanyStartAmount = 12;

        public float Innovation = 0.5f;
        public float Scandals = 0.3f;
        public float ScandalSeverity = 1;
        public readonly float baseChance = 99.0f / 31500000.0f;

        public float MarketShare = 1.1f;
        public float TotalCompetitiveness = 0;
        public float TotalValue = 0;

        int startamount;
        public List<Company> companies = new List<Company>();
        List<Company> startcompanies = null;



        public Field(int Id)
        {

            id = Id;
            rn = new Random(Master.Seed * (Id + 1));



            fieldName = SelectRandomName();

            companyAmount = rn.Next(1, maxCompanyStartAmount);

            StockPriceComparisonTool SPCT = new StockPriceComparisonTool();
            for (int i = 0; i < companyAmount; i++)
            {
                Company cp = new Company(this);
                cp.id = i;
                cp.SetValue((float)rn.NextDouble() * 200 * Master.MoneyScaler);
                cp.stockprice = cp.Value * cp.StockPart;
                cp.Competitiveness = (float)rn.NextDouble() * 100 + 50;

                Master.inst.exchange.RegisterCompany(cp, 50);

                //induce past stockprices
                
                float PMP = SPCT.StrategyOutcome(cp).ExpectedStockPrice;

                cp.stockViewModel.prices1m.Clear();
                cp.stockViewModel.prices10m.Clear();

                cp.stockPrices1m.Clear();

                for (int j = 0; j < 100; j++)
                {
                    StockPriceGraph sp = new StockPriceGraph((-100 + j) * 0.0001f, PMP + MathF.Sin(j - 1) * PMP * 0.01f,
                        PMP + MathF.Sin(j) * PMP * 0.01f, PMP + (float)rn.NextDouble() * PMP * 0.1f,
                        PMP - (float)rn.NextDouble() * PMP * 0.01f);
                    cp.stockPrices1m.Add(sp);
                    cp.stockViewModel.prices1m.Add(sp); 
                }
                //cp.BecomePublic();

                companies.Add(cp);
            }
            startcompanies = new List<Company>(companies);
            startamount = companyAmount;


        }
        int scandaltick = 0;

        public void Update()
        {
            //calculate innovation (FIXED)
            if (rn.NextDouble() < Innovation * MainPage.master.SecondsPerTick / 15 / 1450.461994) // 5617.61515
            {
                if (rn.NextDouble() < Innovation * MainPage.master.SecondsPerTick / 15 / 1450.461994) //5617.61515
                {
                    //an innovation can create a new company
                    if (rn.NextDouble() < 0.4)
                    {
                        Company newcp = new Company(this);
                        newcp.id = companyAmount++;
                        newcp.SetValue((float)rn.NextDouble() * 600);
                        float highvalue = MathF.Pow((float)rn.NextDouble(), 2);

                        float highestcomp = 0;
                        for (int i = 0; i < companies.Count; i++)
                        {
                            if (companies[i].Competitiveness > highestcomp)
                                highestcomp = companies[i].Competitiveness;
                        }

                        newcp.Competitiveness += highvalue * 200;
                        companies.Add(newcp);
                        startcompanies.Add(newcp);


                    }
                    else
                    {
                        Company cp = companies[rn.Next(companies.Count)];
                        float highvalue = MathF.Pow((float)rn.NextDouble(), 2);
                        cp.Competitiveness += highvalue * 0.1f * cp.Value / Master.MoneyScaler;

                    }
                }
            }
            float scandalval = MathF.Sqrt(Scandals * 99 * MainPage.master.SecondsPerTick / 31500000);
            //calculate scandals
            if (rn.NextDouble() < scandalval)
            {
                if (rn.NextDouble() < scandalval)
                {
                    scandaltick++;
                    if (scandaltick > 99)
                    {
                        //impact is based on company value
                        //Company cp = companies[rn.Next(companies.Count)];
                        float rand = MathF.Abs((float)rn.NextDouble() - 0.001f);
                        float totcomp = 0;
                        Company cp = null;

                        for (int i = 0; cp == null; i++)
                        {
                            float qp = companies[i].Competitiveness / TotalCompetitiveness;
                            if (qp + totcomp > rand)
                            {
                                cp = companies[i];
                                break;
                            }
                            totcomp += qp;
                        }

                        float highvalue = MathF.Pow((float)rn.NextDouble() * 1, 2);
                        cp.Competitiveness -= highvalue * 0.1f * ScandalSeverity * cp.Value / Master.MoneyScaler;
                        scandaltick = 0;
                    }
                }
            }
            if (companies.Count < 1 || (companies.Count == 1 && companies[0].Value < 2000 *
                (companies[0].Competitiveness / 100) / Master.Conjucture))
            {
                Company newcp = new Company(this);
                newcp.id = companyAmount++;
                newcp.SetValue((float)rn.NextDouble() * 50 * Master.MoneyScaler);
                companies.Add(newcp);
                startcompanies.Add(newcp);
            }

            //ordinary things VERVANGEN MET CONCURRENTIEPOSITIE => CHECK
            float value = MathF.Pow((float)rn.NextDouble() - 0.5f, 2);
            int select = rn.Next(0, companies.Count);
            companies[select].Competitiveness += value * 0.000001f * companies[select].Value / Master.MoneyScaler;

            TotalCompetitiveness = 0;
            TotalValue = 0;

            double tick = MainPage.master.SecondsPerTick / (24.0 * 60.0 * 60);

            //check for bankrupty
            for (int i = 0; i < companies.Count; i++)
            {
                if (companies[i].Competitiveness < 1)
                    companies[i].Competitiveness = 1;
                TotalCompetitiveness += companies[i].Competitiveness;
                TotalValue += companies[i].Value;
            }
            for (int i = 0; i < companies.Count; i++)
            {
                companies[i].age += (float)tick;
                companies[i].Update();
                if (companies[i].Value < 0.1)
                {
                    //BANKRUPT
                    companies[i].Bankrupt = true;
                    if (MainPage.inst != null)
                    {
                        Company cp = companies[i];
                        var ignore = MainPage.inst.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            cp.stockViewModel.prices1m.Clear();
                        });
                    }
                    companies.RemoveAt(i);


                }
            }
        }
        public string SelectRandomName()
        {
            //throw new NotImplementedException();
            int rng = rn.Next(Master.inst.masterTable.NameInfo.Count);
            NameInfo = Master.inst.masterTable.NameInfo[rng];
            Master.inst.masterTable.NameInfo.RemoveAt(rng);

            return NameInfo.FieldName;
        }
        public void print()
        {

            Debug.WriteLine("\nfield id: " + id + ", started with " + startamount);

            for (int i = 0; i < startcompanies.Count; i++)
            {
                if (startcompanies[i].Bankrupt)
                {
                    Debug.WriteLine("Company " + startcompanies[i].id + ": BANKRUPT, Time alive: " + startcompanies[i].age + " days");
                }
                else
                {
                    Debug.WriteLine("Company " + startcompanies[i].id + ": " + startcompanies[i].Value + ", age: " + startcompanies[i].age + " days");
                }
            }
        }
        public string getInfo()
        {
            string displaay = "";
            displaay += fieldName + ", started with " + startamount + " companies";

            for (int i = 0; i < startcompanies.Count; i++)
            {
                if (startcompanies[i].Bankrupt)
                {
                    displaay += "\n" + startcompanies[i].name + ": BANKRUPT,\t Time alive: " + Math.Floor(startcompanies[i].age) + " days";
                }
                else
                {
                    if (MainPage.master.SecondsPerTick >= 15)
                    {
                        //decemille
                        double lastgain = Math.Floor(startcompanies[i].LastDeceMilleGain * 1000 * 1000 * 100) / 1000;
                        string lastgainstr = lastgain < 0.1 ? (lastgain * 1000).ToString() + " µ%" : (lastgain).ToString() + " m%";


                        string s = startcompanies[i].name;
                        displaay += "\n";
                        for (int k = 0; k < 15 && k < s.Length; k++)
                        {

                            displaay += s[k];
                            if (k == 14)
                            {
                                displaay += "...";
                            }
                        }

                        displaay +=
                            ": " + Math.Floor(startcompanies[i].LastDeceMilleValue) +
                            ",\t\t gain: " + lastgainstr +
                            ",\t age: " + Math.Floor(startcompanies[i].age) + " days";
                    }
                    else if (MainPage.master.SecondsPerTick >= 7.5)
                    {
                        //mille
                        double lastgain = Math.Floor(startcompanies[i].LastMilleGain * 1000 * 1000 * 100) / 1000;
                        string lastgainstr = lastgain < 0.1 ? (lastgain * 1000).ToString() + " µ%" : (lastgain).ToString() + " m%";

                        displaay += "\n" + startcompanies[i].name +
                            ": " + Math.Floor(startcompanies[i].LastMilleValue) +
                            ",\t\t gain: " + lastgainstr +
                            ",\t age: " + Math.Floor(startcompanies[i].age) + " days";
                    }
                    else
                    {
                        //centum
                        double lastgain = Math.Floor(startcompanies[i].LastCentumGain * 1000 * 1000 * 100) / 1000;
                        string lastgainstr = lastgain < 0.1 ? (lastgain * 1000).ToString() + " µ%" : (lastgain).ToString() + " m%";
                        displaay += "\n" + startcompanies[i].name +
                        ": " + Math.Floor(startcompanies[i].LastCentumValue) +
                        ",\t\t gain: " + lastgainstr + ",\t price: " + Math.Floor(startcompanies[i].stockprice * 100) / 100 +
                        ",\t age: " + Math.Floor(startcompanies[i].age) + " days";
                    }
                }
            }



            return displaay + "\n\n";
        }
    }
}
