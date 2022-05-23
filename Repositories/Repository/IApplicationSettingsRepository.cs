using System;
using Models.Models;

namespace Repositories.Repository
{
    public interface IApplicationSettingsRepository
    {
        ApplicationSetting GetApplicationSettingsByName(String settingname);
        ApplicationSetting UpdateApplicationSettingsByName(ApplicationSetting applicationSettings);
    }
}
