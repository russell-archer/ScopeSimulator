#region Using statements
using System;
using System.Data;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
#endregion

namespace ScopeSimulator
{
	public static class DataHelper
    {
        #region GetPropertyList
        public static List<string> PropertyNameList(object objectSource)
        {
            if(objectSource == null)
                return null;

            PropertyInfo[] pi = objectSource.GetType().GetProperties(
                    BindingFlags.NonPublic |
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.Static);

            if(pi == null || pi.Length == 0)
                return null;

            List<string> propertyNames = new List<string>();
            foreach (PropertyInfo p in pi)
                propertyNames.Add(p.Name);

            return propertyNames;
        }
        #endregion

        #region CreateObjectFromDataTable
        public static object CreateObjectFromDataTable(
            System.Type objectType,
            DataTable dtSource,
            bool copyPublicProperties,
            bool copyPrivateMembers,
            int rowIndex)
        {
            object objectDestination = null;
            PropertyInfo[] pi = null;
            FieldInfo[] fi = null;

            if (!copyPrivateMembers && !copyPublicProperties)
                throw new ArgumentException("DataConversion: Bad Params. No copy operation specified");

            if (dtSource == null)
                throw new ArgumentException("DataConversion: Null source DataTable");

            if(dtSource.Rows.Count == 0)
                throw new ArgumentException("DataConversion: DataTable has no rows");

            if(rowIndex < 0 || rowIndex > (dtSource.Rows.Count-1))
                throw new ArgumentException("DataConversion: rowIndex invalid");

            // Get type info on the actual destination class
            string typeName = objectType.UnderlyingSystemType.AssemblyQualifiedName;
            Type targetType = Type.GetType(typeName, true);

            // Create an instance of the type
            objectDestination = Activator.CreateInstance(targetType);

            if (copyPublicProperties)
            {
                // Get info about each property in the destination object using reflection
                pi = objectDestination.GetType().GetProperties(
                    BindingFlags.NonPublic |
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.Static);

                if (pi == null)
                    throw new ArgumentException("DataConversion: Destination has no properties");
            }

            if (copyPrivateMembers)
            {
                // Get info about each field in the destination object using reflection
                fi = objectDestination.GetType().GetFields(
                    BindingFlags.NonPublic |
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.Static);

                if (fi == null)
                    throw new ArgumentException("DataConversion: Destination object has no fields");
            }

            // Now enumerate each property and see if there's a matching named column,
            // if there is, copy the data to the object

            if (copyPublicProperties)
            {
                foreach (PropertyInfo p in pi)
                {
                    if (dtSource.Columns.Contains(p.Name))
                        p.SetValue(objectDestination, dtSource.Rows[rowIndex][p.Name], null);
                }
            }

            if (copyPrivateMembers)
            {
                foreach (FieldInfo f in fi)
                {
                    if (dtSource.Columns.Contains(f.Name))
                        f.SetValue(objectDestination, dtSource.Rows[rowIndex][f.Name]);
                }
            }

            return objectDestination;
        }
        #endregion

