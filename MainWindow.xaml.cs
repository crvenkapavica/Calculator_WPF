using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;


/// <summary>
/// The app closely resembles Windows calculator in the way it works.
/// The main difference is that third-order operations are being dispalyed in a single pair of brackets with updated operands, instead of needlessly spanning across to infinity.
/// Limits are standard 64-bit floating point values; scientific figures are not implemented for this simple example.
/// The buffer only remembers the last lower-order operation, as more is never neede since there are no brackets.
/// The project could be modified to support bigger and longer streams of oprations with brackets in a similar manner that is used here.
/// </summary>

namespace Zadatak
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Calculator calculator = new Calculator() { Result = 0, Buffer = 0, BufferOperation = "", EqualBuffer = 0, Operation = "", SpecialOperation = "" };

        //Used to clear the display before entering a digit, after entering an operation
        bool bClearDisplay = true;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = calculator;
        }

        private void Button0_Click(object sender, RoutedEventArgs e)
        {
            UpdateDisplay("0");
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            UpdateDisplay("1");
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            UpdateDisplay("2");
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            UpdateDisplay("3");
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            UpdateDisplay("4");
        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            UpdateDisplay("5");
        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            UpdateDisplay("6");
        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            UpdateDisplay("7");
        }

        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            UpdateDisplay("8");
        }

        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            UpdateDisplay("9");
        }

        private void ButtonDecimal_Click(object sender, RoutedEventArgs e)
        {
            UpdateDisplay(".");
        }

        private void ButtonSign_Click(object sender, RoutedEventArgs e)
        {
            if (calculator.Display == "0") return;

            if (calculator.Display[0] != '-')
            {
                UpdateDisplay(true, '-' + calculator.Display);
            }
            else
            {
                UpdateDisplay(true, calculator.Display.Remove(0, 1));
            }

            if (calculator.Operation == "=")
            {
                calculator.Result *= -1;
            }
        }

        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            HandleFirstDegreeOperation("+");
        }

        private void ButtonMinus_Click(object sender, RoutedEventArgs e)
        {
            HandleFirstDegreeOperation("-");
        }

        private void ButtonMultiply_Click(object sender, RoutedEventArgs e)
        {
            HandleSecondDegreeOperation("x");
        }

        private void ButtonDivide_Click(object sender, RoutedEventArgs e)
        {
            HandleSecondDegreeOperation("/");
        }

        private void ButtonBackspace_Click(object sender, RoutedEventArgs e)
        {
            if (calculator.Display != "0")
            {
                if (calculator.Display.Length == 1)
                {
                    UpdateDisplay(true, "0");
                    bClearDisplay = true;
                }
                else
                {
                    UpdateDisplay(true, calculator.Display.Remove(calculator.Display.Length - 1));
                }
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            UpdateDisplay(true, "0");
            calculator.Input = calculator.Operation = calculator.BufferOperation = calculator.SpecialOperation = "";
            calculator.Result = calculator.Buffer = calculator.EqualBuffer = 0;
            bClearDisplay = true;
        }

        private void ButtonClearEntry_Click(object sender, RoutedEventArgs e)
        {
            UpdateDisplay(true, "0");
            bClearDisplay = true;
        }

        private void ButtonPercentage_Click(object sender, RoutedEventArgs e)
        {
            if (calculator.Display == "0")
            {
                bClearDisplay = true;
                return;
            }

            if (calculator.Result == 0)
            {
                UpdateInput("0");
                bClearDisplay = true;
            }
            decimal Percent = calculator.Result / 100 * decimal.Parse(calculator.Display);
            UpdateDisplay(true, Percent.ToString());
        }

        private void ButtonInverse_Click(object sender, RoutedEventArgs e)
        {
            decimal Operand = decimal.Parse(calculator.Display);
            decimal Inverse = 1 / Operand;
            UpdateDisplay(true, Inverse.ToString());
            bClearDisplay = true;

            UpdateInputWithSpecialOperation("1/", Operand);
        }

        private void ButtonSquare_Click(object sender, RoutedEventArgs e)
        {
            decimal Operand = decimal.Parse(calculator.Display);
            decimal Square = Operand * Operand;
            UpdateDisplay(true, Square.ToString());
            bClearDisplay = true;

            UpdateInputWithSpecialOperation("SQR", Operand);
        }

        private void ButtonSquareRoot_Click(object sender, RoutedEventArgs e)
        {
            decimal Operand = decimal.Parse(calculator.Display);
            decimal SquareRoot = Convert.ToDecimal(Math.Sqrt(double.Parse(calculator.Display)));
            UpdateDisplay(true, SquareRoot.ToString());
            bClearDisplay = true;

            UpdateInputWithSpecialOperation("√", Operand);
        }

        private void ButtonEquals_Click(object sender, RoutedEventArgs e)
        {
            if (calculator.EqualBuffer == 0)
            {
                calculator.EqualBuffer = decimal.Parse(calculator.Display);
            }

            if (calculator.Input == "" || calculator.Operation == "")
            {
                ClearInput();
                UpdateInput(calculator.Display + " =");
                calculator.EqualBuffer = 0;
                bClearDisplay = true;
            }
            else if (calculator.BufferOperation == "=")
            {
                ClearInput();
                UpdateInput(calculator.Display + " " + calculator.Operation + " " + calculator.EqualBuffer + " =");
                calculator.Calculate(decimal.Parse(calculator.EqualBuffer.ToString()));
            }
            else
            {
                UpdateInput(calculator.Display + " =");
                calculator.Calculate(decimal.Parse(calculator.Display));
                MergeWithBuffer();
            }

            calculator.BufferOperation = "=";
        }

        private void ClearInput()
        {
            calculator.Input = "";
        }

        private void ClearBuffer()
        {
            calculator.Buffer = 0;
            calculator.BufferOperation = "";
        }

        private void UpdateInput(string Value)
        {
            calculator.Input += " " + Value;
        }

        //Update Input TextBox with Display data before calculating
        private void UpdateInputBeforeOperation(string Operation)
        {
            if (calculator.SpecialOperation == "")
            {
                UpdateInput(calculator.Display + " " + Operation);
            }
            else
            {
                UpdateInput(Operation);
            }
        }

        //Used in conjuction with UpdateInputWithSpecialOperation
        private void UpdateInputWithOperand(decimal Operand)
        {
            string updatedInput = "";

            for (int i = calculator.Input.Length - 1; i >= 0; i--)
            {
                if (calculator.Input[i] == '(')
                {
                    updatedInput = calculator.Input.Substring(0, i + 1);
                    break;
                }
            }

            calculator.Input = updatedInput + Operand + ") ";
        }

        //Used for updating Input with Special Operations (SQR, SQRT, Inverse) to avoid needless infinite stacking if being repeated
        private void UpdateInputWithSpecialOperation(string Operation, decimal Operand)
        {
            if (calculator.SpecialOperation == Operation)
            {
                UpdateInputWithOperand(Operand);
            }
            else
            {
                UpdateInput(Operation + "(" + Operand + ") ");
            }
            calculator.SpecialOperation = Operation;
        }
        
        //Update Display without clearing it
        private void UpdateDisplay(string Value)
        {
            calculator.Display = bClearDisplay ? "" + Value : calculator.Display + Value;
            bClearDisplay = false;
        }

        //Update Display after clearing it
        private void UpdateDisplay(bool bClear, string Value)
        {
            bClearDisplay = bClear;
            UpdateDisplay(Value);
        }

        //Handle addition and substraction
        private void HandleFirstDegreeOperation(string Operation)
        {
            UpdateInputBeforeOperation(Operation);

            //If we're repeatedly using =
            if (calculator.BufferOperation == "=")
            {
                calculator.Operation = "=";
                ClearBuffer();
            }
            
            if (calculator.Operation == "" || calculator.Operation == "=")
            {
                calculator.Result = decimal.Parse(calculator.Display);
            }
            //if last operation was of the same order, just calculate
            else if (calculator.Operation == "+" || calculator.Operation == "-")
            {
                calculator.Calculate(decimal.Parse(calculator.Display));
            }
            //if last operation was of the higher order, merge buffer
            else if (calculator.Operation == "x" || calculator.Operation == "/")
            {
                calculator.Calculate(decimal.Parse(calculator.Display));
                MergeWithBuffer();
            }

            calculator.Operation = Operation;
            calculator.SpecialOperation = "";
            bClearDisplay = true;
        }

        //Handle multiplication and division
        private void HandleSecondDegreeOperation(string Operation)
        {
            UpdateInputBeforeOperation(Operation);

            //keep calculating if we're spaming = 
            if (calculator.BufferOperation == "=")
            {
                calculator.Operation = "=";
                ClearBuffer();
            }

            //if its just = and one digit
            if (calculator.Operation == "")
            {
                calculator.Result = decimal.Parse(calculator.Display);
            }
            //if the last operation was lower-order, add digits before the operation in buffer
            else if (calculator.Operation == "+" || calculator.Operation == "-")
            {
                calculator.Buffer = calculator.Result;
                calculator.BufferOperation = calculator.Operation;
                calculator.Result = decimal.Parse(calculator.Display);
            }
            //if it was the same order, just calculate
            else if (calculator.Operation == "x" || calculator.Operation == "/")
            {
                calculator.Calculate(decimal.Parse(calculator.Display));
            }

            calculator.Operation = Operation;
            calculator.SpecialOperation = "";
            bClearDisplay = true;
        }

        //Merge the current Result and Display with the Buffer, which stores digits to the left side of lower-order operations for calculation after higher-order operations have been handled
        private void MergeWithBuffer()
        {
            if (calculator.Buffer != 0)
            {
                calculator.Result = calculator.BufferOperation == "-" ? calculator.Buffer - calculator.Result : calculator.Result + calculator.Buffer;
                calculator.Display = calculator.Result.ToString();
                ClearBuffer();
            }
        }
    }

    public class Calculator : INotifyPropertyChanged
    {
        //Implement PropertyChanged Event Handler for updating the UI automatically
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotiftyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        
        //Stores all the input, which is databound with the TextBoxInput.Text Property (Top Display)
        private string input = "";

        public string Input {
            get { return input; }
            set {
                input = value;
                this.NotiftyPropertyChanged("Input");
            }
        }

        //Shows current digit being entered or calculated upon
        private string display = "0";

        public string Display {
            get { return display; }
            set {
                display = value;
                this.NotiftyPropertyChanged("Display");
            }
        }

        //Stores the decimal value of the Display, doesn't neccessarily match Display value at all times, since it is used as a temp buffer in some cases
        public decimal Result { get; set; }

        //Buffer for storing Digits that are being operated with after higher-order operations
        public decimal Buffer { get; set; }

        //The next operation to take place on the buffered digit(s)
        public string BufferOperation { get; set; }

        //Buffer for equality operation, this is to match the functionality of Windows calculator of being able to spam the equals button
        public decimal EqualBuffer { get; set; }

        //Current(last) operation
        public string Operation { get; set; }

        //SQR, SQRT, Inverse; needs to be seperated to match the functionality of Windows calculator
        public string SpecialOperation { get; set; }

        //When this is called, Result and Display are equal
        public void Calculate(decimal Operand)
        {
            switch (Operation)
            {
                case "+":
                    Result += Operand;
                    break;
                case "-":
                    Result -= Operand;
                    break;
                case "x":
                    Result *= Operand;
                    break;
                case "/":
                    Result /= Operand;
                    break;
            }
            Display = Result.ToString();
        }
    }
}
