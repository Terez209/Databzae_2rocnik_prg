using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace databzae_hry
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<hry> Games { get; } = new ObservableCollection<hry>();
        public ICollectionView GamesView { get; }

        private hry? _selectedHra;
        public hry? SelectedHra
        {
            get => _selectedHra;
            set { if (_selectedHra != value) { _selectedHra = value; OnPropertyChanged(nameof(SelectedHra)); } }
        }

        public string NewJmeno { get => _newJmeno; set { _newJmeno = value; OnPropertyChanged(nameof(NewJmeno)); } }
        public string NewAutor { get => _newAutor; set { _newAutor = value; OnPropertyChanged(nameof(NewAutor)); } }
        public string NewRok { get => _newRok; set { _newRok = value; OnPropertyChanged(nameof(NewRok)); } }
        public string NewZanr { get => _newZanr; set { _newZanr = value; OnPropertyChanged(nameof(NewZanr)); } }
        public bool NewInstalovana { get => _newInstalovana; set { _newInstalovana = value; OnPropertyChanged(nameof(NewInstalovana)); } }

        private string _newJmeno = "";
        private string _newAutor = "";
        private string _newRok = "";
        private string _newZanr = "";
        private bool _newInstalovana = false;

        private string _filterText = "";
        public string FilterText
        {
            get => _filterText;
            set { if (_filterText != value) { _filterText = value; OnPropertyChanged(nameof(FilterText)); GamesView.Refresh(); } }
        }

        private bool _isEditing = false;
        private hry? _editingTarget = null;

        // Pøíkazy
        public ICommand AddCommand { get; }
        public ICommand EditSelectedCommand { get; }
        public ICommand DeleteSelectedCommand { get; }
        public ICommand SaveEditCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand ClearFilterCommand { get; }
        public ICommand OpenAddDialogCommand { get; }

        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public MainViewModel()
        {

            GamesView = CollectionViewSource.GetDefaultView(Games);
            GamesView.Filter = obj =>
            {
                if (obj is hry h)
                {
                    if (string.IsNullOrWhiteSpace(FilterText)) return true;
                    return h.Jmeno?.IndexOf(FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0;
                }
                return false;
            };

            AddCommand = new RelayCommand(_ => AddHru());
            EditSelectedCommand = new RelayCommand(_ => BeginEdit(SelectedHra), _ => SelectedHra != null);
            DeleteSelectedCommand = new RelayCommand(_ => DeleteHru(SelectedHra), _ => SelectedHra != null);
            SaveEditCommand = new RelayCommand(_ => SaveEdit(), _ => _isEditing);
            CancelEditCommand = new RelayCommand(_ => CancelEdit(), _ => _isEditing);
            ClearFilterCommand = new RelayCommand(_ => { FilterText = ""; });
            OpenAddDialogCommand = new RelayCommand(_ => PrepareNew());

            EditCommand = new RelayCommand(p => BeginEdit(p as hry));
            DeleteCommand = new RelayCommand(p => DeleteHru(p as hry));
        }

        private void PrepareNew()
        {
            _isEditing = false;
            _editingTarget = null;
            NewJmeno = "";
            NewAutor = "";
            NewRok = "";
            NewZanr = "";
            NewInstalovana = false;
        }

        private void AddHru()
        {
            if (!ValidateNew(out int rok))
                return;

            var h = new hry
            {
                Jmeno = NewJmeno.Trim(),
                Autor = NewAutor.Trim(),
                RokVydani = rok,
                Zanr = NewZanr?.Trim() ?? "",
                Instalovana = NewInstalovana
            };

            Games.Add(h);
            PrepareNew();
        }

        private void BeginEdit(hry? target)
        {
            if (target == null) return;
            _isEditing = true;
            _editingTarget = target;

            NewJmeno = target.Jmeno;
            NewAutor = target.Autor;
            NewRok = target.RokVydani.ToString();
            NewZanr = target.Zanr;
            NewInstalovana = target.Instalovana;
        }

        private void SaveEdit()
        {
            if (_editingTarget == null) return;
            if (!ValidateNew(out int rok))
                return;

            _editingTarget.Jmeno = NewJmeno.Trim();
            _editingTarget.Autor = NewAutor.Trim();
            _editingTarget.RokVydani = rok;
            _editingTarget.Zanr = NewZanr?.Trim() ?? "";
            _editingTarget.Instalovana = NewInstalovana;

            _isEditing = false;
            _editingTarget = null;
            PrepareNew();
        }

        private void CancelEdit()
        {
            _isEditing = false;
            _editingTarget = null;
            PrepareNew();
        }

        private void DeleteHru(hry? target)
        {
            if (target == null) return;
            var res = MessageBox.Show($"Opravdu chcete smazat hru '{target.Jmeno}'?", "Potvrzení smazání", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
            {
                Games.Remove(target);
                if (SelectedHra == target) SelectedHra = null;
            }
        }

        private bool ValidateNew(out int parsedRok)
        {
            parsedRok = 0;
            if (string.IsNullOrWhiteSpace(NewJmeno))
            {
                MessageBox.Show("Název hry nesmí být prázdný.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(NewAutor))
            {
                MessageBox.Show("Autor nesmí být prázdný.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!int.TryParse(NewRok, out parsedRok) || parsedRok < 1950 || parsedRok > DateTime.Now.Year + 1)
            {
                MessageBox.Show("Zadejte platný rok vydání.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => _execute(parameter);
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}