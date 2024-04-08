using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.LinkLabel;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {

        private SqlConnection conn = null;
        SqlDataAdapter da = null;
        DataSet set = null;
        SqlCommandBuilder cmd = null;
        string cs = "";


        public MainWindow()
        {
            InitializeComponent();
            conn = new SqlConnection();
            cs = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Tea_Shop;Integrated Security=SSPI;";

        }

        private void Button_Click(object sender, RoutedEventArgs e)

        {
            try
            {
                SqlConnection conn = new SqlConnection(cs);
                set = new DataSet();
                string sql = "SELECT * FROM Tea ; select *From TeaType  ";
                da = new SqlDataAdapter(sql, conn);
                DataGrid1.ItemsSource = null;
                cmd = new SqlCommandBuilder(da);
                Debug.WriteLine(cmd.GetInsertCommand().CommandText);
                Debug.WriteLine(cmd.GetUpdateCommand().CommandText);
                Debug.WriteLine(cmd.GetDeleteCommand().CommandText);
                da.Fill(set, "MyTeaShop");
                set.Tables[0].TableName = "Tea";
                set.Tables[1].TableName = "TeaType";

                System.Windows.MessageBox.Show("Данные из бд загружены и готовы к использования в автономном режиме.");

                DataView Source = new DataView(set.Tables["Tea"]);   //-- для отображения в DataGrid1 и редактирования  
                // DataView Source = new DataView(set.Tables["TeaType"]);  -- для отображения в DataGrid1 и редактирования  
                DataGrid1.Items.Refresh();
                DataGrid1.ItemsSource = Source;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

        }

        private DataTable CreateResultTable()
        {
            DataTable resultTable = new DataTable();
            resultTable.Columns.Add("TeaName", typeof(string));
            resultTable.Columns.Add("CountryOfOrigin", typeof(string));
            resultTable.Columns.Add("TeaType", typeof(string));
            resultTable.Columns.Add("Description", typeof(string));
            resultTable.Columns.Add("Grams", typeof(int));
            resultTable.Columns.Add("CostPrice", typeof(decimal));
            return resultTable;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                da.Update(set, "Tea");
                System.Windows.MessageBox.Show("Данные в бд успешно обновлены.");
                //da.Update(set, "TeaType");
            }
            catch (Exception ex) { System.Windows.MessageBox.Show(ex.Message); };
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable resultTable = CreateResultTable();
                if (set != null && set.Tables.Contains("Tea") && set.Tables.Contains("TeaType"))
                {
                    DataTable teaTable = set.Tables["Tea"];
                    DataTable teaTypeTable = set.Tables["TeaType"];

                    var query = from tea in teaTable.AsEnumerable()
                                join teaType in teaTypeTable.AsEnumerable() on tea.Field<int>("TeaTypeID") equals teaType.Field<int>("TeaTypeID")
                                select new
                                {
                                    TeaName = tea.Field<string>("TeaName"),
                                    CountryOfOrigin = tea.Field<string>("CountryOfOrigin"),
                                    TeaType = teaType.Field<string>("TypeName"),
                                    Description = tea.Field<string>("Description"),
                                    Grams = tea.Field<int>("Grams"),
                                    CostPrice = tea.Field<decimal>("CostPrice")
                                };
                    DataGrid1.ItemsSource = resultTable.DefaultView;



                    foreach (var item in query)
                    {
                        resultTable.Rows.Add(item.TeaName, item.CountryOfOrigin, item.TeaType, item.Description, item.Grams, item.CostPrice);
                    }

                    DataGrid1.ItemsSource = resultTable.DefaultView;
                }
                else
                {
                    System.Windows.MessageBox.Show("Данные не загружены или отсутствует таблица Tea или TeaType.");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable resultTable = CreateResultTable();
                if (set != null && set.Tables.Contains("Tea") && set.Tables.Contains("TeaType"))
                {
                    DataTable teaTable = set.Tables["Tea"];
                    DataTable teaTypeTable = set.Tables["TeaType"];

                    var query = from tea in teaTable.AsEnumerable()
                                join teaType in teaTypeTable.AsEnumerable() on tea.Field<int>("TeaTypeID") equals teaType.Field<int>("TeaTypeID")
                                where teaType.Field<string>("TypeName") == "Green"
                                select new
                                {
                                    TeaName = tea.Field<string>("TeaName"),
                                    CountryOfOrigin = tea.Field<string>("CountryOfOrigin"),
                                    TeaType = teaType.Field<string>("TypeName"),
                                    Description = tea.Field<string>("Description"),
                                    Grams = tea.Field<int>("Grams"),
                                    CostPrice = tea.Field<decimal>("CostPrice")
                                };

                    DataGrid1.ItemsSource = resultTable.DefaultView;


                    foreach (var item in query)
                    {
                        resultTable.Rows.Add(item.TeaName, item.CountryOfOrigin, item.TeaType, item.Description, item.Grams, item.CostPrice);
                    }

                    DataGrid1.ItemsSource = resultTable.DefaultView;
                }
                else
                {
                    System.Windows.MessageBox.Show("Данные не загружены или отсутствует таблица Tea или TeaType.");
                }
            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click4(object sender, RoutedEventArgs e)
        {
            try
            {
                var filteredData = from DataRow row in set.Tables["Tea"].Rows
                                   join DataRow teaTypeRow in set.Tables["TeaType"].Rows on (int)row["TeaTypeID"] equals (int)teaTypeRow["TeaTypeID"]
                                   where (string)teaTypeRow["TypeName"] != "Green" && (string)teaTypeRow["TypeName"] != "Black"
                                   select new
                                   {
                                       TeaName = (string)row["TeaName"],
                                       CountryOfOrigin = (string)row["CountryOfOrigin"],
                                       TeaType = (string)teaTypeRow["TypeName"],
                                       Description = (string)row["Description"],
                                       Grams = (int)row["Grams"],
                                       CostPrice = (decimal)row["CostPrice"]
                                   };

                DataGrid1.ItemsSource = filteredData;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);

            }
        }

        private void Button_Click5(object sender, RoutedEventArgs e)
        {
            try
            {
                //1.Показать минимальную себестоимость чая
                //    DataTable resultTable = CreateResultTable();
                //    DataView dataView = set.Tables["Tea"].DefaultView;
                //    string sqlQuery = @"SELECT Tea.TeaName, Tea.CountryOfOrigin, TeaType.TypeName AS TeaType, Tea.Description, Tea.Grams, Tea.CostPrice
                //                FROM Tea
                //                INNER JOIN TeaType ON Tea.TeaTypeID = TeaType.TeaTypeID
                //                WHERE Tea.CostPrice = (SELECT MIN(CostPrice) FROM Tea)";
                //   
                //    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, cs))
                //    {
                //        adapter.Fill(resultTable);
                //    }
                //    DataGrid1.ItemsSource = resultTable.DefaultView;



                //2.показать количество чаев у которых себестоимость больше средней 
                //    decimal averageCostPrice = CalculateAverageCostPrice();
                //    DataTable teaAboveAverageTable = GetTeaAboveAverage(averageCostPrice);
                //    DataGrid1.ItemsSource = teaAboveAverageTable.DefaultView;
                //    System.Windows.MessageBox.Show($"Средняя себестоимость чая: {averageCostPrice}\nКоличество чаев с себестоимостью больше средней: {teaAboveAverageTable.Rows.Count}");



                //3. Показать количество единиц каждого вила чая
                //    DataTable resultTable = new DataTable();
                //    resultTable.Columns.Add("TeaType", typeof(string));
                //    resultTable.Columns.Add("Count", typeof(int));
                //    Dictionary<string, int> teaTypeCounts = new Dictionary<string, int>();
                //    foreach (DataRow row in set.Tables["Tea"].Rows)
                //    {
                //        string teaType = GetTeaTypeName((int)row["TeaTypeID"]); 
                //        if (teaTypeCounts.ContainsKey(teaType))
                //        {
                //            teaTypeCounts[teaType]++;
                //        }
                //        else
                //        {
                //            teaTypeCounts[teaType] = 1;
                //        }
                //    }
                //    foreach (var entry in teaTypeCounts)
                //    {
                //        resultTable.Rows.Add(entry.Key, entry.Value);
                //    }
                //    DataGrid1.ItemsSource = resultTable.DefaultView;
                //}


                //4.оказать информацию о чае в описании которого встречается упоминание о вишне
                //    DataTable cherryTeaTable = CreateResultTable();
                //    foreach (DataRow row in set.Tables["Tea"].Rows)
                //    {
                //        string description = row["Description"].ToString();
                //        if (description.Contains("Cherry"))
                //        {
                //            cherryTeaTable.ImportRow(row);
                //        }
                //    }
                //    DataGrid1.ItemsSource = cherryTeaTable.DefaultView;


                //5.показ информации о чаях из указанных стран
                //    DataTable teaFromSelectedCountries = CreateResultTable();
                //    string[] selectedCountries = { "China", "Japan", "India" };
                //    foreach (DataRow row in set.Tables["Tea"].Rows)
                //    {
                //        string countryOfOrigin = row["CountryOfOrigin"].ToString();
                //        if (selectedCountries.Contains(countryOfOrigin))
                //        {
                //            teaFromSelectedCountries.ImportRow(row);
                //        }
                //    }
                //    DataGrid1.ItemsSource = teaFromSelectedCountries.DefaultView;

                //6 отображения информации о чае с себестоимостью в указанном диапазоне
                //decimal minCost = 5.0m; 
                //decimal maxCost = 10.0m; 
                //DataTable teaByCostTable = CreateResultTable(); 
                //foreach (DataRow row in set.Tables["Tea"].Rows)
                //{
                //    decimal costPrice = (decimal)row["CostPrice"]; 
                //    if (costPrice >= minCost && costPrice <= maxCost)
                //    {
                //        teaByCostTable.ImportRow(row);
                //    }
                //}
                //DataGrid1.ItemsSource = teaByCostTable.DefaultView;

                //7.отобразить название страны и количество чаев из этой страны
                //var query = from DataRow row in set.Tables["Tea"].Rows
                //             group row by row["CountryOfOrigin"] into countryGroup
                //             select new
                //             {
                //                 Country = countryGroup.Key,
                //                 TeaCount = countryGroup.Count()
                //             };
                //DataTable resultTable = new DataTable();
                //resultTable.Columns.Add("Country", typeof(string));
                //resultTable.Columns.Add("TeaCount", typeof(int));
                //foreach (var item in query)
                //{
                //    resultTable.Rows.Add(item.Country, item.TeaCount);
                //}
                //DataGrid1.ItemsSource = resultTable.DefaultView;

                /////8.Отобразить среднее количество грамм чая по каждой стране
                //var query = from DataRow row in set.Tables["Tea"].Rows
                //            group row by row["CountryOfOrigin"] into coutruGroup
                //            select new
                //            {
                //                Contry = coutruGroup.Key,
                //                AverageGrams = coutruGroup.Average(row => (int)row["Grams"])
                //            };
                //DataTable resultTable = new DataTable();
                //resultTable.Columns.Add("Country", typeof(string));
                //resultTable.Columns.Add("AverageGrams",typeof(string));
                //foreach(var item in query)
                //{
                //    resultTable.Rows.Add(item.Contry, item.AverageGrams);
                //}
                //DataGrid1.ItemsSource = resultTable.DefaultView;

                //9.Показать три самых дешевых чая по конкретной стране
                //var filteredData = set.Tables["Tea"].AsEnumerable()
                //    .Where(row => row.Field<string>("CountryOfOrigin") == "China");
                //var sortedData = filteredData.OrderBy(row => row.Field<decimal>("CostPrice"));
                //var cheapestTea = sortedData.Take(3);
                //DataTable resultTable = new DataTable();
                //resultTable.Columns.Add("TeaName", typeof(string));
                //resultTable.Columns.Add("CountryOfOrigin",typeof(string));
                //resultTable.Columns.Add("CostPrice",typeof(decimal));
                //foreach(var row in cheapestTea)
                //{
                //    resultTable.Rows.Add(row.Field<string>("TeaName"),
                //                        row.Field<string>("CountryOfOrigin"),
                //                        row.Field<decimal>("CostPrice"));
                //}
                //DataGrid1.ItemsSource = resultTable.DefaultView;

                //10.Показать три самых дешевых чая по всем странам
                //var cheapetsTeas = set.Tables["Tea"].AsEnumerable()
                //    .OrderBy(row => row.Field<decimal>("CostPrice"))
                //    .Take(3)
                //    .CopyToDataTable();
                //DataGrid1.ItemsSource = cheapetsTeas.DefaultView;

                //11.Показать топ-3 страны по количеству чаев
                //DataTable topCountriesTable = new DataTable();
                //topCountriesTable.Columns.Add("Страна", typeof(string));
                //topCountriesTable.Columns.Add("Количество чаев", typeof(string));
                //Dictionary<string, int> countryTeaCounts = new Dictionary<string, int>();
                //foreach(DataRow row in set.Tables["Tea"].Rows)
                //{
                //    string country = row["CountryOfOrigin"].ToString();
                //    if(countryTeaCounts.ContainsKey(country))
                //    {
                //        countryTeaCounts[country]++;
                //    }
                //    else { countryTeaCounts[country] = 1; }
                //}
                //var sortedCountries = countryTeaCounts.OrderByDescending(kv => kv.Value);
                //int count = 0;
                //foreach(var entry in sortedCountries)
                //{
                //    if (count >= 3) break;
                //    topCountriesTable.Rows.Add(entry.Key, entry.Value);
                //    count++;
                //}
                //DataGrid1.ItemsSource = topCountriesTable.DefaultView;

                //12.Показать топ-3 зеленых чаев по количеству грамм
                var greenTeas = (from teaRow in set.Tables["Tea"].AsEnumerable()
                                 join teaTypeRow in set.Tables["TeaType"].AsEnumerable()
                                 on teaRow.Field<int>("TeaTypeID") equals teaTypeRow.Field<int>("TeaTypeID")
                                 where teaTypeRow.Field<string>("TypeName") == "Green"
                                 orderby teaRow.Field<int>("Grams") descending
                                 select new
                                 {
                                     TeaName = teaRow.Field<string>("TeaName"),
                                     CountryOfOrigin = teaRow.Field<string>("CountryOfOrigin"),
                                     Description = teaRow.Field<string>("Description"),
                                     Grams = teaRow.Field<int>("Grams"),
                                     CostPrice = teaRow.Field<decimal>("CostPrice")
                                 }).Take(3).ToList();
                DataGrid1.ItemsSource = greenTeas;

            }
            catch (Exception ex) 
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

        }

        private decimal CalculateAverageCostPrice()
        {
            decimal totalCostPrice = 0;
            int teaCount = set.Tables["Tea"].Rows.Count;
            foreach (DataRow row in set.Tables["Tea"].Rows)
            {
                totalCostPrice += (decimal)row["CostPrice"];
            }
            return totalCostPrice / teaCount;
        }
        private int CountTeaAboveAverage(decimal averageCostPrice)
        {
            int teaCountAboveAverage = 0;
            foreach (DataRow row in set.Tables["Tea"].Rows)
            {
                decimal costPrice = (decimal)row["CostPrice"];
                if (costPrice > averageCostPrice)
                {
                    teaCountAboveAverage++;
                }
            }
            return teaCountAboveAverage;
        }

        private DataTable GetTeaAboveAverage(decimal averageCostPrice)
        {
            DataTable teaAboveAverageTable = CreateResultTable();
            foreach (DataRow row in set.Tables["Tea"].Rows)
            {
                decimal costPrice = (decimal)row["CostPrice"];
                if (costPrice > averageCostPrice)
                {
                    DataRow newRow = teaAboveAverageTable.NewRow();
                    newRow["TeaName"] = row["TeaName"];
                    newRow["CountryOfOrigin"] = row["CountryOfOrigin"];
                    newRow["TeaType"] = GetTeaTypeName((int)row["TeaTypeID"]); 
                    newRow["Description"] = row["Description"];
                    newRow["Grams"] = row["Grams"];
                    newRow["CostPrice"] = row["CostPrice"];
                    teaAboveAverageTable.Rows.Add(newRow);
                }
            }
            return teaAboveAverageTable;
        }
        private string GetTeaTypeName(int teaTypeID)
        {
            foreach (DataRow row in set.Tables["TeaType"].Rows)
            {
                if ((int)row["TeaTypeID"] == teaTypeID)
                {
                    return (string)row["TypeName"];
                }
            }
            return "";
        }

      
    }
}
     
    
    
