using System.Collections.Generic;

namespace TrayApplication.Action
{
    public class ActionManager
    {
        public IDictionary<string, AbstractAction> actions = new Dictionary<string, AbstractAction>();
    }
}