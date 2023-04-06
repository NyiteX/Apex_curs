using Microsoft.Win32;
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
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Apex_curs
{
    /// <summary>
    /// Логика взаимодействия для Window_Settings.xaml
    /// </summary>
    public partial class Window_Settings : Window
    {
        string connectionString;
        int picIDtmp;
        public Window_Settings(string connectionString)
        {
            InitializeComponent();

            this.connectionString = connectionString;
            btn_addPicture.IsEnabled = false;
            btn_swap.IsEnabled = false;
        }
     

        BitmapImage Load_CharacterIMG(string connectionString)
        {
            BitmapImage BitObj = new BitmapImage();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(
                    $"SELECT Pic_table.Pic,Pic_table.ID FROM Character_my,Pic_table WHERE Pic_table.ID = Character_my.PicID AND Character_my.Names = '{tb_name.Text}'",
                    connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    byte[] result = (byte[])reader.GetValue(0);
                    picIDtmp = Convert.ToInt32(reader.GetValue(1));

                    Stream StreamObj = new MemoryStream(result);

                    BitObj.BeginInit();
                    BitObj.StreamSource = StreamObj;
                    BitObj.EndInit();
                }
            }

            return BitObj;
        }
        //current picture
        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            if (tb_name.Text.Count() > 0)
            {
                try
                {
                    current_Image.Source = Load_CharacterIMG(connectionString);
                    btn_addPicture.IsEnabled = true;        
                }
                catch{ }
            }
            else
            {
                MessageBox.Show("Legend name is empty.");
            }
        }
        //next picture
        private void btn_addPicture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Картинки (*.bmp;*.jpg;*.jpeg,*.png)|*.bmp;*.jpg;*.jpeg;*.png";

                openFileDialog.ShowDialog();

                if (openFileDialog.FileNames.Length > 0)
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDialog.FileName);
                    bitmap.EndInit();
                    new_Image.Source = bitmap;
                }
                btn_swap.IsEnabled = true;
            }
            catch { }
        }

        //upd picture btn
        private void Button_Swap(object sender, RoutedEventArgs e)
        {
            if(tb_name.Text.Count()>0 && current_Image.Source!=null && new_Image.Source != null)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        byte[] data = ConvertBitmapSourceToByteArray(new_Image.Source);
                        SqlCommand command = new SqlCommand("UPDATE Pic_table SET Pic = @Image WHERE ID = @Id", connection);
                        command.Parameters.AddWithValue("@Image", data);
                        command.Parameters.AddWithValue("@Id", picIDtmp);
                        command.ExecuteNonQuery();
                    }
                    current_Image.Source = Load_CharacterIMG(connectionString);
                }
                catch { }
                finally
                {
                    tb_name.Text = "";
                    picIDtmp = -1;
                    new_Image.Source = null;
                    btn_addPicture.IsEnabled = false;
                    btn_swap.IsEnabled = false;
                }                
            }
        }

        byte[] ConvertBitmapSourceToByteArray(ImageSource imageSource)
        {
            var image = imageSource as BitmapSource;
            byte[] data;
            BitmapEncoder encoder = new PngBitmapEncoder();
            /*BitmapEncoder encoder = new JpegBitmapEncoder();*/
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
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
