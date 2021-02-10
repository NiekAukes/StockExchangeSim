using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    //bid ask implementatie
    public class Holder
    {
        public event EventHandler StockTraded;
        public int MaxStockLimit = int.MaxValue;
        public Trader Owner { get; set; }
        public SynchronizedCollection<Stock> Stocks { get; set; }
        public BidAsk bidask { get; set; }
        public Holder(Company cp, Trader trader)
        {
            Owner = trader;
            bidask = new BidAsk(cp);
            Stocks = new SynchronizedCollection<Stock>();
        }
        public override string ToString()
        {
            return "owned by: " + Owner.ToString();
        }

        public virtual void OnStockTraded(EventArgs e)
        {
            EventHandler handler = StockTraded;
            handler?.Invoke(this, e);
        }
    }

    public interface IBidAsk
    {
        Company cp { get; set; }
        float Bid { get; set; }
        float Ask { get; set; }
    }

    public class BidAsk : IBidAsk
    {
        public Company cp { get; set; }
        public float Bid { get; set; }
        public float Ask { get; set; }
        public BidAsk(Company company)
        {
            cp = company;
            Bid = 0;
            Ask = 0;
        }
    }
    public class OLD_BidAsk : BidAsk, IBidAsk
    {
        public List<Stock> Stocks { get; set; }
        public SynchronizedCollection<Liquidity> liquidity1m { get; set; }
        public OLD_BidAsk(Company company) : base(company)
        {
            Stocks = new List<Stock>();
            liquidity1m = new SynchronizedCollection<Liquidity>();
        }
    }

    public class SortedSyncCollection<T>
    {
        public bool Ascending = true;
        public SynchronizedCollection<T> list { get; set; }
        public void RemoveItem (T item)
        {
            lock(list.SyncRoot)
            {
                list.Remove(item);
            }
        }
        public void AddSortedItem(T item, string PropertyName)
        {
            lock (list.SyncRoot)
            {
                object property = item.GetType().GetProperty(PropertyName).GetValue(item, null);
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list.Count > i)
                        {
                            object comparison = list[i].GetType().GetProperty(PropertyName).GetValue(list[i], null);
                            if (Ascending ? (float)property > (float)comparison : (float)property < (float)comparison)
                            {
                                list.Insert(i, item);
                            }

                        }
                        else
                            list.Add(item);
                    }

                }
                else
                {
                    list.Add(item);
                }
            }
        }
        public SortedSyncCollection()
        {
            list = new SynchronizedCollection<T>();
        }
    }
    public class StockTradedEventArgs : EventArgs
    {
        public Stock Stock { get; set; }
    }
    public class SellOrder
    {
        public bool isPermanent { get; set; }
        public event EventHandler StockTraded;
        public int Amount { get; set; }
        public Company cp { get; set; }
        public List<Stock> Stock { get; set; }
        public float LimitPrice { get; set; }
        public SellOrder(Company cp, List<Stock> stock, float limitPrice, int amount = 1)
        {
            this.cp = cp ?? throw new ArgumentNullException(nameof(cp));
            Stock = stock ?? throw new ArgumentNullException(nameof(stock));
            Amount = amount;
            LimitPrice = limitPrice;
        }

        public virtual void OnStockTraded(StockTradedEventArgs e)
        {
            EventHandler handler = StockTraded;
            handler?.Invoke(this, e);
        }
    }
    public class BuyOrder
    {
        public bool isPermanent { get; set; }
        public event EventHandler StockTraded;

        public int Amount { get; set; }
        public Company cp { get; set; }
        public Trader Buyer { get; set; }
        public float LimitPrice { get; set; }
        public BuyOrder(Company cp, Trader buyer, float limitPrice, int amount = 1)
        {
            this.cp = cp ?? throw new ArgumentNullException(nameof(cp));
            Buyer = buyer ?? throw new ArgumentNullException(nameof(buyer));
            Amount = amount;
            LimitPrice = limitPrice;
        }

        public virtual void OnStockTraded(EventArgs e)
        {
            EventHandler handler = StockTraded;
            handler?.Invoke(this, e);
        }
    }

    public interface IExchange
    {
        float money { get; set; }

        bool BuyStock(Company cp, Trader buyer);
        void RegisterCompany(Company cp, int partition);
        void RegisterTrader(Trader t);
        void SellStock(Stock stock);
    }
}
