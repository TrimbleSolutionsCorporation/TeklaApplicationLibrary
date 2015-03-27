namespace ConversionTool
{
    using System;
    using System.Text.RegularExpressions;

    using Tekla.Structures.InpParser;

    /// <summary>
    /// The unit converter.
    /// </summary>
    public class UnitConverter
    {
        #region Constants

        /// <summary>
        /// The foo t_ t o_ inc h_ factor.
        /// </summary>
        private const double FootToInchFactor = 12;

        /// <summary>
        /// The fractio n_ divisor.
        /// </summary>
        private const int FractionDivisor = 128;

        /// <summary>
        /// The inc h_ t o_ m m_ factor.
        /// </summary>
        private const double InchToMmFactor = 25.4;

        /// <summary>
        /// The m m_ t o_ inc h_ factor.
        /// </summary>
        private const double MmToInchFactor = 1.0 / 25.4;

        #endregion

        #region Fields

        /// <summary>
        /// The imperial units.
        /// </summary>
        private readonly bool imperialUnits;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="UnitConverter"/> class.</summary>
        /// <param name="convertToImperial">The convert to imperial.</param>
        public UnitConverter(bool convertToImperial)
        {
            this.imperialUnits = convertToImperial;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Method to convert a length in the current unit being used in TS to mm.</summary>
        /// <param name="value">Value to convert to mm.</param>
        /// <exception cref="WrongFormatException">Thrown when conversion can't be done because input data format is not correct.</exception>
        /// <returns>D.uble containing Value in mm.</returns>
        public double ConvertFromCurrentUnitsToMm(string value)
        {
            double result;
            double inches;

            if (this.imperialUnits)
            {
                inches = this.ConvertTextFractionalInchesToDecimalInches(value);
                result = InchToMmFactor * inches;
            }
            else
            {
                result = Convert.ToDouble(value);
            }

            return result;
        }

        /// <summary>Method to convert a length in mm to the current unit being used in TS.</summary>
        /// <param name="value">Value to convert to current units.</param>
        /// <returns>String containing Value in the current units.</returns>
        public string ConvertFromMmToCurrentUnits(double value)
        {
            return this.imperialUnits ? ConvertFromMmToFractionalInches(value) : value.ToString("f2");
        }

        /// <summary>Method to format the input value in dialogs. For instance it could be used 
        ///     within a callback like EventHandler.Leave.</summary>
        /// <param name="value">Value to format.</param>
        /// <exception cref="WrongFormatException">Thrown when formatting in fractional inches 
        /// can't be done because input data format is not correct.</exception>
        /// <exception cref="System.InvalidCastException">Thrown when formatting in mm 
        /// an't be done because input data format is not correct.</exception>
        /// <returns>String containing Value in the appropriate format.</returns>
        public string FormatNumericTextIfNecessary(string value)
        {
            return this.imperialUnits ? this.FormatFractionalInchesText(value) : Convert.ToDouble(value).ToString("f2");
        }

        /// <summary>Method that tries to convert a length in the current unit being used in TS to mm.</summary>
        /// <param name="value">Value to convert to mm.</param>
        /// <param name="convertedResult">Double containing Value in mm.</param>
        /// <returns>True if Value can be converted, false if not.</returns>
        public bool TryConvertFromCurrentUnitsToMm(string value, out double convertedResult)
        {
            bool result;
            var convertedMm = 0.0;

            try
            {
                convertedMm = this.ConvertFromCurrentUnitsToMm(value);
                result = true;
            }
            catch
            {
                result = false;
            }

            convertedResult = convertedMm;
            return result;
        }

        /// <summary>Method that tries to convert a length in mm to the current unit being used in TS.</summary>
        /// <param name="value">Value to convert to current units.</param>
        /// <param name="convertResult">The Convert Result.</param>
        /// <returns>True if Value can be converted, false if not.</returns>
        public bool TryConvertFromMmToCurrentUnits(double value, out string convertResult)
        {
            bool result;
            var currentUnits = string.Empty;

            try
            {
                currentUnits = this.ConvertFromMmToCurrentUnits(value);
                result = true;
            }
            catch
            {
                result = false;
            }

            convertResult = currentUnits;
            return result;
        }

        /// <summary>Method that tries to format the input value in dialogs.</summary>
        /// <param name="value">Value to format.</param>
        /// <param name="formatedResult">String containing Value in the appropriate format.</param>
        /// <returns>True if Value can be formatted, false if not.</returns>
        public bool TryFormatNumericTextIfNecessary(string value, out string formatedResult)
        {
            bool result;
            var formatText = string.Empty;

            try
            {
                formatText = this.FormatNumericTextIfNecessary(value);
                result = true;
            }
            catch
            {
                result = false;
            }

            formatedResult = formatText;
            return result;
        }

        #endregion

        #region Methods

        /// <summary>The convert from mm to fractional inches.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The System.String.</returns>
        private static string ConvertFromMmToFractionalInches(double value)
        {
            var inchesValue = MmToInchFactor * value;

            return GetFractionalInchesText(inchesValue);
        }

        /// <summary>The get fractional inches text.</summary>
        /// <param name="inchesValue">The inches value.</param>
        /// <returns>The System.String.</returns>
        private static string GetFractionalInchesText(double inchesValue)
        {
            var result = string.Empty;
            int sign = 1, divisor = FractionDivisor;

            if (inchesValue < 0.0)
            {
                sign = -1;
                inchesValue = Math.Abs(inchesValue);
            }

            var feetRemainder = inchesValue / FootToInchFactor;
            var feet = (int)feetRemainder;

            var inchesRemainder = (feetRemainder - feet) * 12.0;
            var inches = (int)inchesRemainder;

            var fractionRemainder = (inchesRemainder - Convert.ToDouble(inches)) * Convert.ToDouble(divisor);
            var fraction = (int)(fractionRemainder + 0.5);

            if (fraction == divisor)
            {
                fraction = 0;
                inches++;
            }

            if (inches == 12)
            {
                inches = 0;
                feet++;
            }

            while ((divisor % 2) == 0 && (fraction % 2) == 0 && fraction > 1 && divisor > 1)
            {
                fraction = fraction / 2;
                divisor = divisor / 2;
            }

            if (sign == -1 && (feet != 0 || inches != 0 || fraction != 0))
            {
                result = "-";
            }

            if (feet != 0)
            {
                result = result + feet.ToString() + "\'";

                if (inches != 0 || fraction != 0)
                {
                    result = result + "-";
                }
            }

            if (inches != 0 || fraction != 0)
            {
                result = result + inches.ToString() + "\"";
            }

            if (fraction != 0)
            {
                result = result + " " + fraction.ToString() + "/" + divisor.ToString();
            }

            if (result == string.Empty)
            {
                result = "0";
            }

            return result;
        }

        /// <summary>The convert fraction to decimal.</summary>
        /// <param name="text">The text value.</param>
        /// <returns>The System.Double.</returns>
        /// <exception cref="Exception">Throws an exception.</exception>
        private double ConvertFractionToDecimal(string text)
        {
            double result;
            const string TextMatch = @"(\d+)[^0-9]*(\d+)/(\d+)";
            const string TextMatch1 = @"(\d+)/(\d+)";
            const string TextMatch2 = @"(\d+)\.(\d+)";
            const string TextMatch3 = @"\.?(\d+)";

            if (text.Equals(string.Empty))
            {
                text = "0";
            }

            var textFound = Regex.Match(text, TextMatch);
            var textFound1 = Regex.Match(text, TextMatch1);
            var textFound2 = Regex.Match(text, TextMatch2);
            var textFound3 = Regex.Match(text, TextMatch3);

            if (textFound.Success)
            {
                var textNumbers = textFound.Groups;
                result = Convert.ToDouble(textNumbers[1].Value);
                result += Convert.ToDouble(textNumbers[2].Value) / Convert.ToDouble(textNumbers[3].Value);
            }
            else if (textFound1.Success)
            {
                var textNumbers = textFound1.Groups;
                result = Convert.ToDouble(textNumbers[1].Value) / Convert.ToDouble(textNumbers[2].Value);
            }
            else if (textFound2.Success)
            {
                var textNumbers = textFound2.Groups;
                result = Convert.ToDouble(textNumbers[0].Value);
            }
            else if (textFound3.Success)
            {
                var textNumbers = textFound3.Groups;
                result = Convert.ToDouble(textNumbers[0].Value);
            }
            else
            {
                throw new Exception("WrongFormatException");
            }

            return result;
        }

        /// <summary>The convert text fractional inches to decimal inches.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The System.Double.</returns>
        private double ConvertTextFractionalInchesToDecimalInches(string value)
        {
            var result = 0.0;
            var feetAndInchesText = value.Split('\'');

            if (feetAndInchesText.Length.CompareTo(2) == 0)
            {
                result = FootToInchFactor * this.ConvertFractionToDecimal(feetAndInchesText[0]);
                result += this.ConvertFractionToDecimal(feetAndInchesText[1]);
            }
            else if (feetAndInchesText.Length.CompareTo(1) == 0)
            {
                result = this.ConvertFractionToDecimal(feetAndInchesText[0]);
            }

            return result;
        }

        /// <summary>The format fractional inches text.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The System.String.</returns>
        private string FormatFractionalInchesText(string value)
        {
            var feetAndInchesText = value.Split('\'');

            var inchesValue = this.ConvertTextFractionalInchesToDecimalInches(value);
            var result = GetFractionalInchesText(inchesValue);

            return result;
        }

        #endregion
    }
}