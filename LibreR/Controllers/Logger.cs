﻿using System;
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
using System.Text.RegularExpressions;
using System.Threading;
using LibreR.Models.Logger;

namespace LibreR.Controllers {
    /// <summary>
    /// Represents a logger handler.
    /// </summary>
    /// <remarks>The log handler implements date rollover, creating a new log file for every day.</remarks>
    public class Logger {

        /// <summary>
        /// Gets the current log file.
        /// </summary>
        /// <value>The currently used log file.</value>
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

        /// <summary>
        /// Creates a new logger handler.
        /// </summary>
        /// <remarks>By default, the assembly's name will be used as the logger's name.</remarks>
        public Logger() : this($"{new StackFrame(1).GetMethod().DeclaringType.GetAssembly().Name}") { }

        /// <summary>
        /// Creates a new logger handler.
        /// </summary>
        /// <param name="name">The name of the log file.</param>
        /// <param name="lineLength">The line length used for the separator.</param>
        public Logger(string name, int lineLength) : this(name, "LOGGER SESSION STARTS", '■', lineLength) { }

        /// <summary>
        /// Creates a new logger handler.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="headerMessage">The header text to write on every instance creation of the handler.</param>
        /// <param name="separatorChar">The character to use as a separator.</param>
        /// <param name="lineLength">The line length used for the separator.</param>
        public Logger(string name, string headerMessage = "LOGGER SESSION STARTS", char separatorChar = '■', int lineLength = 120) {
            _lineLength = lineLength;
            _separator = GetSeparator(separatorChar, lineLength);
            _headerMessage = $"{headerMessage} [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffzzz")}]";
            LogFile = new LogFile(Regex.Replace(name, "[\\\\/\\:\\*\\?\\\"\\<\\>\\|]", "_"));
            Directory.CreateDirectory(LogFile.DirectoryName ?? "Default");
        }

        private void Header() {
            lock (this) {
                var text = String.Format("{0}{2}{1}{2}{0}{2}", _separator, _headerMessage, Environment.NewLine);

                WriteFile(LogFile.ToString(), text, true);
                SecureMessage(text, LogFile.Date);
            }

            _headerWritten = true;
        }

        /// <summary>
        /// Write a message to the logger.
        /// </summary>
        /// <param name="message">The message to write to the file.</param>
        /// <remarks>If the object is not a string, its <see cref="Object.ToString()"/> method will be used.</remarks>
        public void Message(object message) {
            Message(message.ToString());
        }

        /// <summary>
        /// Write a message to the logger.
        /// </summary>
        /// <param name="message">The message to write to the file.</param>
        /// <param name="tag">The tag of the message.</param>
        /// <remarks>If the object is not a string, its <see cref="Object.ToString()"/> method will be used.</remarks>
        public void Message(object message, string tag) {
            message = string.Format("[{1}] {0}", message, tag);
            Message(message);
        }

        /// <summary>
        /// Writes an exception message to the logger.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="ex">The exception to display.</param>
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
                    var hour = DateTime.Now.ToString("HH:mm:ss.ffffzzz");
                    var space = new string(' ', hour.Length + 3);

                    sb.Append(Environment.NewLine);
                    sb.Append(string.Format("{1}{0}", aux, space));

                    continue;
                }

                sb.Append($"[{DateTime.Now.ToString("HH:mm:ss.ffffzzz")}] {aux}");
                registered = true;
            }

            lock (this) {
                var text = $"{sb}{Environment.NewLine}";

                WriteFile(LogFile.FullName, text, true);
                SecureMessage(text, LogFile.Date);
            }
        }

        /// <summary>
        /// Enable the use of a secure copy of the log file.
        /// </summary>
        /// <param name="path">The path where the secure copy will be stored in.</param>
        /// <param name="extension">The extension of the log file.</param>
        /// <param name="key">The security key of the log file.</param>
        public void EnableSecureCopy(string path, string extension, string key) {
            _isSecureCopyActive = true;
            _securePath = path;
            _secureExtension = extension;
            _security = new Security(key);

            var directory = $"{path}/{LogFile.Name}";

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        }

        /// <summary>
        /// Enables the use of a secure copy of the log file.
        /// </summary>
        /// <param name="key">The key to use to secure the log file.</param>
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

            var copyFile = new FileInfo(copy);

            if (!Directory.Exists(copyFile.Directory?.FullName)) {
                copyFile.Directory?.Create();
            }

            WriteFile(copy, sb.ToString());
        }

        private void SecureMessage(string text, DateTime? date = null) {
            if (_isSecureCopyActive) {
                WriteFile(
                    $"{_securePath}/{LogFile.Name}/{date?.ToString("yyyy-MM-dd") ?? string.Empty}.{_secureExtension}",
                    _security.Encrypt(text) + '♦',
                    true
                );
            }
        }

        private static string GetSeparator(char symbol, int length) {
            var sb = new StringBuilder();

            for (var i = 0; i < length; i++) {
                sb.Append(symbol);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Manages the writes and appends to a file.
        /// 
        /// If for some reason the file to write to is busy by another process, this method will attempt to write to 
        /// the target file 5 times (waiting 200 ms on each try). If after the 5 attempts this file is still busy,
        /// this method will create a new file in the format `{file_name} ({file_number}.{extension})`, 
        /// e.g. `file (1).log`.
        /// 
        /// If for some reason the newly created file is busy by another process, the same thing will be done until
        /// the file numbers get to 10 (`file (10).log`) in wich case a IOException will be thrown.
        /// 
        /// </summary>
        /// <param name="path">The file path to write to</param>
        /// <param name="content">The content that will be written to the file</param>
        /// <param name="append">Defines if the content will be written or appended to the file</param>
        /// <param name="attempts">Defines how many attempts have been made to write to the current file</param>
        /// <param name="fileCreations">Defines how many files have been created in an attempt to log the message</param>
        private static void WriteFile(string path, string content, bool append = false, int attempts = 0, int fileCreations = 0) {
            try {
                if (append) {
                    File.AppendAllText(path, content);
                }
                else {
                    File.WriteAllText(path, content);
                }
            }
            catch (IOException ex) {
                if (attempts >= 5) {
                    var rxPath = Regex.Match(path, @"(.+?)(?: \(\d\))?\.(.+)").Groups;

                    if (fileCreations >= 10) {
                        throw new IOException("The log file is not accessible, another process is using it", ex);
                    }

                    WriteFile($"{rxPath[1]} ({++fileCreations}).{rxPath[2]}", content, append, 0, fileCreations);
                }
                else {
                    Thread.Sleep(200);
                    WriteFile(path, content, append, ++attempts, fileCreations);
                }
            }
        }
    }
}
