using Eco;
using Syncfusion.UI.Xaml.Charts;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StockExchangeSim.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TraderDetail : Page
    {
        public Field Field { get; private set; }
        public List<Trader> traders { get; private set; }

        public TraderDetail()
        {
            this.InitializeComponent();



        }

        ~TraderDetail() //When page is destroyed
        {
            System.Diagnostics.Debug.WriteLine("destroyed");
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //init traders
            traders = Master.inst.Traders;
            traderList.Items.Clear();
            //Add all traders to the textboxes
            for (int i = 0; i< traders.Count;i++)
            {
                TextBlock newBlock = new TextBlock();
                newBlock.Text = traders[i].name;
                traderList.Items.Add(newBlock);
            }

            InsertDetails(-1);
        }



        public void InsertDetails(int selectedindex)
        {
            //REPLACE WITH TRADERS
            traderName.Text = traders[selectedindex].name;
            //insert trader names is already done in initalisation of page (page_loaded)

            //load the selected trader into the contentpage, if no trader is selected, load the first trader.
            Trader selectedTrader = selectedindex != -1 ? traders[selectedindex] : traders[0];

            traderName.Text = selectedTrader.name;
            currThought.Text = "NOT IMPLEMENTED YET: implement Trader.Thought";

            currStockPrice.Text = "Stock prices: ";
            for (int i = 0; i < selectedTrader.InterestedCompanies.Count; i++)
            {
                currStockPrice.Text += selectedTrader.InterestedCompanies[i].BidAsk.Bid + " ";
            }

            //load prices of stocks of interested companies of trader into graph
            List<Company> interestedComp = selectedTrader.InterestedCompanies;
            if (companyThoughtSelector.SelectedIndex != -1)
                loadTraderGraphs(interestedComp[companyThoughtSelector.SelectedIndex]);
            else
                loadTraderGraphs(interestedComp[0]);

        }

        private void companyThoughtSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //load the stock prices of the company that is selected via the thought selector into the graph
            loadTraderGraphs(traders[traderList.SelectedIndex].InterestedCompanies[companyThoughtSelector.SelectedIndex]);
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
