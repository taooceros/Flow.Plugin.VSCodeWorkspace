// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Flow.Plugin.VSCodeWorkspaces.SshConfigParser;
using Flow.Plugin.VSCodeWorkspaces.VSCodeHelper;

namespace Flow.Plugin.VSCodeWorkspaces.RemoteMachinesHelper
{
    public class VSCodeRemoteMachinesApi
    {
        public VSCodeRemoteMachinesApi()
        {
        }

        public List<VSCodeRemoteMachine> Machines
        {
            get
            {
                var results = new List<VSCodeRemoteMachine>();

                foreach (var vscodeInstance in VSCodeInstances.Instances)
                {
                    // settings.json contains path of ssh_config
                    var vscode_settings = Path.Combine(vscodeInstance.AppData, "User\\settings.json");

                    if (File.Exists(vscode_settings))
                    {
                        var fileContent = File.ReadAllText(vscode_settings);

                        try
                        {
                            JsonElement vscodeSettingsFile = JsonSerializer.Deserialize<JsonElement>(fileContent, new JsonSerializerOptions
                            {
                                AllowTrailingCommas = true,
                                ReadCommentHandling = JsonCommentHandling.Skip,
                            });
                            if (vscodeSettingsFile.TryGetProperty("remote.SSH.configFile", out var pathElement))
                            {
                                var path = pathElement.GetString();

                                if (File.Exists(path))
                                {
                                    foreach (SshHost h in SshConfig.ParseFile(path))
                                    {
                                        var machine = new VSCodeRemoteMachine();
                                        machine.Host = h.Host;
                                        machine.VSCodeInstance = vscodeInstance;
                                        machine.HostName = h.HostName != null ? h.HostName : string.Empty;
                                        machine.User = h.User != null ? h.User : string.Empty;

                                        results.Add(machine);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var message = $"Failed to deserialize ${vscode_settings}";
                            Main._context.API.LogException("VSCodeWorkSpaces", message, ex);
                        }
                    }
                }

                return results;
            }
        }
    }
}