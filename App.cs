using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using TrayApplication.Action;
using TrayApplication.Config;

namespace TrayApplication
{
    public class App : IDisposable
    {
        private const string CONFIG = "tray-configuration.json";

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

        private IList<string> _services = new List<string>();

        private readonly IDictionary<string, State> _states = new Dictionary<string, State>
        {
            {"stopped", new State()},
            {"partial", new State()},
            {"started", new State()},
        };

        private IDictionary<string, AbstractAction> _actions = new Dictionary<string, AbstractAction>();
        private ConfigTray _config;
        private State _state;

        private readonly ResourceManager _resourceManager;

        private event EventHandler _onUpdate;

        public App()
        {
            _resourceManager = new ResourceManager(GetType().Namespace);

            LoadConfig();
            Build();
            Update();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void LoadConfig()
        {
            _config = File.Exists(CONFIG)
                ? JsonConvert.DeserializeObject<ConfigTray>(File.ReadAllText(CONFIG))
                : new ConfigTray();
        }

        public void Build()
        {
            //TODO use clear + add & try to use without foreach
            _services = _config.services;
            BuildStates();
            
            _menuLeft.Items.Clear();
            BuildMenu(_menuLeft.Items, _config.menu_left);

            _menuRight.Items.Clear();
            BuildMenu(_menuRight.Items, _config.menu_left);
        }

        public void BuildStates()
        {
            foreach (var stateName in _states.Keys)
            {
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
        }

        public AbstractAction BuildAction(ConfigAction configAction)
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
                    return null;
            }
        }

        private void BuildMenu(ToolStripItemCollection collection, IEnumerable<ConfigMenuItem> config)
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
                            Font = new Font(SystemFonts.DefaultFont.FontFamily, SystemFonts.DefaultFont.Size, FontStyle.Bold)
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

                        BuildMenu(item.DropDownItems, configMenuItem.children);
                        break;
                }
            }
        }

        public void Update()
        {
            if (_onUpdate != null) _onUpdate(this, EventArgs.Empty);
        }
    }
}