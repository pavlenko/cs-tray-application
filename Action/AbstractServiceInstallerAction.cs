using System.Collections.Generic;

namespace TrayApplication.Action
{
    public abstract class AbstractServiceInstallerAction : AbstractServiceAction
    {
        /// <summary>
        /// Service executable
        /// </summary>
        protected string bin;

        /// <summary>
        /// Servide display name
        /// </summary>
        protected string label;

        /// <summary>
        /// Service description
        /// </summary>
        protected string description;

        /// <summary>
        /// Service installer log
        /// </summary>
        protected string log = "service.log";

        protected AbstractServiceInstallerAction(IDictionary<string, object> arguments) : base(arguments)
        {}
    }
}