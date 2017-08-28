using SQLiteExtensions.Internal;
using System;
using System.Data;

namespace SQLiteExtensions
{
    public static class DataRecordExtensions
    {
        public static bool IsTheSame(this IDataRecord recordA, IDataRecord recordB)
        {
            return IsTheSame(recordA, recordB, null);
        }


        public static bool IsTheSame(this IDataRecord recordA, IDataRecord recordB, Func<string,bool>ignoreDifferentValueDelegate)
        {
            Guard.ArgumentNotNull(recordA, "recordA");
            Guard.ArgumentNotNull(recordB, "recordB");
            Guard.AssertNotNull(recordA);
            Guard.AssertNotNull(recordB);
            Guard.Assert(recordA.FieldCount == recordB.FieldCount);
            for (int i = 0; i < recordA.FieldCount; i++)
            {
                Guard.Assert(recordA.GetName(i) == recordB.GetName(i));
                object v1 = recordA.GetValue(i);
                object v2 = recordB.GetValue(i);
                bool isThesame = System.Object.Equals(v1, v2);
                if (isThesame == false)
                {
                    if (ignoreDifferentValueDelegate != null)
                    {
                        bool ignore = ignoreDifferentValueDelegate(recordA.GetName(i));
                        if (ignore)
                        {
                            continue;
                        }
                    }
                    return false;
                }
            }
            return true;
        }






        public static string ReadAsString(this IDataRecord record, string fieldName)
        {
            Guard.AssertNotNull(record);
            Guard.AssertNotNullOrEmptyString(fieldName);
            int ord = record.GetOrdinal(fieldName);
            Guard.Assert(ord >= 0);
            if (record.IsDBNull(ord))
            {
                return string.Empty;
            }
            string s = record.GetString(ord);
            Guard.Assert(s == s.Trim(), "trim problem");
            return s;
        }

        public static Nullable<double> ReadAsDouble(this IDataRecord record, string fieldName, bool enforceValue = false)
        {
            Guard.AssertNotNull(record);
            Guard.AssertNotNullOrEmptyString(fieldName);
            int ord = record.GetOrdinal(fieldName);
            Guard.Assert(ord >= 0);
            if (record.IsDBNull(ord))
            {
                if (enforceValue)
                {
                    throw new InvalidOperationException(string.Format("Feld '{0}' darf nicht null sein", fieldName));
                }
                return null;
            }
            return record.GetDouble(ord);
        }

        public static Nullable<int> ReadAsInt(this IDataRecord record, string fieldName, bool enforceValue=false)
        {
            Guard.AssertNotNull(record);
            Guard.AssertNotNullOrEmptyString(fieldName);
            int ord = record.GetOrdinal(fieldName);
            Guard.Assert(ord >= 0);
            if (record.IsDBNull(ord))
            {
                if (enforceValue)
                {
                    throw new InvalidOperationException(string.Format("Feld '{0}' darf nicht null sein",fieldName));
                }
                return null;
            }
            //if we trunc Real/Double to int, we should ensure the we don't lost any value!
            if (record.GetFieldType(ord) == typeof(System.Double))
            {
                double d = record.GetDouble(ord);
                Guard.Assert(double.Equals(d, System.Math.Truncate(d)), "be sure to not lost values");
                int i = Convert.ToInt32(d);
                return i;
            }
            return record.GetInt32(ord);
        }

        public static Nullable<DateTime> ReadAsDateTime(this IDataRecord record, string fieldName, bool enforceValue = false)
        {
            Guard.AssertNotNull(record);
            Guard.AssertNotNullOrEmptyString(fieldName);
            int ord = record.GetOrdinal(fieldName);
            Guard.Assert(ord >= 0);
            if (record.IsDBNull(ord))
            {
                if (enforceValue)
                {
                    throw new InvalidOperationException(string.Format("Feld '{0}' darf nicht null sein", fieldName));
                }
                return null;
            }
            return record.GetDateTime(ord);
        }




    }
}