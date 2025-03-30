// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Flow.Plugin.VSCodeWorkspaces.Properties;
using Flow.Plugin.VSCodeWorkspaces.VSCodeHelper;

namespace Flow.Plugin.VSCodeWorkspaces.WorkspacesHelper
{
    public record VsCodeWorkspace
    {
        public PathString Path { get; init; }

        public PathString RelativePath { get; init; }

        public PathString FolderName { get; init; }
        
        public string Label { get; init; }

        public string ExtraInfo { get; init; }

        public WorkspaceLocation WorkspaceLocation { get; init; }
        
        public WorkspaceType WorkspaceType { get; init; }

        public VSCodeInstance VSCodeInstance { get; init; }

        public string WorkspaceTypeToString()
        {
            return WorkspaceLocation switch
            {
                WorkspaceLocation.Local => Resources.TypeWorkspaceLocal,
                WorkspaceLocation.Codespaces => "Codespaces",
                WorkspaceLocation.RemoteContainers => Resources.TypeWorkspaceContainer,
                WorkspaceLocation.RemoteSSH => "SSH",
                WorkspaceLocation.RemoteWSL => "WSL",
                WorkspaceLocation.DevContainer => Resources.TypeWorkspaceDevContainer,
                _ => string.Empty
            };
        }
    }

    public enum WorkspaceLocation
    {
        Local = 1,
        Codespaces = 2,
        RemoteWSL = 3,
        RemoteSSH = 4,
        RemoteContainers = 5,
        DevContainer = 6,
    }

    public enum WorkspaceType
    {
        Folder = 1,
        Workspace = 2,
    }
}
