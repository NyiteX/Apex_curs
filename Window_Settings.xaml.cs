﻿using Apex_curs.Classes;
using Microsoft.Win32;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Apex_curs
{
    /// <summary>
    /// Логика взаимодействия для Window_Settings.xaml
    /// </summary>
    public partial class Window_Settings : Window
    {
        const string connectDBLogin = @"Data Source = WIN-U669V8L9R5E; Initial Catalog = Users_GameDB; Trusted_Connection=True";
        string connectionString;
        int picIDtmp;
        public Window_Settings(string connectstr)
        {
            InitializeComponent();

            connectionString = connectstr;
            btn_addPicture.IsEnabled = false;
            btn_swap.IsEnabled = false;
        }
        public Window_Settings(string connectstr, string login,string pass)
        {
            InitializeComponent();

            connectionString = connectstr;            
            btn_addPicture.IsEnabled = false;
            btn_swap.IsEnabled = false;

            tb_login.Text = login;
            passwordBox_.Password = pass;

            border_login.Visibility = Visibility.Hidden;
            panel_menu.Visibility = Visibility.Visible;
            chkBox_loginSave.IsChecked = true;
        }

        byte[] ConvertBitmapSourceToByteArray(ImageSource imageSource)
        {
            var image = imageSource as BitmapSource;
            byte[] data;
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }
        BitmapImage LoadImageFromFile()
        {
            BitmapImage bitmap = null;
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Картинки (*.bmp;*.jpg;*.jpeg,*.png)|*.bmp;*.jpg;*.jpeg;*.png";

                openFileDialog.ShowDialog();

                if (openFileDialog.FileNames.Length > 0)
                {
                    bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDialog.FileName);
                    bitmap.EndInit();
                }
            }
            catch { }
            return bitmap;
        }

        //current picture func
        BitmapImage Load_CharacterIMG(string connectionString,string name)
        {
            BitmapImage BitObj = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(
                        $"SELECT Pic_table.Pic,Pic_table.ID FROM Character_my,Pic_table WHERE Pic_table.ID = Character_my.PicID AND Character_my.Names = '{name}'",
                        connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        BitObj = new BitmapImage();
                    }
                    else
                    {
                        MessageBox.Show("Not found.");
                    }
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
            }
            catch { }

            return BitObj;
        }

        //
        //current picture(update)
        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            if (tb_name.Text.Count() > 0)
            {
                try
                {
                    current_Image.Source = Load_CharacterIMG(connectionString, tb_name.Text);
                    if(current_Image.Source != null)
                    {
                        btn_addPicture.IsEnabled = true;
                    }                  
                }
                catch{ }
            }
            else
            {
                MessageBox.Show("Legend name is empty.");
            }
        }
        //new picture(update)
        private void btn_addPicture_Click(object sender, RoutedEventArgs e)
        {
            new_Image.Source = LoadImageFromFile();
            if(new_Image.Source != null)
            {
                btn_swap.IsEnabled = true;
            }           
        }

        //upd picture btn(update)
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
                    current_Image.Source = Load_CharacterIMG(connectionString, tb_name.Text);
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

        //
        //
        //create Legend
        private void btn_createLegend_Click(object sender, RoutedEventArgs e)
        {
            if(tb_loreAdd.Text != "Lore..." && tb_nameAdd.Text != "Name..." && new_ImageAdd.Source != null)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        byte[] data = ConvertBitmapSourceToByteArray(new_ImageAdd.Source);
                        SqlCommand command = new SqlCommand("INSERT INTO Pic_table (Pic) VALUES(@Image)", connection);
                        command.Parameters.AddWithValue("@Image", data);
                        command.ExecuteNonQuery();

                        command = new SqlCommand("SELECT MAX(ID) FROM Pic_table", connection);
                        SqlDataReader reader = command.ExecuteReader();
                        int IDtmp = -1;
                        while(reader.Read())
                        {
                            IDtmp = Convert.ToInt32(reader.GetValue(0));
                        }
                        reader.Close();

                        command = new SqlCommand($"INSERT INTO Character_my(Names,PicID,Lore) VALUES ('{tb_nameAdd.Text}',{IDtmp},'{tb_loreAdd.Text}')", connection);
                        command.ExecuteNonQuery();
                    }
                }
                catch 
                {
                    MessageBox.Show("Name doesnt exist.");
                    tb_loreAdd.Text = "Lore...";
                    tb_nameAdd.Text = "Name...";
                    new_ImageAdd.Source = null;
                }
            }
            else
            {
                MessageBox.Show("U need to write Lore, Name and export Image before start.","Warning");
            }
        }

        //add image to Addpanel
        private void btn_addPic_addPanel(object sender, RoutedEventArgs e)
        {
            new_ImageAdd.Source = LoadImageFromFile();
        }


        //
        //textbox ui
        //
        private void tb_loreAdd_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tb_loreAdd.Text == "Lore...")
            {
                tb_loreAdd.Text = "";
                tb_loreAdd.Foreground = Brushes.Black;
            }
        }

        private void tb_loreAdd_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tb_loreAdd.Text == "")
            {
                tb_loreAdd.Text = "Lore...";
                tb_loreAdd.Foreground = Brushes.Gray;
            }
        }

        private void tb_nameAdd_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tb_nameAdd.Text == "Name...")
            {
                tb_nameAdd.Text = "";
                tb_nameAdd.Foreground = Brushes.Black;
            }
        }

        private void tb_nameAdd_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tb_nameAdd.Text == "")
            {
                tb_nameAdd.Text = "Name...";
                tb_nameAdd.Foreground = Brushes.Gray;
            }
        }
        ////////
       
        //visibility changer(menu)
        private void RB_create_Checked(object sender, RoutedEventArgs e)
        {
            border_AddLegends.Visibility = Visibility.Visible;

            border_ImageChanger.Visibility = Visibility.Hidden;
            border_DelLegend.Visibility = Visibility.Hidden;
            Clear_Borders();
        }
        private void RB_update_Checked(object sender, RoutedEventArgs e)
        {
            border_ImageChanger.Visibility = Visibility.Visible;

            border_AddLegends.Visibility = Visibility.Hidden;
            border_DelLegend.Visibility = Visibility.Hidden;
            Clear_Borders();
        }
        private void RB_delete_Checked(object sender, RoutedEventArgs e)
        {
            border_DelLegend.Visibility= Visibility.Visible;

            border_AddLegends.Visibility = Visibility.Hidden;
            border_ImageChanger.Visibility = Visibility.Hidden;
            Clear_Borders();
        }

        //lf Legend by name
        private void btn_SearchDel_Click(object sender, RoutedEventArgs e)
        {
            if (tb_nameDel.Text.Count() > 0)
            {
                del_Image.Source = Load_CharacterIMG(connectionString, tb_nameDel.Text);
                if (del_Image.Source != null)
                {
                    btn_Del.IsEnabled = true;
                }
            }
            else
            {
                MessageBox.Show("Legend name is empty.");
            }
        }

        //delete Legend
        private void btn_Del_Click(object sender, RoutedEventArgs e)
        {
            if (tb_nameDel.Text.Count() > 0 && del_Image.Source != null)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        SqlCommand command = new SqlCommand($"DELETE FROM Character_my WHERE Names = '{tb_nameDel.Text}'", connection);
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("Deleted");
                }
                catch (Exception s){ MessageBox.Show(s.Message); }
                finally
                {
                    tb_nameDel.Text = "";
                    btn_Del.IsEnabled = false;
                    del_Image.Source = null;
                }
            }
        }
        ///////////////////////////////////////////////////////////////////////////

        void Clear_Borders()
        {
            tb_nameAdd.Text = "Name...";
            tb_name.Text = "";
            tb_nameDel.Text = "";
            tb_loreAdd.Text = "Lore...";
            picIDtmp = -1;

            new_Image.Source = null;
            current_Image.Source = null;
            del_Image.Source = null;
            new_ImageAdd.Source = null;


            btn_addPicture.IsEnabled = false;
            btn_swap.IsEnabled = false;
            btn_Del.IsEnabled = false;
        }


        //
        //basic buttons
        //
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

        //login
        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
