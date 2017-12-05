using System;
using System.Collections.Generic;

namespace TrayApplication.Config
{
    public class ConfigAction
    {
        /// <summary>
        /// Action type
        /// </summary>
        public string type;
        
        /// <summary>
        /// Action arguments
        /// </summary>
        public IDictionary<string, object> arguments = new Dictionary<string, object>();
    }
}