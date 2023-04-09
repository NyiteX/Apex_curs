using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;

namespace Apex_curs
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string apiKey = "dbb6ed3d331dda882b02101bcc0c608b";
        string connectionString = @"Data Source = WIN-U669V8L9R5E; Initial Catalog = GameDB; Trusted_Connection=True";
        Character_VM Characters_;  //legends list
        Map_VM Maps_;  //maps list
        Map_timer_M mapUpd_M = new Map_timer_M(); //map timer

        DispatcherTimer timerMapUpd = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
     
            MapTimerUpdate();
            Load_backgroundImg(1);

            Characters_ = new Character_VM(connectionString);
            Maps_ = new Map_VM(connectionString);

            timerMapUpd.Interval = TimeSpan.FromMilliseconds(1000);
            timerMapUpd.Tick += timerTick;

            timerMapUpd.Start();
        }

        void timerTick(object sender, EventArgs e)
        {
            MapTimerUpdate();
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

        async void MapTimerUpdate()
        {
            try
            {
                var httpClient = new HttpClient();

                var test = $"https://api.mozambiquehe.re/maprotation?auth={apiKey}&version=2";
                var response = await httpClient.GetAsync(test);
                var result = await response.Content.ReadAsStringAsync();

                string typeMap = "battle_royale";
                if(chk_ranked.IsChecked == true) typeMap = "ranked";
                else if(chk_rankedArena.IsChecked == true) typeMap = "arenasRanked";
                else if(chk_funMap.IsChecked == true) typeMap = "ltm";

                var stats = JObject.Parse(result);
                var battleRoyale = stats[typeMap];
                var maps = battleRoyale["current"];
                var map = maps["map"];
                var remeiningTime = maps["remainingTimer"];

                mapUpd_M.CurrentMap_msg1 = "Current map: ";
                mapUpd_M.CurrentMap_msg2 = $"{map}";

                var maps2 = battleRoyale["next"];
                var map2 = maps2["map"];
                mapUpd_M.NextMap_msg1 = "Next map: ";
                mapUpd_M.NextMap_msg2 = $"{map2} ";
                mapUpd_M.NextMap_msg3 = "start in ";
                mapUpd_M.NextMap_msg4 = $"{remeiningTime}";
            }
            catch { }
            finally 
            {
                label_currmap.DataContext = mapUpd_M;
                label_nextmap.DataContext = mapUpd_M;
            }
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

        //added 'About' menu items here
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
        // menu visibility changer
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
        /////////////////////////////////////////////////////////////////////////////////////////
        private void btn_CheckAcc(object sender, RoutedEventArgs e)
        {
            if (tb_acccount.Text != "Enter account name..." && tb_acccount.Text.Count() > 0)
            {
                Window_Acc_stats form = new Window_Acc_stats(tb_acccount.Text);
                form.Closed += delegate
                {
                    tb_acccount.Text = "Enter account name...";
                };

                form.ShowDialog();
            }
        }

        private void tb_acccount_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tb_acccount.Text == "Enter account name...")
            {
                tb_acccount.Text = "";
                tb_acccount.Foreground = Brushes.Black;
            }
        }

        private void tb_acccount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tb_acccount.Text == "")
            { 
                tb_acccount.Text = "Enter account name...";
                tb_acccount.Foreground = Brushes.Gray;
            }
        }

        string login_tmp ="";
        string pass_tmp="";
        private void Btn_Settings(object sender, RoutedEventArgs e)
        {
            Window_Settings form;
            if(login_tmp.Count() > 0 && pass_tmp.Count() > 0)
            {
                form = new Window_Settings(connectionString, login_tmp, pass_tmp);  //if 'remember me' checked
                pass_tmp = "";
                login_tmp = "";
            }
            else
            {
                form = new Window_Settings(connectionString);
            }
            form.Closed += delegate
            {
                if(form.chkBox_loginSave.IsChecked == true)
                {
                    login_tmp = form.tb_login.Text;
                    pass_tmp = form.passwordBox_.Password;
                }
                Characters_ = new Character_VM(connectionString);
                l_box.DataContext = Characters_;                //udp Legends list
            };

            form.ShowDialog();
        }
    }
}
