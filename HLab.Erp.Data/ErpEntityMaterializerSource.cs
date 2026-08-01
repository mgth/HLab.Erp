using System;
using System.Data.Common;
using NPoco;

namespace HLab.Erp.Data
{
    /// <summary>
    /// Politique de dates de la couche data (Npgsql refuse tout DateTime non-UTC
    /// pour timestamptz) : à l'écriture les dates locales/non qualifiées sont
    /// converties en UTC — même sémantique que le DateEx WPF qui convertissait
    /// avant de poser la valeur — et à la lecture l'UTC redevient heure locale,
    /// pour que les vues affichent les dates telles que saisies.
    /// </summary>
    public class UtcDateTimeMapper : DefaultMapper
    {
        public override Func<object, object> GetParameterConverter(DbCommand dbCommand, Type sourceType)
        {
            if (sourceType == typeof(DateTime) || sourceType == typeof(DateTime?))
                return v => v is DateTime d
                    ? d.Kind == DateTimeKind.Utc ? d : d.ToUniversalTime()
                    : v;

            return base.GetParameterConverter(dbCommand, sourceType);
        }

        public override Func<object, object> GetFromDbConverter(Type destType, Type sourceType)
        {
            if ((destType == typeof(DateTime) || destType == typeof(DateTime?))
                && sourceType == typeof(DateTime))
                return v => v is DateTime { Kind: DateTimeKind.Utc } d
                    ? d.ToLocalTime()
                    : v;

            return base.GetFromDbConverter(destType, sourceType);
        }
    }
}
