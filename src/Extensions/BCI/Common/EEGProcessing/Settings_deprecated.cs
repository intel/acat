using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ACAT.Extensions.Default.Actuators.EEG.EEGProcessing
{
    [Serializable]
    public class Settings : PreferencesBase
    {

        /// <summary>
        /// Name of the settings file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String SettingsFilePath;

        /// <summary>
        /// Sample rate of the device
        /// </summary>
        [NonSerialized]
        public int SampleRate = 250; //This is set to 250Hz

        // ****************************** Feature extraction ********************************//
        public int OffsetTarget;

        public int FeatureExtraction_WindowDurationInMs;

        public bool[] DimReduct_ChannelSubset;

        public int DimReduct_DownsampleRate;


        /// <summary>
        /// Component to sort eigenvalues: 
        /// Options: firstNcomponents (params: MinEigenvalue), threshold (params: VarianceThreshold), minRelativeEigenvalue (params:nComponents)
        /// </summary>
        public string DimReductPCA_ComponentSortMethod;

        public double DimReductPCA_MinEigenvalue;

        public float DimReductPCA_VarianceThreshold;

        public int DimRecudtPCA_nComponents; 

        public double DimReductRDA_ShrinkParam;

        public double DimReductRDA_RegParam;


        // ****************************** Classification ********************************//
        public int CrossValidation_NumFolds;

        public string CrossaValidation_SortMethod; 

        public int Classifier_maxDecisionSequences;

        public float Classifier_confidenceThreshold;

        public bool Classifier_useNextCharacterProbabilities;

        // ***************************** Data parser ********************************** //

        public int DataParser_idxSampleIdx;

        public int DataParser_idxStartEEGData;

        public int DataParser_numEEGDataChannels;

        public int DataParser_idxTriggerSignal_Hw;

        public int DataParser_idxTriggerSignal_Sw;

        public int DataParser_headerLinesToSkip;

        public bool DataParser_useSoftwareTrigers;


        // ************************** Calibration *********************************** //
        public string Calibration_TrainedClassifiersFilePath;

        public bool Calibration_ForceRecalibrateFromFile;

        //public string Calibration_DirectoryCalibrationFiles;

        public string Calibration_CalibrationFileId; // if empty use session

        public int DataCollection_TestID; // For data collection V2


        /// <summary>
        /// Default values
        /// </summary>
        public Settings()
        {
            FeatureExtraction_WindowDurationInMs = 500;
            OffsetTarget = 1000;

            DimReduct_ChannelSubset = new bool[8];
            for (int i = 0; i < 8; i++)
                DimReduct_ChannelSubset[i] = true;

            DimReduct_DownsampleRate = 2;
            DimReductPCA_ComponentSortMethod = "minRelativeEigenvalue";
            DimReductPCA_MinEigenvalue = 0.00001;
            DimReductRDA_ShrinkParam = 0.9;
            DimReductRDA_RegParam = 0.1;

            CrossaValidation_SortMethod = "sequential"; 
            CrossValidation_NumFolds = 10;

            Classifier_confidenceThreshold = 0.9f;
            Classifier_maxDecisionSequences = 8;
            Classifier_useNextCharacterProbabilities = true;

            Calibration_TrainedClassifiersFilePath = "Actuators\\BCI\\TrainedClassifiers";
           // Calibration_DirectoryCalibrationFiles = "Actuators\\BCI\\CalibrationFiles";
            Calibration_CalibrationFileId = "";
            Calibration_ForceRecalibrateFromFile = false;

            DataCollection_TestID = 1;

            DataParser_idxSampleIdx = 0;
            DataParser_idxStartEEGData = 2;
            DataParser_numEEGDataChannels = 8;
            DataParser_idxTriggerSignal_Hw = 16;
            DataParser_idxTriggerSignal_Sw = 24;
            DataParser_headerLinesToSkip = 7;
            DataParser_useSoftwareTrigers = false;
        }


        /// <summary>
        /// Loads settings from the file
        /// </summary>
        /// <returns>Settings object</returns>
        public static Settings Load()
        {
            Settings retVal = PreferencesBase.Load<Settings>(SettingsFilePath);
            Save(retVal, SettingsFilePath);
            return retVal;
        }

        /// <summary>
        /// Saves the current settings to the preferences file
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return Save<Settings>(this, SettingsFilePath);
        }

        /// <summary>
        /// Loads settings from the file
        /// </summary>
        /// <returns>Settings object</returns>
        public static Settings LoadDefaults()
        {
            return PreferencesBase.LoadDefaults<Settings>();
        }

    }
}
