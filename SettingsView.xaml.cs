using Flow.Launcher.Plugin;
using Flow.Plugin.VSCodeWorkspaces.WorkspacesHelper;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Flow.Plugin.VSCodeWorkspaces
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private readonly PluginInitContext _context;
        private readonly Settings _settings;

        public SettingsView(PluginInitContext context, Settings settings)
        {
            _context = context;
            DataContext = _settings = settings;

            InitializeComponent();
        }

        public void Save(object sender = null, RoutedEventArgs e = null) => _context.API.SaveSettingJsonStorage<Settings>();

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            foreach (var ws in listView.SelectedItems.Cast<string>().ToArray())
            {
                _settings.CustomWorkspaces.Remove(ws);
            }
            Save();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var uri = addUri.Text;

                // System.Uri fails to parse vscode-remote://XXX+YYY URIs, skip them
                var type = ParseVSCodeUri.GetTypeWorkspace(uri).workspaceLocation;
                if (!type.HasValue || type.Value == WorkspaceLocation.Local)
                {
                    // Converts file paths to proper URI
                    uri = new Uri(uri).AbsoluteUri;
                }
                addUri.Clear();

                if (_settings.CustomWorkspaces.Contains(uri))
                {
                    return;
                }
                _settings.CustomWorkspaces.Add(uri);
                Save();
            }
            catch (Exception ex)
            {
                _context.API.ShowMsgError("Error", ex.Message);
            }
        }
    }
}
