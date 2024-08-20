using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace ACAT.Lib.Core.DialogSenseManagement
{
    /// <summary>
    /// Maintains a list of preferred spell checkers
    /// specific for each culture (language)
    /// The class is serialized to a file and loaded
    /// from a file as well
    /// </summary>
    [Serializable]
    public class PreferredDialogSenseAgents : PreferencesBase
    {
        /// <summary>
        /// Path to the file to serialize to
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String FilePath;

        /// <summary>
        /// List of preferred word predictors
        /// </summary>
        public List<PreferredDialogSenseAgent> DialogSenseAgents;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public PreferredDialogSenseAgents()
        {
            DialogSenseAgents = new List<PreferredDialogSenseAgent>();
        }

        /// <summary>
        /// Returns the list of the preferred spellchekers
        /// </summary>
        public IEnumerable<PreferredDialogSenseAgent> List
        {
            get { return DialogSenseAgents; }
        }

        /// <summary>
        /// Deserializes list of spellcheckers from the file and
        /// returns an instance of this class
        /// </summary>
        /// <returns>an object of this class</returns>
        public static PreferredDialogSenseAgents Load()
        {
            return Load<PreferredDialogSenseAgents>(FilePath);
        }

        /// <summary>
        /// Returns the ID of the preferred spellchecker
        /// for the specified culture
        /// </summary>
        /// <param name="ci">culture</param>
        /// <returns>id, guid.empty if none found</returns>
        public Guid GetByCulture(CultureInfo ci)
        {
            if (ci == null)
            {
                return getByLanguage(String.Empty);
            }

            var guid = getByLanguage(ci.Name);

            if (Guid.Equals(guid, Guid.Empty))
            {
                guid = getByLanguage(ci.TwoLetterISOLanguageName);
            }

            return guid;
        }

        /// <summary>
        /// Persists this object to a file
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            if (!String.IsNullOrEmpty(FilePath))
            {
                return Save(this, FilePath);
            }

            return false;
        }

        /// <summary>
        /// Sets the specified id of the spellchecker as the
        /// default for the language
        /// </summary>
        /// <param name="language">Language (culture)</param>
        /// <param name="guid">ID of the spellchecker</param>
        /// <returns></returns>
        public bool SetAsDefault(String language, Guid guid)
        {
            var preferredDialogSenseAgent = DialogSenseAgents.FirstOrDefault(agent => String.Compare(language, agent.Language, true) == 0);
            if (preferredDialogSenseAgent != null)
            {
                preferredDialogSenseAgent.ID = guid;
            }
            else
            {
                DialogSenseAgents.Add(new PreferredDialogSenseAgent(guid, language));
            }

            return true;
        }

        /// <summary>
        /// Gets the preferred spellchecker for the specified language
        /// </summary>
        /// <param name="language">Language (culture)</param>
        /// <returns>ID, Guid.empty if none found</returns>
        private Guid getByLanguage(String language)
        {
            foreach (var agent in DialogSenseAgents)
            {
                if (String.IsNullOrEmpty(language))
                {
                    if (String.IsNullOrEmpty(agent.Language))
                    {
                        return agent.ID;
                    }
                }
                else if (String.Compare(agent.Language, language, true) == 0)
                {
                    return agent.ID;
                }
            }

            return Guid.Empty;
        }
    }
}