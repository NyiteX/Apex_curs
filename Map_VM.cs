using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Apex_curs
{
    public class Map_VM : INotifyPropertyChanged
    {
        Character_M map;
        string connectionString;
        public ObservableCollection<Character_M> Items { get; set; }
        public Character_M Map
        {
            get { return map; }
            set
            {
                map = value;
                OnPropertyChanged("Map");
            }
        }
        public Map_VM(string connectionString)
        {
            Items = new ObservableCollection<Character_M>();
            this.connectionString = connectionString;

            Load_MapsToList(connectionString);
        }

        void Load_MapsToList(string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(
                        "SELECT Names,Lore,Pic FROM Pic_Map",
                        connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        byte[] result = (byte[])reader.GetValue(2);

                        Stream StreamObj = new MemoryStream(result);
                        BitmapImage BitObj = new BitmapImage();

                        BitObj.BeginInit();
                        BitObj.StreamSource = StreamObj;
                        BitObj.EndInit();

                        Items.Add(new Character_M(BitObj, Convert.ToString(reader.GetValue(0)), Convert.ToString(reader.GetValue(1))));
                    }
                }
            }
            catch(Exception e) { MessageBox.Show(e.Message); }
        }




        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
