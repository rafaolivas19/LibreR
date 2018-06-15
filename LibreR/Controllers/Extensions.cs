using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Windows;
#if !(MONO)
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
#endif
using System.Windows.Threading;
using LibreR.Embedded;
using LibreR.Embedded.Newtonsoft.Json;
using LibreR.Embedded.Newtonsoft.Json.Converters;
using LibreR.Models;
using LibreR.Models.Enums;
using System.Windows.Media;

namespace LibreR.Controllers {
    /// <summary>
    /// Contains a collection of extensions methods.
    /// </summary>
    public static class Extensions {
        #region Assembly
        /// <summary>
        /// Gets the name of the assembly containing the given type.
        /// </summary>
        /// <typeparam name="T">The type contained in the assembly.</typeparam>
        /// <returns>The assembly's name.</returns>
        public static AssemblyName GetAssembly<T>() {
            return System.Reflection.Assembly.GetAssembly(typeof(T)).GetName();
        }

        /// <summary>
        /// Gets the name of the assembly containing the given type.
        /// </summary>
        /// <param name="type">The type contained in the assembly.</param>
        /// <returns>The assembly's name.</returns>
        public static AssemblyName GetAssembly(this Type type) {
            return System.Reflection.Assembly.GetAssembly(type).GetName();
        }
        #endregion

