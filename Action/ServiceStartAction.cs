using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;

namespace TrayApplication.Action
{
    public class ServiceStartAction : AbstractServiceAction
    {
        public ServiceStartAction(IDictionary<string, object> arguments) : base(arguments)
        {}

        public override string GetIcon()
        {
            return "Resources.icons.icon-play.ico";
        }

        public override bool IsEnabled()
        {
            return ServiceChecker.IsStopped(name);
        }

        protected override void _doExecute()
        {
            var controller = new ServiceController(name);
            controller.Start();
            if (wait_until_terminated)
            {
                controller.WaitForStatus(ServiceControllerStatus.Running);
            }
        }
    }
}