using System;
using ACAT.Lib.Core.Utility;

namespace ACAT.Extensions.Default.WordPredictors.PresageWCF
{
    [Serializable]
    public class Settings : PreferencesBase
    {
        [NonSerialized]
        public static String PreferencesFilePath;

        public String DatabaseFileName = Strings.database_db;
        public String LearningDatabaseFileName = Strings.learn_db;
        public String ConfigFileName = Strings.presage_xml;
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
