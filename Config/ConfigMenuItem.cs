using System.Collections.Generic;
using TrayApplication.Exception;

namespace TrayApplication.Config
{
    public class ConfigMenuItem
    {
        /// <summary>
        /// Item type (label, separator, normal)
        /// </summary>
        public string type;
        
        /// <summary>
        /// Item label
        /// </summary>
        public string label;
        
        /// <summary>
        /// Item icon
        /// </summary>
        public string icon;
        
        /// <summary>
        /// Item action definition, ignored if has children
        /// </summary>
        public ConfigAction action_def;
        
        /// <summary>
        /// Item action reference, ignored if has children
        /// </summary>
        public string action_ref;

        /// <summary>
        /// Item children
        /// </summary>
        public IList<ConfigMenuItem> children = new List<ConfigMenuItem>();
    }
}