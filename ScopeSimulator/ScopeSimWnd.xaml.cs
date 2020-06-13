using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScopeSimulator
{
    public partial class ScopeSimWnd : Window
    {
        #region Declarations
        private enum Equipment { Telescopes, Eyepieces, CCDs };

        private MyEquipment                                     m_myEquipment;
        private ScopeSimulatorDataSet.MyEquipmentDataTable      m_tableMyEquipment;
        private ScopeSimulatorDataSet.TelescopesDataTable       m_tableTelescopes;
        private ScopeSimulatorDataSet.EyepiecesDataTable        m_tableEyepieces;
        private ScopeSimulatorDataSet.CCDsDataTable             m_tableCCDs;
        private ScopeSimulatorDataSet.BarlowsDataTable          m_tableBarlows;
        private ScopeSimulatorDataSet.FocalReducersDataTable    m_tableFocalReducers;
        private Dictionary<string, Binding>                     m_bindingsList;
        private Path                                            m_circlePathVisualFOV;
        private Path                                            m_circlePathCCDFOV;
        private IEnumerable<string>                             m_fovImageFiles;

        #endregion

        #region Initialization
        public ScopeSimWnd()
        {
            InitializeComponent();
        }

        private void Init()
        {
            GetEquipmentData();
            GetMyEquipment();
            UpdateUI();
            CreateBindings();

            GetFoVImages();
            //UpdateFoVDiagram();
            //UpdateScopeDiagram();
        }

        private void GetEquipmentData()
        {
            // Read the database tables and fill the Telescope, Eyepiece and CCD, Barlow and Focal Reducer datasets
            m_tableTelescopes       = new ScopeSimulatorDataSet.TelescopesDataTable();
            m_tableEyepieces        = new ScopeSimulatorDataSet.EyepiecesDataTable();
            m_tableCCDs             = new ScopeSimulatorDataSet.CCDsDataTable();
            m_tableBarlows          = new ScopeSimulatorDataSet.BarlowsDataTable();
            m_tableFocalReducers    = new ScopeSimulatorDataSet.FocalReducersDataTable();

            ScopeSimulatorDataSet.TelescopesRow r1 = m_tableTelescopes.NewTelescopesRow();
            r1["Name"] = "LX200 12inch";
            r1["Aperture"] = 305;
            r1["FocalLength"] = 3048;
            m_tableTelescopes.Rows.Add(r1);

            ScopeSimulatorDataSet.EyepiecesRow r2 = m_tableEyepieces.NewEyepiecesRow();
            r2["Name"] = "Meade 26mm";
            r2["FocalLength"] = 26;  // 26mm 
            r2["FieldOfView"] = 60;  // 60 degrees is the stated fov
            m_tableEyepieces.Rows.Add(r2); 
            
            ScopeSimulatorDataSet.CCDsRow r3 = m_tableCCDs.NewCCDsRow();
            r3["Name"] = "SXVR-H9";
            r3["NPixelsWidth"] = 1392;
            r3["NPixelsHeight"] = 1040;
            r3["PixelSizeWidth"] = 6.45;
            r3["PixelSizeHeight"] = 6.45;
            m_tableCCDs.Rows.Add(r3);

            ScopeSimulatorDataSet.BarlowsRow r4 = m_tableBarlows.NewBarlowsRow();
            r4["Name"] = "Example Barlow";
            r4["Value"] = 1;
            m_tableBarlows.Rows.Add(r4);

            ScopeSimulatorDataSet.FocalReducersRow r5 = m_tableFocalReducers.NewFocalReducersRow();
            r5["Name"] = "Example FR";
            r5["Value"] = 1;
            m_tableFocalReducers.Rows.Add(r5);

            //ScopeSimulatorDataSetTableAdapters.TelescopesTableAdapter tta       = new ScopeSimulatorDataSetTableAdapters.TelescopesTableAdapter();
            //ScopeSimulatorDataSetTableAdapters.EyepiecesTableAdapter eta        = new ScopeSimulatorDataSetTableAdapters.EyepiecesTableAdapter();
            //ScopeSimulatorDataSetTableAdapters.CCDsTableAdapter cta             = new ScopeSimulatorDataSetTableAdapters.CCDsTableAdapter();
            //ScopeSimulatorDataSetTableAdapters.BarlowsTableAdapter bta          = new ScopeSimulatorDataSetTableAdapters.BarlowsTableAdapter();
            //ScopeSimulatorDataSetTableAdapters.FocalReducersTableAdapter fta    = new ScopeSimulatorDataSetTableAdapters.FocalReducersTableAdapter();

            //tta.Fill(m_tableTelescopes);
            //eta.Fill(m_tableEyepieces);
            //cta.Fill(m_tableCCDs);
            //bta.Fill(m_tableBarlows);
            //fta.Fill(m_tableFocalReducers);
        }

        private void UpdateUI()
        {
            // Update UI controls - fill available equipment comboboxes
            comboBoxScopeName.DataContext       = m_tableTelescopes;
            comboBoxEyepieceName.DataContext    = m_tableEyepieces;
            comboBoxCCDName.DataContext         = m_tableCCDs;
            comboBoxBarlow.DataContext          = m_tableBarlows;
            comboBoxFocalReducer.DataContext    = m_tableFocalReducers;
        }

        private void GetMyEquipment()
        {
            // Get the user's current collection of equipment
            m_tableMyEquipment = new ScopeSimulatorDataSet.MyEquipmentDataTable();
            ScopeSimulatorDataSet.MyEquipmentRow r = m_tableMyEquipment.NewMyEquipmentRow();
            r["Name"] = "Default";
            r["Barlow"] = 1;
            r["CCDNumberOfPixelsHeight"] = 1042;
            r["CCDNumberOfPixelsWidth"] = 1392;
            r["CCDPixelSizeHeight"] = 6.45;
            r["CCDPixelSizeWidth"] = 6.45;
            r["EyepieceApparentFOV"] = 60;  // 60 degrees = manufacturer's advertised FoV
            r["EyepieceFocalLength"] = 26;
            r["FocalReducer"] = 1;
            r["ScopeAperture"] = 305;
            r["ScopeFocalLength"] = 1048;
            r["ScopeName"] = "LX200 12inch";
            r["CCDName"] = "SXVR-H9";
            r["EyepieceName"] = "Meade 26mm";
            r["Selected"] = true;
            m_tableMyEquipment.Rows.Add(r);

            //ScopeSimulatorDataSetTableAdapters.MyEquipmentTableAdapter mta = new ScopeSimulatorDataSetTableAdapters.MyEquipmentTableAdapter();
            //mta.Fill(m_tableMyEquipment);

            // Copy the data from the data table into a MyEquipment object
            m_myEquipment = (MyEquipment)DataHelper.CreateObjectFromDataTable(typeof(MyEquipment), m_tableMyEquipment, true, true, 0);
            
            m_myEquipment.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(m_myEquipment_PropertyChanged);
            m_myEquipment.Refresh();
            m_myEquipment.Inintialized = true;
        }

        private void SaveMyEquipment()
        {
            // Save the current state of MyEquipment
            //if (DataHelper.FillDataTableFromObject(m_myEquipment, m_tableMyEquipment, 0, true, true))
            //{
            //    ScopeSimulatorDataSetTableAdapters.MyEquipmentTableAdapter mta = new ScopeSimulatorDataSetTableAdapters.MyEquipmentTableAdapter();
            //    mta.Update(m_tableMyEquipment);
            //}
            //else
            //{
            //    MessageBox.Show("Unable to save changes to the current list of equipment", "Save Error");
            //}
        }

        private void CreateBindings()
        {
            m_bindingsList = new Dictionary<string, Binding>();

            // Create data bindings between controls and the MyEquipment object...
            m_bindingsList.Add("ScopeName",                 CreateBinding("ScopeName",                  ComboBox.SelectedValueProperty, comboBoxScopeName));
            m_bindingsList.Add("EyepieceName",              CreateBinding("EyepieceName",               ComboBox.SelectedValueProperty, comboBoxEyepieceName));
            m_bindingsList.Add("CCDName",                   CreateBinding("CCDName",                    ComboBox.SelectedValueProperty, comboBoxCCDName));
            m_bindingsList.Add("ScopeAperture",             CreateBinding("ScopeAperture",              TextBox.TextProperty,           textBoxScopeAperture));
            m_bindingsList.Add("ScopeApertureSlider",       CreateBinding("ScopeAperture",              Slider.ValueProperty,           sliderTelescopeAperture));
            m_bindingsList.Add("ScopeFocalLength",          CreateBinding("ScopeFocalLength",           TextBox.TextProperty,           textBoxScopeFocalLength));
            m_bindingsList.Add("ScopeFocalLengthSlider",    CreateBinding("ScopeFocalLength",           Slider.ValueProperty,           sliderTelescopeFocalLength));
            m_bindingsList.Add("EyepieceFocalLength",       CreateBinding("EyepieceFocalLength",        TextBox.TextProperty,           textBoxEyepieceFocalLength));
            m_bindingsList.Add("EyepieceFocalLengthSlider", CreateBinding("EyepieceFocalLength",        Slider.ValueProperty,           sliderEyepieceFocalLength));
            m_bindingsList.Add("Barlow",                    CreateBinding("Barlow",                     ComboBox.SelectedValueProperty, comboBoxBarlow));
            m_bindingsList.Add("FocalReducer",              CreateBinding("FocalReducer",               ComboBox.SelectedValueProperty, comboBoxFocalReducer));
            m_bindingsList.Add("CCDNumberOfPixelsWidth",    CreateBinding("CCDNumberOfPixelsWidth",     TextBox.TextProperty,           textBoxCCDNumberOfPixelsWidth));
            m_bindingsList.Add("CCDNumberOfPixelsHeight",   CreateBinding("CCDNumberOfPixelsHeight",    TextBox.TextProperty,           textBoxCCDNumberOfPixelsHeight));
            m_bindingsList.Add("CCDPixelSizeWidth",         CreateBinding("CCDPixelSizeWidth",          TextBox.TextProperty,           textBoxCCDPixelSizeWidth));
            m_bindingsList.Add("CCDPixelSizeHeight",        CreateBinding("CCDPixelSizeHeight",         TextBox.TextProperty,           textBoxCCDPixelSizeHeight));
            m_bindingsList.Add("ScopeFocalRatio",           CreateBinding("ScopeFocalRatio",            TextBox.TextProperty,           textBoxResultScopeFocalRatio));
            m_bindingsList.Add("EffectiveFocalRatio",       CreateBinding("EffectiveFocalRatio",        TextBox.TextProperty,           textBoxResultEffectiveFocalRatio));
            m_bindingsList.Add("EffectiveFocalLength",      CreateBinding("EffectiveFocalLength",       TextBox.TextProperty,           textBoxResultEffectiveFocalLength));
            m_bindingsList.Add("Magnification",             CreateBinding("Magnification",              TextBox.TextProperty,           textBoxResultEyepieceMag));
            m_bindingsList.Add("MaxMagnitude",              CreateBinding("MaxMagnitude",               TextBox.TextProperty,           textBoxResultHighestMag));
            m_bindingsList.Add("EyepieceActualFOV",         CreateBinding("EyepieceActualFOV",          TextBox.TextProperty,           textBoxResultEyepieceActualFieldOfView));
            m_bindingsList.Add("LimitingMagnitude",         CreateBinding("LimitingMagnitude",          TextBox.TextProperty,           textBoxResultLimitingMag));
            m_bindingsList.Add("DawesLimit",                CreateBinding("DawesLimit",                 TextBox.TextProperty,           textBoxResultResolvingPower));
            m_bindingsList.Add("CCDChipSizeWidth",          CreateBinding("CCDChipSizeWidth",           TextBox.TextProperty,           textBoxResultCCDChipSizeWidth));
            m_bindingsList.Add("CCDChipSizeHeight",         CreateBinding("CCDChipSizeHeight",          TextBox.TextProperty,           textBoxResultCCDChipSizeHeight));
            m_bindingsList.Add("CCDResolutionWidth",        CreateBinding("CCDResolutionWidth",         TextBox.TextProperty,           textBoxResultCCDImageResolutionWidth));
            m_bindingsList.Add("CCDResolutionHeight",       CreateBinding("CCDResolutionHeight",        TextBox.TextProperty,           textBoxResultCCDImageResolutionHeight));
            m_bindingsList.Add("CCDFOVWidth",               CreateBinding("CCDFOVWidth",                TextBox.TextProperty,           textBoxResultCCDFieldOfViewWidth));
            m_bindingsList.Add("CCDFOVHeight",              CreateBinding("CCDFOVHeight",               TextBox.TextProperty,           textBoxResultCCDFieldOfViewHeight));
        }

        private Binding CreateBinding(string propertyName, DependencyProperty propertyToBind, Control controlToBind)
        {
            Binding binding = new Binding(propertyName);
            binding.Source = m_myEquipment;
            binding.Mode = BindingMode.TwoWay;
            controlToBind.SetBinding(propertyToBind, binding);
            return binding;
        }

        private void GetFoVImages()
        {
            // Get all file names in the FoVImages directory
            m_fovImageFiles = System.IO.Directory.EnumerateFiles("FoVImages", "*.jpg");

            // Add the file names to the FoV image combo
            foreach (string filename in m_fovImageFiles)
                comboBoxFOVImage.Items.Add(filename.Substring(filename.LastIndexOf(@"\") + 1));

            comboBoxFOVImage.SelectedIndex = 0;
        }
        #endregion

        #region Event Handlers
        void m_myEquipment_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateFoVDiagram();
            UpdateScopeDiagram();
        }

        private void buttonRestoreDefaults_Click(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveMyEquipment();
        }

        private void buttonAddBarlowData_Click(object sender, RoutedEventArgs e)
        {
            ScopeSimulator.Windows.AddBarlowDataWindow addBarlow = new Windows.AddBarlowDataWindow();
            if (addBarlow.ShowDialog() == true)
            {
                // Update the Barlow combo
                ScopeSimulatorDataSet.BarlowsRow br = m_tableBarlows.NewBarlowsRow();
                br.Name = "x" + addBarlow.Value.ToString();
                br.Value = addBarlow.Value;
                m_tableBarlows.Rows.Add(br);
                comboBoxBarlow.Items.Refresh();
                comboBoxBarlow.SelectedIndex = m_tableBarlows.Rows.Count - 1;

                try
                {
                    // Save the new barlow row
                    ScopeSimulatorDataSetTableAdapters.BarlowsTableAdapter bta = new ScopeSimulatorDataSetTableAdapters.BarlowsTableAdapter();
                    bta.Insert(br.Name, br.Value);
                }
                catch
                {
                    MessageBox.Show("Unable to save new Barlow value", "Save error");
                }
            }
        }

        private void buttonAddFocalReducerData_Click(object sender, RoutedEventArgs e)
        {
            ScopeSimulator.Windows.AddFocalReducerWindow addFocalReducer = new Windows.AddFocalReducerWindow();
            if (addFocalReducer.ShowDialog() == true)
            {
                // Update the Focal Reducer combo
                ScopeSimulatorDataSet.FocalReducersRow fr = m_tableFocalReducers.NewFocalReducersRow();
                fr.Name = "f/" + addFocalReducer.Value.ToString();
                fr.Value = addFocalReducer.Value / 10;
                m_tableFocalReducers.Rows.Add(fr);
                comboBoxFocalReducer.Items.Refresh();
                comboBoxFocalReducer.SelectedIndex = m_tableFocalReducers.Rows.Count - 1;

                try
                {
                    // Save the new focal reducer row
                    ScopeSimulatorDataSetTableAdapters.FocalReducersTableAdapter fta = new ScopeSimulatorDataSetTableAdapters.FocalReducersTableAdapter();
                    fta.Insert(fr.Name, fr.Value);
                }
                catch
                {
                    MessageBox.Show("Unable to save new Focal Reducer value", "Save error");
                }
            }
        }

        private void comboBoxBarlow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxBarlow.SelectedIndex > 0)  // If anything other than no Barlow has been selected
                comboBoxFocalReducer.SelectedIndex = 0;  // Turn off the FR - can't allow Barlow and FR to be active at the same time

        }

        private void comboBoxFocalReducer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxFocalReducer.SelectedIndex > 0)  // If anything other than no FR has been selected
                comboBoxBarlow.SelectedIndex = 0;  // Turn off the Barlow - can't allow Barlow and FR to be active at the same time

        }

        private void comboBoxScopeName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!m_myEquipment.Inintialized || e.AddedItems == null || e.AddedItems.Count == 0 || e.AddedItems[0] == null)
                return;

            try
            {
                DataRowView drv = (DataRowView)e.AddedItems[0];
                string scopeName = drv.Row["Name"].ToString();
                int scopeAperture = (int)drv.Row["Aperture"];
                int scopeFocalLength = (int)drv.Row["FocalLength"];

                m_myEquipment.ScopeName         = scopeName;
                m_myEquipment.ScopeAperture     = scopeAperture;
                m_myEquipment.ScopeFocalLength  = scopeFocalLength;
            }
            catch
            {
                MessageBox.Show("Unable to read properties for selected telescope", "Equipment Property Error");
            }
        }

        private void comboBoxEyepieceName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!m_myEquipment.Inintialized || e.AddedItems == null || e.AddedItems.Count == 0 || e.AddedItems[0] == null)
                return;

            try
            {
                DataRowView drv = (DataRowView)e.AddedItems[0];
                string eyepieceName = drv.Row["Name"].ToString();
                int eyepieceFocalLength = (int)drv.Row["FocalLength"];
                double eyepieceFoV = (double)drv.Row["FieldOfView"];

                m_myEquipment.EyepieceName          = eyepieceName;
                m_myEquipment.EyepieceFocalLength   = eyepieceFocalLength;
                m_myEquipment.EyepieceApparentFOV   = eyepieceFoV;
            }
            catch
            {
                MessageBox.Show("Unable to read properties for selected eyepiece", "Equipment Property Error");
            }
        }

        private void comboBoxCCDName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!m_myEquipment.Inintialized || e.AddedItems == null || e.AddedItems.Count == 0 || e.AddedItems[0] == null)
                return;

            try
            {
                DataRowView drv = (DataRowView)e.AddedItems[0];
                string ccdName = drv.Row["Name"].ToString();
                int ccdNPixelsWidth = (int)drv.Row["NPixelsWidth"];
                int ccdNPixelsHeight = (int)drv.Row["NPixelsHeight"];
                double ccdPixelSizeWidth = (double)drv.Row["PixelSizeWidth"];
                double ccdPixelSizeHeight = (double)drv.Row["PixelSizeHeight"];

                m_myEquipment.CCDName                   = ccdName;
                m_myEquipment.CCDNumberOfPixelsWidth    = ccdNPixelsWidth;
                m_myEquipment.CCDNumberOfPixelsHeight   = ccdNPixelsHeight;
                m_myEquipment.CCDPixelSizeWidth         = ccdPixelSizeWidth;
                m_myEquipment.CCDPixelSizeHeight        = ccdPixelSizeHeight;
            }
            catch
            {
                MessageBox.Show("Unable to read properties for selected CCD Camera", "Equipment Property Error");
            }
        }

        private void comboBoxFOVImage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!m_myEquipment.Inintialized)
                return;

            // Create an image source for the selected image
            BitmapImage newImage = new BitmapImage();
            newImage.BeginInit();

            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            exePath = exePath.Remove(exePath.LastIndexOf(@"\"));
            newImage.UriSource = new Uri(exePath + @"\FoVImages\" + comboBoxFOVImage.SelectedValue.ToString());
            newImage.EndInit();
            imageFOV.Source = newImage;
        }

        private void checkBoxShowVisualFOV_Checked(object sender, RoutedEventArgs e)
        {
            if(m_myEquipment != null && m_myEquipment.Inintialized)
                UpdateFoVDiagram();
        }

        private void checkBoxShowCCDFOV_Checked(object sender, RoutedEventArgs e)
        {
            if (m_myEquipment != null && m_myEquipment.Inintialized)
                UpdateFoVDiagram();
        }

        private void checkBoxShowVisualFOV_Unchecked(object sender, RoutedEventArgs e)
        {
            canvasFOV.Children.Remove(m_circlePathVisualFOV);
        }

        private void checkBoxShowCCDFOV_Unchecked(object sender, RoutedEventArgs e)
        {
            canvasFOV.Children.Remove(m_circlePathCCDFOV);
        }

        private void buttonAddTelescope_Click(object sender, RoutedEventArgs e)
        {
            ScopeSimulator.Windows.AddTelescopeWindow addTelescopeWindow = new Windows.AddTelescopeWindow();

            if (addTelescopeWindow.ShowDialog() == true)
            {
                // Update the telescope combo
                ScopeSimulatorDataSet.TelescopesRow tr = m_tableTelescopes.NewTelescopesRow();
                tr.Name = addTelescopeWindow.TelescopeName;
                tr.Aperture = addTelescopeWindow.Aperture;
                tr.FocalLength = addTelescopeWindow.FocalLength;

                m_tableTelescopes.Rows.Add(tr);
                comboBoxScopeName.Items.Refresh();
                comboBoxScopeName.SelectedIndex = m_tableTelescopes.Rows.Count - 1;

                try
                {
                    // Save the new telescope row
                    ScopeSimulatorDataSetTableAdapters.TelescopesTableAdapter tta = new ScopeSimulatorDataSetTableAdapters.TelescopesTableAdapter();
                    tta.Insert(tr.Name, tr.Aperture, tr.FocalLength);
                }
                catch
                {
                    MessageBox.Show("Unable to save new Telescope data", "Save error");
                }
            }
        }

        private void buttonEditTelescope_Click(object sender, RoutedEventArgs e)
        {
            ScopeSimulator.Windows.AddTelescopeWindow addTelescopeWindow = new Windows.AddTelescopeWindow();

            // Find the row using the primary key
            string selectStatement = "Name = '" + comboBoxScopeName.SelectedValue.ToString() + "'";
            DataRow[] dr = m_tableTelescopes.Select(selectStatement);
            if (dr.Length == 0)
            {
                MessageBox.Show("Unable to read Telescope data", "Internal error");
                return;
            }

            addTelescopeWindow.TelescopeName = (string)dr[0]["Name"];
            addTelescopeWindow.Aperture = (int)dr[0]["Aperture"];
            addTelescopeWindow.FocalLength = (int)dr[0]["FocalLength"];
            addTelescopeWindow.EditMode = true;

            if (addTelescopeWindow.ShowDialog() == true)
            {
                // Update the appropriate row in the table
                dr[0]["Name"] = addTelescopeWindow.TelescopeName;
                dr[0]["Aperture"] = addTelescopeWindow.Aperture;
                dr[0]["FocalLength"] = addTelescopeWindow.FocalLength;

                comboBoxScopeName.Items.Refresh();

                // Force the edited scope values to be refreshed
                int saveIndex = comboBoxScopeName.SelectedIndex;
                comboBoxScopeName.SelectedIndex = 0;
                comboBoxScopeName.SelectedIndex = saveIndex;

                try
                {
                    // Save the updated telescope row
                    ScopeSimulatorDataSetTableAdapters.TelescopesTableAdapter tta = new ScopeSimulatorDataSetTableAdapters.TelescopesTableAdapter();
                    tta.Update(dr);
                }
                catch
                {
                    MessageBox.Show("Unable to save edited Telescope data", "Save error");
                }
            }
        }

        private void buttonAddEyepiece_Click(object sender, RoutedEventArgs e)
        {
            ScopeSimulator.Windows.AddEyepieceWindow addEyepieceWindow = new Windows.AddEyepieceWindow();

            if (addEyepieceWindow.ShowDialog() == true)
            {
                // Update the eyepiece combo
                ScopeSimulatorDataSet.EyepiecesRow er = m_tableEyepieces.NewEyepiecesRow();
                er.Name = addEyepieceWindow.EyepieceName;
                er.FocalLength = addEyepieceWindow.FocalLength;
                er.FieldOfView = addEyepieceWindow.FieldOfView;

                m_tableEyepieces.Rows.Add(er);
                comboBoxEyepieceName.Items.Refresh();
                comboBoxEyepieceName.SelectedIndex = m_tableEyepieces.Rows.Count - 1;

                try
                {
                    // Save the new eyepiece row
                    ScopeSimulatorDataSetTableAdapters.EyepiecesTableAdapter eta = new ScopeSimulatorDataSetTableAdapters.EyepiecesTableAdapter();
                    eta.Insert(er.Name, er.FocalLength, er.FieldOfView);
                }
                catch
                {
                    MessageBox.Show("Unable to save new Eyepiece data", "Save error");
                }
            }
        }

        private void buttonEditEyepiece_Click(object sender, RoutedEventArgs e)
        {
            ScopeSimulator.Windows.AddEyepieceWindow addEyepieceWindow = new Windows.AddEyepieceWindow();

            // Find the row using the primary key
            string selectStatement = "Name = '" + comboBoxEyepieceName.SelectedValue.ToString() + "'";
            DataRow[] dr = m_tableEyepieces.Select(selectStatement);
            if (dr.Length == 0)
            {
                MessageBox.Show("Unable to read Eyepiece data", "Internal error");
                return;
            }

            addEyepieceWindow.EyepieceName = (string)dr[0]["Name"];
            addEyepieceWindow.FocalLength = (int)dr[0]["FocalLength"];
            addEyepieceWindow.FieldOfView = (double)dr[0]["FieldOfView"];
            addEyepieceWindow.EditMode = true;

            if (addEyepieceWindow.ShowDialog() == true)
            {
                // Update the appropriate row in the table
                dr[0]["Name"] = addEyepieceWindow.EyepieceName;
                dr[0]["FocalLength"] = addEyepieceWindow.FocalLength;
                dr[0]["FieldOfView"] = addEyepieceWindow.FieldOfView;

                comboBoxEyepieceName.Items.Refresh();

                // Force the edited eyepiece values to be refreshed
                int saveIndex = comboBoxEyepieceName.SelectedIndex;
                comboBoxEyepieceName.SelectedIndex = 0;
                comboBoxEyepieceName.SelectedIndex = saveIndex;

                try
                {
                    // Save the updated telescope row
                    ScopeSimulatorDataSetTableAdapters.EyepiecesTableAdapter eta = new ScopeSimulatorDataSetTableAdapters.EyepiecesTableAdapter();
                    eta.Update(dr);
                }
                catch
                {
                    MessageBox.Show("Unable to save edited Eyepiece data", "Save error");
                }
            }
        }
        private void buttonAddCCD_Click(object sender, RoutedEventArgs e)
        {
            ScopeSimulator.Windows.AddCCDWindow addCCDWindow = new Windows.AddCCDWindow();

            if (addCCDWindow.ShowDialog() == true)
            {
                // Update the CCD combo
                ScopeSimulatorDataSet.CCDsRow ccdr = m_tableCCDs.NewCCDsRow();
                ccdr.Name = addCCDWindow.CCDName;
                ccdr.NPixelsWidth = addCCDWindow.NPixelsWidth;
                ccdr.NPixelsHeight = addCCDWindow.NPixelsHeight;
                ccdr.PixelSizeWidth = addCCDWindow.PixelSizeWidth;
                ccdr.PixelSizeHeight = addCCDWindow.PixelSizeHeight;

                m_tableCCDs.Rows.Add(ccdr);
                comboBoxCCDName.Items.Refresh();
                comboBoxCCDName.SelectedIndex = m_tableCCDs.Rows.Count - 1;

                try
                {
                    // Save the new CCD row
                    ScopeSimulatorDataSetTableAdapters.CCDsTableAdapter ccdta = new ScopeSimulatorDataSetTableAdapters.CCDsTableAdapter();
                    ccdta.Insert(ccdr.Name, ccdr.NPixelsWidth, ccdr.NPixelsHeight, ccdr.PixelSizeWidth, ccdr.PixelSizeHeight);
                }
                catch
                {
                    MessageBox.Show("Unable to save new CCD data", "Save error");
                }
            }
        }

        private void buttonEditCCD_Click(object sender, RoutedEventArgs e)
        {
            ScopeSimulator.Windows.AddCCDWindow addCCDWindow = new Windows.AddCCDWindow();

            // Find the row using the primary key
            string selectStatement = "Name = '" + comboBoxCCDName.SelectedValue.ToString() + "'";
            DataRow[] dr = m_tableCCDs.Select(selectStatement);
            if (dr.Length == 0)
            {
                MessageBox.Show("Unable to read CCD data", "Internal error");
                return;
            }

            addCCDWindow.CCDName = (string)dr[0]["Name"];
            addCCDWindow.NPixelsWidth = (int)dr[0]["NPixelsWidth"];
            addCCDWindow.NPixelsHeight = (int)dr[0]["NPixelsHeight"];
            addCCDWindow.PixelSizeWidth = (double)dr[0]["PixelSizeWidth"];
            addCCDWindow.PixelSizeHeight = (double)dr[0]["PixelSizeHeight"];
            addCCDWindow.EditMode = true;

            if (addCCDWindow.ShowDialog() == true)
            {
                // Update the appropriate row in the table
                dr[0]["Name"] = addCCDWindow.CCDName;
                dr[0]["NPixelsWidth"] = addCCDWindow.NPixelsWidth;
                dr[0]["NPixelsHeight"] = addCCDWindow.NPixelsHeight;
                dr[0]["PixelSizeWidth"] = addCCDWindow.PixelSizeWidth;
                dr[0]["PixelSizeHeight"] = addCCDWindow.PixelSizeHeight;

                comboBoxCCDName.Items.Refresh();

                // Force the edited ccd values to be refreshed
                int saveIndex = comboBoxCCDName.SelectedIndex;
                comboBoxCCDName.SelectedIndex = 0;
                comboBoxCCDName.SelectedIndex = saveIndex;

                try
                {
                    // Save the updated ccd row
                    ScopeSimulatorDataSetTableAdapters.CCDsTableAdapter cta = new ScopeSimulatorDataSetTableAdapters.CCDsTableAdapter();
                    cta.Update(dr);
                }
                catch
                {
                    MessageBox.Show("Unable to save edited CCD data", "Save error");
                }
            }
        }

        private void buttonDeleteBarlow_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxBarlow.SelectedIndex == -1 || comboBoxScopeName.Items.Count == 1)
                return;

            string currentBarlow = comboBoxBarlow.SelectedValue.ToString();
            int currentBarlowValue;
            bool parsed = int.TryParse(currentBarlow, out currentBarlowValue);

            if (parsed && currentBarlowValue == 1)
            {
                MessageBox.Show("You cannot delete the default Barlow", "Delete Error");
                return;
            }

            if (MessageBox.Show("Delete Barlow: x" + currentBarlow + "\n\nAre you sure?", "Delete Barlow", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            // Find the row using the primary key
            string selectStatement = "Name = 'x" + currentBarlow + "'";
            DataRow[] dr = m_tableBarlows.Select(selectStatement);
            if (dr.Length == 0)
            {
                MessageBox.Show("Unable to read Barlow data", "Internal error");
                return;
            }

            // Delete the appropriate row in the table
            try
            {
                ScopeSimulatorDataSetTableAdapters.BarlowsTableAdapter bta = new ScopeSimulatorDataSetTableAdapters.BarlowsTableAdapter();
                dr[0].Delete();
                if (bta.Update(m_tableBarlows) != 1)
                    throw new Exception();
            }
            catch
            {
                MessageBox.Show("Unable to save edited Barlow data", "Save error");
            }

            // Update the combo
            comboBoxScopeName.SelectedIndex = 0;
            comboBoxScopeName.Items.Refresh();
        }

        private void buttonDeleteFocalReducer_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxFocalReducer.SelectedIndex == -1 || comboBoxFocalReducer.Items.Count == 1)
                return;

            string currentFocalReducer = comboBoxFocalReducer.SelectedValue.ToString();
            double currentFocalReducerValue;
            bool parsed = double.TryParse(currentFocalReducer, out currentFocalReducerValue);

            if (parsed && currentFocalReducerValue == 1)
            {
                MessageBox.Show("You cannot delete the default Focal Reducer", "Delete Error");
                return;
            }

            if (MessageBox.Show("Delete Focal Reducer: f/" + (currentFocalReducerValue * 10) + "\n\nAre you sure?", "Delete Focal Reducer", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            // Find the row using the primary key
            string selectStatement = "Name = 'f/" + (currentFocalReducerValue * 10) + "'";
            DataRow[] dr = m_tableFocalReducers.Select(selectStatement);
            if (dr.Length == 0)
            {
                MessageBox.Show("Unable to read Focal Reducer data", "Internal error");
                return;
            }

            // Delete the appropriate row in the table
            try
            {
                ScopeSimulatorDataSetTableAdapters.FocalReducersTableAdapter fta = new ScopeSimulatorDataSetTableAdapters.FocalReducersTableAdapter();
                dr[0].Delete();
                if (fta.Update(m_tableFocalReducers) != 1)
                    throw new Exception();
            }
            catch
            {
                MessageBox.Show("Unable to save edited Focal Reducer data", "Save error");
            }

            // Update the combo
            comboBoxFocalReducer.SelectedIndex = 0;
            comboBoxFocalReducer.Items.Refresh();
        }

        private void buttonDeleteTelescope_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxScopeName.SelectedIndex == -1 || comboBoxScopeName.Items.Count == 1)
                return;

            string currentScopeName = comboBoxScopeName.SelectedValue.ToString();
            if (MessageBox.Show("Delete Telescope: " + currentScopeName + "\n\nAre you sure?", "Delete Telescope", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            // Find the row using the primary key
            string selectStatement = "Name = '" + currentScopeName + "'";
            DataRow[] dr = m_tableTelescopes.Select(selectStatement);
            if (dr.Length == 0)
            {
                MessageBox.Show("Unable to read Telescope data", "Internal error");
                return;
            }

            // Delete the appropriate row in the table
            try
            {
                ScopeSimulatorDataSetTableAdapters.TelescopesTableAdapter tta = new ScopeSimulatorDataSetTableAdapters.TelescopesTableAdapter();
                dr[0].Delete();
                if (tta.Update(m_tableTelescopes) != 1)
                    throw new Exception();
            }
            catch
            {
                MessageBox.Show("Unable to save edited Telescope data", "Save error");
            }

            // Update the combo
            comboBoxScopeName.SelectedIndex = 0;
            comboBoxScopeName.Items.Refresh();
        }

        private void buttonDeleteEyepiece_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxEyepieceName.SelectedIndex == -1 || comboBoxEyepieceName.Items.Count == 1)
                return;

            string currentEyepieceName = comboBoxEyepieceName.SelectedValue.ToString();
            if (MessageBox.Show("Delete Eyepiece: " + currentEyepieceName + "\n\nAre you sure?", "Delete Eyepiece", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            // Find the row using the primary key
            string selectStatement = "Name = '" + currentEyepieceName + "'";
            DataRow[] dr = m_tableEyepieces.Select(selectStatement);
            if (dr.Length == 0)
            {
                MessageBox.Show("Unable to read Eyepiece data", "Internal error");
                return;
            }

            // Delete the appropriate row in the table
            try
            {
                ScopeSimulatorDataSetTableAdapters.EyepiecesTableAdapter eta = new ScopeSimulatorDataSetTableAdapters.EyepiecesTableAdapter();
                dr[0].Delete();
                if (eta.Update(m_tableEyepieces) != 1)
                    throw new Exception();
            }
            catch
            {
                MessageBox.Show("Unable to save edited Eyepiece data", "Save error");
            }

            // Update the combo
            comboBoxEyepieceName.SelectedIndex = 0;
            comboBoxEyepieceName.Items.Refresh();
        }

        private void buttonDeleteCCD_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxCCDName.SelectedIndex == -1 || comboBoxCCDName.Items.Count == 1)
                return;

            string currentCCDName = comboBoxCCDName.SelectedValue.ToString();
            if (MessageBox.Show("Delete CCD: " + currentCCDName + "\n\nAre you sure?", "Delete CCD", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            // Find the row using the primary key
            string selectStatement = "Name = '" + currentCCDName + "'";
            DataRow[] dr = m_tableCCDs.Select(selectStatement);
            if (dr.Length == 0)
            {
                MessageBox.Show("Unable to read CCD data", "Internal error");
                return;
            }

            // Delete the appropriate row in the table
            try
            {
                ScopeSimulatorDataSetTableAdapters.CCDsTableAdapter ccdta = new ScopeSimulatorDataSetTableAdapters.CCDsTableAdapter();
                dr[0].Delete();
                if (ccdta.Update(m_tableCCDs) != 1)
                    throw new Exception();
            }
            catch
            {
                MessageBox.Show("Unable to save edited CCD data", "Save error");
            }

            // Update the combo
            comboBoxCCDName.SelectedIndex = 0;
            comboBoxCCDName.Items.Refresh();
        }
        #endregion

        #region Drawing
        private void UpdateScopeDiagram()
        {
         	int aperture    		= m_myEquipment.ScopeAperture / 10;
            int apertureScaler  	= 25;
         	int	focalLengthScaler 	= 16;
         	int	leftMargin			= 20;
         	int	rightMarginMin		= 50;

         	Point topLeft;
         	Point topRight;
         	Point btmLeft;
         	Point btmRight;

            topLeft = new Point(leftMargin, (canvasDiagram.Height / 2) - (aperture + apertureScaler));
            btmLeft = new Point(leftMargin, (canvasDiagram.Height / 2) + (aperture + apertureScaler));

            if (m_myEquipment.EffectiveFocalLength < 5000)
            {
                topRight = new Point(leftMargin + (m_myEquipment.EffectiveFocalLength / focalLengthScaler), (canvasDiagram.Height / 2) - (aperture + apertureScaler));
                btmRight = new Point(leftMargin + (m_myEquipment.EffectiveFocalLength / focalLengthScaler), (canvasDiagram.Height / 2) + (aperture + apertureScaler));
            }
            else
            {
                topRight = new Point(leftMargin + (5000 / focalLengthScaler), (canvasDiagram.Height / 2) - (aperture + apertureScaler));
                btmRight = new Point(leftMargin + (5000 / focalLengthScaler), (canvasDiagram.Height / 2) + (aperture + apertureScaler));
            }

            canvasDiagram.Children.Clear();  // Clear the previous drawing

            if (topRight.X < rightMarginMin)
                topRight.X = rightMarginMin;

            if (btmRight.X < rightMarginMin)
                btmRight.X = rightMarginMin;

            // Top of scope
            Line scopeLine = new Line();
            scopeLine.Stroke = Brushes.Black;
            scopeLine.StrokeThickness = 2;
            scopeLine.X1 = topLeft.X;
            scopeLine.X2 = topRight.X+3;
            scopeLine.Y1 = topLeft.Y;
            scopeLine.Y2 = topRight.Y;
            canvasDiagram.Children.Add(scopeLine);

            // Top half of mirror
            Line scopeLine2 = new Line();
            scopeLine2.Stroke = Brushes.Turquoise; 
            scopeLine2.StrokeThickness = 4;
            scopeLine2.X1 = topRight.X+2;
            scopeLine2.X2 = btmRight.X+2;
            scopeLine2.Y1 = topRight.Y;
            scopeLine2.Y2 = ((topRight.Y + btmRight.Y) / 2) - 5;
            canvasDiagram.Children.Add(scopeLine2);

            // Bottom half of mirror
            Line scopeLine3 = new Line();
            scopeLine3.Stroke = Brushes.Turquoise;
            scopeLine3.StrokeThickness = 4;
            scopeLine3.X1 = btmRight.X+2;
            scopeLine3.X2 = btmRight.X+2;
            scopeLine3.Y1 = ((topRight.Y + btmRight.Y) / 2) + 5;
            scopeLine3.Y2 = btmRight.Y;
            canvasDiagram.Children.Add(scopeLine3);

            // Bottom of scope
            Line scopeLine4 = new Line();
            scopeLine4.Stroke = Brushes.Black;
            scopeLine4.StrokeThickness = 2;
            scopeLine4.X1 = btmRight.X+3;
            scopeLine4.X2 = btmLeft.X;
            scopeLine4.Y1 = btmRight.Y;
            scopeLine4.Y2 = btmLeft.Y;
            canvasDiagram.Children.Add(scopeLine4);

            // Draw the corrector plate
            Line scopeLine5 = new Line();
            scopeLine5.Stroke = Brushes.Turquoise;
            scopeLine5.StrokeThickness = 4;
            scopeLine5.X1 = topLeft.X + 3;
            scopeLine5.X2 = topLeft.X + 3;
            scopeLine5.Y1 = topLeft.Y + 2;
            scopeLine5.Y2 = btmLeft.Y - 2;
            canvasDiagram.Children.Add(scopeLine5);

            // Draw the secondary mirror
            Line scopeLine6 = new Line();
            scopeLine6.Stroke = Brushes.Black;
            scopeLine6.StrokeThickness = 6;
            scopeLine6.X1 = topLeft.X + 3;
            scopeLine6.X2 = topLeft.X + 3;
            scopeLine6.Y1 = ((topLeft.Y + btmLeft.Y)/2) - 15;
            scopeLine6.Y2 = ((topLeft.Y + btmLeft.Y)/2) + 15;
            canvasDiagram.Children.Add(scopeLine6);

            // Draw the light path - top
            Line scopeLine7 = new Line();
            scopeLine7.Stroke = Brushes.Red;
            scopeLine7.StrokeThickness = 2;
            scopeLine7.X1 = topLeft.X - 20;
            scopeLine7.X2 = topRight.X - 2;
            scopeLine7.Y1 = topLeft.Y + 20;
            scopeLine7.Y2 = topRight.Y + 20;
            canvasDiagram.Children.Add(scopeLine7);

            // Draw the light path - bottom
            Line scopeLine8 = new Line();
            scopeLine8.Stroke = Brushes.Red;
            scopeLine8.StrokeThickness = 2;
            scopeLine8.X1 = topLeft.X - 20;
            scopeLine8.X2 = topRight.X - 2;
            scopeLine8.Y1 = btmLeft.Y - 20;
            scopeLine8.Y2 = btmRight.Y - 20;
            canvasDiagram.Children.Add(scopeLine8);

            // Draw the light path - secondary to eyepiece
            Line scopeLine9 = new Line();
            scopeLine9.Stroke = Brushes.Red;
            scopeLine9.StrokeThickness = 2;
            scopeLine9.X1 = topLeft.X + 7;
            scopeLine9.X2 = topRight.X + 25;
            scopeLine9.Y1 = ((topLeft.Y + btmLeft.Y) / 2);
            scopeLine9.Y2 = ((topLeft.Y + btmLeft.Y) / 2);
            canvasDiagram.Children.Add(scopeLine9);

            // Draw the light path - secondary to top of main mirror
            Line scopeLine10 = new Line();
            scopeLine10.Stroke = Brushes.Red;
            scopeLine10.StrokeThickness = 2;
            scopeLine10.X1 = topLeft.X + 7;
            scopeLine10.X2 = topRight.X;
            scopeLine10.Y1 = ((topLeft.Y + btmLeft.Y) / 2);
            scopeLine10.Y2 = topRight.Y + 20;
            canvasDiagram.Children.Add(scopeLine10);

            // Draw the light path - secondary to bottom of main mirror
            Line scopeLine11 = new Line();
            scopeLine11.Stroke = Brushes.Red;
            scopeLine11.StrokeThickness = 2;
            scopeLine11.X1 = topLeft.X + 7;
            scopeLine11.X2 = btmRight.X;
            scopeLine11.Y1 = ((topLeft.Y + btmLeft.Y) / 2);
            scopeLine11.Y2 = btmRight.Y - 20;
            canvasDiagram.Children.Add(scopeLine11);

            // Draw the light path arrows - top
            Line scopeLine12 = new Line();
            scopeLine12.Stroke = Brushes.Red;
            scopeLine12.StrokeThickness = 2;
            scopeLine12.X1 = topLeft.X + 20;
            scopeLine12.X2 = topLeft.X + 26;
            scopeLine12.Y1 = topLeft.Y + 16;
            scopeLine12.Y2 = topLeft.Y + 20;
            canvasDiagram.Children.Add(scopeLine12);
            Line scopeLine13 = new Line();
            scopeLine13.Stroke = Brushes.Red;
            scopeLine13.StrokeThickness = 2;
            scopeLine13.X1 = topLeft.X + 26;
            scopeLine13.X2 = topLeft.X + 20;
            scopeLine13.Y1 = topLeft.Y + 20;
            scopeLine13.Y2 = topLeft.Y + 24;
            canvasDiagram.Children.Add(scopeLine13);

            // Draw the light path arrows - bottom
            Line scopeLine14 = new Line();
            scopeLine14.Stroke = Brushes.Red;
            scopeLine14.StrokeThickness = 2;
            scopeLine14.X1 = topLeft.X + 20;
            scopeLine14.X2 = topLeft.X + 26;
            scopeLine14.Y1 = btmLeft.Y - 16;
            scopeLine14.Y2 = btmLeft.Y - 20;
            canvasDiagram.Children.Add(scopeLine14);
            Line scopeLine15 = new Line();
            scopeLine15.Stroke = Brushes.Red;
            scopeLine15.StrokeThickness = 2;
            scopeLine15.X1 = topLeft.X + 26;
            scopeLine15.X2 = topLeft.X + 20;
            scopeLine15.Y1 = btmLeft.Y - 20;
            scopeLine15.Y2 = btmLeft.Y - 24;
            canvasDiagram.Children.Add(scopeLine15);

            // Draw the light path arrows - centre
            Line scopeLine16 = new Line();
            scopeLine16.Stroke = Brushes.Red;
            scopeLine16.StrokeThickness = 2;
            scopeLine16.X1 = topLeft.X + 50;
            scopeLine16.X2 = topLeft.X + 56;
            scopeLine16.Y1 = ((topLeft.Y + btmLeft.Y)/2)-4;
            scopeLine16.Y2 = ((topLeft.Y + btmLeft.Y) / 2);
            canvasDiagram.Children.Add(scopeLine16);
            Line scopeLine17 = new Line();
            scopeLine17.Stroke = Brushes.Red;
            scopeLine17.StrokeThickness = 2;
            scopeLine17.X1 = topLeft.X + 56;
            scopeLine17.X2 = topLeft.X + 50;
            scopeLine17.Y1 = ((topLeft.Y + btmLeft.Y) / 2);
            scopeLine17.Y2 = ((topLeft.Y + btmLeft.Y) / 2)+4;
            canvasDiagram.Children.Add(scopeLine17);

            // Draw the eyepiece/CCD
            Line scopeLine18 = new Line();
            scopeLine18.Stroke = Brushes.ForestGreen;
            scopeLine18.StrokeThickness = 8;
            scopeLine18.X1 = topRight.X + 27;
            scopeLine18.X2 = topRight.X + 27;
            scopeLine18.Y1 = ((topLeft.Y + btmLeft.Y) / 2) - 15;
            scopeLine18.Y2 = ((topLeft.Y + btmLeft.Y) / 2) + 15;
            canvasDiagram.Children.Add(scopeLine18);
        }

        private void UpdateFoVDiagram()
        {
            canvasFOV.Children.Clear();  // Clear the previous drawing

            Point fovCanvasCenter = new Point(canvasFOV.Width / 2, canvasFOV.Height / 2);
            if (checkBoxShowVisualFOV.IsChecked.Value)
            {
                // Get the eyepiece field of view (which is in degrees), convert to arcsecs and then convert to pixels
                // The field of view image is 360x360 so each pixel in the image represents 10 arcsecs of sky
                // The radius of the circle is half the calculated field of view
                int radius = (int)((m_myEquipment.EyepieceActualFOV * (60 * 60)) / 10) / 2;
                if (radius <= 180)
                {
                    labelVisualFOVWarning.Visibility = System.Windows.Visibility.Hidden;
                    EllipseGeometry circleVisualFOV = new EllipseGeometry();

                    circleVisualFOV.Center = fovCanvasCenter;
                    circleVisualFOV.RadiusX = radius;
                    circleVisualFOV.RadiusY = radius;

                    m_circlePathVisualFOV = new Path();
                    m_circlePathVisualFOV.Stroke = Brushes.Green;
                    m_circlePathVisualFOV.StrokeThickness = 2;
                    m_circlePathVisualFOV.Data = circleVisualFOV;
                    canvasFOV.Children.Add(m_circlePathVisualFOV);
                }
                else
                {
                    labelVisualFOVWarning.Visibility = System.Windows.Visibility.Visible;
                }
            }

            if (checkBoxShowCCDFOV.IsChecked.Value)
            {
                // Get the CCD width and height field of view (which is in arcmins), convert to arcsecs and then convert to pixels
                // The field of view image is 360x360 so each pixel in the image represents 10 arcsecs of sky
                int width = (int)(m_myEquipment.CCDFOVWidth * 60) / 10;
                int height = (int)(m_myEquipment.CCDFOVHeight * 60) / 10;
                if (width <= 360 && height <= 360)
                {
                    labelCCDFOVWarning.Visibility = System.Windows.Visibility.Hidden;
                    RectangleGeometry rectCCDFOV = new RectangleGeometry();
                    rectCCDFOV.Rect = new Rect(fovCanvasCenter.X - (width / 2), fovCanvasCenter.Y - (height / 2), width, height);

                    m_circlePathCCDFOV = new Path();
                    m_circlePathCCDFOV.Stroke = Brushes.Red;
                    m_circlePathCCDFOV.StrokeThickness = 2;
                    m_circlePathCCDFOV.Data = rectCCDFOV;
                    canvasFOV.Children.Add(m_circlePathCCDFOV);
                }
                else
                {
                    labelCCDFOVWarning.Visibility = System.Windows.Visibility.Hidden;
                }	
            }

        }
        #endregion
    }
}
