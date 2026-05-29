using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfCalculator.Commands;
using WpfCalculator.Models;
using WpfCalculator.Services;

namespace WpfCalculator.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly CalculatorModel _model = new();

        public string Display => _model.Display;

        public ICommand NumberCommand { get; }
        public ICommand OperatorCommand { get; }
        public ICommand EqualsCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand SignCommand { get; }
        public ICommand PercentCommand { get; }
        public ICommand DecimalCommand { get; }

        public MainViewModel()
        {
            _model.DisplayChanged += (_, _) => OnPropertyChanged(nameof(Display));
            _model.SuccessfulCalculation += (_, e) =>
            {
                HistoryLogger.Log(e.Expression, e.Result, DateTime.Now);
            };

            NumberCommand = new RelayCommand(p => { if (p is string d) _model.InputDigit(d); });
            OperatorCommand = new RelayCommand(p => { if (p is string op) _model.SetOperator(op); });
            EqualsCommand = new RelayCommand(_ => _model.CalculateResult());
            ClearCommand = new RelayCommand(_ => _model.Clear());
            SignCommand = new RelayCommand(_ => _model.ToggleSign());
            PercentCommand = new RelayCommand(_ => _model.ApplyPercent());
            DecimalCommand = new RelayCommand(_ => _model.InputDecimal());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}