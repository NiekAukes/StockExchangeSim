using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StockExchangeSim.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CompanyDetail : Page
    {
        Eco.Field Field { get; set; }
        List<Eco.Company> listofCompanies;
        public CompanyDetail()
        {
            this.InitializeComponent();
            companyList.Items.Clear();


        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InsertDetails(Field.companies, -1);

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
            listofCompanies = companies;

            

            //insert company names:
            if (companyList.Items.Count < 1)
            {
                companyList.Items.Clear();
                for (int i = 0; i < listofCompanies.Count; i++)
                {
                    listofCompanies[i].initName();

                    //make listboxitem to shove name into (with force)
                    TextBlock item = new TextBlock();
                    item.Text = (listofCompanies[i].id).ToString();
                    //item.PointerPressed += Item_PointerPressed;
                    //item.PointerReleased += Item_PointerPressed;
                    companyList.Items.Add(item);
                }
            }

            if (selectedindex != -1)
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
        }

        

        void loadCompanyGraphs(Eco.Company comp)
        {
            stockPriceChart.Series.Clear();
            companyValueChart.Series.Clear();

            //INSERT COMPANY VALUES
            LineSeries series = new LineSeries()
            {
                ItemsSource = comp.ValueviewModel.values,
                XBindingPath = "Year",
                YBindingPath = "Value"
            };
            series.ListenPropertyChange = true;
            companyValueChart.Series.Add(series);

            //INSERT STOCK PRICES
            CandleSeries candleSeries = new CandleSeries()
            {
                ItemsSource = comp.stockViewModel.prices,
                XBindingPath = "Year",
                High = "High",
                Low = "Low",
                Open = "Open",
                Close = "Close"
            };
            candleSeries.ListenPropertyChange = true;
            candleSeries.Trendlines.Add(new Trendline() { Label = "Trend", Stroke = new SolidColorBrush(Colors.Aqua), Type = TrendlineType.Linear });
            stockPriceChart.Series.Add(candleSeries);
        }

        private void companyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(companyList.SelectedIndex != -1)
            {
                InsertDetails(listofCompanies, companyList.SelectedIndex);
            }
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
