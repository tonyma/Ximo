using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ximo.Data
{
    public static class DatabaseTypesConverter
    {
        private static readonly List<DbTypeMapEntry> DbTypeMap = new List<DbTypeMapEntry>();

        static DatabaseTypesConverter()
        {
            var dbTypeMapEntry = new DbTypeMapEntry(typeof (bool), typeof (bool?), DbType.Boolean, SqlDbType.Bit);
            DbTypeMap.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof (byte), typeof (byte?), DbType.Byte, SqlDbType.TinyInt);
            DbTypeMap.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof (byte[]), typeof (byte[]), DbType.Binary, SqlDbType.Image);
            DbTypeMap.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof (DateTime), typeof (DateTime?), DbType.DateTime2,
                SqlDbType.DateTime2);
            DbTypeMap.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof (decimal), typeof (decimal?), DbType.Decimal, SqlDbType.Decimal);
            DbTypeMap.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof (double), typeof (double?), DbType.Double, SqlDbType.Float);
            DbTypeMap.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof (Guid), typeof (Guid?), DbType.Guid, SqlDbType.UniqueIdentifier);
            DbTypeMap.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof (short), typeof (short?), DbType.Int16, SqlDbType.SmallInt);
            DbTypeMap.Add(dbTypeMapEntry);

            dbTypeMapEntry
                = new DbTypeMapEntry(typeof (int), typeof (int?), DbType.Int32, SqlDbType.Int);
            DbTypeMap.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof (long), typeof (long?), DbType.Int64, SqlDbType.BigInt);
            DbTypeMap.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof (object), typeof (object), DbType.Object, SqlDbType.Variant);
            DbTypeMap.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof (string), typeof (string), DbType.AnsiString, SqlDbType.NVarChar);
            DbTypeMap.Add(dbTypeMapEntry);
        }

        public static Type ToNetType(DbType dbType)
        {
            var entry = DbTypeMap.FirstOrDefault(d => d.DbType == dbType);
            if (entry == null)
            {
                throw new NotSupportedException($"The DbType '{dbType}' is not supported.");
            }
            return entry.NetType;
        }

        public static Type ToNetType(SqlDbType sqlDbType)
        {
            var entry = DbTypeMap.FirstOrDefault(d => d.SqlDbType == sqlDbType);
            if (entry == null)
            {
                throw new NotSupportedException($"The SqlDbType '{sqlDbType}' is not supported.");
            }
            return entry.NetType;
        }

        public static DbType ToDbType(Type netType)
        {
            var entry = DbTypeMap.FirstOrDefault(d => d.NetType == netType || d.NullableNetType == netType);
            if (entry == null)
            {
                throw new NotSupportedException($"The .net type '{netType}' is not supported.");
            }
            return entry.DbType;
        }

        public static DbType ToDbType(SqlDbType sqlDbType)
        {
            var entry = DbTypeMap.FirstOrDefault(d => d.SqlDbType == sqlDbType);
            if (entry == null)
            {
                throw new NotSupportedException($"The SqlDbType '{sqlDbType}' is not supported.");
            }
            return entry.DbType;
        }

        public static SqlDbType ToSqlDbType(Type netType)
        {
            var entry = DbTypeMap.FirstOrDefault(d => d.NetType == netType || d.NullableNetType == netType);
            if (entry == null)
            {
                throw new NotSupportedException($"The .net type '{netType}' is not supported.");
            }
            return entry.SqlDbType;
        }

        public static SqlDbType ToSqlDbType(DbType dbType)
        {
            var entry = DbTypeMap.FirstOrDefault(d => d.DbType == dbType);
            if (entry == null)
            {
                throw new NotSupportedException($"The DbType '{dbType}' is not supported.");
            }
            return entry.SqlDbType;
        }

        public static DbTypeMapEntry FromDbType(DbType dbType)
        {
            var entry = DbTypeMap.FirstOrDefault(d => d.DbType == dbType);
            if (entry == null)
            {
                throw new NotSupportedException($"The DbType '{dbType}' is not supported.");
            }
            return entry;
        }

        public static DbTypeMapEntry FromNetType(Type netType)
        {
            var entry = DbTypeMap.FirstOrDefault(d => d.NetType == netType || d.NullableNetType == netType);
            if (entry == null)
            {
                throw new NotSupportedException($"The .net type '{netType}' is not supported.");
            }
            return entry;
        }

        public static DbTypeMapEntry FromSqlDbType(SqlDbType sqlDbType)
        {
            var entry = DbTypeMap.FirstOrDefault(d => d.SqlDbType == sqlDbType);
            if (entry == null)
            {
                throw new NotSupportedException($"The SqlDbType '{sqlDbType}' is not supported.");
            }
            return entry;
        }
    }

    public sealed class DbTypeMapEntry
    {
        public DbTypeMapEntry(Type netType, Type nullableNetType, DbType dbType, SqlDbType sqlDbType) : this()
        {
            NetType = netType;
            NullableNetType = nullableNetType;
            DbType = dbType;
            SqlDbType = sqlDbType;
        }

        private DbTypeMapEntry()
        {
        }

        public Type NetType { get; }
        public Type NullableNetType { get; }
        public DbType DbType { get; }
        public SqlDbType SqlDbType { get; }
    }
}