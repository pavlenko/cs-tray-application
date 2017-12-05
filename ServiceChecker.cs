using System.Linq;
using System.ServiceProcess;

namespace TrayApplication
{
    public class ServiceChecker
    {
        public static bool IsStatus(string name, ServiceControllerStatus status)
        {
            var controller = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == name);
            return controller != null && controller.Status == status;
        }

        public static ServiceController GetController(string name)
        {
            return ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == name);
        }
        
        public static bool IsInstalled(string name)
        {
            return GetController(name) != null;
        }
        
        public static bool IsRunning(string name)
        {
            return IsInstalled(name) && GetController(name).Status == ServiceControllerStatus.Running;
        }
        
        public static bool IsStopped(string name)
        {
            return IsInstalled(name) && GetController(name).Status == ServiceControllerStatus.Stopped;
        }
    }
}