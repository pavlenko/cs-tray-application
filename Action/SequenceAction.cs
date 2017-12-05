using System;
using System.Collections.Generic;
using NLog;

namespace TrayApplication.Action
{
    public class SequenceAction : AbstractAction
    {
        private ActionManager _actionManager;//TODO remove
        
        protected IList<string> actions = new List<string>();

        // Add constructor with actions list ref
        public SequenceAction(IDictionary<string, object> arguments) : base(arguments)
        {}

        public override void Execute(object sender, EventArgs e)
        {
            if (_actionManager == null)//TODO use actions list from constructor
            {
                throw new System.Exception("Action manager not set");
            }

            foreach (string action in actions)
            {
                if (!_actionManager.actions.ContainsKey(action))
                {
                    throw new System.Exception("Unknown action " + action);
                }

                _actionManager.actions[action].Execute(sender, e);
            }
        }
    }
}