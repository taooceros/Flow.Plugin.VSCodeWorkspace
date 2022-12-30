// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Flow.Plugin.VSCodeWorkspaces.VSCodeHelper;

namespace Flow.Plugin.VSCodeWorkspaces.RemoteMachinesHelper
{
    public class VSCodeRemoteMachine : IEquatable<VSCodeRemoteMachine>
    {
        public string Host { get; set; }

        public string User { get; set; }

        public string HostName { get; set; }

        public VSCodeInstance VSCodeInstance { get; set; }
        public bool Equals(VSCodeRemoteMachine other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Host == other.Host && User == other.User && HostName == other.HostName && Equals(VSCodeInstance, other.VSCodeInstance);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((VSCodeRemoteMachine)obj);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Host, User, HostName, VSCodeInstance);
        }
    }
}
