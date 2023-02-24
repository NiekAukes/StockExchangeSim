﻿using Syncfusion.UI.Xaml.Charts;
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
    public sealed partial class CompanyDetail : Page
    {
        Eco.Field Field { get; set; }
        List<Eco.Company> listofCompanies;
        public static CompanyDetail inst;

        public CompanyDetail()
        {
            this.InitializeComponent();
            companyList.Items.Clear();

            Unloaded += CompanyDetail_Unloaded;
            Loaded += CompanyDetail_Loaded;
        }

        private void CompanyDetail_Loaded(object sender, RoutedEventArgs e)
        {
            stockPriceChart.ResumeSeriesNotification();
            companyValueChart.ResumeSeriesNotification();
        }

        private void CompanyDetail_Unloaded(object sender, RoutedEventArgs e)
        {
            stockPriceChart.SuspendSeriesNotification();
            companyValueChart.SuspendSeriesNotification();
        }

        ~CompanyDetail()
        {
            System.Diagnostics.Debug.WriteLine("CompanyDetail page destroyed.");
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            inst = this;
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
            FieldName.Text = Field.fieldName;


            //insert company names:
            if (companyList.Items.Count < 1)
            {
                companyList.Items.Clear();
                for (int i = 0; i < listofCompanies.Count; i++)
                {
                    //make listboxitem to shove name into (with force)
                    TextBlock item = new TextBlock();
                    item.Text = listofCompanies[i].name;
                    //item.PointerPressed += Item_PointerPressed;
                    //item.PointerReleased += Item_PointerPressed;
                    companyList.Items.Add(item);
                }
            }
            //load selected company into contentpage, if not, load the first one
                Eco.Company selectedcomp = selectedindex != -1 ? listofCompanies[companyList.SelectedIndex] : listofCompanies[0];
                CompanyName.Text = selectedcomp.name;
                companyValue.Text = "Company value: " + selectedcomp.Value.ToString();
                currStockPrice.Text = "Stock price: " + selectedcomp.stockprice.ToString();

                //shove prices into graphs
                loadCompanyGraphs(selectedcomp);
            
        }



        void loadCompanyGraphs(Eco.Company comp)
        {
            stockPriceChart.Series.Clear();
            companyValueChart.Series.Clear();

            //INSERT COMPANY VALUES
            FastLineSeries series = new FastLineSeries()
            {
                ItemsSource = comp.ValueviewModel.values,
                XBindingPath = "Year",
                YBindingPath = "Value"
            };
            series.ListenPropertyChange = true;

            companyValueChart.Series.Add(series);

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

            (stockPriceChart.SecondaryAxis as NumericalAxis).ZoomFactor = 1.4;
            candleSeries.ComparisonMode = FinancialPrice.None;

            stockPriceChart.Series.Add(candleSeries);
        }

        private void companyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (companyList.SelectedIndex != -1)
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