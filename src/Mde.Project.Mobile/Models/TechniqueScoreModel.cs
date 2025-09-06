using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mde.Project.Mobile.Models
{
    public class TechniqueScoreModel : INotifyPropertyChanged
    {
        private string _technique = string.Empty;
        public string Technique
        {
            get => _technique;
            set { _technique = value; OnPropertyChanged(); }
        }

        private int _scoreCount;
        public int ScoreCount
        {
            get => _scoreCount;
            set
            {
                if (_scoreCount == value) return;
                _scoreCount = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
