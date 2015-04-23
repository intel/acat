using System;
using ACAT.Lib.Core.Utility;

namespace ACAT.Extensions.Default.WordPredictors.PresageWCF
{
    [Serializable]
    public class Settings : PreferencesBase
    {
        [NonSerialized]
        public static String PreferencesFilePath;

        public String DatabaseFileName = "database.db";
        public String LearningDatabaseFileName = "learn.db";
        public String ConfigFileName = "presage.xml";
        public bool StartPresageIfNotRunning = true;

        public static Settings Load()
        {
            Settings retVal = Load<Settings>(PreferencesFilePath);
            Save(retVal, PreferencesFilePath);
            return retVal;
        }

        override public bool Save()
        {
            return Save<Settings>(this, PreferencesFilePath);
        }
    }
}
