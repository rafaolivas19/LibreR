using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using LibreR.Models.Logger;

namespace LibreR.Controllers {
    public class Logger {
        public LogFile LogFile {
            get {
                if (DateTime.Now.Date != _logFile.Date.Date) {
                    _logFile = new LogFile(_logFile.Name);
                }

                return _logFile;
            }
            set { _logFile = value; }
        }

        private LogFile _logFile;
        private readonly int _lineLength;
        private bool _headerWritten;
        private readonly string _separator;
        private readonly string _headerMessage;
        private bool _isSecureCopyActive;
        private string _securePath;
        private string _secureExtension;
        private Security _security;

        public Logger() : this($"{new StackFrame(1).GetMethod().DeclaringType.GetAssembly().Name}") { }

        public Logger(string name, int lineLength) : this(name, "LOGGER SESION STARTS", '■', lineLength) { }

        public Logger(string name, string headerMessage = "LOGGER SESION STARTS", char separatorChar = '■', int lineLength = 120) {
            _lineLength = lineLength;
            _separator = GetSeparator(separatorChar, lineLength);
            _headerMessage = headerMessage;

            LogFile = new LogFile(name);
            Directory.CreateDirectory(LogFile.DirectoryName ?? "Default");
        }

        private void Header() {
            lock (this) {
                var text = String.Format("{0}{2}{1}{2}{0}{2}", _separator, _headerMessage, Environment.NewLine);

                File.AppendAllText(LogFile.ToString(), text);
                SecureMessage(text, LogFile.Date);
            }

            _headerWritten = true;
        }

        public void Message(object message) {
            Message(message.ToString());
        }

        public void Message(object message, string tag) {
            message = string.Format("[{1}] {0}", message, tag);
            Message(message);
        }

        public void Message(object message, Exception ex) {
            var m = $"{message}: {ex}";
            Message(m);
        }

        private void Message(string message) {
            if (!_headerWritten) Header();

            var msg = message;
            var registered = false;
            var sb = new StringBuilder();

            foreach (var aux in msg.Split(new[] { Environment.NewLine }, StringSplitOptions.None)) {
                if (registered) {
                    var hour = DateTime.Now.ToString("hh:mm:ss.fff tt");
                    var space = new string(' ', hour.Length + 3);

                    sb.Append(Environment.NewLine);
                    sb.Append(string.Format("{1}{0}", aux, space));

                    continue;
                }

                sb.Append($"[{DateTime.Now.ToString("hh:mm:ss.fff tt")}] {aux}");
                registered = true;
            }

            lock (this) {
                var text = $"{sb}{Environment.NewLine}";

                File.AppendAllText(LogFile.FullName, text);
                SecureMessage(text, LogFile.Date);
            }
        }

        public void EnableSecureCopy(string path, string extension, string key) {
            _isSecureCopyActive = true;
            _securePath = path;
            _secureExtension = extension;
            _security = new Security(key);

            var directory = $"{path}/{LogFile.Name}";

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        }

        public void EnableSecureCopy(string key) {
            EnableSecureCopy($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/ol-GS", "bin", key);
        }

        public static void GenerateReadableCopy(string ori, string copy, string key) {
            var security = new Security(key);
            var content = File.ReadAllText(ori).Split('♦').ToList();
            var sb = new StringBuilder();

            content.Remove(content.Last());

            foreach (var x in content) {
                sb.Append(security.Decrypt(x));
            }

            File.WriteAllText(copy, sb.ToString());
        }

        private void SecureMessage(string text, DateTime? date = null) {
            if (_isSecureCopyActive)
                File.AppendAllText(
                    $"{_securePath}/{LogFile.Name}/{date?.ToString("dd-MM-yyyy") ?? string.Empty}.{_secureExtension}",
                    _security.Encrypt(text) + '♦');
        }

        private static string GetSeparator(char symbol, int length) {
            var sb = new StringBuilder();

            for (var i = 0; i < length; i++) {
                sb.Append(symbol);
            }

            return sb.ToString();
        }
    }
}
