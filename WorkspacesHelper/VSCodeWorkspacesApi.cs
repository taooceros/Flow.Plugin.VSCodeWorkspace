// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Flow.Plugin.VSCodeWorkspaces.VSCodeHelper;
using Microsoft.Data.Sqlite;

namespace Flow.Plugin.VSCodeWorkspaces.WorkspacesHelper
{
    public class VSCodeWorkspacesApi
    {
        public VSCodeWorkspacesApi()
        {
        }

        private VSCodeWorkspace ParseVSCodeUri(string uri, VSCodeInstance vscodeInstance)
        {
            if (uri != null && uri is string)
            {
                string unescapeUri = Uri.UnescapeDataString(uri);
                var typeWorkspace = WorkspacesHelper.ParseVSCodeUri.GetTypeWorkspace(unescapeUri);
                if (typeWorkspace.TypeWorkspace.HasValue)
                {
                    var folderName = Path.GetFileName(unescapeUri);

                    // Check we haven't returned '' if we have a path like C:\
                    if (string.IsNullOrEmpty(folderName))
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(unescapeUri);
                        folderName = dirInfo.Name.TrimEnd(':');
                    }

                    return new VSCodeWorkspace()
                    {
                        Path = uri,
                        RelativePath = typeWorkspace.Path,
                        FolderName = folderName,
                        ExtraInfo = typeWorkspace.MachineName,
                        TypeWorkspace = typeWorkspace.TypeWorkspace.Value,
                        VSCodeInstance = vscodeInstance,
                    };
                }
            }

            return null;
        }

        public List<VSCodeWorkspace> Workspaces
        {
            get
            {
                var results = new List<VSCodeWorkspace>();

                foreach (var vscodeInstance in VSCodeInstances.Instances)
                {
                    // storage.json contains opened Workspaces
                    var vscode_storage = Path.Combine(vscodeInstance.AppData, "storage.json");

                    if (File.Exists(vscode_storage))
                    {
                        var fileContent = File.ReadAllText(vscode_storage);

                        try
                        {
                            VSCodeStorageFile vscodeStorageFile = JsonSerializer.Deserialize<VSCodeStorageFile>(fileContent);

                            if (vscodeStorageFile != null)
                            {
                                // for previous versions of vscode
                                if (vscodeStorageFile.OpenedPathsList?.Workspaces3 != null)
                                {
                                    foreach (var workspaceUri in vscodeStorageFile.OpenedPathsList.Workspaces3)
                                    {
                                        var uri = ParseVSCodeUri(workspaceUri, vscodeInstance);
                                        if (uri != null)
                                        {
                                            results.Add(uri);
                                            continue;
                                        }
                                    }
                                }

                                // vscode v1.55.0 or later
                                if (vscodeStorageFile.OpenedPathsList?.Entries != null)
                                {
                                    foreach (var workspaceUri in vscodeStorageFile.OpenedPathsList.Entries.Select(x => x.FolderUri))
                                    {
                                        var uri = ParseVSCodeUri(workspaceUri, vscodeInstance);
                                        if (uri != null)
                                        {
                                            results.Add(uri);
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var message = $"Failed to deserialize ${vscode_storage}";
                            Main._context.API.LogException("VSCodeWorkspaceApi", message, ex);
                        }
                    }

                    using var connection = new SqliteConnection($"Data Source={vscodeInstance.AppData}/User/globalStorage/state.vscdb;mode=readonly;cache=shared;");
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT value FROM ItemTable where key = 'history.recentlyOpenedPathsList'";
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        using var historyDoc = JsonDocument.Parse(result.ToString()!);
                        var root = historyDoc.RootElement;
                        if (root.TryGetProperty("entries", out var entries))
                        {
                            foreach (var entry in entries.EnumerateArray())
                            {
                                if (entry.TryGetProperty("folderUri", out var folderUri))
                                {
                                    var workspaceUri = folderUri.GetString();
                                    var uri = ParseVSCodeUri(workspaceUri, vscodeInstance);
                                    if (uri != null)
                                    {
                                        results.Add(uri);
                                        continue;
                                    }
                                }
                            }
                        }
                    }

                }

                return results;
            }
        }
    }
}
