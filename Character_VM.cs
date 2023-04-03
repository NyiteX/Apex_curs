using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Apex_curs
{
    public class Character_VM : INotifyPropertyChanged
    {
        Character_M character;
        string connectionString;
        public ObservableCollection<Character_M> Items { get; set; }
        public Character_M Character
        {
            get { return character; }
            set
            {
                character = value;
                OnPropertyChanged("Character");
            }
        }
        public Character_VM(string connectionString)
        {
            Items = new ObservableCollection<Character_M>();
            this.connectionString = connectionString;

            Load_CharactersToList(connectionString);
        }

        void Load_CharactersToList(string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(
                        "SELECT Character_my.Names,Character_my.Lore,Pic_table.Pic FROM Character_my,Pic_table WHERE Pic_table.ID = Character_my.PicID",
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
            catch { }
        }




        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
