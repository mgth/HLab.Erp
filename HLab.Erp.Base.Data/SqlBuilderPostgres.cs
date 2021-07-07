using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Base.Data
{
    public interface ISqlTableBuilder<T> : ISqlBuilder  where T : class, IEntity
    {
        ISqlTableBuilder<T> Create();
        ISqlTableBuilder<T> AddColumn(Expression<Func<T, object>> property);
        ISqlTableBuilder<T> AlterColumn(Expression<Func<T, object>> property);
        ISqlTableBuilder<T> RenameColumn(string oldName, Expression<Func<T, object>> property);

        ISqlTableBuilder<T> Insert(Action<T> factory);

    }


    public class SqlBuilderPostgres : ISqlBuilder
    {
        private readonly StringBuilder _builder = new();
        private readonly string _module;
        private readonly string _version;

        public SqlBuilderPostgres(string module, string version)
        {
            _module = module;
            _version = version;
        }

        public override string ToString()
        {
            return _builder.ToString();
        }




        public ISqlBuilder Include(Func<string, ISqlBuilder, ISqlBuilder> callFunc)
        {
            return callFunc(_version, this);
        }

        public ISqlTableBuilder<T> Table<T>() where T : class, IEntity => new SqlTableBuilderPostgres<T>(this);

        public ISqlBuilder Version(string version)
        {
            var result = @$"UPDATE public.""DataVersion"" SET ""Version"" = '{version}' WHERE ""Module"" = '{_module}';";
            _builder.Append(result);
            return this;
        }



        public ISqlBuilder SqlResource(string path)
        {
            var thisAssembly = this.GetType().Assembly;// Assembly.GetExecutingAssembly();
            using var s = thisAssembly.GetManifestResourceStream(path);
            if (s == null) return this;

            using var sr = new StreamReader(s);
            _builder.Append(sr.ReadToEnd());
            return this;
        }

    class SqlTableBuilderPostgres<T> : ISqlTableBuilder<T> where T : class, IEntity
    {
        private readonly SqlBuilderPostgres _builder;

        public SqlTableBuilderPostgres(SqlBuilderPostgres builder)
        {
            _builder = builder;
        }

        public override string ToString()
        {
            return _builder.ToString();
        }

            public ISqlTableBuilder<T> AddColumn(Expression<Func<T, object>> property)
        {
                var p = GetPropertyInfo(property);

                var type = p.PropertyType;
                var name = p.Name;

                var result = "";

                if (p.PropertyType.IsClass && !(p.PropertyType == typeof(string)))
                {
                    var backingField = GetBackingField(name);

                    if (backingField.FieldType.IsConstructedGenericType &&
                        backingField.FieldType.GetGenericTypeDefinition() == typeof(IForeign<>))
                    {
                        name = $"{name}Id";
                        type = typeof(int?);

                        var foreignType  = backingField.FieldType.GetGenericArguments()[0];

                        result = @$"
                        ALTER TABLE public.""{typeof(T).Name}""
                        ADD FOREIGN KEY (""{name}"")
                        REFERENCES public.""{foreignType.Name}"" (""Id"")
                        ON UPDATE NO ACTION
                        ON DELETE CASCADE
                        NOT VALID;	
                    ";
                    }
                }


                result = @$"
                ALTER TABLE public.""{typeof(T).Name}""
                ADD COLUMN IF NOT EXISTS ""{name}"" {GetSqlType(type,p)};
            " + result;

                _builder._builder.Append(result);

                return this;
        }

        public ISqlTableBuilder<T> AlterColumn(Expression<Func<T, object>> property)
        {
                var p = GetPropertyInfo(property);

                var type = p.PropertyType;
                var name = p.Name;

                var result = @$"
                ALTER TABLE public.""{typeof(T).Name}""
                ALTER COLUMN ""{name}"" TYPE {GetSqlType(type,p)};";

                _builder._builder.Append(result);
                return this;
        }

        public ISqlTableBuilder<T> Create()
        {
            var columns = "";
            var foreign = "";
            foreach (var property in typeof(T).GetProperties())
            {
                if (property.GetCustomAttributes().OfType<IgnoreAttribute>().Any()) continue;
                if (!property.CanWrite) continue;
                var type = property.PropertyType;
                if(type != typeof(string) && type.IsClass && !type.IsArray) continue;

                columns += $"\"{property.Name}\" ";
                if(property.Name=="Id") columns += @" serial NOT NULL,";
                else
                {

                    columns += $" {GetSqlType(type,property)},\n";



                    if (type != typeof(int) || type != typeof(int?)) continue;

                    var backingField = GetBackingField(property.Name);

                    if (!backingField.FieldType.IsConstructedGenericType ||
                        backingField.FieldType.GetGenericTypeDefinition() != typeof(IForeign<>)) continue;

                    var t = backingField.FieldType.GetGenericArguments()[0];
                    //ALTER TABLE public.""{property.Name}""
                    //ADD CONSTRAINT ""{t.Name}_{property.Name}_fkey"" 
                    foreign += $@"
                                    FOREIGN KEY (""{property.Name}"")
                                    REFERENCES public.""{t.Name}"" (""Id"")
                                        ON UPDATE NO ACTION
                                        ON DELETE NO ACTION
                                        NOT VALID;
                                ";



                }
            }

            var name = typeof(T).Name;
            _builder._builder.Append(@$"
                CREATE TABLE IF NOT EXISTS public.""{name}""
                (
                    {columns}
                    {foreign}
                    PRIMARY KEY (""Id"")
                );

                ALTER TABLE public.""{name}""
                OWNER to postgres;

                GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE public.""{name}"" TO lims;

                GRANT ALL ON TABLE public.""{name}"" TO postgres;
            ");

            return this;
        }

        public ISqlBuilder Include(Func<string, ISqlBuilder, ISqlBuilder> callFunc)
        {
            return callFunc(_builder._version, this);
        }

        public ISqlTableBuilder<T> RenameColumn(string oldName, Expression<Func<T, object>> property)
        {
                var p = GetPropertyInfo(property);

                var type = p.PropertyType;
                var name = p.Name;
                var table = typeof(T).Name;

                //var result = @$"
                //    ALTER TABLE public.""{table}""
                //    RENAME ""{oldName}"" TO ""{name}"";";

                var result = @$"
            DO $$
            BEGIN
                IF EXISTS(SELECT * FROM information_schema.columns WHERE table_name='{table}' and column_name='{oldName}')
                    AND NOT EXISTS(SELECT * FROM information_schema.columns WHERE table_name='{table}' and column_name='{name}')
                THEN
                    ALTER TABLE ""public"".""{table}"" RENAME COLUMN ""{oldName}"" TO ""{name}"";
                END IF;
            END $$;
            ";

                _builder._builder.Append(result);
                return this;
        }

        public ISqlTableBuilder<T> Insert(Action<T> factory)
        {
            throw new NotImplementedException();
        }

        public ISqlBuilder SqlResource(string filePath) => _builder.SqlResource(filePath);

        public ISqlTableBuilder<T1> Table<T1>() where T1 : class, IEntity => _builder.Table<T1>();

        public ISqlBuilder Version(string version) => _builder.Version(version);

        private static PropertyInfo GetPropertyInfo<TSource>(
            Expression<Func<TSource, object>> lambda)
        {
            var type = typeof(TSource);

            var body = lambda?.Body;
            while (body?.NodeType == ExpressionType.Convert)
                body = (body as UnaryExpression)?.Operand;

            if (body is not MemberExpression member)
                throw new ArgumentException(
                    $"Expression '{lambda}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(
                    $"Expression '{lambda}' refers to a field, not a property.");

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(
                    $"Expression '{lambda}' refers to a property that is not from type {type}.");

            return propInfo;
        }
        private static FieldInfo GetBackingField(string name)
        {
            var backingFieldName = "_";
            if (name.Length > 0) backingFieldName += name.Substring(0, 1).ToLower();
            if (name.Length > 1) backingFieldName += name.Substring(1);

            return typeof(T).GetField(backingFieldName,BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static string GetSqlType(Type type, PropertyInfo p)
        {
            if (type == typeof(int)) return "integer DEFAULT 0 NOT NULL";
            if(type == typeof(int?)) return "integer";

            if(type == typeof(string)) return "text";

            if(type == typeof(double)) return "double precision DEFAULT 0 NOT NULL";
            if(type == typeof(double?)) return "double precision";

            if(type == typeof(float)) return "real DEFAULT 0 NOT NULL";
            if(type == typeof(float?)) return "real";

            if(type == typeof(decimal)) return "numeric DEFAULT 0 NOT NULL";
            if(type == typeof(decimal?)) return "numeric";


            if(type == typeof(bool)) return "boolean DEFAULT 0 NOT NULL";
            if(type == typeof(bool?)) return "boolean";

            if(type == typeof(byte[])) return "bytea";
            if (type.IsEnum) return "integer";

            if (type == typeof(DateTime?))
            {
                if (p.GetCustomAttributes<TimestampAttribute>().Any())
                    return "timestamp";
                return "date";
            }
            if (type == typeof(DateTime))
            {
                if (p.GetCustomAttributes<TimestampAttribute>().Any())
                    return "timestamp NOT NULL";
                return "date NOT NULL";
            }

            throw new ArgumentException($"Type : {type} not supported");
        }

    }



    }
}