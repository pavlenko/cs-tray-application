using System.Collections.Generic;
using System.ServiceProcess;

namespace TrayApplication.Action
{
    public abstract class AbstractServiceAction : AbstractRunAction
    {
        /// <summary>
        /// Service name
        /// </summary>
        protected string name;

        protected AbstractServiceAction(IDictionary<string, object> arguments) : base(arguments)
        {}
    }
}