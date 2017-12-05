using System.Drawing;
using System.IO;
using System.Reflection;
using NLog;

namespace TrayApplication
{
    public class ResourceManager
    {
        private readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        private readonly string _namespaceName;
        
        public ResourceManager(string namespaceName)
        {
            _namespaceName = namespaceName;
        }

        public Icon GetIcon(string name)
        {
            if (name.ToLower().EndsWith(".ico"))
            {
                Stream stream = _assembly.GetManifestResourceStream(_namespaceName + "." + name);
                
                if (stream != null) return new Icon(stream);
                if (File.Exists(name)) return new Icon(name);
                
                return null;
            }

            Image image = GetImage(name);
            if (image != null)
            {
                return Icon.FromHandle(new Bitmap(image).GetHicon());
            }
            return null;
        }

        public Image GetImage(string name)
        {
            Stream stream = _assembly.GetManifestResourceStream(_namespaceName + "." + name);
            
            if (stream != null) return Image.FromStream(stream);
            
            if (File.Exists(name)) return Image.FromFile(name);
            
            return null;
        }
    }
}