        #region CreateDataTableFromObject
        public static DataTable CreateDataTableFromObject(
			object objectSource, 
			bool copyPublicProperties, 
			bool copyPrivateMembers)
		{
			PropertyInfo[]	pi = null;
			FieldInfo[] fi = null;
			DataTable table = null;

			try
			{
				if(!copyPrivateMembers && !copyPublicProperties)
					throw new ArgumentException("DataConversion: Bad Params. No copy operation specified");

				if( objectSource == null )
					throw new ArgumentException("DataConversion: Null objectSource");

				if(copyPublicProperties)
				{
					// Get info about each property in the source object using reflection
					pi = objectSource.GetType().GetProperties(
						BindingFlags.NonPublic |
						BindingFlags.Public |
						BindingFlags.Instance |
						BindingFlags.Static);

					if(pi == null)
						throw new ArgumentException("DataConversion: Source object has no properties");
				}

				if(copyPrivateMembers)
				{
					// Get info about each field in the source object using reflection
                    fi = objectSource.GetType().GetFields(
						BindingFlags.NonPublic |
						BindingFlags.Public |
						BindingFlags.Instance |
						BindingFlags.Static);

					if(fi == null)
                        throw new ArgumentException("DataConversion: Source object has no fields");
				}

				// Add a table to the DataSet with the same field types
				// as the source array
                DataColumn dc;
                table = new DataTable();
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                table.TableName = objectSource.GetType().Name;

				if(copyPublicProperties)
				{
					foreach(PropertyInfo p in pi)
					{
                        // Is the property type is nullable?
                        // If it is, the dataset's column type is set to the nullable type's underlying type.
                        // Does not support value types that are not primitive or DateTime
                        Type propType = Nullable.GetUnderlyingType(p.PropertyType);
						if (propType != null)
							dc = new DataColumn(p.Name, propType);
                        else
                            dc = new DataColumn(p.Name, p.PropertyType);

                        table.Columns.Add(dc);
					}
				}

				if(copyPrivateMembers)
				{
					foreach(FieldInfo f in fi)
					{
                        // Is the field type is nullable?
                        Type fieldType = Nullable.GetUnderlyingType(f.FieldType);
						if (fieldType != null)
							dc = new DataColumn(f.Name, fieldType);
						else
							dc = new DataColumn(f.Name, f.FieldType);

                        table.Columns.Add(dc);
					}
				}

				// Fill the DataSet with values from the source object
				DataRow dr = table.NewRow();

			    if(copyPublicProperties)
			    {
                    foreach (PropertyInfo p in pi)
					{
						object propObj = p.GetValue(objectSource, null);
						if (propObj == null)
						{
							dr[p.Name] = DBNull.Value;
						}
						else
						{
							dr[p.Name] = propObj;
						}
						if (dr[p.Name].GetType() == typeof(DateTime))
						{
							if (((DateTime)(p.GetValue(objectSource, null))) == DateTime.MinValue)
								dr[p.Name] = DBNull.Value;
						}
					}
			    }

			    if(copyPrivateMembers)
			    {
                    foreach (FieldInfo f in fi)
					{
						object fieldObj = f.GetValue(objectSource);
						if (fieldObj == null)
						{
							dr[f.Name] = DBNull.Value;
						}
						else
						{
							dr[f.Name] = fieldObj;
						}
						if(dr[f.Name].GetType() == typeof(DateTime))
						{
							if(((DateTime)(f.GetValue(objectSource))) == DateTime.MinValue)
								dr[f.Name] = DBNull.Value;
						}
					}
			    }

			    table.Rows.Add(dr);
				
			}
			catch(Exception ex)
			{
                throw new ArgumentException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "DataConversion: Generic Error", ex.Message));
			}
			return table;
		}
		#endregion

        #region FillDataTableFromObject
        public static bool FillDataTableFromObject(
            object objectSource,
            DataTable tableDest,
            int tableRowIndex,
            bool copyPublicProperties,
            bool copyPrivateMembers)
        {
            bool filledOK = false;
            PropertyInfo[] pi = null;
            FieldInfo[] fi = null;

            try
            {
                if (!copyPrivateMembers && !copyPublicProperties)
                    throw new ArgumentException("DataConversion: Bad Params. No copy operation specified");

                if (objectSource == null)
                    throw new ArgumentException("DataConversion: Null objectSource");

                if (tableDest == null)
                    throw new ArgumentException("DataConversion: Null tableDest");

                if (copyPublicProperties)
                {
                    // Get info about each property in the source object using reflection
                    pi = objectSource.GetType().GetProperties(
                        BindingFlags.NonPublic |
                        BindingFlags.Public |
                        BindingFlags.Instance |
                        BindingFlags.Static);

                    if (pi == null)
                        throw new ArgumentException("DataConversion: Source object has no properties");
                }

                if (copyPrivateMembers)
                {
                    // Get info about each field in the source object using reflection
                    fi = objectSource.GetType().GetFields(
                        BindingFlags.NonPublic |
                        BindingFlags.Public |
                        BindingFlags.Instance |
                        BindingFlags.Static);

                    if (fi == null)
                        throw new ArgumentException("DataConversion: Source object has no fields");
                }

                // Fill the Table with values from the source object
                if (copyPublicProperties)
                {
                    foreach (PropertyInfo p in pi)
                    {
                        // Is the source property in the destination table? If not, skip it
                        if (tableDest.Columns.Contains(p.Name))
                        {
                            object propObj = p.GetValue(objectSource, null);
                            if (propObj == null)
                            {
                                tableDest.Rows[tableRowIndex][p.Name] = DBNull.Value;
                            }
                            else
                            {
                                tableDest.Rows[tableRowIndex][p.Name] = propObj;
                            }

                            if (tableDest.Rows[tableRowIndex][p.Name].GetType() == typeof(DateTime))
                            {
                                if (((DateTime)(p.GetValue(objectSource, null))) == DateTime.MinValue)
                                    tableDest.Rows[tableRowIndex][p.Name] = DBNull.Value;
                            }
                        }
                    }
                }

                if (copyPrivateMembers)
                {
                    foreach (FieldInfo f in fi)
                    {
                        // Is the source field in the destination table? If not, skip it
                        if (tableDest.Columns.Contains(f.Name))
                        {
                            object fieldObj = f.GetValue(objectSource);
                            if (fieldObj == null)
                            {
                                tableDest.Rows[tableRowIndex][f.Name] = DBNull.Value;
                            }
                            else
                            {
                                tableDest.Rows[tableRowIndex][f.Name] = fieldObj;
                            }

                            if (tableDest.Rows[tableRowIndex][f.Name].GetType() == typeof(DateTime))
                            {
                                if (((DateTime)(f.GetValue(objectSource))) == DateTime.MinValue)
                                    tableDest.Rows[tableRowIndex][f.Name] = DBNull.Value;
                            }
                        }
                    }
                }

                filledOK = true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "DataConversion: Generic Error", ex.Message));
            }
            return filledOK;
        }
        #endregion

		#region Null Type Support
		/// <summary>
        /// Detect if a data type is nullable type
        /// </summary>
        /// <param name="t"></param>
        /// <returns>boolean - true if the data type is nullable type; false otherwise</returns>
        public static bool IsNullable(Type t)
        {
            bool isNullable = false;

            if (t.IsGenericType)
            {
                Type typ = t.GetGenericTypeDefinition();
                isNullable = typ.Equals(typeof(Nullable<>));
            }

            return isNullable;
		}
		#endregion

		#region String to Type Conversion
		/// <summary>
		/// Attempts to convert a string to the type specified. 
		/// If the string is null or empty then 
		/// If conversion is not possible then then null is returned.
		/// </summary>
		/// <param name="s">The string to be converted</param>
		/// <param name="conversionTarget">The Type to convert the string into</param>
		/// <returns>The string converted to the specified type. If conversion is not possible then null is returned</returns>
		/// <example>
		/// object o;
		/// DateTime myDt = new DateTime();
		/// o = DataConversionHelper.ConvertString("12/12/2006", typeof(DateTime));
		/// if(o.GetType() == typeof(DateTime))
		///     myDt = (DateTime)o;
		///
		/// long myLong;
		/// o = DataConversionHelper.ConvertString("999999999", typeof(long));
		/// if(o.GetType() == typeof(long))
		///     myLong = (long)o;
		///
		/// bool myBool;
		/// o = DataConversionHelper.ConvertString("true", typeof(bool));
		/// if(o.GetType() == typeof(bool))
		///     myBool = (bool)o;
		/// </example>
		public static object ConvertString(string s, System.Type conversionTarget)
		{
			if(conversionTarget == typeof(string))
				return s;

			object convertedObject = Activator.CreateInstance(conversionTarget);
			try
			{
				if(string.IsNullOrEmpty(s))
					return convertedObject;

				TypeConverter tc = TypeDescriptor.GetConverter(conversionTarget);
				if(tc.CanConvertFrom(typeof(string)))
				{
					// Using System.Globalization.CultureInfo.InvariantCulture
					convertedObject = tc.ConvertFromInvariantString(s);
				}
				else
				{
					return convertedObject;
				}
			}
			catch
			{
				return Activator.CreateInstance(conversionTarget);
			}
			return convertedObject;
		}
		#endregion
	}
}
