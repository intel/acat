////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ThemeRepository.cs
//
// Repository for Theme objects.
// The key is the full path to the theme configuration file.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ThemeManagement;
using Microsoft.Extensions.Logging;

namespace ACAT.Core.DataAccess
{
    /// <summary>
    /// Repository for <see cref="Theme"/> objects.
    /// Loading delegates to <see cref="Theme.Create(string, string, string, ILogger{Theme})"/>;
    /// saving is not supported for themes because theme assets are managed as
    /// directory trees by <see cref="ThemeManager"/>.
    /// </summary>
    public class ThemeRepository : RepositoryBase<Theme>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ThemeRepository"/>.
        /// </summary>
        /// <param name="logger">Optional logger.</param>
        public ThemeRepository(ILogger logger = null) : base(logger) { }

        /// <summary>
        /// Loads a theme from <paramref name="themeFilePath"/>.
        /// The theme name is derived from the containing directory name.
        /// </summary>
        public override Theme Load(string themeFilePath)
        {
            if (string.IsNullOrEmpty(themeFilePath))
            {
                Logger.LogWarning("ThemeRepository.Load called with null/empty path");
                return null;
            }

            string themeDir = System.IO.Path.GetDirectoryName(themeFilePath);
            string themeName = string.IsNullOrEmpty(themeDir)
                ? ThemeManager.DefaultThemeName
                : System.IO.Path.GetFileName(themeDir);

            Theme theme = Theme.Create(themeName, themeDir, themeFilePath);

            if (theme == null)
            {
                Logger.LogWarning("ThemeRepository could not load theme from {ThemeFilePath}", themeFilePath);
            }

            return theme;
        }

        /// <summary>
        /// Theme assets are directory-based and not written through this repository.
        /// This method always returns false and logs a warning.
        /// </summary>
        public override bool Save(Theme entity, string key)
        {
            Logger.LogWarning("ThemeRepository.Save is not supported – themes are managed as asset directories");
            return false;
        }

        /// <summary>
        /// Returns the default theme (no assets loaded).
        /// </summary>
        public override Theme GetDefault() => Theme.Create(ThemeManager.DefaultThemeName);
    }
}
