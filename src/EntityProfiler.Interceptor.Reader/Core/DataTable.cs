namespace EntityProfiler.Interceptor.Reader.Core {
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Represents a table of data
    /// </summary>
    public sealed class DataTable : IEnumerable<DataTable.DataTableEntry> {
        private readonly Dictionary<string, object>[] _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DataTable(Dictionary<string, object>[] data) {
            this._data = data;
        }

        /// <summary>
        /// Gets the rows in the data table
        /// </summary>
        public IEnumerable<Dictionary<string, object>> Rows {
            get { return this._data; }
        }

        /// <summary>
        /// Gets the columns in the data table
        /// </summary>
        public IEnumerable<string> Columns {
            get {
                var dict = this._data.FirstOrDefault();
                if (dict != null) {
                    return dict.Keys;
                }

                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Gets the enumerator which allows enumerating over this table
        /// </summary>
        /// <returns></returns>
        public DataTableEnumerator GetEnumerator() {
            return new DataTableEnumerator(this._data);
        }

        /// <summary>
        /// Represents an entry in the data table
        /// </summary>
        public struct DataTableEntry {
            private readonly int _row;
            private readonly Dictionary<string, object> _columns;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            public DataTableEntry(Dictionary<string, object> columns, int row) {
                this._columns = columns;
                this._row = row;
            }

            /// <summary>
            /// Gets the row number
            /// </summary>
            public int Row {
                [DebuggerStepThrough] get { return this._row; }
            }

            /// <summary>
            /// Gets the columns in the current row
            /// </summary>
            public Dictionary<string, object> Columns {
                [DebuggerStepThrough] get { return this._columns; }
            }
        }

        /// <summary>
        /// Enumerator for <see cref="DataTable"/>
        /// </summary>
        public struct DataTableEnumerator : IEnumerator<DataTableEntry> {
            private readonly Dictionary<string, object>[] _rows;
            private int _pointer;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            internal DataTableEnumerator(Dictionary<string, object>[] rows) : this() {
                this._rows = rows;
                this._pointer = -1;
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <returns>
            /// The element in the collection at the current position of the enumerator.
            /// </returns>
            public DataTableEntry Current {
                get {
                    var currentDict = this._pointer >= 0 ? this._rows[this._pointer] : null;

                    return new DataTableEntry(currentDict, this._pointer);
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose() {
                // nop
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            /// <returns>
            /// The current element in the collection.
            /// </returns>
            object IEnumerator.Current {
                get { return this.Current; }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext() {
                if (this._pointer + 1 < this._rows.Length) {
                    this._pointer++;
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset() {
                this._pointer = 0;
            }
        }

        IEnumerator<DataTable.DataTableEntry> IEnumerable<DataTable.DataTableEntry>.GetEnumerator() {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}