using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Apex_curs
{
    internal class Acc_info_M : INotifyPropertyChanged
    {
        string name;
        string platform;
        string rankName;
        string currentState;
        int level;
        BitmapImage rankImg;
        BitmapImage selectedChar_Img;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public string Platform
        {
            get { return platform; }
            set
            {
                platform = value;
                OnPropertyChanged("Platform");
            }
        }
        public string RankName
        {
            get { return rankName; }
            set
            {
                rankName = value;
                OnPropertyChanged("RankName");
            }
        }
        public string CurrentState
        {
            get { return currentState; }
            set
            {
                currentState = value;
                OnPropertyChanged("CurrentState");
            }
        }
        public int Level
        {
            get { return level; }
            set
            {
                level = value;
                OnPropertyChanged("Level");
            }
        }
        public BitmapImage RankImg
        {
            get { return rankImg; }
            set
            {
                rankImg = value;
                OnPropertyChanged("RankImg");
            }
        }
        public BitmapImage SelectedChar_Img
        {
            get { return selectedChar_Img; }
            set
            {
                selectedChar_Img = value;
                OnPropertyChanged("SelectedChar_Img");
            }
        }

        public Acc_info_M(string names, string platforms, string rankNames, string currentStates, int levels, BitmapImage rankImgs, BitmapImage selected)
        {
            Name = names;
            Platform = platforms;
            RankName = rankNames;
            CurrentState = currentStates;
            Level = levels;
            RankImg = rankImgs;
            SelectedChar_Img = selected;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
