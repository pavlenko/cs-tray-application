using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.ServiceProcess;

namespace TrayApplication.Action
{
    /// <summary>
    /// <see href="http://dotnetstep.blogspot.com/2009/06/programmatically-install-window-service.html" />
    /// </summary>
    public class ServiceInstallAction : AbstractServiceInstallerAction
    {
        public ServiceInstallAction(IDictionary<string, object> arguments) : base(arguments)
        {}

        public override string GetIcon()
        {
            return "Resources.icons.icon-settings.ico";
        }

        public override bool IsEnabled()
        {
            return !ServiceChecker.IsInstalled(name);
        }

        protected override void _doExecute()
        {
            // Initialize installer process
            var process = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalService
            };

            Console.WriteLine(bin);
            Console.WriteLine(name);
            Console.WriteLine(label);
            Console.WriteLine(description);
            Console.WriteLine(log);

            // Initialize command line arguments
            string[] commandLine = {string.Format("/assemblypath={0}", Path.GetFullPath(bin))};

            // Configure service
            var installer = new ServiceInstaller
            {
                Context     = new InstallContext(log, commandLine),
                DisplayName = label,
                Description = description,
                ServiceName = name,
                StartType   = ServiceStartMode.Manual,
                Parent      = process
            };

            // Execute
            installer.Install(new ListDictionary());
        }
    }
}