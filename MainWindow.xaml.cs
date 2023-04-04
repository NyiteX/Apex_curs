using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace Apex_curs
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string connectionString = @"Data Source = WIN-U669V8L9R5E; Initial Catalog = GameDB; Trusted_Connection=True";
        Character_VM Characters_;
        Map_VM Maps_;
        public MainWindow()
        {
            InitializeComponent();
            Load_backgroundImg(1);

            Characters_ = new Character_VM(connectionString);
            Maps_ = new Map_VM(connectionString);
        }

        void Load_backgroundImg(int pic_id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(
                        $"SELECT Pic FROM Pic_FON WHERE ID = {pic_id}",
                        connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        byte[] result = (byte[])reader.GetValue(0);

                        Stream StreamObj = new MemoryStream(result);
                        BitmapImage BitObj = new BitmapImage();

                        BitObj.BeginInit();
                        BitObj.StreamSource = StreamObj;
                        BitObj.EndInit();

                        Background = new ImageBrush(BitObj);
                    }
                }
            }
            catch {}
        }
        bool Load_account_info(string playerName = "Tendikyrrap")
        { 
            bool f = false;
            Task.Factory.StartNew(async() => 
            {
                var httpClient = new HttpClient();
                var apiKey = "dbb6ed3d331dda882b02101bcc0c608b";
                var platform = "PC";
                var test = $"https://api.mozambiquehe.re/bridge?auth={apiKey}&player={playerName}&platform={platform}";
                var response = await httpClient.GetAsync(test);
                var result = await response.Content.ReadAsStringAsync();

                Console.WriteLine(result + "\n---------------------------------------------------\n");

                // Разбиваем информацию по группам
                var stats = JObject.Parse(result);
                var overallStats = stats["global"];
                var legendStats = stats["legends"];
                var weaponStats = stats["weapons"];

                var name = overallStats["name"];

                /*Console.WriteLine(name);
                foreach (JProperty legend in overallStats)
                {
                    Console.WriteLine(legend.Value.ToString());
                }*/
            });
            return f;
        }


        //lore text for Legends list
        private void l_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                tb_lore.Text = Characters_.Items[l_box.SelectedIndex].Lore;
            }
            catch { }
        }
        //close btn
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        //window move
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        //maximize window
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        //added About menu here
        private void l_box_Loaded(object sender, RoutedEventArgs e)
        {
            l_box.Visibility = Visibility.Hidden;
            tb_lore.Visibility = Visibility.Hidden;
            tb_map_lore.Visibility = Visibility.Hidden;

            l_box.DataContext = Characters_;
            list_maps.DataContext = Maps_;

            list_about.Items.Add("Legends");
        }

        private void Minimize_btn_click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void list_maps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (list_maps.SelectedIndex != -1)
                {
                    list_about.SelectedIndex = -1;
                    img_map.Visibility = Visibility.Visible;
                    tb_map_lore.Visibility = Visibility.Visible;

                    tb_map_lore.Text = Maps_.Items[list_maps.SelectedIndex].Name + "\n\n" + Maps_.Items[list_maps.SelectedIndex].Lore;
                    img_map.Source = Maps_.Items[list_maps.SelectedIndex].Image;
                }
                else
                {
                    img_map.Visibility = Visibility.Hidden;
                    tb_map_lore.Visibility = Visibility.Hidden;
                }
            }
            catch { }
        }
        private void list_about_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (list_about.SelectedIndex != -1)
                {
                    list_maps.SelectedIndex = -1;

                    l_box.Visibility = Visibility.Visible;
                    tb_lore.Visibility = Visibility.Visible;
                }
                else
                {
                    l_box.Visibility = Visibility.Hidden;
                    tb_lore.Visibility = Visibility.Hidden;
                }
            }
            catch { }
        }

        private void btn_CheckAcc(object sender, RoutedEventArgs e)
        {
            if(tb_acccount.Text != "Enter account name..." && tb_acccount.Text.Count() > 0)
            {
                list_maps.SelectedIndex = -1;
                list_about.SelectedIndex = -1;


                Window_Acc_stats form = new Window_Acc_stats();
                form.ShowDialog();             
            }
        }

        private void tb_acccount_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tb_acccount.Text == "Enter account name...") tb_acccount.Text = "";
        }

        private void tb_acccount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tb_acccount.Text == "") tb_acccount.Text = "Enter account name...";
        }
    }
}
