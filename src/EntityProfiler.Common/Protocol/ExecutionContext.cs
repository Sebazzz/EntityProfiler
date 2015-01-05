namespace EntityProfiler.Common.Protocol {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Represents a generic container for an execution context - a execution context may be a thread, HttpContext, ActiveForm
    /// </summary>
    [DebuggerDisplay("#{Identifier,nq} {Description} @ {Timestamp,nq}")]
    public class ExecutionContext : IEquatable<ExecutionContext>, IComparable<ExecutionContext> {
        /// <summary>
        /// Gets or sets an unique identifying sequence number for this context
        /// </summary>
        public int Identifier { get; set; }

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
        public ExecutionContext(int identifier) : this(null) {
            this.Identifier = identifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ExecutionContext(int identifier, string description) : this(description) {
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

            return other.Identifier == this.Identifier;
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
            int value = this.Identifier;
            return value.GetHashCode();
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

            return other.Identifier - this.Identifier;
        }
    }
}