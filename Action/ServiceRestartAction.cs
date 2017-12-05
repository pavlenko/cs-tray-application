using System.Collections.Generic;
using System.ServiceProcess;

namespace TrayApplication.Action
{
    public class ServiceRestartAction : AbstractServiceAction
    {
        public ServiceRestartAction(IDictionary<string, object> arguments) : base(arguments)
        {}

        public override string GetIcon()
        {
            return "Resources.icons.icon-refresh.ico";
        }

        public override bool IsEnabled()
        {
            return ServiceChecker.IsInstalled(name);
        }

        protected override void _doExecute()
        {
            var controller = new ServiceController(name);
            if (controller.Status != ServiceControllerStatus.Stopped)
            {
                controller.Stop();
                controller.WaitForStatus(ServiceControllerStatus.Stopped);
            }
            
            if (controller.Status != ServiceControllerStatus.Running)
            {
                controller.Start();
                controller.WaitForStatus(ServiceControllerStatus.Running);
            }
        }
    }
}