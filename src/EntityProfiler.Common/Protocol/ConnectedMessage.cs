namespace EntityProfiler.Common.Protocol {
    using System;
    using System.Collections.Generic;
    using SysVersion = System.Version;

    /// <summary>
    /// Represents a message that is shown when a client connects
    /// </summary>
    public sealed class ConnectedMessage : Message {
        /// <summary>
        /// Gets the version of the other party
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ConnectedMessage(Version version) {
            this.Version = version;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ConnectedMessage() {}
    }

    /// <summary>
    /// Represents a two-part version number
    /// </summary>
    public sealed class Version : IComparable<Version>, IEquatable<Version> {
        /// <summary>
        /// Gets the major version
        /// </summary>
        public int Major { get; set; }
        /// <summary>
        /// Gets the minor version
        /// </summary>
        public int Minor { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Version(int major, int minor) {
            this.Major = major;
            this.Minor = minor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Version() {}

        /// <summary>
        /// Gets a version number from the current assembly
        /// </summary>
        /// <returns></returns>
        public static Version FromCurrentAssembly() {
            SysVersion version = typeof (Version).Assembly.GetName().Version;
            return new Version(version.Major, version.Minor);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(Version other) {
            if (other == null) {
                return 1;
            }

            if (other.Major != this.Major) {
                return this.Major - other.Major;
            }

            return this.Minor - other.Minor;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Version other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return this.Major == other.Major && this.Minor == other.Minor;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            return obj is Version && this.Equals((Version) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode() {
            unchecked {
                return (this.Major*397) ^ this.Minor;
            }
        }

        /// <summary>
        /// Returns if the specified version numbers are equal
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Version left, Version right) {
            return Equals(left, right);
        }

        /// <summary>
        /// Returns if the specified version numbers are not equal
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Version left, Version right) {
            return !Equals(left, right);
        }

        /// <summary>
        /// Returns if the specified version is greater
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(Version left, Version right) {
            return Comparer<Version>.Default.Compare(left, right) > 0;
        }

        /// <summary>
        /// Returns if the specified version is smaller
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(Version left, Version right) {
            return Comparer<Version>.Default.Compare(left, right) < 0;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() {
            return String.Format("{0}.{1}", this.Major, this.Minor);
        }
    }
}