using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Apex_curs
{
    public class Map_timer_M : INotifyPropertyChanged
    {
        string currentMap_msg1;
        string currentMap_msg2;
        string nextMap_msg1;
        string nextMap_msg2;
        string nextMap_msg3;
        string nextMap_msg4;
        string timeToNextMap;


        public string CurrentMap_msg1
        {
            get { return currentMap_msg1; }
            set
            {
                currentMap_msg1 = value;
                OnPropertyChanged("CurrentMap_msg1");
            }
        }
        public string CurrentMap_msg2
        {
            get { return currentMap_msg2; }
            set
            {
                currentMap_msg2 = value;
                OnPropertyChanged("CurrentMap_msg2");
            }
        }
        public string NextMap_msg1
        {
            get { return nextMap_msg1; }
            set
            {
                nextMap_msg1 = value;
                OnPropertyChanged("NextMap_msg1");
            }
        }
        public string NextMap_msg2
        {
            get { return nextMap_msg2; }
            set
            {
                nextMap_msg2 = value;
                OnPropertyChanged("NextMap_msg2");
            }
        }
        public string NextMap_msg3
        {
            get { return nextMap_msg3; }
            set
            {
                nextMap_msg3 = value;
                OnPropertyChanged("NextMap_msg3");
            }
        }
        public string NextMap_msg4
        {
            get { return nextMap_msg4; }
            set
            {
                nextMap_msg4 = value;
                OnPropertyChanged("NextMap_msg4");
            }
        }
        public string TimeToNextMap
        {
            get { return timeToNextMap; }
            set
            {
                timeToNextMap = value;
                OnPropertyChanged("TimeToNextMap");
            }
        }

        public Map_timer_M() { }
        public Map_timer_M(string current1, string current2, string next1, string next2, string next3, string next4, string time)
        {
            CurrentMap_msg1 = current1;
            CurrentMap_msg2 = current2;
            NextMap_msg1 = next1;
            NextMap_msg2 = next2;
            NextMap_msg3 = next3;
            NextMap_msg4 = next4;
            TimeToNextMap = time;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
