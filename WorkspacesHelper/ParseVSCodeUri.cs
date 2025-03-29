// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;

namespace Flow.Plugin.VSCodeWorkspaces.WorkspacesHelper
{
    public class ParseVSCodeUri
    {
        private static readonly Regex LocalWorkspace = new Regex("^file:///(.+)$", RegexOptions.Compiled);

        private static readonly Regex RemoteSSHWorkspace = new Regex(@"^vscode-remote://ssh-remote\+(.+?(?=\/))(.+)$", RegexOptions.Compiled);

        private static readonly Regex RemoteWSLWorkspace = new Regex(@"^vscode-remote://wsl\+(.+?(?=\/))(.+)$", RegexOptions.Compiled);

        private static readonly Regex CodespacesWorkspace = new Regex(@"^vscode-remote://vsonline\+(.+?(?=\/))(.+)$", RegexOptions.Compiled);

        private static readonly Regex DevContainerWorkspace = new Regex(@"^vscode-remote://dev-container\+(.+?(?=\/))(.+)$", RegexOptions.Compiled);

        public static (WorkspaceLocation? workspaceLocation, string MachineName, string Path) GetTypeWorkspace(string uri)
        {
            if (LocalWorkspace.IsMatch(uri))
            {
                var match = LocalWorkspace.Match(uri);

                if (match.Groups.Count > 1)
                {
                    return (WorkspaceLocation.Local, null, match.Groups[1].Value);
                }
            }
            else if (RemoteSSHWorkspace.IsMatch(uri))
            {
                var match = RemoteSSHWorkspace.Match(uri);

                if (match.Groups.Count > 1)
                {
                    return (WorkspaceLocation.RemoteSSH, match.Groups[1].Value, match.Groups[2].Value);
                }
            }
            else if (RemoteWSLWorkspace.IsMatch(uri))
            {
                var match = RemoteWSLWorkspace.Match(uri);

                if (match.Groups.Count > 1)
                {
                    return (WorkspaceLocation.RemoteWSL, match.Groups[1].Value, match.Groups[2].Value);
                }
            }
            else if (CodespacesWorkspace.IsMatch(uri))
            {
                var match = CodespacesWorkspace.Match(uri);

                if (match.Groups.Count > 1)
                {
                    return (WorkspaceLocation.Codespaces, null, match.Groups[2].Value);
                }
            }
            else if (DevContainerWorkspace.IsMatch(uri))
            {
                var match = DevContainerWorkspace.Match(uri);

                if (match.Groups.Count > 1)
                {
                    return (WorkspaceLocation.DevContainer, null, match.Groups[2].Value);
                }
            }

            return (null, null, null);
        }
    }
}
