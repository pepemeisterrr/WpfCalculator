using System;
using System.Globalization;

namespace WpfCalculator.Models
{
    public class CalculationCompletedEventArgs : EventArgs
    {
        public string Expression { get; }
        public double Result { get; }

        public CalculationCompletedEventArgs(string expression, double result)
        {
            Expression = expression;
            Result = result;
        }
    }

    public class CalculatorModel
    {
        private string _display = "0";
        private double _accumulator = 0;
        private string _pendingOperator = "";
        private bool _startNewNumber = true;
        private bool _hasDecimalPoint = false;
        private bool _hasError = false;

        private string _firstOperandString = "";
        private string _secondOperandString = "";

        public string Display => _display;
        public bool HasError => _hasError;
        public string LastExpression { get; private set; } = "";

        public event EventHandler<string>? DisplayChanged;
        public event EventHandler<CalculationCompletedEventArgs>? SuccessfulCalculation;

        private void OnDisplayChanged() => DisplayChanged?.Invoke(this, _display);

        private double GetCurrentValue()
        {
            string normalized = _display.Replace(",", ".");
            return double.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out double val) ? val : 0;
        }

        private string FormatNumber(double value)
        {
            return value.ToString("G15", CultureInfo.InvariantCulture).Replace(".", ",");
        }

        private double PerformCalculation(double a, string op, double b)
        {
            return op switch
            {
                "+" => a + b,
                "−" or "-" => a - b,
                "×" or "*" => a * b,
                "÷" or "/" => b == 0 ? throw new DivideByZeroException() : a / b,
                _ => b
            };
        }

        private void SetError()
        {
            _display = "Error";
            _hasError = true;
            _pendingOperator = "";
            OnDisplayChanged();
        }

        public void Clear()
        {
            _display = "0";
            _accumulator = 0;
            _pendingOperator = "";
            _startNewNumber = true;
            _hasDecimalPoint = false;
            _hasError = false;
            _firstOperandString = "";
            _secondOperandString = "";
            LastExpression = "";
            OnDisplayChanged();
        }

        public void InputDigit(string digit)
        {
            if (_hasError)
                Clear();

            if (_startNewNumber)
            {  
                _display = (digit == "0") ? "0" : digit;
                _startNewNumber = false;
                _hasDecimalPoint = false;
            }
            else
            {
                if (_display == "0" && !_hasDecimalPoint)
                {
                   
                    _display = digit;
                }
                else if (_display.Length < 16)
                {
                    _display += digit;
                }
            }

            OnDisplayChanged();
        }

        public void InputDecimal()
        {
            if (_hasError) Clear();

            if (_startNewNumber)
            {
                _display = "0,";
                _startNewNumber = false;
                _hasDecimalPoint = true;
            }
            else if (!_hasDecimalPoint)
            {
                _display += ",";
                _hasDecimalPoint = true;
            }
            OnDisplayChanged();
        }

        public void SetOperator(string op)
        {
            if (_hasError) { Clear(); return; }

            if (!_startNewNumber)
            {
                double currentVal = GetCurrentValue();

                if (!string.IsNullOrEmpty(_pendingOperator))
                {
                    try
                    {
                        double res = PerformCalculation(_accumulator, _pendingOperator, currentVal);
                        _display = FormatNumber(res);
                        _accumulator = res;
                        _firstOperandString = _display;
                    }
                    catch (DivideByZeroException)
                    {
                        SetError();
                        return;
                    }
                }
                else
                {
                    _accumulator = currentVal;
                    _firstOperandString = _display;
                }
            }
            else if (string.IsNullOrEmpty(_pendingOperator))
            {
                _firstOperandString = _display;
                _accumulator = GetCurrentValue();
            }

            _pendingOperator = op;
            _startNewNumber = true;
            _hasDecimalPoint = false;
            OnDisplayChanged();
        }

        public void CalculateResult()
        {
            if (_hasError || string.IsNullOrEmpty(_pendingOperator) || _startNewNumber)
                return;

            _secondOperandString = _display;
            double secondVal = GetCurrentValue();

            try
            {
                double result = PerformCalculation(_accumulator, _pendingOperator, secondVal);
                string expression = $"{_firstOperandString} {_pendingOperator} {_secondOperandString}";

                LastExpression = expression;
                _display = FormatNumber(result);
                _accumulator = result;
                _pendingOperator = "";
                _startNewNumber = true;
                _hasDecimalPoint = _display.Contains(",");

                SuccessfulCalculation?.Invoke(this, new CalculationCompletedEventArgs(expression, result));
            }
            catch (DivideByZeroException)
            {
                SetError();
            }

            OnDisplayChanged();
        }

        public void ToggleSign()
        {
            if (_hasError || _display == "0") return;

            if (_display.StartsWith("-"))
                _display = _display.Substring(1);
            else
                _display = "-" + _display;

            OnDisplayChanged();
        }

        public void ApplyPercent()
        {
            if (_hasError) return;

            double value = GetCurrentValue();
            value /= 100;
            _display = FormatNumber(value);
            _hasDecimalPoint = _display.Contains(",");
            OnDisplayChanged();
        }
    }
}