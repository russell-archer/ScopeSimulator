using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScopeSimulator.Windows
{
    /// <summary>
    /// Interaction logic for AddFocalReducerWindow.xaml
    /// </summary>
    public partial class AddFocalReducerWindow : Window
    {
        private double m_value = 0;
        public double Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        private Brush m_saveBrushTextBoxBorder = null;

        public AddFocalReducerWindow()
        {
            InitializeComponent();

            m_saveBrushTextBoxBorder = textBoxNewValue.BorderBrush;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateTextBox(textBoxNewValue))
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("The form contains one or more invalid values.", "Data validation errors");
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void textBoxNewValue_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateTextBox(textBoxNewValue);
        }

        private bool ValidateTextBox(TextBox textbox)
        {
            double val;
            if (!double.TryParse(textbox.Text, out val))
            {
                textBoxNewValue.BorderBrush = Brushes.Red;
                textBoxNewValue.ToolTip = "Data must be numeric";

                return false;
            }
            else
            {
                this.Value = val;
                textbox.Text = val.ToString();
                textBoxNewValue.BorderBrush = m_saveBrushTextBoxBorder;
                textBoxNewValue.ToolTip = "Data is valid";
                return true;
            }
        }
    }
}
