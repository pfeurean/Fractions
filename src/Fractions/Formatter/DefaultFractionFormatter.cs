﻿using System;
using System.Globalization;
using System.Numerics;
using System.Text;
using Fractions.Properties;

namespace Fractions.Formatter {
    internal class DefaultFractionFormatter : ICustomFormatter {
        public static readonly ICustomFormatter Instance = new DefaultFractionFormatter();

        public string Format(string format, object arg, IFormatProvider formatProvider) {
            if (arg == null) {
                return string.Empty;
            }

            if (!(arg is Fraction)) {
                throw new FormatException(string.Format(Resources.TypeXnotSupported, arg.GetType()));
            }

            var fraction = (Fraction)arg;

            if (string.IsNullOrEmpty(format) || format == "G") {
                return FormatGeneral(fraction);
            }
            
            var sb = new StringBuilder(32);
            foreach (var character in format) {
                switch (character) {
                    case 'G':
                        sb.Append(FormatGeneral(fraction));
                        break;
                    case 'n':
                        sb.Append(fraction.Numerator.ToString(CultureInfo.InvariantCulture));
                        break;
                    case 'd':
                        sb.Append(fraction.Denominator.ToString(CultureInfo.InvariantCulture));
                        break;
                    case 'z':
                        sb.Append(FormatInteger(fraction));
                        break;
                    case 'r':
                        sb.Append(FormatRemainder(fraction));
                        break;
                    case 'm':
                    case '-':
                        sb.Append(FormatMixed(fraction, character == '-' ? '-' : ' '));
                        break;
                    default:
                        sb.Append(character);
                        break;
                }
            }
            return sb.ToString();
        }

        private static string FormatMixed(Fraction fraction, char separator) {
            if (BigInteger.Abs(fraction.Numerator) < BigInteger.Abs(fraction.Denominator)) {
                return FormatGeneral(fraction);
            }

            var integer = fraction.Denominator != BigInteger.Zero
                ? fraction.Numerator / fraction.Denominator
                : BigInteger.Zero;

            var remainder = Fraction.Abs(fraction - integer);

            return remainder.IsZero
                ? integer.ToString(CultureInfo.InvariantCulture)
                : string.Concat(
                    integer.ToString(CultureInfo.InvariantCulture),
                    separator,
                    FormatGeneral(remainder));
        }

        private static string FormatInteger(Fraction fraction) {
            return fraction.Denominator != BigInteger.Zero
                ? (fraction.Numerator / fraction.Denominator).ToString(CultureInfo.InvariantCulture)
                : BigInteger.Zero.ToString(CultureInfo.InvariantCulture);
        }

        private static string FormatRemainder(Fraction fraction) {
            if (BigInteger.Abs(fraction.Numerator) < BigInteger.Abs(fraction.Denominator)) {
                return FormatGeneral(fraction);
            }

            var integer = fraction.Denominator != BigInteger.Zero
                ? fraction.Numerator / fraction.Denominator
                : BigInteger.Zero;

            var remainder = fraction - integer;
            return FormatGeneral(remainder);
        }

        private static string FormatGeneral(Fraction fraction) {
            if (fraction.Denominator == BigInteger.Zero) {
                return BigInteger.Zero.ToString(CultureInfo.InvariantCulture);
            }

            if (fraction.Denominator == BigInteger.One) {
                return fraction.Numerator.ToString(CultureInfo.InvariantCulture);
            }

            return string.Concat(
                fraction.Numerator.ToString(CultureInfo.InvariantCulture),
                "/",
                fraction.Denominator.ToString(CultureInfo.InvariantCulture));
        }
    }
}
