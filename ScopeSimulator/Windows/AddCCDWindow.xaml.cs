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
    public partial class AddCCDWindow : Window
    {
        private static string m_defaultHelptext = "Add a new CCD.";
        private Brush m_saveBrushTextBoxBorder = null;

        private bool m_editMode = false;
        public bool EditMode
        {
            get { return m_editMode; }
            set { m_editMode = value; }
        }

        private string m_ccdName;
        public string CCDName
        {
            get { return m_ccdName; }
            set { m_ccdName = value; }
        }

        private int m_nPixelsWidth;
        public int NPixelsWidth
        {
            get { return m_nPixelsWidth; }
            set { m_nPixelsWidth = value; }
        }

        private int m_nPixelsHeight;
        public int NPixelsHeight
        {
            get { return m_nPixelsHeight; }
            set { m_nPixelsHeight = value; }
        }

        private double m_pixelSizeWidth;
        public double PixelSizeWidth
        {
            get { return m_pixelSizeWidth; }
            set { m_pixelSizeWidth = value; }
        }

        private double m_pixelSizeHeight;
        public double PixelSizeHeight
        {
            get { return m_pixelSizeHeight; }
            set { m_pixelSizeHeight = value; }
        }

        public AddCCDWindow()
        {
            InitializeComponent();

            m_saveBrushTextBoxBorder = textBoxCCDName.BorderBrush;
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
            if (ValidateCCDName() && ValidateNPixelsWidth() && ValidateNPixelsHeight() && ValidatePixelSizeWidth() && ValidatePixelSizeHeight())
                return true;
            else
                return false;
        }

        private bool ValidateCCDName()
        {
            if (string.IsNullOrEmpty(textBoxCCDName.Text))
            {
                textBoxCCDName.BorderBrush = Brushes.Red;
                textBoxCCDName.ToolTip = "CCD Name cannot be empty";

                return false;
            }
            if (textBoxCCDName.Text.Length > 50)
            {
                textBoxCCDName.BorderBrush = Brushes.Red;
                textBoxCCDName.ToolTip = "Maximum number of characters for CCD Name is 50";

                return false;
            }
            else
            {
                this.CCDName = textBoxCCDName.Text.Trim(); ;
                textBoxCCDName.Text = this.CCDName;
                textBoxCCDName.BorderBrush = m_saveBrushTextBoxBorder;
                textBoxCCDName.ToolTip = "Data is valid";
                return true;
            }
        }

        private bool ValidateNPixelsWidth()
        {
            int val;
            if (!int.TryParse(textBoxNPixelsWidth.Text, out val))
            {
                textBoxNPixelsWidth.BorderBrush = Brushes.Red;
                textBoxNPixelsWidth.ToolTip = "Data must be numeric and a whole number (integer)";
                return false;
            }
            else
            {
                this.NPixelsWidth = val;
                textBoxNPixelsWidth.Text = val.ToString();
                textBoxNPixelsWidth.BorderBrush = m_saveBrushTextBoxBorder;
                textBoxNPixelsWidth.ToolTip = "Data is valid";
                return true;
            }
        }

        private bool ValidateNPixelsHeight()
        {
            int val;
            if (!int.TryParse(textBoxNPixelsHeight.Text, out val))
            {
                textBoxNPixelsHeight.BorderBrush = Brushes.Red;
                textBoxNPixelsHeight.ToolTip = "Data must be numeric and a whole number (integer)";
                return false;
            }
            else
            {
                this.NPixelsHeight = val;
                textBoxNPixelsHeight.Text = val.ToString();
                textBoxNPixelsHeight.BorderBrush = m_saveBrushTextBoxBorder;
                textBoxNPixelsHeight.ToolTip = "Data is valid";
                return true;
            }
        }

        private bool ValidatePixelSizeWidth()
        {
            double val;
            if (!double.TryParse(textBoxPixelSizeWidth.Text, out val))
            {
                textBoxPixelSizeWidth.BorderBrush = Brushes.Red;
                textBoxPixelSizeWidth.ToolTip = "Data must be numeric";
                return false;
            }
            else
            {
                this.PixelSizeWidth = val;
                textBoxPixelSizeWidth.Text = val.ToString();
                textBoxPixelSizeWidth.BorderBrush = m_saveBrushTextBoxBorder;
                textBoxPixelSizeWidth.ToolTip = "Data is valid";
                return true;
            }
        }

        private bool ValidatePixelSizeHeight()
        {
            double val;
            if (!double.TryParse(textBoxPixelSizeHeight.Text, out val))
            {
                textBoxPixelSizeHeight.BorderBrush = Brushes.Red;
                textBoxPixelSizeHeight.ToolTip = "Data must be numeric";
                return false;
            }
            else
            {
                this.PixelSizeHeight = val;
                textBoxPixelSizeHeight.Text = val.ToString();
                textBoxPixelSizeHeight.BorderBrush = m_saveBrushTextBoxBorder;
                textBoxPixelSizeHeight.ToolTip = "Data is valid";
                return true;
            }
        }

        private void textBoxCCDName_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateCCDName();
            textBoxHelp.Text = m_defaultHelptext;
        }

        private void textBoxNPixelsWidth_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateNPixelsWidth();
            textBoxHelp.Text = m_defaultHelptext;
        }

        private void textBoxNPixelsHeight_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateNPixelsHeight();
            textBoxHelp.Text = m_defaultHelptext;
        }

        private void textBoxPixelSizeWidth_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidatePixelSizeWidth();
            textBoxHelp.Text = m_defaultHelptext;
        }

        private void textBoxPixelSizeHeight_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidatePixelSizeHeight();
            textBoxHelp.Text = m_defaultHelptext;
        }

        private void textBoxCCDName_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxHelp.Text = "Add a descriptive name for the new CCD. Example: Starlight Xpress H16";
        }

        private void textBoxNPixelsWidth_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxHelp.Text = "Define the number of CCD pixels (width). Example: 1340";
        }

        private void textBoxNPixelsHeight_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxHelp.Text = "Define the number of CCD pixels (height). Example: 1340";
        }

        private void textBoxPixelSizeWidth_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxHelp.Text = "Define the CCD pixel size (width) in microns. Example: 6.45";
        }

        private void textBoxPixelSizeHeight_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxHelp.Text = "Define the CCD pixel size (height) in microns. Example: 6.45";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Do we need to populate our controls with preset values (i.e. we're editing rather than adding a scope)?
            if (!string.IsNullOrEmpty(CCDName))
                textBoxCCDName.Text = CCDName;

            if (NPixelsWidth != 0)
                textBoxNPixelsWidth.Text = NPixelsWidth.ToString();

            if (NPixelsHeight != 0)
                textBoxNPixelsHeight.Text = NPixelsHeight.ToString();

            if (PixelSizeWidth != 0)
                textBoxPixelSizeWidth.Text = PixelSizeWidth.ToString();

            if (PixelSizeHeight != 0)
                textBoxPixelSizeHeight.Text = PixelSizeHeight.ToString();

            if (EditMode)
            {
                this.Title = "Edit CCD.";
                textBoxCCDName.IsEnabled = false;  // Can't edit the ccd name because it's the primary key
            }
        }
    }
}
