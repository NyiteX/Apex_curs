using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Apex_curs
{
    public class Map_timer_M : INotifyPropertyChanged
    {
        string currentMap;
        string nextMap;
        string timeToNextMap;


        public string CurrentMap
        {
            get { return currentMap; }
            set
            {
                currentMap = value;
                OnPropertyChanged("CurrentMap");
            }
        }
        public string NextMap
        {
            get { return nextMap; }
            set
            {
                nextMap = value;
                OnPropertyChanged("NextMap");
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
        public Map_timer_M(string current, string next, string time)
        {
            CurrentMap = current;
            NextMap = next;
            TimeToNextMap = time;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
