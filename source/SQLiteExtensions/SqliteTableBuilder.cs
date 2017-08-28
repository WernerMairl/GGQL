using Newtonsoft.Json.Linq;
using SQLiteExtensions.Internal;
using System;
using System.Collections.Generic;

namespace SQLiteExtensions
{
    public class SqliteTableBuilder : ITableBuilder
    {

        internal static string GetColumnDefinitionStatement(string columnName, string dataType)
        {
            Guard.ArgumentNotNullOrEmptyString(columnName, nameof(columnName));
            Guard.ArgumentNotNullOrEmptyString(dataType, nameof(dataType));

            //string datatype = null;
            //if (column.Datatype.ToLowerInvariant().Contains("string"))
            //{
            //    datatype = "TEXT";
            //}

            //if (column.Datatype.ToLowerInvariant().Contains("int"))
            //{
            //    datatype = "INTEGER";
            //}

            //if (column.Datatype.ToLowerInvariant().Contains("bcd"))
            //{
            //    datatype = "REAL";
            //}


            //if (column.Datatype.ToLowerInvariant().Contains("datetime"))
            //{
            //    datatype = "TEXT"; //ISO8601
            //}
            return string.Format("{0} {1}", columnName, dataType);
        }
        private static IDictionary<string, string> Empty = new Dictionary<string, string>();
        public SqliteTableBuilder(string tableName)
        {
            Guard.ArgumentNotNullOrEmptyString(tableName, nameof(tableName));
            this.Columns = Empty;
            this.TableName = tableName;
        }
        /// <summary>
        /// columName, typeDeclaration in mysql syntax
        /// </summary>
        public IDictionary<string, string> Columns { get; set; }

        public IEnumerable<string> GetInsertStatements(IEnumerable<Tuple<JObject, string>> nodes)
        {
            Guard.ArgumentNotNull(nodes, nameof(nodes));
            //Guard.ArgumentNotNull(rows, "rows");
            string insertTemplate = "INSERT INTO {0} ({1}) values ({2});";
            string columnNames = string.Join(", ", this.Columns.Keys);

            foreach (Tuple< JObject, string> node in nodes)
            {
                Guard.AssertNotNull(node.Item1);
                Guard.AssertNotNull(node.Item2);
                List<string> rowValues = new List<string>();
                foreach (string prop in this.Columns.Keys)
                {
                    if (prop == "owner")
                    {
                        rowValues.Add(SqliteExpression.GetExpressionString(node.Item2, typeof(string)));
                    }
                    else
                    {
                        string cv = node.Item1[prop].ToString();
                        System.Type t = typeof(string);

                        if ((prop == "isPrivate") || (prop.StartsWith("is_"))) //FB is using "is_" pattern!
                        {
                            int isp = 0;
                            if (cv.ToLowerInvariant().Contains("tr"))
                            {
                                isp = 1;
                            }
                            rowValues.Add(SqliteExpression.GetExpressionString(isp, typeof(int)));
                        }
                        else
                        {
                            rowValues.Add(SqliteExpression.GetExpressionString(cv, typeof(string)));
                        }
                    }
                }
                string serializedRowValues = string.Join(", ", rowValues);

                yield return string.Format(insertTemplate, this.TableName, columnNames, serializedRowValues);

            } //nodes

        }

        public string TableName { get; private set; }

        /// <summary>
        /// column names that should be used for PK
        /// </summary>
        public string[] PrimaryKey { get; set; }

        public IEnumerable<string> Build()
        {
            if (string.IsNullOrEmpty(this.TableName))
            {
                throw new InvalidOperationException("TableName must be defined/not empty");
            }
            if (this.PrimaryKey.Length == 0)
            {
                throw new NotImplementedException("Empty Primarykey");
            }
            if (this.Columns.Count == 0)
            {
                throw new InvalidOperationException("Table must contain at least one Column");
            }


            string createTableTemplate = "create table {0} ({1}, PRIMARY KEY({2}) )";
            List<string> columndefs = new List<string>();
            List<string> pkIdentified = new List<string>(this.PrimaryKey);
            Guard.Assert(pkIdentified.Count > 0);
            foreach (string key in this.Columns.Keys)
            {
                Guard.AssertNotNullOrEmptyString(key, nameof(key));
                Guard.Assert(key.Trim() == key, "trim-bug 01");

                if (pkIdentified.Contains(key))
                {
                    pkIdentified.Remove(key);
                }
                string cdef = this.Columns[key];
                if (string.IsNullOrEmpty(cdef))
                {
                    throw new InvalidOperationException(string.Format("Datatype not defined for Column '{0}'", key));
                }
                Guard.Assert(cdef.Trim() == cdef, "trim-bug 02");
                string s1 = string.Format("{0} {1}", key, cdef);
                columndefs.Add(s1);
            }

            if (pkIdentified.Count > 0)
            {
                throw new InvalidOperationException("PK field not found on table");
            }
            string columnDefsResolved = string.Join(", ", columndefs);

            string pkResolved = string.Join(", ", this.PrimaryKey);

            string createTableResolved = string.Format(createTableTemplate, this.TableName, columnDefsResolved, pkResolved);
            yield return createTableResolved;
        }
    }
}