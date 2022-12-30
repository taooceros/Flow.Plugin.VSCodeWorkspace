// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Flow.Plugin.VSCodeWorkspaces.VSCodeHelper
{
    public enum VSCodeVersion
    {
        Stable = 1,
        Insiders = 2,
        Exploration = 3,
    }

    public class VSCodeInstance : IEquatable<VSCodeInstance>
    {
        public VSCodeVersion VSCodeVersion { get; set; }

        public string ExecutablePath { get; set; } = string.Empty;

        public string AppData { get; set; } = string.Empty;

        public ImageSource WorkspaceIcon() => WorkspaceIconBitMap;

        public ImageSource RemoteIcon() => RemoteIconBitMap;

        public BitmapImage WorkspaceIconBitMap { get; set; }

        public BitmapImage RemoteIconBitMap { get; set; }
        public bool Equals(VSCodeInstance other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return VSCodeVersion == other.VSCodeVersion
                   && string.Equals(ExecutablePath, other.ExecutablePath, StringComparison.InvariantCultureIgnoreCase)
                   && string.Equals(AppData, other.AppData, StringComparison.InvariantCultureIgnoreCase);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return obj is VSCodeInstance instance && Equals(instance);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine((int)VSCodeVersion,
                ExecutablePath.GetHashCode(StringComparison.InvariantCultureIgnoreCase),
                AppData.GetHashCode(StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
