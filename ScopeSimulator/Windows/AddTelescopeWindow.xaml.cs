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
    public partial class AddTelescopeWindow : Window
    {
        private static string m_defaultHelptext = "Define the parameters of a telescope.";
        private Brush m_saveBrushTextBoxBorder = null;

        private bool m_editMode = false;
        public bool EditMode
        {
            get { return m_editMode; }
            set { m_editMode = value; }
        }
        
        private string m_telescopeName;
        public string TelescopeName
        {
            get { return m_telescopeName; }
            set { m_telescopeName = value; }
        }

        private int m_aperture;
        public int Aperture
        {
            get { return m_aperture; }
            set { m_aperture = value; }
        }

        private int m_focalLength;
        public int FocalLength
        {
            get { return m_focalLength; }
            set { m_focalLength = value; }
        }

        public AddTelescopeWindow()
        {
            InitializeComponent();

            m_saveBrushTextBoxBorder = textBoxTelescopeName.BorderBrush;
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
            if (ValidateTelescopeName() && ValidateAperture() && ValidateFocalLength())
                return true;
            else
                return false;
        }

        private bool ValidateTelescopeName()
        {
            if (string.IsNullOrEmpty(textBoxTelescopeName.Text))
            {
                textBoxTelescopeName.BorderBrush = Brushes.Red;
                textBoxTelescopeName.ToolTip = "Telescope Name cannot be empty";

                return false;
            }
            if (textBoxTelescopeName.Text.Length > 50)
            {
                textBoxTelescopeName.BorderBrush = Brushes.Red;
                textBoxTelescopeName.ToolTip = "Maximum number of characters for Telescope Name is 50";

                return false;
            }
            else
            {
                this.TelescopeName = textBoxTelescopeName.Text.Trim(); ;
                textBoxTelescopeName.Text = this.TelescopeName;
                textBoxTelescopeName.BorderBrush = m_saveBrushTextBoxBorder;
                textBoxTelescopeName.ToolTip = "Data is valid";
                return true;
            }
        }

        private bool ValidateAperture()
        {
            int val;
            if (!int.TryParse(textBoxAperture.Text, out val))
            {
                textBoxAperture.BorderBrush = Brushes.Red;
                textBoxAperture.ToolTip = "Data must be numeric and a whole number (integer)";
                return false;
            }
            else
            {
                this.Aperture = val;
                textBoxAperture.Text = val.ToString();
                textBoxAperture.BorderBrush = m_saveBrushTextBoxBorder;
                textBoxAperture.ToolTip = "Data is valid";
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

        private void textBoxTelescopeName_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateTelescopeName();
            textBoxHelp.Text = m_defaultHelptext;
        }

        private void textBoxAperture_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateAperture();
            textBoxHelp.Text = m_defaultHelptext;
        }

        private void textBoxFocalLength_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateFocalLength();
            textBoxHelp.Text = m_defaultHelptext;
        }

        private void textBoxTelescopeName_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxHelp.Text = "Add a descriptive name for the new telescope. Example: Meade ABC";
        }


        private void textBoxFocalLength_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxHelp.Text = "Define the focal length for the telescope in mm. Example: 2005";
        }

        private void textBoxAperture_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxHelp.Text = "Define the aperture for the telescope in mm. Example: 205";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Do we need to populate our controls with preset values (i.e. we're editing rather than adding a scope)?
            if (!string.IsNullOrEmpty(TelescopeName))
                textBoxTelescopeName.Text = TelescopeName;

            if (Aperture != 0)
                textBoxAperture.Text = Aperture.ToString();

            if (FocalLength != 0)
                textBoxFocalLength.Text = FocalLength.ToString();

            if (EditMode)
            {
                this.Title = "Edit Telescope";
                textBoxTelescopeName.IsEnabled = false;  // Can't edit the scope name because it's the primary key
            }
        }
    }
}