        #region BitmapSource
		#if !(MONO)
        /// <summary>
        /// Converts a Icon to a Bitmap.
        /// </summary>
        /// <param name="icon">The icon to convert.</param>
        /// <returns>A bitmap source.</returns>
        public static BitmapSource ToImageSource(this System.Drawing.Icon icon) {
            return Imaging.CreateBitmapSourceFromHBitmap(icon.ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
		#endif
        #endregion

        #region DataGrid
		#if !(MONO)
        /// <summary>
        /// Gets the ScrollViewer of a DataGrid.
        /// </summary>
        /// <param name="datagrid">The datagrid.</param>
        /// <returns>The datagrid's ScrollViewer.</returns>
        public static ScrollViewer GetScrollViewer(this DataGrid datagrid) {
            if (VisualTreeHelper.GetChildrenCount(datagrid) == 0) return null;
            var x = VisualTreeHelper.GetChild(datagrid, 0);
            if (x == null) return null;
            if (VisualTreeHelper.GetChildrenCount(x) == 0) return null;
            return VisualTreeHelper.GetChild(x, 0) as ScrollViewer;
        }
		#endif
        #endregion

        #region DateTime

        /// <summary>
        /// Convers a DateTime object to a Unix timestamp.
        /// </summary>
        /// <param name="date">The DateTime object.</param>
        /// <returns>The unix timestamp in seconds.</returns>
        public static long DateTimeToUnixEpoch(DateTime date = default(DateTime)) {
            if (date == default(DateTime)) date = DateTime.Now;

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var today = date.ToUniversalTime();
            var elapsed = (long)(today - epoch).TotalSeconds;

            return elapsed;
        }

        /// <summary>
        /// Convers a DateTime object to a Unix timestamp.
        /// </summary>
        /// <param name="date">The DateTime object.</param>
        /// <returns>The unix timestamp in seconds.</returns>
        public static long ToUnixEpoch(this DateTime date) {
            if (date == default(DateTime)) date = DateTime.Now;

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var today = date.ToUniversalTime();
            var elapsed = (long)(today - epoch).TotalSeconds;

            return elapsed;
        }

        /// <summary>
        /// Gets a DateTime object from a Unix timestamp.
        /// </summary>
        /// <param name="timestamp">The unix timestamp in seconds.</param>
        /// <returns>The corresponding DateTime object.</returns>
        public static DateTime GetDateTimeFromUnixEpoch(double timestamp) {
            var epochTimestamp = timestamp;
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var date = epoch.AddSeconds(epochTimestamp);

            return date;
        }

        #endregion

        #region Dictionary
        /// <summary>
        /// Gets a dictionary's value or the default value for the given type.
        /// </summary>
        /// <typeparam name="T">The type of the dictionary's keys.</typeparam>
        /// <typeparam name="T1">The type of the dictionary's items.</typeparam>
        /// <param name="dictionary">The dictionary object.</param>
        /// <param name="key">The key to look for.</param>
        /// <returns>The value found, or the default value for the type <typeparamref name="T1"/>.</returns>
        public static T1 GetValueOrDefault<T, T1>(this Dictionary<T, T1> dictionary, T key) {
            return dictionary.ContainsKey(key) ? dictionary[key] : default(T1);
        }

        #endregion

        #region Dispatcher

        /// <summary>
        /// Safely invokes an action.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action that will be invoked.</param>
        public static void SafelyInvoke(this Dispatcher dispatcher, Action action) {
            if (!dispatcher.CheckAccess()) {
                dispatcher.BeginInvoke(new Action(() => { SafelyInvoke(dispatcher, action); }));
                return;
            }

            action.Invoke();
        }

        #endregion

        #region FileInfo
        /// <summary>
        /// Creates a new file, writes the specified string to the file, and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <remarks>Extension method for easier access to <see cref="File.WriteAllText(string, string)"/>.</remarks>
        /// <param name="file">The file to write to.</param>
        /// <param name="s">The string to write to the file.</param>
        public static void WriteAllText(this FileInfo file, string s) {
            File.WriteAllText(file.FullName, s);
        }

        /// <summary>
        /// Opens a text file, reads all lines of the file into a string, and then closes the file.
        /// </summary>
        /// <remarks>Extension method for easier access to <see cref="File.ReadAllText(string)"/>.</remarks>
        /// <param name="file">The file to open for reading.</param>
        /// <returns>A string containing all lines of the file.</returns>
        public static string ReadAllText(this FileInfo file) {
            return File.ReadAllText(file.FullName);
        }

        /// <summary>
        /// Gets the name of a file, without its extension.
        /// </summary>
        /// <param name="file">The file to get the name of.</param>
        /// <returns>The name of the file with no extension.</returns>
        public static string GetNameWithoutExtension(this FileInfo file) {
            return file.Extension != string.Empty ?
                file.Name.Split('.')[0] :
                file.Name;
        }
        #endregion

        #region FileSystemInfo

        /// <summary>
        /// Copies a directory to another directory.
        /// </summary>
        /// <param name="sourceDirName">The directory to copy from.</param>
        /// <param name="destDirName">The destination to copy to.</param>
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

        /// <summary>
        /// Enables serialization of Enums.
        /// </summary>
        public static void EnableEnumStringFormatSerialization() {
            JsonConvert.DefaultSettings = () => {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringEnumConverter());
                return settings;
            };
        }

        /// <summary>
        /// Disables serialization of Enums.
        /// </summary>
        public static void DisableEnumStringFormatSerialization() {
            JsonConvert.DefaultSettings = () => {
                var settings = new JsonSerializerSettings();
                settings.Converters.Remove(new StringEnumConverter());
                return settings;
            };
        }

        /// <summary>
        /// Serializes any object to its JSON representation.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="serializer">The serializer to use.</param>
        /// <returns>A string representing the object.</returns>
        /// <seealso cref="Serializer"/>
        public static string Serialize(this object obj, Serializer serializer = Serializer.PrettyFormat) {
            if (_isEnumStringFormatEnable) return JsonConvert.SerializeObject(obj, BinderSerializers[serializer]);

            EnableEnumStringFormatSerialization();
            _isEnumStringFormatEnable = true;

            return JsonConvert.SerializeObject(obj, BinderSerializers[serializer]);
        }

        /// <summary>
        /// Deserealizes a serial string into an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The expected type of the object.</typeparam>
        /// <param name="obj">The string to deserealize.</param>
        /// <param name="converterType">The type of the object that will be desearialized.</param>
        /// <returns>The resulting object.</returns>
        public static T Deserialize<T>(this string obj, Type converterType = null) {
            if (converterType == null) return JsonConvert.DeserializeObject<T>(obj);

            if (!converterType.IsSubclassOf(typeof(JsonConverter))) throw new ApplicationException("Type is not a JsonConverter");

            if (!BinderConverters.ContainsKey(converterType)) {
                BinderConverters.Add(converterType, (JsonConverter)Activator.CreateInstance(converterType));
            }

            return JsonConvert.DeserializeObject<T>(obj, BinderConverters[converterType]);
        }

        /// <summary>
        /// Deserealizes a serial string into an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The expected type of the object.</typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="obj">The string to deserealize.</param>
        /// <param name="converterType">The type of the object that will be desearialized.</param>
        /// <returns>The resulting object.</returns>
        public static T Deserialize<T, T1>(this string obj) {
            return obj.Deserialize<T>(typeof(T1));
        }
        #endregion

        #region MethodInfo
        /// <summary>
        /// Invokes a method.
        /// </summary>
        /// <param name="methodInfo">The method to invoke.</param>
        /// <param name="obj">The instance of the class from where the method will be invoked.</param>
        /// <param name="parameters">The method's parameters.</param>
        /// <returns>The returned value of the method.</returns>
        public static object Invoke(this MethodInfo methodInfo, object obj, params object[] parameters) {
            return methodInfo.Invoke(obj, parameters);
        }
        #endregion

        #region NetworkInterface
        /// <summary>
        /// Gets the MAC address of the host device.
        /// </summary>
        /// <returns>The MAC address.</returns>
        public static string GetMacAddress() {
            return (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
                ).FirstOrDefault();
        }

        #endregion

        #region Object
        /// <summary>
        /// Gets an objects property by name.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="obj">The object to get the property from.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The property.</returns>
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
		#if !(MONO)
        /// <summary>
        /// Gets the children of a <see cref="Panel"/>.
        /// </summary>
        /// <typeparam name="T">The type of the panel's children.</typeparam>
        /// <param name="panel">The panel to get the children from.</param>
        /// <returns>The panel's children.</returns>
        public static T[] GetChildren<T>(this Panel panel) {
            return panel.Children.Cast<object>().Where(x => x.GetType() == typeof(T)).Cast<T>().ToArray();
        }

        /// <summary>
        /// Gets the children of a panel recursively.
        /// </summary>
        /// <typeparam name="T">The type of the children.</typeparam>
        /// <param name="panel">The panel to get the children from.</param>
        /// <returns>The panel's children.</returns>
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

        /// <summary>
        /// Gets the content of a <see cref="Decorator"/>.
        /// </summary>
        /// <param name="decorator">The decorator to get the content from.</param>
        /// <returns>The decorator's content.</returns>
        public static object GetDecoratorContent(this Decorator decorator) {
            while (true) {
                var child = decorator.Child as Decorator;

                if (child != null)
                    decorator = child;
                else
                    return decorator.Child;
            }
        }
		#endif
        #endregion

        #region Property
        /// <summary>
        /// Checks if a Property has a specific attribute.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <param name="attributeType">The type of attribute to check for.</param>
        /// <returns><c>true</c> if the Property has the attribute, <c>false</c> otherwise.</returns>
        public static bool HasAttribute(this PropertyInfo property, Type attributeType) {
            return Attribute.IsDefined(property, attributeType);
        }

        /// <summary>
        /// Checks if a Property has an attribute of a certain type.
        /// </summary>
        /// <typeparam name="T">The type of attribute to check for.</typeparam>
        /// <param name="property">The property to check.</param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this PropertyInfo property) {
            return property.HasAttribute(typeof(T));
        }

