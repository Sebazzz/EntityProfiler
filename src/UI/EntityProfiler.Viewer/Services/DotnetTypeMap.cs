using System;
using System.Collections.Generic;
using System.Data;

namespace EntityProfiler.Viewer.Services
{
    public static class DotnetTypeMap
    {
        public const string DefaultValueWapper = "\'";

        private static readonly Lazy<Dictionary<Type, string>> _typeMapValueWapperLazy = 
            new Lazy<Dictionary<Type, string>>(() => 
            {
                var typeMapValueWapper = new Dictionary<Type, string>
                {
                    [typeof (byte)] = null,
                    [typeof (sbyte)] = null,
                    [typeof (short)] = null,
                    [typeof (ushort)] = null,
                    [typeof (int)] = null,
                    [typeof (uint)] = null,
                    [typeof (long)] = null,
                    [typeof (ulong)] = null,
                    [typeof (float)] = null,
                    [typeof (double)] = null,
                    [typeof (decimal)] = null,
                    [typeof (bool)] = null,
                    [typeof (string)] = DefaultValueWapper,
                    [typeof (char)] = DefaultValueWapper,
                    [typeof (char[])] = DefaultValueWapper,
                    [typeof (Guid)] = DefaultValueWapper,
                    [typeof (DateTime)] = DefaultValueWapper,
                    [typeof (DateTimeOffset)] = DefaultValueWapper,
                    [typeof (byte[])] = DefaultValueWapper,
                    [typeof (byte?)] = null,
                    [typeof (sbyte?)] = null,
                    [typeof (short?)] = null,
                    [typeof (ushort?)] = null,
                    [typeof (int?)] = null,
                    [typeof (uint?)] = null,
                    [typeof (long?)] = null,
                    [typeof (ulong?)] = null,
                    [typeof (float?)] = null,
                    [typeof (double?)] = null,
                    [typeof (decimal?)] = null,
                    [typeof (bool?)] = null,
                    [typeof (char?)] = null,
                    [typeof (Guid?)] = DefaultValueWapper,
                    [typeof (DateTime?)] = DefaultValueWapper,
                    [typeof (DateTimeOffset?)] = DefaultValueWapper
                };
                return typeMapValueWapper;
            });

        private static readonly Lazy<Dictionary<Type, DbType>> _typeMapLazy = 
            new Lazy<Dictionary<Type, DbType>>(() =>
            {
                var typeMap = new Dictionary<Type, DbType>
                {
                    [typeof (byte)] = DbType.Byte,
                    [typeof (sbyte)] = DbType.SByte,
                    [typeof (short)] = DbType.Int16,
                    [typeof (ushort)] = DbType.UInt16,
                    [typeof (int)] = DbType.Int32,
                    [typeof (uint)] = DbType.UInt32,
                    [typeof (long)] = DbType.Int64,
                    [typeof (ulong)] = DbType.UInt64,
                    [typeof (float)] = DbType.Single,
                    [typeof (double)] = DbType.Double,
                    [typeof (decimal)] = DbType.Decimal,
                    [typeof (bool)] = DbType.Boolean,
                    [typeof (string)] = DbType.String,
                    [typeof (char)] = DbType.StringFixedLength,
                    [typeof (char[])] = DbType.AnsiStringFixedLength,
                    [typeof (Guid)] = DbType.Guid,
                    [typeof (DateTime)] = DbType.DateTime,
                    [typeof (DateTimeOffset)] = DbType.DateTimeOffset,
                    [typeof (byte[])] = DbType.Binary,
                    [typeof (byte?)] = DbType.Byte,
                    [typeof (sbyte?)] = DbType.SByte,
                    [typeof (short?)] = DbType.Int16,
                    [typeof (ushort?)] = DbType.UInt16,
                    [typeof (int?)] = DbType.Int32,
                    [typeof (uint?)] = DbType.UInt32,
                    [typeof (long?)] = DbType.Int64,
                    [typeof (ulong?)] = DbType.UInt64,
                    [typeof (float?)] = DbType.Single,
                    [typeof (double?)] = DbType.Double,
                    [typeof (decimal?)] = DbType.Decimal,
                    [typeof (bool?)] = DbType.Boolean,
                    [typeof (char?)] = DbType.StringFixedLength,
                    [typeof (Guid?)] = DbType.Guid,
                    [typeof (DateTime?)] = DbType.DateTime,
                    [typeof (DateTimeOffset?)] = DbType.DateTimeOffset
                };
                return typeMap;
            });

        public static IReadOnlyDictionary<Type, string> TypeMapValueWapper
        {
            get { return _typeMapValueWapperLazy.Value; }
        }

        public static IReadOnlyDictionary<Type, DbType> TypeMap
        {
            get { return _typeMapLazy.Value; }
        }
    }
}