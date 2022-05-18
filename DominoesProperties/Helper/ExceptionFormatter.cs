using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Repositories.Repository;

namespace DominoesProperties.Helper
{
    public class ExceptionFormatter
    {
        public ExceptionFormatter(ILoggerManager logger, Exception ex)
        {
            object[] errors = new object[2];
            errors[0] = new KeyValuePair<string, string>("message", ex.Message);
            errors[1] = new KeyValuePair<string, string>("code", ex.StackTrace);
            JObject jObject = new(errors);
            logger.LogDebug(jObject.ToString());
        }
    }
}
