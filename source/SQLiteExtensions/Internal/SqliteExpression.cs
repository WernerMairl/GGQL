using System;

namespace SQLiteExtensions.Internal
{

    /// <summary>
    /// support for Expressions in .Net DataTable/DataSet (System.Data)
    /// </summary>
     public static class SqliteExpression
    {

        /// <summary>
        /// Null(pointer) is not allowed but System.DBNull!!
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool ValueRequiresIsOperator(object value)
        {
            Guard.ArgumentNotNull(value,"value");
            return ((value == System.DBNull.Value));
        }


        internal static string GetExpressionStringFromDateTime(object value)
        {
            DateTime dt = Convert.ToDateTime(value);

            string s = dt.ToString("yyyy-MM-ddTHH:mm:ssZ");
            return "'" + s + "'";
        }

        public static string GetExpressionString(object value, Type dataType)
        {
            Guard.ArgumentNotNull(value, "Value");
            Guard.ArgumentNotNull(dataType, "dataType");
            if (value == System.DBNull.Value) return "Null";

            if (dataType == typeof(System.Guid))
            {
                //Ensure the Type of the value
                Guid g;
                if (value is Guid)
                {
                    g = (Guid)value;
                }
                else
                {
                    //try to convert!
                    g = new Guid(value.ToString());
                }
                return "Convert('{" + g.ToString() + "}',System.Guid)";
            }

            if (dataType == typeof(string))
            {
                //TODO replace special Characters with Escape
                string st = value.ToString();
                if (st.Contains("'"))
                {
                    st = st.Replace("'", "''");
                    return "'" + st + "'";
                }
                else
                {
                  return "'" + st+ "'";
                }
            }
            if (dataType == typeof(DateTime))
            {
                return GetExpressionStringFromDateTime(value);

            }
            //else integer o.ä.
            return value.ToString();

        }
    }
}
