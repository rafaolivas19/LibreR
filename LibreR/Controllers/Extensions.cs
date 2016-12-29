using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using LibreR.Embedded;
using LibreR.Embedded.Newtonsoft.Json;
using LibreR.Embedded.Newtonsoft.Json.Converters;
using LibreR.Models;
using LibreR.Models.Enums;

namespace LibreR.Controllers {
    public static class Extensions {
        #region Assembly
        public static AssemblyName GetAssembly<T>() {
            return System.Reflection.Assembly.GetAssembly(typeof(T)).GetName();
        }

        public static AssemblyName GetAssembly(this Type type) {
            return System.Reflection.Assembly.GetAssembly(type).GetName();
        }
        #endregion

        #region BitmapSource
        public static BitmapSource ToImageSource(this System.Drawing.Icon icon) {
            return Imaging.CreateBitmapSourceFromHBitmap(icon.ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
        #endregion

        #region DateTime

        public static long DateTimeToUnixEpoch(DateTime date = default(DateTime)) {
            if (date == default(DateTime)) date = DateTime.Now;

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var today = date.ToUniversalTime();
            var elapsed = (long)(today - epoch).TotalSeconds;

            return elapsed;
        }

        public static DateTime GetDateTimeFromUnixEpoch(long timestamp) {
            var epochTimestamp = timestamp;
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var date = epoch.AddSeconds(epochTimestamp);

            return date;
        }

        #endregion

        #region Dictionary

        public static T1 GetValueOrDefault<T, T1>(this Dictionary<T, T1> dictionary, T key) {
            return dictionary.ContainsKey(key) ? dictionary[key] : default(T1);
        }

        #endregion

        #region Dispatcher

        public static void SafelyInvoke(this Dispatcher dispatcher, Action action) {
            if (!dispatcher.CheckAccess()) {
                dispatcher.BeginInvoke(new Action(() => { SafelyInvoke(dispatcher, action); }));
                return;
            }

            action.Invoke();
        }

        #endregion

        #region FileInfo
        public static void WriteAllText(this FileInfo file, string s) {
            File.WriteAllText(file.FullName, s);
        }

        public static string ReadAllText(this FileInfo file) {
            return File.ReadAllText(file.FullName);
        }

        public static string GetNameWithoutExtension(this FileInfo file) {
            return file.Extension != string.Empty ?
                file.Name.Split('.')[0] :
                file.Name;
        }
        #endregion

        #region FileSystemInfo

        public static void CopyTo(this FileSystemInfo sourceDirName, string destDirName) {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourceDirName.FullName, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourceDirName.FullName, destDirName));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourceDirName.FullName, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourceDirName.FullName, destDirName), true);
        }

        #endregion

        #region Json

        private static bool _isEnumStringFormatEnable;

        private static readonly Dictionary<Type, JsonConverter> BinderConverters = new Dictionary<Type, JsonConverter>();

        private static readonly Dictionary<Serializer, JsonSerializerSettings> BinderSerializers = new Dictionary<Serializer, JsonSerializerSettings> {
            { Serializer.PrettyFormat, new JsonSerializerSettings{ NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented } },
            { Serializer.PrettyFormatWithNullValues, new JsonSerializerSettings{ Formatting = Formatting.Indented } },
            { Serializer.OneLine, new JsonSerializerSettings{ NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.None } },
            { Serializer.OneLineWithNullValues, new JsonSerializerSettings{ Formatting = Formatting.None } }
        };

        public static void EnableEnumStringFormatSerialization() {
            JsonConvert.DefaultSettings = () => {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringEnumConverter());
                return settings;
            };
        }

        public static void DisableEnumStringFormatSerialization() {
            JsonConvert.DefaultSettings = () => {
                var settings = new JsonSerializerSettings();
                settings.Converters.Remove(new StringEnumConverter());
                return settings;
            };
        }

        public static string Serialize(this object obj, Serializer serializer = Serializer.PrettyFormat) {
            if (_isEnumStringFormatEnable) return JsonConvert.SerializeObject(obj, BinderSerializers[serializer]);

            EnableEnumStringFormatSerialization();
            _isEnumStringFormatEnable = true;

            return JsonConvert.SerializeObject(obj, BinderSerializers[serializer]);
        }

        public static T Deserialize<T>(this string obj, Type converterType = null) {
            if (converterType == null) return JsonConvert.DeserializeObject<T>(obj);

            if (!converterType.IsSubclassOf(typeof(JsonConverter))) throw new ApplicationException("Type is not a JsonConverter");

            if (!BinderConverters.ContainsKey(converterType)) {
                BinderConverters.Add(converterType, (JsonConverter)Activator.CreateInstance(converterType));
            }

            return JsonConvert.DeserializeObject<T>(obj, BinderConverters[converterType]);
        }

        public static T Deserialize<T, T1>(this string obj) {
            return obj.Deserialize<T>(typeof(T1));
        }
        #endregion

        #region MethodInfo
        public static object Invoke(this MethodInfo methodInfo, object obj, params object[] parameters) {
            return methodInfo.Invoke(obj, parameters);
        }
        #endregion

        #region NetworkInterface

        public static string GetMacAddress()
        {
            return (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
                ).FirstOrDefault();
        }

        #endregion

        #region Object
        public static T GetProperty<T>(this object obj, string name) {
            var propertyInstance = obj.GetType().GetProperties().FirstOrDefault(x => x.Name == name);
            var result = propertyInstance?.GetValue(obj, null);

            if (result == null) return default(T);

            if (result.GetType() != typeof(T))
                throw new ApplicationException($"Property type is {result.GetType()} not {typeof(T)}");

            return (T)result;
        }
        #endregion

        #region Panel
        public static T[] GetChildren<T>(this Panel panel) {
            return panel.Children.Cast<object>().Where(x => x.GetType() == typeof(T)).Cast<T>().ToArray();
        }

        public static List<T> GetChildrenRecursively<T>(this Panel panel) {
            var components = new List<T>();

            foreach (var x in panel.Children) {
                var control = x is Decorator ? ((Decorator)x).GetDecoratorContent() : x;

                if (control is Panel) {
                    components = components.Concat((control as Panel).GetChildrenRecursively<T>()).ToList();
                }
                else if (control is T) {
                    components.Add((T)control);
                }
            }

            return components;
        }

        public static object GetDecoratorContent(this Decorator decorator) {
            while (true) {
                var child = decorator.Child as Decorator;

                if (child != null)
                    decorator = child;
                else
                    return decorator.Child;
            }
        }

        #endregion

        #region Property

        public static bool HasAttribute(this PropertyInfo property, Type attributeType) {
            return Attribute.IsDefined(property, attributeType);
        }

        public static bool HasAttribute<T>(this PropertyInfo property) {
            return property.HasAttribute(typeof(T));
        }

        public static object GetAttribute(this PropertyInfo property, Type attribute) {
            return property
                .GetCustomAttributes(true)
                .FirstOrDefault(x => x.GetType() == attribute);
        }

        public static T GetAttribute<T>(this PropertyInfo property) {
            return (T)property.GetAttribute(typeof(Attribute));
        }

        public static object GetAttributeValue(this PropertyInfo property, Type attributeType, string valueName) {
            if (!property.HasAttribute(attributeType)) return null;
            var attribute = property.GetAttribute(attributeType);
            return attribute.GetType().GetProperty(valueName).GetValue(attribute, null);
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(this PropertyInfo property, string valueName) {
            return (TValue)property.GetAttributeValue(typeof(TAttribute), valueName);
        }

        #endregion

        #region RichTextBox
        public static string GetText(this System.Windows.Controls.RichTextBox box) {
            return new TextRange(box.Document.ContentStart, box.Document.ContentEnd).Text;
        }
        #endregion

        #region String
        public static bool IsInt(this string x) {
            int aux;
            return int.TryParse(x, out aux);
        }

        public static bool IsUShort(this string x) {
            ushort aux;
            return ushort.TryParse(x, out aux);
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

        public static bool IsDecimal(this string x) {
            decimal aux;
            return decimal.TryParse(x, out aux);
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
            if (!s.IsInt()) throw new LibrerException("Value is not an int");
            return int.Parse(s);
        }

        public static ushort ToUShort(this string s) {
            if (!s.IsUShort()) throw new LibrerException("Value is not an ushort");
            return ushort.Parse(s);
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

        public static decimal ToDecimal(this string s) {
            if (s.Contains('$')) s = s.Replace("$", string.Empty);
            if (!s.IsDecimal()) throw new LibrerException("Value is not a decimal");
            return decimal.Parse(s);
        }

        public static bool IsNullOrEmpty(this string s) {
            return string.IsNullOrEmpty(s);
        }
        #endregion
    }
}
