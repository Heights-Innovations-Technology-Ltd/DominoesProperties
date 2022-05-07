using System;
using System.Collections.Generic;

#nullable disable

namespace Models.Models
{
    public partial class ApplicationSetting
    {
        public int Id { get; set; }
        public bool TestingMode { get; set; }
        public string TestingEmail { get; set; }
        public string SettingName { get; set; }
    }
}
