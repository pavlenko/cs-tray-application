using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace TrayApplication.Action
{
    /// <summary>
    /// <see href="http://dotnetstep.blogspot.com/2009/06/programmatically-install-window-service.html" />
    /// </summary>
    public class ServiceUninstallAction : AbstractServiceInstallerAction
    {
        public ServiceUninstallAction(IDictionary<string, object> arguments) : base(arguments)
        {}

        public override string GetIcon()
        {
            return "Resources.icons.icon-trash-empty.ico";
        }
        
        public override bool IsEnabled()
        {
            return ServiceChecker.IsInstalled(name);
        }

        protected override void _doExecute()
        {
            // Configure service
            var installer = new ServiceInstaller
            {
                Context     = new InstallContext(log, null),
                ServiceName = name
            };

            // Execute
            installer.Uninstall(null); 
        }
    }
}