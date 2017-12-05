using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using TrayApplication.Action;
using TrayApplication.Config;
using Timer = System.Windows.Forms.Timer;

//TODO move application code outside of main function -> Main.cs + Application.cs
namespace TrayApplication
{
    public class TrayApplication : Form
    {
        [STAThread]
        public static void Main()
        {
            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                var info = new ProcessStartInfo
                {
                    //TODO hide window in production mode
                    UseShellExecute  = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName         = Application.ExecutablePath,
                    Verb             = "runas"
                };

                try
                {
                    Process.Start(info);
                    Application.Exit();
                }
                catch (Win32Exception ex)
                {
                    MessageBox.Show(
                        "This utility requires elevated priviledges to complete correctly.",
                        "Error: UAC Authorisation Required",
                        MessageBoxButtons.OK
                    );
                    return;
                }
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new TrayApplication());
            }
            return;
            /*AppDomain.CurrentDomain.UnhandledException += ExceptionEventHandler;


			//TODO configure logger
			// Step 1. Create configuration object
            var config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration
            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Step 3. Set target properties
            consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
            fileTarget.FileName = "${basedir}/file.txt";
            fileTarget.Layout = "${message}";

            // Step 4. Define rules
            var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;*/

            Application.Run(new TrayApplication());
        }

        /*public static void ExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
        {
            //_Exception ex = (_Exception) e.ExceptionObject;
            //MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButtons.OK);//TODO log exception
            //Environment.Exit(1);
            LogManager.GetCurrentClassLogger().Error((System.Exception) e.ExceptionObject);
        }*/

        private readonly IDictionary<string, string> _labels = new Dictionary<string, string>
        {
            {"stopped", "None of services started"},
            {"partial", "Some of services started"},
            {"started", "All services started"}
        };

        private readonly IDictionary<string, string> _icons = new Dictionary<string, string>
        {
            {"stopped", "Resources.icons.state-stopped.ico"},
            {"partial", "Resources.icons.state-partial.ico"},
            {"started", "Resources.icons.state-started.ico"}
        };

        private event EventHandler _onUpdate;

        private State _state;

        private readonly IList<string> _services = new List<string>();
        private readonly IDictionary<string, State> _states = new Dictionary<string, State>();
        private readonly IDictionary<string, AbstractAction> _actions = new Dictionary<string, AbstractAction>();

        private readonly ResourceManager _resourceManager;

        private const string _path = "tray-configuration.json";
        private readonly ConfigTray _config;

        private readonly ContextMenuStrip _menuLeft;
        private readonly ContextMenuStrip _menuRight;
        private readonly NotifyIcon _icon;

        private readonly Timer _timer;

        public TrayApplication()
        {
            // Initialize managers
            _resourceManager = new ResourceManager(GetType().Namespace);

            // Initialize UI
            _icon      = new NotifyIcon();
            _menuLeft  = new ContextMenuStrip();
            _menuRight = new ContextMenuStrip();

            // Load configuration
            _config = File.Exists(_path)
                ? JsonConvert.DeserializeObject<ConfigTray>(File.ReadAllText(_path))
                : new ConfigTray();

            // Build UI and global actions
            Build();

            UpdateState();

            // Config tray icon
            _icon.Text    = _state.label;
            _icon.Icon    = _state.icon;
            _icon.Visible = true;

            _icon.MouseClick += OnClick;
            _onUpdate += (sender, e) => { _icon.Text = _state.label; _icon.Icon = _state.icon; };

            //TODO run startup action

            // Init check state of services timer
            _timer = new Timer
            {
                Interval = 200,
                Enabled  = true
            };

            _timer.Tick += (sender, e) => UpdateState();
        }

        /// <summary>
        /// Handle click on tray icon - show left button or right button menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClick(object sender, MouseEventArgs e)
        {
            MethodInfo info = typeof(NotifyIcon).GetMethod(
                "ShowContextMenu",
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            if (e.Button == MouseButtons.Left)
            {
                _icon.ContextMenuStrip = _menuLeft;
                info.Invoke(_icon, null);
            }
            else if (e.Button == MouseButtons.Right)
            {
                _icon.ContextMenuStrip = _menuRight;
                info.Invoke(_icon, null);
            }
        }

        private void UpdateState()
        {
            State state;
            var total   = _services.Count;
            var running = _services.Count(ServiceChecker.IsRunning);

            if (total > 0)
            {
                if (running == 0)
                {
                    state = _states["stopped"];
                }
                else if (total > running)
                {
                    state = _states["partial"];
                }
                else
                {
                    state = _states["started"];
                }
            }
            else
            {
                state = _states["started"];
            }

            _state = state;

            if (_onUpdate != null) _onUpdate.Invoke(null, null);
        }

        /// <summary>
        /// Prevent window visibility on application load
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            Visible       = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        /// <summary>
        /// Cleanup resources
        /// </summary>
        /// <param name="isDisposing"></param>
        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _icon.Dispose();
                _timer.Dispose();
            }

            base.Dispose(isDisposing);
        }

        /// <summary>
        /// Build UI and global actions
        /// </summary>
        private void Build()
        {
            _services.Clear();
            foreach (var configService in _config.services)
            {
                _services.Add(configService);
            }

            _states.Clear();
            foreach (var stateName in new []{"stopped", "partial", "started"})
            {
                _states[stateName] = new State();

                if (_config.states.ContainsKey(stateName))
                {
                    _states[stateName].icon  = _resourceManager.GetIcon(_config.states[stateName].icon ?? "");
                    _states[stateName].label = _config.states[stateName].label;
                }

                if (_states[stateName].icon == null)
                {
                    _states[stateName].icon = _resourceManager.GetIcon(_icons[stateName]);
                }

                if (_states[stateName].label == null)
                {
                    _states[stateName].label = _labels[stateName];
                }
            }

            _actions.Clear();
            foreach (var configAction in _config.actions)
            {
                _actions[configAction.Key] = BuildAction(configAction.Value);
            }

            _menuLeft.Items.Clear();
            BuildItems(_menuLeft.Items, _config.menu_left);

            _menuRight.Items.Clear();
            BuildItems(_menuRight.Items, _config.menu_left);
        }

        /// <summary>
        /// Build menu items and add to menu node
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="config"></param>
        private void BuildItems(ToolStripItemCollection collection, IEnumerable<ConfigMenuItem> config)
        {
            foreach (var configMenuItem in config)
            {
                switch (configMenuItem.type)
                {
                    case "separator":
                        collection.Add(new ToolStripSeparator());
                        break;
                    case "label":
                        collection.Add(new ToolStripLabel
                        {
                            Text = configMenuItem.label,
                            Font = new Font(DefaultFont.FontFamily, DefaultFont.Size, FontStyle.Bold)
                        });
                        break;
                    default:
                        var item = (ToolStripMenuItem) collection.Add(configMenuItem.label);

                        if (configMenuItem.icon != null && File.Exists(configMenuItem.icon))
                        {
                            item.Image = _resourceManager.GetImage(configMenuItem.icon);
                        }
                        else if (configMenuItem.children.Count > 0)
                        {
                            item.Image = _resourceManager.GetImage("Resources.icons.icon-folder.ico");
                        }

                        var itemAction = BuildAction(configMenuItem.action_def);

                        if (
                            itemAction == null &&
                            configMenuItem.action_ref != null &&
                            _actions.ContainsKey(configMenuItem.action_ref))
                        {
                            itemAction = _actions[configMenuItem.action_ref];
                        }

                        if (itemAction != null)
                        {
                            item.Enabled = itemAction.IsEnabled();
                            item.Image   = item.Image ?? _resourceManager.GetImage(itemAction.GetIcon());

                            item.Click += itemAction.Execute;
                            _onUpdate  += (sender, e) => { item.Enabled = itemAction.IsEnabled(); };
                        }

                        BuildItems(item.DropDownItems, configMenuItem.children);
                        break;
                }
            }
        }

        /// <summary>
        /// Build action handler
        /// </summary>
        /// <param name="configAction"></param>
        /// <returns></returns>
        private AbstractAction BuildAction(ConfigAction configAction)
        {
            if (configAction == null) return null;

            switch (configAction.type)
            {
                case "service_start":
                    return new ServiceStartAction(configAction.arguments);
                case "service_stop":
                    return new ServiceStopAction(configAction.arguments);
                case "service_restart":
                    return new ServiceRestartAction(configAction.arguments);
                case "service_install":
                    return new ServiceInstallAction(configAction.arguments);
                case "service_uninstall":
                    return new ServiceUninstallAction(configAction.arguments);
                case "run":
                    return new RunAction(configAction.arguments);
                case "exit":
                    return new ExitAction(configAction.arguments);
                case "sequence":
                    return new SequenceAction(configAction.arguments);
                default:
                    //TODO unknown action type message
                    return null;
            }
        }
    }
}