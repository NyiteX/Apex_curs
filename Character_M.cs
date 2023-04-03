using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Apex_curs
{
    internal class Character_M : INotifyPropertyChanged
    {
        BitmapImage img;
        public BitmapImage Image
        {
            get { return img; }
            set
            {
                img = value;
                OnPropertyChanged("Image");
            }
        }
        string lore;
        public string Lore
        {
            get { return lore; }
            set
            {
                lore = value;
                OnPropertyChanged("Lore");
            }
        }

        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public Character_M(BitmapImage image, string name, string lore)
        {
            Image = image;
            Name = name;
            Lore = lore;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
