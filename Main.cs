// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Flow.Plugin.VSCodeWorkspaces
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using Flow.Launcher.Plugin;
    using Properties;
    using RemoteMachinesHelper;
    using VSCodeHelper;
    using WorkspacesHelper;

    public class Main : IPlugin, IPluginI18n
    {
        internal static PluginInitContext _context { get; private set; }

        public string Name => GetTranslatedPluginTitle();

        public string Description => GetTranslatedPluginDescription();


        private readonly VSCodeWorkspacesApi _workspacesApi = new VSCodeWorkspacesApi();

        private readonly VSCodeRemoteMachinesApi _machinesApi = new VSCodeRemoteMachinesApi();

        public List<Result> Query(Query query)
        {
            var results = new List<Result>();

            // Search opened workspaces
            _workspacesApi.Workspaces.ForEach(a =>
            {
                var title = $"{a.FolderName}";

                var typeWorkspace = a.WorkspaceTypeToString();
                if (a.TypeWorkspace != TypeWorkspace.Local)
                {
                    title = $"{title}{(a.ExtraInfo != null ? $" - {a.ExtraInfo}" : string.Empty)} ({typeWorkspace})";
                }

                var tooltip = $"{Resources.Workspace}{(a.TypeWorkspace != TypeWorkspace.Local ? $" {Resources.In} {typeWorkspace}" : string.Empty)}: {SystemPath.RealPath(a.RelativePath)}";

                results.Add(new Result
                {
                    Title = title,
                    SubTitle = $"{Resources.Workspace}{(a.TypeWorkspace != TypeWorkspace.Local ? $" {Resources.In} {typeWorkspace}" : string.Empty)}: {SystemPath.RealPath(a.RelativePath)}",
                    Icon = a.VSCodeInstance.WorkspaceIcon,
                    TitleToolTip = tooltip,
                    Action = c =>
                    {
                        bool hide;
                        try
                        {
                            var process = new ProcessStartInfo
                            {
                                FileName = a.VSCodeInstance.ExecutablePath,
                                UseShellExecute = true,
                                Arguments = $"--folder-uri {a.Path}",
                                WindowStyle = ProcessWindowStyle.Hidden,
                            };
                            Process.Start(process);

                            hide = true;
                        }
                        catch (Win32Exception)
                        {
                            var name = $"Plugin: {_context.CurrentPluginMetadata.Name}";
                            var msg = "Can't Open this file";
                            _context.API.ShowMsg(name, msg, string.Empty);
                            hide = false;
                        }

                        return hide;
                    },
                    ContextData = a,
                });
            });

            // Search opened remote machines
            _machinesApi.Machines.ForEach(a =>
            {
                var title = $"{a.Host}";

                if (a.User != null && a.User != string.Empty && a.HostName != null && a.HostName != string.Empty)
                {
                    title += $" [{a.User}@{a.HostName}]";
                }

                var tooltip = Resources.SSHRemoteMachine;

                results.Add(new Result
                {
                    Title = title,
                    SubTitle = Resources.SSHRemoteMachine,
                    Icon = a.VSCodeInstance.RemoteIcon,
                    TitleToolTip = tooltip,
                    Action = c =>
                    {
                        bool hide;
                        try
                        {
                            var process = new ProcessStartInfo
                            {
                                FileName = a.VSCodeInstance.ExecutablePath,
                                UseShellExecute = true,
                                Arguments = $"--new-window --enable-proposed-api ms-vscode-remote.remote-ssh --remote ssh-remote+{((char)34) + a.Host + ((char)34)}",
                                WindowStyle = ProcessWindowStyle.Hidden,
                            };
                            Process.Start(process);

                            hide = true;
                        }
                        catch (Win32Exception)
                        {
                            var name = $"Plugin: {_context.CurrentPluginMetadata.Name}";
                            const string msg = "Can't Open this file";
                            _context.API.ShowMsg(name, msg, string.Empty);
                            hide = false;
                        }

                        return hide;
                    },
                    ContextData = a,
                });
            });


            if (query.ActionKeyword == string.Empty || (query.ActionKeyword != string.Empty && query.Search != string.Empty))
            {
                results = results.Where(r =>
                {
                    r.Score = _context.API.FuzzySearch(query.Search, r.Title).Score;
                    return r.Score > 0;
                }).ToList();
            }


            return results;
        }

        public void Init(PluginInitContext context)
        {
            _context = context;
            VSCodeInstances.LoadVSCodeInstances();

        }

        public string GetTranslatedPluginTitle()
        {
            return Resources.PluginTitle;
        }

        public string GetTranslatedPluginDescription()
        {
            return Resources.PluginDescription;
        }
    }
}