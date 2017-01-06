using Borg.Infra.EF6.Exceptions;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Borg
{
    internal static class Ef6Extensions
    {
        private static int GetNextIdFromSequence(DbContext db, string seqName, bool tryToCreate = false)
        {
            var sqlText = $"SELECT NEXT VALUE FOR {seqName};";
            int id;
            try
            {
                id = db.Database.SqlQuery<int>(sqlText).First();
            }
            catch (Exception ex)
            {
                if (!tryToCreate && ex.Source != ".Net SqlClient Data Provider") throw;
                var sqlToCreate = $"CREATE SEQUENCE {seqName} AS INTEGER MINVALUE 1 NO CYCLE CACHE 100; ";
                id = db.Database.SqlQuery<int>(sqlToCreate + sqlText).First();
            }
            return id;
        }

        public static int GetNextIdFromSequence<T>(this DbContext db, bool tryToCreate = false)
        {
            var seqName = typeof(T).SequenceName();
            return GetNextIdFromSequence(db, seqName, tryToCreate);
        }

        public static int GetNextIdFromSequence(this DbContext db, DbEntityEntry entry, bool tryToCreate = false)
        {
            var entity = entry.Entity;
            var entityType = entity.GetType();
            var seqName = entityType.SequenceName();
            return GetNextIdFromSequence(db, seqName, tryToCreate);
        }
    }

    internal static class TypeExtensions
    {
        public static string SequenceName(this Type type, string format = "seq_{0}")
        {
            if (!type.IsSequenceEntity()) throw new InvalidSequenceEntityException(type);

            var seqName = type.Name;
            //var baseType = type.BaseType;
            //while (baseType?.GetInterface(typeof(ISequenceEntity).Name, false) != null)
            //{
            //    seqName = baseType.Name;
            //    baseType = baseType.BaseType;
            //}
            return string.Format(format, seqName);
        }
    }
}