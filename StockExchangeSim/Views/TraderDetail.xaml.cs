using Eco;
using Syncfusion.UI.Xaml.Charts;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Random = System.Random;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StockExchangeSim.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TraderDetail : Page
    {
        public SfChart stockpricechrt;
        public static TraderDetail inst;
        public Field Field { get; private set; }
        public List<Trader> traders { get; private set; }

        public Trader selectedTrader;
        public Random rnd = new Random();

        public TraderDetail()
        {
            this.InitializeComponent();
            
            rnd.Next();
        }

        ~TraderDetail() //When page is destroyed
        {
            System.Diagnostics.Debug.WriteLine("TraderDetail page destoyed.");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            inst = this;
            this.stockpricechrt = stockpriceChart;
            //init traders
            traders = Master.inst.Traders;
            traderList.Items.Clear();
            //Add all traders to the textboxes
            for (int i = 0; i < traders.Count; i++)
            {
                TextBlock newBlock = new TextBlock();
                newBlock.Text = traders[i].name;
                traderList.Items.Add(newBlock);
            }

            InsertDetails(-1);
        }
        public void InsertDetails(int selectedindex)
        {
            //insert trader names is already done in initalisation of page (page_loaded)

            //load the selected trader into the contentpage, if no trader is selected, load the first trader.
            selectedTrader = selectedindex != -1 ? traders[selectedindex] : traders[0];
            companyThoughtSelector.Items.Clear();
            foreach (Company comp in selectedTrader.InterestedCompanies) {
                companyThoughtSelector.Items.Add(comp.name);
            }
            companyThoughtSelector.SelectedIndex = 0; // so that a company is selected in the choose menu

            traderName.Text = selectedTrader.name;
            currThought.Text = "NOT IMPLEMENTED YET: implement Trader.Thought";

            currStockPrice.Text = "Stock prices: ";
            for (int i = 0; i < selectedTrader.InterestedCompanies.Count; i++)
            {
                currStockPrice.Text += selectedTrader.InterestedCompanies[i].BidAsk.Bid + " ";
            }

            //load prices of stocks of interested companies of trader into graph
            List<Company> interestedComp = selectedTrader.InterestedCompanies;
            loadTraderGraphs(
                companyThoughtSelector.SelectedIndex != -1
                ? interestedComp[companyThoughtSelector.SelectedIndex]
                : interestedComp[0]);

            ChangeTraderThought();
        }

        private void companyThoughtSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //load the stock prices of the company that is selected via the thought selector into the graph
            if(companyThoughtSelector.SelectedIndex != -1 && traderList.SelectedIndex != -1)
                loadTraderGraphs(traders[traderList.SelectedIndex].InterestedCompanies[companyThoughtSelector.SelectedIndex]);

            //Change the trader thought
            ChangeTraderThought();
        }
        public void ChangeTraderThought()
        {
            if (companyThoughtSelector.SelectedIndex == -1) {
                int rand = rnd.Next(0, 3);
                switch(rand){
                    case 0:
                        currThought.Text = Thoughts.idk;
                        break;
                       case 1:
                        currThought.Text = Thoughts.hold;
                        break;
                       case 2:
                        currThought.Text = Thoughts.wait;
                        break;
                }
            }
            else
            {
                Company selectedComp = selectedTrader.InterestedCompanies[companyThoughtSelector.SelectedIndex];
                if (selectedTrader.latestResults != null)
                {
                    for (int i = 0; i < selectedTrader.latestResults.Results.Count; i++)
                    {
                        if (selectedTrader.latestResults.Results[i].Item1 == selectedComp)
                        {
                            //formulate thought based on market results
                            float MarketOutcome = selectedTrader.latestResults.Results[i].Item2;
                            //Bij de breakout is een breakout -5 of +5, dus de float schommelt daartussen.
                            if (MarketOutcome <= -1)
                            {
                                //sell thought
                                //inefficientie is leuk
                                currThought.Text = Thoughts.sell;
                                if (MarketOutcome <= -4.5)
                                {
                                    //despise thought
                                    currThought.Text = Thoughts.hate;

                                }
                            }
                            else if (MarketOutcome >= 1)
                            {
                                //buy thought
                                currThought.Text = Thoughts.buy;
                            }
                            else
                            {
                                int rand = rnd.Next(0, 3);
                                //doubt thought
                                switch (rand)
                                {
                                    case 0:
                                        currThought.Text = Thoughts.idk;
                                        break;
                                    case 1:
                                        currThought.Text = Thoughts.hold;
                                        break;
                                    case 2:
                                        currThought.Text = Thoughts.wait;
                                        break;
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }

        //REPLACE WITH LOAD TRADER GRAPHS
        void loadTraderGraphs(Company comp)
        {
            stockpriceChart.Series.Clear();

            //INSERT STOCK PRICES
            FastCandleBitmapSeries candleSeries = new FastCandleBitmapSeries()
            {
                ItemsSource = comp.stockViewModel.prices1m,
                XBindingPath = "Year",
                High = "High",
                Low = "Low",
                Open = "Open",
                Close = "Close"
            };
            candleSeries.ListenPropertyChange = true;
            candleSeries.BullFillColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 70, 220, 75));
            candleSeries.BearFillColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 235, 30, 30));
            //stockPriceChart.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 30, 30, 35));

            (stockpriceChart.SecondaryAxis as NumericalAxis).ZoomFactor = 1.4;
            candleSeries.ComparisonMode = FinancialPrice.None;

            stockpriceChart.Series.Add(candleSeries);
        }

        private void traderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (traderList.SelectedIndex != -1)
            {
                InsertDetails(traderList.SelectedIndex);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Eco.Field)
            {
                Field = (e.Parameter as Eco.Field);
            }
            base.OnNavigatedTo(e);
        }
    }
}
