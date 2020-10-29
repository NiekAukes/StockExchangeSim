using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using StockExchangeSim;
using StockExchangeSim.Views;
using Telerik.UI.Xaml.Controls.Data;
using Windows.Security.Authentication.Web.Provider;

namespace Eco
{
    public interface IStockOwner
    {
        Stock[] GetStocks();
        void AddStock(Stock stock);
        void UpdateHoldings();
    }
    public class Company : IStockOwner
    {
        Random rn = new Random(Master.Seed);
        Stock CompanyStock = null;
        public double value = 50; 
        public double stockprice = 0;

        //setup values
        public int id;
        public Company()
        {
            CompanyStock = CreateStock(100);
            //calculate cumulative demand (cumulatieve vraag)
            //so you can calculate
        }
        //variable values
        #region variableValues
        public void SetValue(double val)
        {
            value = val;
            LastDecemValue = val;
            LastCentumValue = val;
            LastMilleValue = val;
            LastDeceMilleValue = val;
            LastCentuMilleValue = val;
        }
        public double age = 0;
        public bool Bankrupt = false;

        public double LastDecemValue = 0;
        public double LastCentumValue = 0;
        public double LastMilleValue = 0;
        public double LastDeceMilleValue = 0;
        public double LastCentuMilleValue = 0;

        //Gain Calculation
        public double LastTickGain = 0.0; //value Gained per tick
        public double LastTickSlope = 0.0; // gain Gained per tick
        public double LastDecemGain = 0;
        public double LastCentumGain = 0;
        public double LastMilleGain = 0;
        public double LastDeceMilleGain = 0;
        public double LastCentuMilleGain = 0;
        #endregion

        UInt64 CurrentTick = 0;
        public void Update()
        {
            CurrentTick++;
            //value += Math.Pow((rn.NextDouble() - 0.5) * 5, 3); 
            LastTickGain += -LastTickGain * 0.001 * MainPage.master.SecondsPerTick;
            CompanyStock.Update();
            value += CompanyStock.Collect();

            if (CurrentTick % 10 == 0)
            {
                LastDecemGain = -(LastDecemValue - value) / LastDecemValue;
                LastDecemValue = value;
            }
            if (CurrentTick % 100 == 0)
            {
                LastCentumGain = -(LastCentumValue - value) / LastCentumValue;
                LastCentumValue = value;
            }
            if (CurrentTick % 1000 == 0)
            {
                LastMilleGain = -(LastMilleValue - value) / LastMilleValue;
                LastMilleValue = value;
            }
            if (CurrentTick % 10000 == 0)
            {
                LastDeceMilleGain = -(LastDeceMilleValue - value) / LastDeceMilleValue;
                LastDeceMilleValue = value;
            }
            if (CurrentTick % 100000 == 0)
            {
                LastCentuMilleGain = -(LastCentuMilleValue - value) / LastCentuMilleValue;
                LastCentuMilleValue = value;
            }
            
        }

       

        
        private Stock CreateStock(double percentage)
        {
            Stock ret = new Stock(this, percentage);
            return ret;
        }

        public Stock[] GetStocks()
        {
            throw new NotImplementedException();
        }

        public void AddStock(Stock stock)
        {
            throw new NotImplementedException();
        }

        public void UpdateHoldings()
        {
            throw new NotImplementedException();
        }
    }
    public class Stock
    {
        public IStockOwner Owner;
        public Company company = null;
        public double value { get { return company.value * Percentage / 100; } }
        public double tradevalue { get { return company.stockprice * Percentage; } }
        public double Percentage = 0;
        public double Collected = 0;
        public Stock(Company cp, double percentage)
        {
            //create new stock from company
            company = cp;
            Percentage = percentage;
        }

        public Stock(Stock s, double percentage)
        {
            //create new stock from other stock
        }
        public Stock SplitStock(double percentage)
        {
            Stock ret = new Stock(this, percentage);
            return ret;
        }
        public void CombineStock(Stock stock)
        {
            if (company != stock.company)
            {
#if DEBUG
                throw new Exception("Stock didn't have same company");
#endif
                return;
            }
            Percentage += stock.Percentage;
        }
        public void Update()
        {

            Collected += company.LastTickGain * MainPage.master.SecondsPerTick;
        }
        public double Collect()
        {
            double ret = Collected;
            Collected = 0;
            return ret;
        }
    }
    public class Field
    {
        Random rn = null;

        public int id;
        public int companyAmount;

        public double Innovation = 1.5;
        public double Scandals = 0.2;
        public float ScandalSeverity = 1;

        int startamount;
        List<Company> companies = new List<Company>();
        List<Company> startcompanies = null;

