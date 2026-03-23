using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace databzae_hry
{
    public class hry : INotifyPropertyChanged
    {
        private string _jmeno = "";
        private string _autor = "";
        private int _rokVydani;
        private string _zanr = "";
        private bool _instalovana;

        public string Jmeno
        {
            get => _jmeno;
            set { if (_jmeno != value) { _jmeno = value; OnPropertyChanged(nameof(Jmeno)); } }
        }

        public string Autor
        {
            get => _autor;
            set { if (_autor != value) { _autor = value; OnPropertyChanged(nameof(Autor)); } }
        }

        public int RokVydani
        {
            get => _rokVydani;
            set { if (_rokVydani != value) { _rokVydani = value; OnPropertyChanged(nameof(RokVydani)); } }
        }

        public string Zanr
        {
            get => _zanr;
            set { if (_zanr != value) { _zanr = value; OnPropertyChanged(nameof(Zanr)); } }
        }

        public bool Instalovana
        {
            get => _instalovana;
            set { if (_instalovana != value) { _instalovana = value; OnPropertyChanged(nameof(Instalovana)); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
