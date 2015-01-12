namespace EntityProfiler.Common.Protocol {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// /Represents an identifier for an <see cref="ExecutionContext"/>
    /// </summary>
    public sealed class ContextIdentifier : IEquatable<ContextIdentifier>, IComparable<ContextIdentifier> {
        /// <summary>
        /// Represents a unique sequencing number
        /// </summary>
        public int SequenceNumber { get; set; }

        /// <summary>
        /// Represents an app-domain agnostic unique identifier
        /// </summary>
        public DateTime AppDomainTimeStamp { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ContextIdentifier(DateTime appDomainTimeStamp, int sequenceNumber) : this() {
            this.AppDomainTimeStamp = appDomainTimeStamp;
            this.SequenceNumber = sequenceNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ContextIdentifier() {}

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ContextIdentifier other) {
            return this.SequenceNumber == other.SequenceNumber && this.AppDomainTimeStamp.Equals(other.AppDomainTimeStamp);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(ContextIdentifier other) {
            if (other.AppDomainTimeStamp.Ticks == this.AppDomainTimeStamp.Ticks) {
                return this.SequenceNumber - other.SequenceNumber;
            }

            return checked ((int) (this.AppDomainTimeStamp.Ticks - other.AppDomainTimeStamp.Ticks));
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false. 
        /// </returns>
        /// <param name="obj">The object to compare with the current instance. </param>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            return obj is ContextIdentifier && this.Equals((ContextIdentifier) obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode() {
            unchecked {
                return (this.SequenceNumber*397) ^ this.AppDomainTimeStamp.GetHashCode();
            }
        }

        /// <summary/>
        public static bool operator ==(ContextIdentifier left, ContextIdentifier right) {
            return left.Equals(right);
        }

        /// <summary/>
        public static bool operator !=(ContextIdentifier left, ContextIdentifier right) {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        public override string ToString() {
            return this.SequenceNumber.ToString();
        }
    }

    /// <summary>
    /// Represents a generic container for an execution context - a execution context may be a thread, HttpContext, ActiveForm
    /// </summary>
    [DebuggerDisplay("#{Identifier,nq} {Description} @ {Timestamp,nq}")]
    public class ExecutionContext : IEquatable<ExecutionContext>, IComparable<ExecutionContext> {
        /// <summary>
        /// Gets or sets an unique identifying sequence number for this context
        /// </summary>
        public ContextIdentifier Identifier { get; set; }

        /// <summary>
        /// Gets a general text that provides a descriptive value for the execution context
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the call stack at time of execution
        /// </summary>
        public StackTrace CallStack { get; set; }

        /// <summary>
        /// Gets any generic values from the execution context
        /// </summary>
        public Dictionary<string, object> Values { get; set; }

        /// <summary>
        /// Timestamp of query execution
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        [Obsolete("This constructor is reserved for serialization purposes", true)]
        public ExecutionContext() {
            this.Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ExecutionContext(string description) {
            this.Timestamp = DateTime.UtcNow;
            this.Description = description;
            this.Values = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ExecutionContext(ContextIdentifier identifier) : this((string)null) {
            this.Identifier = identifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ExecutionContext(ContextIdentifier identifier, string description) : this(description) {
            this.Identifier = identifier;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ExecutionContext other) {
            if (other == null) {
                return false;
            }

            return other.Identifier.Equals(this.Identifier);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode() {
            // a hash code shouldn't be calculated from a 
            // mutable property but Identifier is only
            // assigned in either SimpleJson or is assigned 
            // in a constructor so for that sake it is constant
            // we'Il just need to make sure we never change it
            int value = this.Identifier.GetHashCode();
            return value;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            return this.Equals(obj as ExecutionContext);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(ExecutionContext other) {
            if (other == null) {
                return 1;
            }

            return this.Identifier.CompareTo(other.Identifier);
        }
    }
}