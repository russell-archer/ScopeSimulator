using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;

namespace ScopeSimulator
{
    [System.ComponentModel.Bindable(true)]
    public class MyEquipment : INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler PropertyChanged;

        private bool m_inintialized = false;
        public  bool Inintialized
        {
            get { return m_inintialized; }
            set { m_inintialized = value; }
        }

        // The name given to this collection of equipment
        private string m_name;
        public  string Name
        {
            get { return m_name; }
            set { m_name = value; OnPropertyChanged("Name"); }
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------------
		// The following values are associated with UI controls:
        // ----------------------------------------------------------------------------------------------------------------------------------------------------

        // Telescope product name
        private string m_scopeName;
        public  string ScopeName
        {
            get { return m_scopeName; }
            set 
            { 
                m_scopeName = value; 
                OnPropertyChanged("ScopeName"); 
            }
        }

        // Eyepiece product name
        private string m_eyepieceName;
        public  string EyepieceName
        {
            get { return m_eyepieceName; }
            set 
            { 
                m_eyepieceName = value; 
                OnPropertyChanged("EyepieceName"); 
            }
        }

        // CCD product name
        private string m_ccdName;
        public  string CCDName
        {
            get { return m_ccdName; }
            set 
            { 
                m_ccdName = value; 
                OnPropertyChanged("CCDName"); 
            }
        }

        // Size in mm of the primary mirror
		private int m_scopeAperture;  
        public  int ScopeAperture
        {
		    get { return m_scopeAperture;  }
            set 
            { 
                m_scopeAperture = value; 
                OnPropertyChanged("ScopeAperture");

                if (Inintialized)
                {
                    RefreshScopeFocalRatio();
                    RefreshEffectiveFocalRatio();
                    RefreshDawesLimit();
                    RefreshMaxgMagnitude();
                    RefreshLimitingMagnitude();
                }
            } 
        }

        // The distance from the primary mirror in mm required to bring light to a focus
        private int m_scopeFocalLength;  
        public  int ScopeFocalLength
        {
            get { return m_scopeFocalLength;  }
            set 
            { 
                m_scopeFocalLength = value; 
                OnPropertyChanged("ScopeFocalLength");


                if (Inintialized)
                {
                    RefreshScopeFocalRatio();
                    RefreshEffectiveFocalLength();
                }
            }
        }

        // Focal length of the eyepiece
        private int m_eyepieceFocalLength;  
        public  int EyepieceFocalLength
        {
            get { return m_eyepieceFocalLength; }
            set 
            { 
                m_eyepieceFocalLength = value; 
                OnPropertyChanged("EyepieceFocalLength");

                
                if (Inintialized)
                    RefreshMagnification();
            }
        }

        // Manufacterer's advertised apparent field of view
        private double m_eyepieceApparentFOV;
        public  double EyepieceApparentFOV
        {
            get { return m_eyepieceApparentFOV; }
            set 
            { 
                m_eyepieceApparentFOV = value; 
                OnPropertyChanged("EyepieceApparentFOV");

                
                if (Inintialized)
                    RefreshEyepieceActualFOV();
            }
        }

        // Barlow focal length multiplier
        private double m_barlow;  
        public  double Barlow
        {
            get { return m_barlow; }
            set 
            { 
                m_barlow = value; 
                OnPropertyChanged("Barlow");

                
                if (Inintialized)
                    RefreshEffectiveFocalLength(); 
            }
        }

        // Focal length reducer
        private double m_focalReducer;  
        public  double FocalReducer
        {
            get { return m_focalReducer; }
            set 
            { 
                m_focalReducer = value; 
                OnPropertyChanged("FocalReducer");

                
                if (Inintialized)
                    RefreshEffectiveFocalLength(); 
            }
        }

        // Number of pixels in the CCD (width)
        private int m_ccdNumberOfPixelsWidth;  
        public  int CCDNumberOfPixelsWidth
        {
            get { return m_ccdNumberOfPixelsWidth; }
            set 
            { 
                m_ccdNumberOfPixelsWidth = value; 
                OnPropertyChanged("CCDNumberOfPixelsWidth");


                if (Inintialized)
                {
                    RefreshCCDChipSizeWidth();
                    RefreshCCDFOVWidth();
                }
            }
        }

        // Number of pixels in the CCD (height)
        private int m_ccdNumberOfPixelsHeight;  
        public  int CCDNumberOfPixelsHeight
        {
            get { return m_ccdNumberOfPixelsHeight; }
            set 
            { 
                m_ccdNumberOfPixelsHeight = value; 
                OnPropertyChanged("CCDNumberOfPixelsHeight");


                if (Inintialized)
                {
                    RefreshCCDChipSizeHeight();
                    RefreshCCDFOVHeight();
                }
            }
        }

        // Size of each pixel in microns (width)
        private double m_ccdPixelSizeWidth;  
        public  double CCDPixelSizeWidth
        {
            get { return m_ccdPixelSizeWidth; }
            set 
            { 
                m_ccdPixelSizeWidth = value; 
                OnPropertyChanged("CCDPixelSizeWidth");


                if (Inintialized)
                {
                    RefreshCCDChipSizeWidth();
                    RefreshCCDResolutionWidth();
                    RefreshCCDFOVWidth();
                }
            }
        }

        // Size of each pixel in microns (height)  
        private double m_ccdPixelSizeHeight;  
        public  double CCDPixelSizeHeight
        {
            get { return m_ccdPixelSizeHeight; }
            set 
            { 
                m_ccdPixelSizeHeight = value; 
                OnPropertyChanged("CCDPixelSizeHeight");


                if (Inintialized)
                {
                    RefreshCCDChipSizeHeight();
                    RefreshCCDResolutionHeight();
                    RefreshCCDFOVHeight();
                }
            }
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------------
        // The following values that are calculated (not set via the UI, and the setter should not be called directly)
        // ----------------------------------------------------------------------------------------------------------------------------------------------------

        // Eyepiece actual field of view (approx.) in degrees
        private double m_eyepieceActualFOV;
        private void RefreshEyepieceActualFOV() 
        { 
            EyepieceActualFOV = Math.Round(EyepieceApparentFOV / Magnification, 2); 
        }
        public  double EyepieceActualFOV
        {
            get { return m_eyepieceActualFOV;  }
            set 
            { 
                m_eyepieceActualFOV = value; 
                OnPropertyChanged("EyepieceActualFOV"); 
            }
        }

        // Scope focal ratio (focal length / aperture) all measurements in mm
        private double m_scopeFocalRatio;
        private void RefreshScopeFocalRatio() { ScopeFocalRatio = Math.Round((double)ScopeFocalLength / ScopeAperture, 3);  }
        public  double ScopeFocalRatio
        {
            get { return m_scopeFocalRatio;  }
            set 
            {
                m_scopeFocalRatio = value; 
                OnPropertyChanged("ScopeFocalRatio"); 
            }
        }

        // Effective focal ratio (focal length / aperture) in combination with barlow or focal reducer. All measurements in mm
        private double m_effectiveFocalRatio;
        private void RefreshEffectiveFocalRatio() { EffectiveFocalRatio = Math.Round((double)EffectiveFocalLength / ScopeAperture, 3);  }
        public  double EffectiveFocalRatio
        {
            get { return m_effectiveFocalRatio;  }
            set 
            { 
                m_effectiveFocalRatio = value; 
                OnPropertyChanged("EffectiveFocalRatio"); 
            }
        }

        // Combined scope focal length in combination with barlow or focal reducer
        private int m_effectiveFocalLength;
        private void RefreshEffectiveFocalLength() { EffectiveFocalLength = (int)((ScopeFocalLength * Barlow) * FocalReducer); }
        public  int EffectiveFocalLength
        { 
            get { return m_effectiveFocalLength;  }
            set 
            { 
                m_effectiveFocalLength = value; 
                OnPropertyChanged("EffectiveFocalLength");

                if (Inintialized)
                {

                    RefreshEffectiveFocalRatio();
                    RefreshMagnification();
                    RefreshCCDResolutionWidth();
                    RefreshCCDResolutionHeight();
                    RefreshCCDFOVWidth();
                    RefreshCCDFOVHeight();
                }
            }
        }

        // Dawes limit (11.6 / Scope Aperture (in cm))
        private double m_dawesLimit;
        public  void RefreshDawesLimit() { DawesLimit = Math.Round((double)(11.6 / (ScopeAperture / 10)), 2); } // 11.6 / Scope Aperture (in cm)
		public  double DawesLimit
        {
            get { return m_dawesLimit;  }
            set 
            { 
                m_dawesLimit = value; 
                OnPropertyChanged("DawesLimit"); 
            }
        }

        // Magnification
        private double m_magnification;
        public  void RefreshMagnification() { Magnification = Math.Round((double)(EffectiveFocalLength / EyepieceFocalLength));  }
		public  double Magnification
        {
            get { return m_magnification;  }
            set 
            { 
                m_magnification = value; 
                OnPropertyChanged("Magnification");
                
                if (Inintialized)
                    RefreshEyepieceActualFOV();
            }
        }

        //  Telescopic faintest magnitude = 7.5 + 5 * (log base 10 (aperture)) (in cm)	
        private double m_limitingMagnitude;
        public  void RefreshLimitingMagnitude() { LimitingMagnitude = Math.Round((double)(7.5 + (5 * (Math.Log(ScopeAperture / 10) / 2.30258509299405))), 2);  }  // Dividing log(n) by 2.30... converts from a natural log to log10
		public  double LimitingMagnitude
        {
            get { return m_limitingMagnitude;  }
            set 
            { 
                m_limitingMagnitude = value; 
                OnPropertyChanged("LimitingMagnitude"); 
            }
        }

        //  Approx. Max useful magnification = 50 * (Apterture in inches)
        private int m_maxMagnitude;
        public  void RefreshMaxgMagnitude() { MaxMagnitude = (int)(50 * (ScopeAperture / 25.4)); }  // Dividing by 25.4 converts mm to inches
		public  int MaxMagnitude
        {
            get { return m_maxMagnitude;  }
            set 
            { 
                m_maxMagnitude = value; 
                OnPropertyChanged("MaxMagnitude"); 
            }
        }

        // Size of the CCD sensor chip in microns
        private double m_ccdChipSizeWidth;
        public  void RefreshCCDChipSizeWidth() { CCDChipSizeWidth = Math.Round((double)(CCDPixelSizeWidth * CCDNumberOfPixelsWidth) / 1000, 2);  }
		public  double CCDChipSizeWidth
        {
            get { return m_ccdChipSizeWidth; }
            set 
            { 
                m_ccdChipSizeWidth = value; 
                OnPropertyChanged("CCDChipSizeWidth"); 
            }
        }

        // Size of the CCD sensor chip in microns
        private double m_ccdChipSizeHeight;
        public  void RefreshCCDChipSizeHeight() { CCDChipSizeHeight = Math.Round((double)(CCDPixelSizeHeight * CCDNumberOfPixelsHeight) / 1000, 2);  }
		public  double CCDChipSizeHeight
        {
            get { return m_ccdChipSizeHeight; }
            set 
            { 
                m_ccdChipSizeHeight = value; 
                OnPropertyChanged("CCDChipSizeHeight"); 
            }
        }

        // CCD resolution in seconds of arc per pixel
        private double m_ccdResolutionWidth;
        public  void RefreshCCDResolutionWidth() { CCDResolutionWidth = Math.Round((double)(CCDPixelSizeWidth / EffectiveFocalLength) * 206, 3);  }  
		public  double CCDResolutionWidth
        {
            get { return m_ccdResolutionWidth;  }
            set 
            { 
                m_ccdResolutionWidth = value; 
                OnPropertyChanged("CCDResolutionWidth"); 
            }
        }

        // CCD resolution in seconds of arc per pixel
        private double m_ccdResolutionHeight;
        public  void RefreshCCDResolutionHeight() { CCDResolutionHeight = Math.Round((double)(CCDPixelSizeHeight / EffectiveFocalLength) * 206, 3);  }  
		public  double CCDResolutionHeight
        {
            get { return m_ccdResolutionHeight;  }
            set 
            { 
                m_ccdResolutionHeight = value; 
                OnPropertyChanged("CCDResolutionHeight"); 
            }
        }

        // CCD field of view in arc seconds per pixel
        private double m_ccdFOVWidth;
        public  void RefreshCCDFOVWidth() { CCDFOVWidth = Math.Round((double)(CCDPixelSizeWidth / EffectiveFocalLength) * ((206 * CCDNumberOfPixelsWidth) / 60), 3);  }  
		public  double CCDFOVWidth
        {
            get { return m_ccdFOVWidth;  }
            set 
            { 
                m_ccdFOVWidth = value; 
                OnPropertyChanged("CCDFOVWidth"); 
            }
        }

        // CCD field of view in arc seconds per pixel
        private double m_ccdFOVHeight;
        public  void RefreshCCDFOVHeight() { CCDFOVHeight = Math.Round((double)(CCDPixelSizeHeight / EffectiveFocalLength) * ((206 * CCDNumberOfPixelsHeight) / 60), 3); }  
		public  double CCDFOVHeight
        {
            get { return m_ccdFOVHeight;  }
            set 
            { 
                m_ccdFOVHeight = value; 
                OnPropertyChanged("CCDFOVHeight"); 
            }
        }

		public MyEquipment()
		{
            Name = "";
            Barlow = 1;
            CCDChipSizeHeight = 0;
            CCDChipSizeWidth = 0;
            CCDFOVHeight = 0;
            CCDFOVWidth = 0;
            CCDName = "";
            CCDNumberOfPixelsHeight = 0;
            CCDNumberOfPixelsWidth = 0;
            CCDPixelSizeHeight = 0;
            CCDPixelSizeWidth = 0;
            CCDResolutionHeight = 0;
            CCDResolutionWidth = 0;
            DawesLimit = 0;
            EffectiveFocalLength = 0;
            EffectiveFocalRatio = 0;
            EyepieceActualFOV = 0;
            EyepieceApparentFOV = 0;
            EyepieceFocalLength = 0;
            EyepieceName = "";
            FocalReducer = 1;
            LimitingMagnitude = 0;
            Magnification = 0;
            MaxMagnitude = 0;
            ScopeAperture = 0;
            ScopeFocalLength = 0;
            ScopeFocalRatio = 0;
            ScopeName = "";
		}	
		
		public void Refresh()
		{
			RefreshScopeFocalRatio();
            RefreshEffectiveFocalLength();
            RefreshEffectiveFocalRatio();
			RefreshDawesLimit();
			RefreshMagnification();
			RefreshEyepieceActualFOV();
			RefreshLimitingMagnitude();
			RefreshCCDChipSizeWidth();
			RefreshCCDChipSizeHeight();
			RefreshCCDResolutionWidth();
			RefreshCCDResolutionHeight();
			RefreshCCDFOVWidth();
			RefreshCCDFOVHeight();
			RefreshMaxgMagnitude();
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}