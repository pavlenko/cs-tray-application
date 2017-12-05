using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;

namespace TrayApplication.Action
{
    public class ServiceStopAction : AbstractServiceAction
    {
        public ServiceStopAction(IDictionary<string, object> arguments) : base(arguments)
        {}

        public override string GetIcon()
        {
            return "Resources.icons.icon-stop.ico";
        }

        public override bool IsEnabled()
        {
            return ServiceChecker.IsRunning(name);
        }

        protected override void _doExecute()
        {
            var controller = new ServiceController(name);
            controller.Stop();
            if (wait_until_terminated)
            {
                controller.WaitForStatus(ServiceControllerStatus.Stopped);
            }
        }
    }
}