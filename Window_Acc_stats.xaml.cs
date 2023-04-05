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
using System.Reflection.Emit;
using System.Xml.Linq;

namespace Apex_curs
{
    /// <summary>
    /// Логика взаимодействия для Window_Acc_stats.xaml
    /// </summary>
    public partial class Window_Acc_stats : Window
    {
        Acc_info_M info;
        public Window_Acc_stats(string playername)
        {
            InitializeComponent();

            
            Load_account_info(playername);
        }
        async void Load_account_info(string playerName)
        {
            try
            {
                var httpClient = new HttpClient();
                var apiKey = "dbb6ed3d331dda882b02101bcc0c608b";
                var platform = "PC";

                var test = $"https://api.mozambiquehe.re/bridge?auth={apiKey}&player={playerName}&platform={platform}";
                var response = await httpClient.GetAsync(test);
                var result = await response.Content.ReadAsStringAsync();

                //MessageBox.Show(result + "\n---------------------------------------------------\n");

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

                BitmapImage rankIMG = new BitmapImage();
                BitmapImage selectedIMG = new BitmapImage();
                rankIMG = DownloadPic(rankURL.ToString());
                selectedIMG = DownloadPic(imageselectedURL.ToString());

                info = new Acc_info_M(name.ToString(), platfo.ToString(), rankName.ToString(), state.ToString(), Convert.ToInt32(level), rankIMG, selectedIMG);
            }
            catch 
            { 
                MessageBox.Show("Аккаунт не найден.");
                Close();
            }
            finally { DataContext = info; }
        }

        BitmapImage DownloadPic(string url)
        {
            BitmapImage image = new BitmapImage();
            using (var client = new WebClient())
            {
                byte[] imageData = client.DownloadData(url);
                using (var stream = new MemoryStream(imageData))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                }
            }
            return image;
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
        private void Minimize_btn_click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
