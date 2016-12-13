using System;
using LibreR.Models;

namespace LibreR.Controllers {
    public static class TypeTry {
        public static bool IsInt(this string x) {
            int aux;
            return int.TryParse(x, out aux);
        }

        public static bool IsByte(this string x) {
            byte aux;
            return byte.TryParse(x, out aux);
        }

        public static bool IsLong(this string x) {
            long aux;
            return long.TryParse(x, out aux);
        }

        public static bool IsDouble(this string x) {
            double aux;
            return double.TryParse(x, out aux);
        }

        public static bool IsBool(this string x) {
            bool aux;
            return bool.TryParse(x, out aux);
        }

        public static bool IsDateTime(this string x) {
            DateTime aux;
            return DateTime.TryParse(x, out aux);
        }

        public static bool EqualsIgnoreCase(this string a, string b) {
            return String.Equals(a, b, StringComparison.CurrentCultureIgnoreCase);
        }

        public static int ToInt(this string s) {
            if(!s.IsInt()) throw new LibrerException("Value is not an int");
            return int.Parse(s);
        }

        public static byte ToByte(this string s) {
            if (!s.IsByte()) throw new LibrerException("Value is not an byte");
            return byte.Parse(s);
        }

        public static long ToLong(this string s) {
            if (!s.IsLong()) throw new LibrerException("Value is not a long");
            return long.Parse(s);
        }

        public static double ToDouble(this string s) {
            if (!s.IsDouble()) throw new LibrerException("Value is not a double");
            return double.Parse(s);
        }
    }
}
