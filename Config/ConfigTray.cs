using System.Collections.Generic;

namespace TrayApplication.Config
{
    public class ConfigTray
    {
        /// <summary>
        /// Application name
        /// </summary>
        public string name;
        
        /// <summary>
        /// Application icon
        /// </summary>
        public string icon;
        
        /// <summary>
        /// Application services
        /// </summary>
        public IList<string> services = new List<string>();
        
        /// <summary>
        /// Application states
        /// </summary>
        public IDictionary<string, ConfigState> states = new Dictionary<string, ConfigState>();
        
        /// <summary>
        /// Application custom actions
        /// </summary>
        public IDictionary<string, ConfigAction> actions = new Dictionary<string, ConfigAction>();
        
        /// <summary>
        /// Application menu on mouse left button click
        /// </summary>
        public IList<ConfigMenuItem> menu_left = new List<ConfigMenuItem>();
        
        /// <summary>
        /// Application menu on mouse right button click
        /// </summary>
        public IList<ConfigMenuItem> menu_right = new List<ConfigMenuItem>();
    }
}