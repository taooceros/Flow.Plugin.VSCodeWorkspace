using System;

namespace Flow.Plugin.VSCodeWorkspaces
{
    public readonly struct PathString : IEquatable<PathString>
    {
        public readonly string Value;

        public PathString(string value) => Value = value;

        // Better debugging experience :)
        public override string ToString() => Value;

        // Linq compoares HashCode first
        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

        // Then IEquatable.Equals, follow MS best practice
        // https://learn.microsoft.com/en-us/dotnet/standard/base-types/best-practices-strings?redirectedfrom=MSDN#:~:text=XML%20and%20HTTP.-,File%20paths.,-Registry%20keys%20and
        public bool Equals(PathString other) => string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

        // Default object.Equals, just in case
        public override bool Equals(object other)
        {
            if (other is PathString ps)
                return Equals(ps);
            if (other is string s)
                return string.Equals(Value, s, StringComparison.OrdinalIgnoreCase);
            return base.Equals(other);
        }

        public static bool operator ==(PathString left, PathString right) => left.Equals(right);
        public static bool operator !=(PathString left, PathString right) => !(left == right);

        public static implicit operator string(PathString h) => h.Value;
        public static implicit operator PathString(string s) => new(s);
    }
}
