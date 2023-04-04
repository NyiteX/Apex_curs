using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Policy;

namespace Apex_curs
{
    /// <summary>
    /// Логика взаимодействия для Window_Acc_stats.xaml
    /// </summary>
    public partial class Window_Acc_stats : Window
    {
        Acc_info_M info;
        public Window_Acc_stats()
        {
            InitializeComponent();

            

            Load_account_info();
        }
        bool Load_account_info(string playerName = "Tendikyrrap")
        {
            bool f = false;
            Task.Factory.StartNew(async () =>
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
                var realStats = stats["realtime"];
                var rank = overallStats["rank"];
                var selectedLegends = legendStats["selected"];
                var selectedi = selectedLegends["ImgAssets"];

                var imageselectedURL = selectedi["banner"];

                /////////////////////////////////////////////////////////////////
                var name = overallStats["name"];
                var platfo = overallStats["platform"];
                var level = overallStats["level"];
                var state = realStats["currentState"];
                var rankName = rank["rankName"];

                var rankURL = rank["rankImg"];

                System.Windows.Controls.Image rankIMG = new System.Windows.Controls.Image();
                System.Windows.Controls.Image selectedIMG = new System.Windows.Controls.Image();

             

                info = new Acc_info_M(name.ToString(),platfo.ToString(), rankName.ToString(), state.ToString(), Convert.ToInt32(level), rankIMG, selectedIMG);

                Dispatcher.Invoke(() => { name_tb.DataContext = info; });
            });

            
            return f;
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
        private void Minimize_btn_click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
