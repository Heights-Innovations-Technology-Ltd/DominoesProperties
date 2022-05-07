using System;
namespace Models
{
    public partial class ApplicationSetting
    {
        public long Id { get; set; }
        public bool TestingMode { get; set; }
        public string TestingEmail { get; set; }
        public string SettingName { get; set; }
    }
}