        public Field(int Id)
        {
            id = Id;
            rn = new Random(Master.Seed * (Id + 1));
            companyAmount = rn.Next(1, 5);
            for (int i = 0; i < companyAmount; i++)
            {
                Company cp = new Company();
                cp.id = i;
                cp.SetValue(rn.NextDouble() * 2000);
                cp.stockprice = cp.value / 100;
                companies.Add(cp);
            }
            startcompanies = new List<Company>(companies);
            startamount = companyAmount;
        }
        public void Update()
        {
            //calculate innovation (FIXED)
            if (rn.NextDouble() < Innovation * MainPage.master.SecondsPerTick / 15 / 1450.461994) // 5617.61515
            {
                if (rn.NextDouble() < Innovation * MainPage.master.SecondsPerTick / 15 / 1450.461994) //5617.61515
                {
                    //an innovation can create a new company
                    if (rn.NextDouble() < 0.2)
                    {
                        Company newcp = new Company();
                        newcp.id = companyAmount++;
                        newcp.SetValue(rn.NextDouble() * 300);
                        double highvalue = Math.Pow(rn.NextDouble(), 2);
                        newcp.LastTickGain += highvalue * 0.2;
                        companies.Add(newcp);
                        startcompanies.Add(newcp);


                    }
                    else
                    {
                        Company cp = companies[rn.Next(companies.Count)];
                        double highvalue = Math.Pow(rn.NextDouble(), 2);
                        cp.LastTickGain -= highvalue * (cp.value + 300) / 1000 * 0.2;

                        for (int i = 0; i < companies.Count; i++)
                        {
                            companies[i].LastTickGain -= highvalue * 0.1 * 1/companies.Count;
                        }
                    }
                }
            }
            int scandaltick = 0;
            //calculate scandals
            if (rn.NextDouble() < Scandals * 100 * MainPage.master.SecondsPerTick / 15 / 1450.461994)
            {
                if (rn.NextDouble() < Scandals * 100 * MainPage.master.SecondsPerTick / 15 / 1450.461994)
                {
                    scandaltick++;
                    if (scandaltick > 9999)
                    {
                        //impact is based on company value
                        Company cp = companies[rn.Next(companies.Count)];
                        double highvalue = Math.Pow(rn.NextDouble() * 1, 3);
                        cp.LastTickGain -= highvalue * (cp.value + 300) * 0.0002 * ScandalSeverity;
                        scandaltick = 0;
                    }
                }
            }
            if (companies.Count < 1 || (companies.Count == 1 && companies[0].value < 500))
            {
                Company newcp = new Company();
                newcp.id = companyAmount++;
                newcp.SetValue(rn.NextDouble() * 300);
                companies.Add(newcp);
                startcompanies.Add(newcp);
            }

            //ordinary things VERVANGEN MET CONCURRENTIEPOSITIE
            double value = Math.Pow(rn.NextDouble() - 0.5, 3);
            int select = rn.Next(0, companies.Count);
            companies[select].LastTickGain += ((value * 0.001) + ((value * 0.001) * companies[select].value * 0.001));

            //check for bankrupty
            for (int i = 0; i < companies.Count; i++)
            {
                double tick = MainPage.master.SecondsPerTick / (24.0 * 60.0 * 60);
                companies[i].age += tick;
                companies[i].Update();
                if (companies[i].value < -300)
                {
                    //BANKRUPT
                    companies[i].Bankrupt = true;
                    companies.RemoveAt(i);

                }
            }
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
                    Debug.WriteLine("Company " + startcompanies[i].id + ": " + startcompanies[i].value + ", age: " + startcompanies[i].age + " days");
                }
            }
        }
        public string getInfo()
        {
            string displaay = "";
            displaay += "field id: " + id + ", started with " + startamount;

            for (int i = 0; i < startcompanies.Count; i++)
            {
                if (startcompanies[i].Bankrupt)
                {
                    displaay += "\nCompany " + startcompanies[i].id + ": BANKRUPT,\t Time alive: " + Math.Floor(startcompanies[i].age) + " days";
                }
                else
                {
                    if (MainPage.master.SecondsPerTick >= 15)
                    {
                        //decemille
                        double lastgain = Math.Floor(startcompanies[i].LastDeceMilleGain * 1000 * 1000 * 100) / 1000;
                        string lastgainstr = lastgain < 0.1 ? (lastgain * 1000).ToString() + " µ%" : (lastgain).ToString() + " m%";
                        displaay += "\nCompany " + startcompanies[i].id +
                            ": " + Math.Floor(startcompanies[i].LastDeceMilleValue) +
                            ",\t gain: " + lastgainstr +
                            ",\t age: " + Math.Floor(startcompanies[i].age) + " days";
                    }
                    else if (MainPage.master.SecondsPerTick >= 7.5)
                    {
                        //mille
                        double lastgain = Math.Floor(startcompanies[i].LastMilleGain * 1000 * 1000 * 100) / 1000;
                        string lastgainstr = lastgain < 0.1 ? (lastgain * 1000).ToString() + " µ%" : (lastgain).ToString() + " m%";

                        displaay += "\nCompany " + startcompanies[i].id +
                            ": " + Math.Floor(startcompanies[i].LastMilleValue) +
                            ",\t gain: " + lastgainstr +
                            ",\t age: " + Math.Floor(startcompanies[i].age) + " days";
                    }
                    else
                    {
                        //centum
                        double lastgain = Math.Floor(startcompanies[i].LastCentumGain * 1000 * 1000 * 100) / 1000;
                        string lastgainstr = lastgain < 0.1 ? (lastgain * 1000).ToString() + " µ%" : (lastgain).ToString() + " m%";
                        displaay += "\nCompany " + startcompanies[i].id +
                        ": " + Math.Floor(startcompanies[i].LastCentumValue) +
                        ",\t gain: " + lastgainstr + ",\t price: " + Math.Floor(startcompanies[i].stockprice * 100) / 100 + 
                        ",\t age: " + Math.Floor(startcompanies[i].age) + " days";
                    }
                }
            }
            
            

            return displaay + "\n\n";
        }
    }
}
