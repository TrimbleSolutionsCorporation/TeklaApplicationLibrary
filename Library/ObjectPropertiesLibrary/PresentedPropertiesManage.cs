namespace Tekla.Structures.ObjectPropertiesLibrary
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;

    using Tekla.Structures.Datatype;
    using Tekla.Structures.Model;

    /// <summary>
    /// The presented properties manage.
    /// </summary>
    public class PresentedPropertiesManage
    {
        #region Public Methods and Operators

        /// <summary>Change properties hidden values in first property file
        /// so that properties that are in second property file are shown.</summary>
        /// <param name="all">All the properties.</param>
        /// <param name="shown">The shown.</param>
        /// <returns>false if write to file fails.</returns>
        public static bool ChangeHiddenValuesByOtherFile(
            ref PresentedPropertiesXml all, ref PresentedPropertiesXml shown)
        {
            foreach (var allProperty in all.PropertiesList)
            {
                allProperty.Visible = false;

                foreach (var shownProperty in shown.PropertiesList)
                {
                    if (shownProperty.Name == allProperty.Name)
                    {
                        allProperty.Visible = true;
                    }
                }
            }

            return all.XmlWriteProperties(all.PropertiesList);
        }

        /// <summary>The get base type.</summary>
        /// <param name="propertyType">The property type.</param>
        /// <returns>The System.String.</returns>
        public static string GetBaseType(string propertyType)
        {
            switch (propertyType.ToLower())
            {
                case PropertyTypes.String:
                    return PropertyTypes.String;
                case PropertyTypes.Date:
                    return PropertyTypes.Date;
                case PropertyTypes.Pieces:
                case PropertyTypes.Int:
                    return PropertyTypes.Int;
                default:
                    return PropertyTypes.Double;
            }
        }

        /// <summary>The value of objects property (report or UDA), returned in units given in PresentedProperties propertyType.</summary>
        /// <param name="thisPart">The object for which to get amount.</param>
        /// <param name="visibleName">Name of the property is properties xml file.</param>
        /// <param name="presentedPropertiesInstance">The Presented Properties Instance.</param>
        /// <returns>Value of objects property. If both UDA and Report properties are declared, returns value of report property.</returns>
        public static object GetPartPropertyValue(ModelObject thisPart, string visibleName, PresentedPropertiesXml presentedPropertiesInstance)
        {
            object result = null;
            PresentedProperties thisProperty = null;

            if (presentedPropertiesInstance.GetPropertyByName(visibleName, ref thisProperty))
            {
                result = GetPartPropertyValue(thisPart, thisProperty, presentedPropertiesInstance);
            }

            return result;
        }

        /// <summary>The value of objects property (report or UDA), returned in units given in PresentedProperties propertyType.</summary>
        /// <param name="thisPart">The object for which to get amount.</param>
        /// <param name="property">The property to get the value for.</param>
        /// <param name="presentedPropertiesInstance">The Presented Properties Instance.</param>
        /// <returns>Value of objects property. If both UDA and Report properties are declared, returns value of report property.</returns>
        public static object GetPartPropertyValue(ModelObject thisPart, PresentedProperties property, PresentedPropertiesXml presentedPropertiesInstance)
        {
            var isReportProperty = !string.IsNullOrEmpty(property.ReportPropertyName);

            var propertvalueY = GetPropertvalueYBasedOnType(
                presentedPropertiesInstance, thisPart, property, isReportProperty);

            try
            {
                switch (GetBaseType(property.PropertyType))
                {
                    case PropertyTypes.Int:
                        return string.IsNullOrEmpty(propertvalueY) ? 0 : Convert.ToInt32(propertvalueY);
                    case PropertyTypes.String:
                        return propertvalueY;
                    case PropertyTypes.Date:
                        return string.IsNullOrEmpty(propertvalueY) ? null : (object)Convert.ToDateTime(propertvalueY);
                    default:
                        return string.IsNullOrEmpty(propertvalueY) ? 0.0 : Convert.ToDouble(propertvalueY);
                }
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee);
            }

            return null;
        }

        /// <summary>Makes a new properties file from other files shown properties.</summary>
        /// <param name="shown">New properties.</param>
        /// <param name="all">File from which visible new properties is done.</param>
        /// <returns><c>false.</c> if write to file fails.</returns>
        public static bool MakeFileFromOtherFilesHiddenProperties(
            ref PresentedPropertiesXml shown, ref PresentedPropertiesXml all)
        {
            // unordered list of shown properties to be constructed
            var newShownProperties =
                new SearchableSortableBindingList<PresentedProperties>();

            // add all non-hidden properties to the shown list 
            foreach (var allProperty
                in all.PropertiesList.Where(allProperty => !allProperty.Hidden))
            {
                newShownProperties.Add(allProperty);
            }

            // ordered list of shown properties to be constructed
            var orderedNewShownProperties =
                new SearchableSortableBindingList<PresentedProperties>();

            // copy original ordering from shown.PropertiesList to orderedNewShownProperties
            foreach (var shownProperty in shown.PropertiesList)
            {
                for (var i = 0; i < newShownProperties.Count; i++)
                {
                    // Compares to Name 
                    if (!newShownProperties[i].Equals(shownProperty))
                    {
                        continue;
                    }

                    orderedNewShownProperties.Add(newShownProperties[i]);
                    newShownProperties.RemoveAt(i);
                    break;
                }
            }

            // put newly shown properties to the end of the list
            foreach (var shownProperty
                in newShownProperties.Where(shownProperty => !orderedNewShownProperties.Contains(shownProperty)))
            {
                orderedNewShownProperties.Add(shownProperty);
            }

            shown.PropertiesList = orderedNewShownProperties;

            return shown.XmlWriteProperties(shown.PropertiesList);
        }

        #endregion

        #region Methods

        /// <summary>Gets the value of an attibute defined by a formula.</summary>
        /// <param name="presentedPropertiesInstance">The presented properties instance.</param>
        /// <param name="modelObj">The model obj.</param>
        /// <param name="udaEquation">The text of the formula.</param>
        /// <param name="resultValue">The result of the calculation.</param>
        /// <returns><c>true.</c> if a result has been found.</returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "US is OK. :)")]
        private static bool GetAttributeByUDAFormula(
            PresentedPropertiesXml presentedPropertiesInstance,
            ModelObject modelObj,
            string udaEquation,
            out double resultValue)
        {
            var removeChar = new[] { '+', '-', '*', '/', '(', ')' };
            const string DelimStr = " ";
            var delimiter = DelimStr.ToCharArray();
            string insertAddstr;
            var findIndex = 0;
            double doubleValue;
            var culInfo = CultureInfo.CurrentCulture;
            var usCulInfo = new CultureInfo("en-us");

            resultValue = 0.0;
            var findString = udaEquation;

            for (var i = 0; i < removeChar.Length; i++)
            {
                findString = findString.Replace(removeChar[i].ToString(CultureInfo.InvariantCulture), " ");
            }

            var splitKey = findString.Split(delimiter);
            var tempKey = new string[splitKey.Length];

            for (var i = 0; i < splitKey.Length; i++)
            {
                if (splitKey[i].Length <= 0)
                {
                    continue;
                }

                var isDouble = double.TryParse(splitKey[i], NumberStyles.Any, culInfo, out doubleValue);

                if (isDouble == false)
                {
                    isDouble = double.TryParse(splitKey[i], NumberStyles.Any, usCulInfo, out doubleValue);
                }

                if (isDouble == false)
                {
                    tempKey[i] = splitKey[i];

                    insertAddstr = "_UDA_" + i.ToString(CultureInfo.InvariantCulture);
                    findIndex = udaEquation.IndexOf(splitKey[i], findIndex, StringComparison.Ordinal);

                    findIndex = findIndex + splitKey[i].Length;
                    udaEquation = udaEquation.Insert(findIndex, insertAddstr);
                }
            }

            for (var i = 0; i < tempKey.Length; i++)
            {
                if (null == tempKey[i] || tempKey[i].Length == 0)
                {
                    continue;
                }

                PresentedProperties property = null;

                if (!presentedPropertiesInstance.GetPropertyByNameOrModelPropertyName(tempKey[i], ref property))
                {
                    continue;
                }

                // Maybe should use just visible property name?
                var propertvalueY = GetPropertvalueYBasedOnType(
                    presentedPropertiesInstance, modelObj, property, true, false);

                try
                {
                    doubleValue = Convert.ToDouble(propertvalueY);
                    insertAddstr = tempKey[i] + "_UDA_" + i.ToString(CultureInfo.InvariantCulture);
                    udaEquation = udaEquation.Replace(insertAddstr, doubleValue.ToString(CultureInfo.InvariantCulture));
                }
                catch (Exception ee)
                {
                    Debug.WriteLine(ee);
                    return false;
                }
            }

            var mathEval = new MathEvaluate();

            if (mathEval.Parse(udaEquation) && mathEval.Error == false)
            {
                mathEval.Infix2Postfix();
                mathEval.EvaluatePostfix();
            }

            if (mathEval.Error)
            {
                if (mathEval.ErrorDescription != MathEvaluate.DivideByZero)
                {
                    return false;
                }
            }
            else
            {
                resultValue = mathEval.Result;
            }

            return true;
        }

        /// <summary>The get property.</summary>
        /// <param name="propertyOwner">The property owner.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="isReportProperty">The is report property.</param>
        /// <param name="returnValue">The return value.</param>
        /// <returns>The System.Boolean.</returns>
        private static bool GetProperty(
            ModelObject propertyOwner, string propertyName, bool isReportProperty, ref string returnValue)
        {
            return isReportProperty
                       ? propertyOwner.GetReportProperty(propertyName, ref returnValue)
                       : propertyOwner.GetUserProperty(propertyName, ref returnValue);
        }

        /// <summary>The get property.</summary>
        /// <param name="propertyOwner">The property owner.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="isReportProperty">The is report property.</param>
        /// <param name="returnValue">The return value.</param>
        /// <returns>The System.Boolean.</returns>
        private static bool GetProperty(
            ModelObject propertyOwner, string propertyName, bool isReportProperty, ref int returnValue)
        {
            return isReportProperty
                       ? propertyOwner.GetReportProperty(propertyName, ref returnValue)
                       : propertyOwner.GetUserProperty(propertyName, ref returnValue);
        }

        /// <summary>The get property.</summary>
        /// <param name="propertyOwner">The property owner.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="isReportProperty">The is report property.</param>
        /// <param name="returnValue">The return value.</param>
        /// <returns>The System.Boolean.</returns>
        private static bool GetProperty(
            ModelObject propertyOwner, string propertyName, bool isReportProperty, ref double returnValue)
        {
            bool result;
            var stringValue = string.Empty;

            if (isReportProperty)
            {
                if (propertyName.Contains("EXTERNAL.") || propertyName.Contains("TOP_LEVEL"))
                {
                    // If reference model object, try first getting string and converting it to double 
                    result = propertyOwner.GetReportProperty(propertyName, ref stringValue);

                    // Try converting from fractional imperial units
                    if (stringValue.Contains("'") || stringValue.Contains("\""))
                    {
                        try
                        {
                            Distance.UseFractionalFormat = true;
                            Distance topLevel;

                            if (Distance.TryParse(
                                stringValue.Trim(), CultureInfo.CurrentCulture, Distance.UnitType.Inch, out topLevel))
                            {
                                returnValue = topLevel.Value;
                                result = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                    else if (string.IsNullOrEmpty(stringValue))
                    {
                        result = propertyOwner.GetReportProperty(propertyName, ref returnValue);

                        // Try double if no string was returned.
                    }
                    else
                    {
                        try
                        {
                            var tempStringValue = stringValue;

                            // Top level is cultured string. And returnend in meters in default. With three decimals. Great.
                            if (propertyName.Contains("TOP_LEVEL"))
                            {
                                tempStringValue = stringValue.Replace(".", string.Empty);
                            }

                            if (
                                !double.TryParse(
                                    tempStringValue, NumberStyles.Any, CultureInfo.CurrentCulture, out returnValue))
                            {
                                // Both ,  and . accepted in all cultures.
                                if (stringValue.Contains("."))
                                {
                                    tempStringValue = stringValue.Replace(".", ",");
                                }
                                else if (stringValue.Contains(","))
                                {
                                    tempStringValue = stringValue.Replace(",", ".");
                                }
                            }

                            if (
                                !double.TryParse(
                                    tempStringValue, NumberStyles.Any, CultureInfo.CurrentCulture, out returnValue))
                            {
                                Debug.WriteLine("GetProperty: string was not convertable.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                }
                else
                {
                    result = propertyOwner.GetReportProperty(propertyName, ref returnValue);
                }
            }
            else
            {
                result = propertyOwner.GetUserProperty(propertyName, ref returnValue);
            }

            return result;
        }

        /// <summary>The get property value based on type.</summary>
        /// <param name="presentedPropertiesInstance">The presented properties instance.</param>
        /// <param name="part">The part value.</param>
        /// <param name="thisProperty">The this property.</param>
        /// <param name="isReportProperty">The is report property.</param>
        /// <param name="roundDoubles">The round doubles.</param>
        /// <returns>The System.String.</returns>
        private static string GetPropertvalueYBasedOnType(
            PresentedPropertiesXml presentedPropertiesInstance, 
            ModelObject part, 
            PresentedProperties thisProperty, 
            bool isReportProperty, 
            bool roundDoubles = true)
        {
            var type = thisProperty.PropertyType;
            var decimals = thisProperty.Decimals;
            var propertyName = thisProperty.ReportPropertyName;

            if (!isReportProperty)
            {
                propertyName = thisProperty.UdaPropertyName;
            }

            var result = string.Empty;
            var intValue = 0;
            var doubleValue = 0.0;
            var stringValue = string.Empty;
            var baseDate = new DateTime(1970, 1, 1);
            var doubleFromat = roundDoubles ? "N" + decimals.ToString(CultureInfo.InvariantCulture) : "G";

            switch (type.ToLower())
            {
                case PropertyTypes.Pieces:
                    result = "1";
                    break;
                case PropertyTypes.Formula:
                    if (GetAttributeByUDAFormula(presentedPropertiesInstance, part, propertyName, out doubleValue))
                    {
                        result = doubleValue.ToString(doubleFromat);
                    }

                    break;
                case PropertyTypes.Int:
                    GetProperty(part, propertyName, isReportProperty, ref intValue);
                    result = intValue.ToString(CultureInfo.InvariantCulture);
                    break;
                case PropertyTypes.Mm:
                case PropertyTypes.Mm2:
                case PropertyTypes.Mm3:
                case PropertyTypes.Double:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.String:
                    GetProperty(part, propertyName, isReportProperty, ref stringValue);
                    result = stringValue;
                    break;
                case PropertyTypes.Date:
                    if (!propertyName.StartsWith("EXTERNAL."))
                    {
                        GetProperty(part, propertyName, isReportProperty, ref intValue);

                        if (intValue != 0)
                        {
                            baseDate = baseDate.AddSeconds(intValue);
                            result = baseDate.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                    else
                    {
                        GetProperty(part, propertyName, isReportProperty, ref stringValue);
                        result = stringValue;
                    }

                    break;
                case PropertyTypes.Kg:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.Ton:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 1000;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.Cm:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 10;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.M:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 1000;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.Cm2:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 100;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.M2:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 1000000;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.Cm3:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 1000;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.M3:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 1000000000;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.Inch:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 25.4;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.Inch2:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 645.16;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.Inch3:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 16387.064;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.Ft:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 304.8;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.Ft2:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 92903.04;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.Ft3:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 28316846.6;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.Yard:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 914.4;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.Yard2:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 836127.36;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.Yard3:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 764554857.984;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.Lbs:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 0.45359237;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.TonShort:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 907.18474;
                    result = doubleValue.ToString(doubleFromat);
                    break;
                case PropertyTypes.TonLong:
                    GetProperty(part, propertyName, isReportProperty, ref doubleValue);
                    doubleValue = doubleValue / 1016.0469088;
                    result = doubleValue.ToString(doubleFromat);
                    break;
            }

            return result ?? string.Empty;
        }

        #endregion

        /// <summary>
        /// The property types.
        /// </summary>
        public struct PropertyTypes
        {
            #region Constants

            /// <summary>
            /// The date value.
            /// </summary>
            public const string Date = "date";

            /// <summary>
            /// The double.
            /// </summary>
            public const string Double = "double";

            /// <summary>
            /// The formula.
            /// </summary>
            public const string Formula = "calc";

            /// <summary>
            /// The int value.
            /// </summary>
            public const string Int = "int";

            /// <summary>
            /// The kg value.
            /// </summary>
            public const string Kg = "kg";

            /// <summary>
            /// The pieces.
            /// </summary>
            public const string Pieces = "pcs";

            /// <summary>
            /// The string.
            /// </summary>
            public const string String = "string";

            /// <summary>
            /// The ton value.
            /// </summary>
            public const string Ton = "ton";

            /// <summary>
            /// The cm value.
            /// </summary>
            public const string Cm = "cm";

            /// <summary>
            /// The cm2 value.
            /// </summary>
            public const string Cm2 = "cm2";

            /// <summary>
            /// The cm3 value.
            /// </summary>
            public const string Cm3 = "cm3";

            /// <summary>
            /// The ft value.
            /// </summary>
            public const string Ft = "ft";

            /// <summary>
            /// The ft2 value.
            /// </summary>
            public const string Ft2 = "ft2";

            /// <summary>
            /// The ft3 value.
            /// </summary>
            public const string Ft3 = "ft3";

            /// <summary>
            /// The inch value.
            /// </summary>
            public const string Inch = "in";

            /// <summary>
            /// The inch 2.
            /// </summary>
            public const string Inch2 = "in2";

            /// <summary>
            /// The inch 3.
            /// </summary>
            public const string Inch3 = "in3";

            /// <summary>
            /// The lbs value.
            /// </summary>
            public const string Lbs = "lbs";

            /// <summary>
            /// The m value.
            /// </summary>
            public const string M = "m";

            /// <summary>
            /// The m2 value.
            /// </summary>
            public const string M2 = "m2";

            /// <summary>
            /// The m3 value.
            /// </summary>
            public const string M3 = "m3";

            /// <summary>
            /// The mm value.
            /// </summary>
            public const string Mm = "mm";

            /// <summary>
            /// The mm2 value.
            /// </summary>
            public const string Mm2 = "mm2";

            /// <summary>
            /// The mm3 value.
            /// </summary>
            public const string Mm3 = "mm3";

            /// <summary>
            /// The ton_long.
            /// </summary>
            public const string TonLong = "ton_long";

            /// <summary>
            /// The ton_short.
            /// </summary>
            public const string TonShort = "ton_short";

            /// <summary>
            /// The yard value.
            /// </summary>
            public const string Yard = "yard";

            /// <summary>
            /// The yard 2.
            /// </summary>
            public const string Yard2 = "yard2";

            /// <summary>
            /// The yard 3.
            /// </summary>
            public const string Yard3 = "yard3";

            #endregion
        }
    }
}