        /// <summary>
        /// Gets an attribute from a Property.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <param name="attribute">The type of attribute.</param>
        /// <returns>The attribute.</returns>
        public static object GetAttribute(this PropertyInfo property, Type attribute) {
            return property
                .GetCustomAttributes(true)
                .FirstOrDefault(x => x.GetType() == attribute);
        }

        /// <summary>
        /// Gets an attribute from a Property.
        /// </summary>
        /// <typeparam name="T">The type of attribute</typeparam>
        /// <param name="property">The property to get the attribute of.</param>
        /// <returns>The attribute.</returns>
        public static T GetAttribute<T>(this PropertyInfo property) {
            return (T)property.GetAttribute(typeof(Attribute));
        }

        /// <summary>
        /// Gets the value of Property's attribute.
        /// </summary>
        /// <param name="property">The property to get the attribute's value of.</param>
        /// <param name="attributeType">The type of attribute.</param>
        /// <param name="valueName">The value of the attribute./param>
        /// <returns></returns>
        public static object GetAttributeValue(this PropertyInfo property, Type attributeType, string valueName) {
            if (!property.HasAttribute(attributeType)) return null;
            var attribute = property.GetAttribute(attributeType);
            return attribute.GetType().GetProperty(valueName).GetValue(attribute, null);
        }

