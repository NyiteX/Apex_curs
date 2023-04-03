using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
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

namespace Apex_curs
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string connectionString = @"Data Source = WIN-U669V8L9R5E; Initial Catalog = GameDB; Trusted_Connection=True";
        public MainWindow()
        {
            InitializeComponent();

            Ini_fon(1);
        }

        void Ini_fon(int pic_id)
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
    }
}
