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

        ~TraderDetail()
        {
            System.Diagnostics.Debug.WriteLine("destroyed");
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            traders = Master.inst.Traders;
            for(int i = 0; i< traders.Count;i++)
            {
                TextBlock newBlock = new TextBlock();
                //newBlock.Text = traders[i].Name;
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

        public void InsertDetails(List<Eco.Company> companies, int selectedindex)
        {
            //REPLACE WITH TRADERS
            traders = Master.inst.Traders;
            FieldName.Text = Field.fieldName;


            //insert trader names is already done in initalisation of page (page_loaded)
            
            //if (companyList.Items.Count < 1)
            //{
            //    companyList.Items.Clear();
            //    for (int i = 0; i < listofCompanies.Count; i++)
            ////    {
            //        //make listboxitem to shove name into (with force)
            //        TextBlock item = new TextBlock();
            //        item.Text = listofCompanies[i].name;
            //        //item.PointerPressed += Item_PointerPressed;
            //        //item.PointerReleased += Item_PointerPressed;
            //        companyList.Items.Add(item);
            //    }
            //}

            /*if (selectedindex != -1)
            {
                Eco.Company selectedcomp = listofCompanies[companyList.SelectedIndex];
                CompanyName.Text = selectedcomp.name;
                companyValue.Text = "Company value: " + selectedcomp.Value.ToString();
                currStockPrice.Text = "Stock price: " + selectedcomp.stockprice.ToString();

                //shove prices into graphs
                loadCompanyGraphs(selectedcomp);
            }
            else
            {
                //load the first company's details in already, before the user has to click anything
                Eco.Company comp1 = listofCompanies[0];
                CompanyName.Text = comp1.name;
                companyValue.Text = "Company value: " + comp1.Value.ToString();
                currStockPrice.Text = "Stock price: " + comp1.stockprice.ToString();

                //shove stock prices into chart: biek baukes moet daarmit mar eem helpen
                loadCompanyGraphs(listofCompanies[0]);
            }
            */
        }


        //REPLACE WITH LOAD TRADER GRAPHS
        void loadCompanyGraphs(Eco.Company comp)
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
            /*if (companyList.SelectedIndex != -1)
            {
                InsertDetails(listofCompanies, companyList.SelectedIndex);
            }*/
        }

        /*private void companyList_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            //make style of selected listboxitem different
            //doen we niks mee aangezien windows' standaardstijl zeer schmecksy is
        }

        private void companyList_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //if pressed and released load new company data into the page
            //LOAD DATA INTO PAGE
            if(companyList.SelectedIndex != -1)
            {
                InsertDetails(listofCompanies);
            }
        }
        private void Item_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            companyList_PointerReleased(sender, e);
        }*/

    }
}