        /// <summary>
        /// Gets the value of a Property's attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <typeparam name="TValue">The type of the attribute's value.</typeparam>
        /// <param name="property">The property to get the attribute's value of.</param>
        /// <param name="valueName">The attribute's name.</param>
        /// <returns>The value of the attribute.</returns>
        public static TValue GetAttributeValue<TAttribute, TValue>(this PropertyInfo property, string valueName) {
            return (TValue)property.GetAttributeValue(typeof(TAttribute), valueName);
        }

        #endregion

        #region RichTextBox
		#if !(MONO)
        /// <summary>
        /// Gets the text if a <see cref="RichTextBox"/>.
        /// </summary>
        /// <param name="box">The box to get the text of.</param>
        /// <returns>The text of the box.</returns>
        public static string GetText(this System.Windows.Controls.RichTextBox box) {
            return new TextRange(box.Document.ContentStart, box.Document.ContentEnd).Text;
        }
		#endif
        #endregion

        #region String
        /// <summary>
        /// Checks if a string represents an <see cref="int"/>.
        /// </summary>
        /// <param name="x">The string to check.</param>
        /// <returns><c>true</c> if the string represents an <see cref="int"/>, <c>false</c> otherwise.</returns>
        public static bool IsInt(this string x) {
            int aux;
            return int.TryParse(x, out aux);
        }

        /// <summary>
        /// Checks if a string represents an <see cref="int"/>.
        /// </summary>
        /// <param name="x">The string to check.</param>
        /// <returns><c>true</c> if the string represents an <see cref="int"/>, <c>false</c> otherwise.</returns>
        public static bool IsUShort(this string x) {
            ushort aux;
            return ushort.TryParse(x, out aux);
        }

        /// <summary>
        /// Checks if a string represents an <see cref="int"/>.
        /// </summary>
        /// <param name="x">The string to check.</param>
        /// <returns><c>true</c> if the string represents an <see cref="int"/>, <c>false</c> otherwise.</returns>
        public static bool IsByte(this string x) {
            byte aux;
            return byte.TryParse(x, out aux);
        }

        /// <summary>
        /// Checks if a string represents an <see cref="int"/>.
        /// </summary>
        /// <param name="x">The string to check.</param>
        /// <returns><c>true</c> if the string represents an <see cref="int"/>, <c>false</c> otherwise.</returns>
        public static bool IsLong(this string x) {
            long aux;
            return long.TryParse(x, out aux);
        }

        /// <summary>
        /// Checks if a string represents an <see cref="int"/>.
        /// </summary>
        /// <param name="x">The string to check.</param>
        /// <returns><c>true</c> if the string represents an <see cref="int"/>, <c>false</c> otherwise.</returns>
        public static bool IsDouble(this string x) {
            double aux;
            return double.TryParse(x, out aux);
        }

        /// <summary>
        /// Checks if a string represents an <see cref="decimal"/>.
        /// </summary>
        /// <param name="x">The string to check.</param>
        /// <returns><c>true</c> if the string represents an <see cref="decimal"/>, <c>false</c> otherwise.</returns>
        public static bool IsDecimal(this string x) {
            decimal aux;
            return decimal.TryParse(x, out aux);
        }

        /// <summary>
        /// Checks if a string represents an <see cref="bool"/>.
        /// </summary>
        /// <param name="x">The string to check.</param>
        /// <returns><c>true</c> if the string represents an <see cref="bool"/>, <c>false</c> otherwise.</returns>
        public static bool IsBool(this string x) {
            bool aux;
            return bool.TryParse(x, out aux);
        }

        /// <summary>
        /// Checks if a string represents an <see cref="DateTime"/>.
        /// </summary>
        /// <param name="x">The string to check.</param>
        /// <returns><c>true</c> if the string represents an <see cref="DateTime"/>, <c>false</c> otherwise.</returns>
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
