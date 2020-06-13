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
    public partial class AddEyepieceWindow : Window
    {
        private static string m_defaultHelptext = "Define an eyepiece.";
        private Brush m_saveBrushTextBoxBorder = null;

        private bool m_editMode = false;
        public bool EditMode
        {
            get { return m_editMode; }
            set { m_editMode = value; }
        }

        private string m_eyepieceName;
        public string EyepieceName
        {
            get { return m_eyepieceName; }
            set { m_eyepieceName = value; }
        }

        private int m_focalLength;
        public int FocalLength
        {
            get { return m_focalLength; }
            set { m_focalLength = value; }
        }

        private double m_fov;
        public double FieldOfView
        {
            get { return m_fov; }
            set { m_fov = value; }
        }

        public AddEyepieceWindow()
        {
            InitializeComponent();

            m_saveBrushTextBoxBorder = textBoxEyepieceName.BorderBrush;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm())
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

        private bool ValidateForm()
        {
            if (ValidateEyepieceName() && ValidateFocalLength() && ValidateFoV())
                return true;
            else
                return false;
        }

        private bool ValidateEyepieceName()
        {
            if (string.IsNullOrEmpty(textBoxEyepieceName.Text))
            {
                textBoxEyepieceName.BorderBrush = Brushes.Red;
                textBoxEyepieceName.ToolTip = "Eyepiece Name cannot be empty";

                return false;
            }
            if (textBoxEyepieceName.Text.Length > 50)
            {
                textBoxEyepieceName.BorderBrush = Brushes.Red;
                textBoxEyepieceName.ToolTip = "Maximum number of characters for Eyepiece Name is 50";

                return false;
            }
            else
            {
                this.EyepieceName = textBoxEyepieceName.Text.Trim(); ;
                textBoxEyepieceName.Text = this.EyepieceName;
                textBoxEyepieceName.BorderBrush = m_saveBrushTextBoxBorder;
                textBoxEyepieceName.ToolTip = "Data is valid";
                return true;
            }
        }

        private bool ValidateFoV()
        {
            double val;
            if (!double.TryParse(textBoxFoV.Text, out val))
            {
                textBoxFoV.BorderBrush = Brushes.Red;
                textBoxFoV.ToolTip = "Data must be numeric";
                return false;
            }
            else
            {
                this.FieldOfView = val;
                textBoxFoV.Text = val.ToString();
                textBoxFoV.BorderBrush = m_saveBrushTextBoxBorder;
                textBoxFoV.ToolTip = "Data is valid";
                return true;
            }
        }

        private bool ValidateFocalLength()
        {
            int val;
            if (!int.TryParse(textBoxFocalLength.Text, out val))
            {
                textBoxFocalLength.BorderBrush = Brushes.Red;
                textBoxFocalLength.ToolTip = "Data must be numeric and a whole number (integer)";
                return false;
            }
            else
            {
                this.FocalLength = val;
                textBoxFocalLength.Text = val.ToString();
                textBoxFocalLength.BorderBrush = m_saveBrushTextBoxBorder;
                textBoxFocalLength.ToolTip = "Data is valid";
                return true;
            }
        }

        private void textBoxEyepieceName_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateEyepieceName();
            textBoxHelp.Text = m_defaultHelptext;
        }

        private void textBoxFoV_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateFoV();
            textBoxHelp.Text = m_defaultHelptext;
        }

        private void textBoxFocalLength_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateFocalLength();
            textBoxHelp.Text = m_defaultHelptext;
        }

        private void textBoxEyepieceName_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxHelp.Text = "Add a descriptive name for the new eyepiece. Example: Meade Series 5000 26mm";
        }

        private void textBoxFocalLength_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxHelp.Text = "Define the focal length for the eyepiece in mm. Example: 26";
        }

        private void textBoxFoV_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxHelp.Text = "Provide the manufactures stated field of view for the eyepiece in degrees. Example: 60";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Do we need to populate our controls with preset values (i.e. we're editing rather than adding a scope)?
            if (!string.IsNullOrEmpty(EyepieceName))
                textBoxEyepieceName.Text = EyepieceName;

            if (FocalLength != 0)
                textBoxFocalLength.Text = FocalLength.ToString();

            if (FieldOfView != 0)
                textBoxFoV.Text = FieldOfView.ToString();

            if (EditMode)
            {
                this.Title = "Edit Eyepiece";
                textBoxEyepieceName.IsEnabled = false;  // Can't edit the eyepiece name because it's the primary key
            }
        }
    }
}