/*            tb_login.Text = (HashClass.ToSHA256(passwordBox_.Password));
            return;*/
            if (tb_login.Text.Count() > 0 && passwordBox_.Password.Count() > 0)
            {
                using (SqlConnection connection = new SqlConnection(connectDBLogin))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(
                    $"SELECT * FROM Users WHERE Login_ = '{tb_login.Text}' AND Password_ = '{HashClass.ToSHA256(passwordBox_.Password)}' AND IsAdmin = 1",
                    connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        passwordBox_.Password = "";
                        tb_login.Text = "";

                        chkBox_loginSave.IsChecked = false;
                        MessageBox.Show("Wrong Login or Password.", "Error");
                    }
                    else
                    {
                        border_login.Visibility = Visibility.Hidden;
                        panel_menu.Visibility = Visibility.Visible;
                    }
                }             
            }
            else
            {
                MessageBox.Show("Login and Password are necessary.", "Warning");
            }
        }

        private void Button_LogOut_Click(object sender, RoutedEventArgs e)
        {
            Clear_Borders();
            passwordBox_.Password = "";
            tb_login.Text = "";
            connectionString = "";

            border_login.Visibility = Visibility.Visible;
            panel_menu.Visibility = Visibility.Hidden;
            chkBox_loginSave.IsChecked = false;

            border_AddLegends.Visibility = Visibility.Hidden;
            border_DelLegend.Visibility= Visibility.Hidden;
            border_ImageChanger.Visibility = Visibility.Hidden;
        }
    }
}
