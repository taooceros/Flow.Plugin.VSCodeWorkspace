// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Flow.Plugin.VSCodeWorkspaces.Properties;
using Flow.Plugin.VSCodeWorkspaces.VSCodeHelper;

namespace Flow.Plugin.VSCodeWorkspaces.WorkspacesHelper
{
    public record VSCodeWorkspace
    {
        public string Path { get; init; }

        public string RelativePath { get; init; }

        public string FolderName { get; init; }

        public string ExtraInfo { get; init; }

        public TypeWorkspace TypeWorkspace { get; init; }

        public VSCodeInstance VSCodeInstance { get; init; }

        public string WorkspaceTypeToString()
        {
            return TypeWorkspace switch
            {
                TypeWorkspace.Local => Resources.TypeWorkspaceLocal,
                TypeWorkspace.Codespaces => "Codespaces",
                TypeWorkspace.RemoteContainers => Resources.TypeWorkspaceContainer,
                TypeWorkspace.RemoteSSH => "SSH",
                TypeWorkspace.RemoteWSL => "WSL",
                TypeWorkspace.DevContainer => Resources.TypeWorkspaceDevContainer,
                _ => string.Empty
            };

        }
    }

    public enum TypeWorkspace
    {
        Local = 1,
        Codespaces = 2,
        RemoteWSL = 3,
        RemoteSSH = 4,
        RemoteContainers = 5,
        DevContainer = 6,
    }
}
