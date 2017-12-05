using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace TrayApplication.Action
{
    public abstract class AbstractAction
    {
        protected AbstractAction(IDictionary<string, object> arguments)
        {
            foreach (var name in arguments.Keys)
            {
                var info = GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (info == null) continue;
                
                if (info.FieldType == typeof(bool))
                {
                    info.SetValue(this, (string) arguments[name] == "1" || (string) arguments[name] == "true");
                }
                else
                {
                    info.SetValue(this, arguments[name]);
                }
            }
        }
        
        public virtual string GetIcon()
        {
            return null;
        }

        public virtual bool IsEnabled()
        {
            return true;
        }

        public abstract void Execute(object sender, EventArgs e);
    }
}