using System;
using System.Linq;
using Models;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationSettingsService : BaseRepository, IApplicationSettingsRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSettingsService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ApplicationSettingsService(dominoespropertiesContext context) : base(context)
        {

        }

        /// <summary>
        /// Gets the name of the application settings by.
        /// </summary>
        /// <param name="settingname">The settingname.</param>
        /// <returns></returns>
        public ApplicationSetting GetApplicationSettingsByName(string settingname)
        {
            return _context.ApplicationSettings.FirstOrDefault(x => x.SettingName == settingname);
        }

        public ApplicationSetting UpdateApplicationSettingsByName(ApplicationSetting applicationSettings)
        {
            try
            {
                if (applicationSettings != null)
                {
                    _context.ApplicationSettings.Update(applicationSettings);
                    _context.SaveChanges();
                    return applicationSettings;
                }

            }
            catch (Exception)
            {
            }
            return null;
        }
    }
}
