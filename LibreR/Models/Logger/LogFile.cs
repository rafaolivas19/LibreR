using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LibreR.Models.Logger {
    public class LogFile {
        public DateTime Date { get; set; }
        public string Name { get; set; }

        public string DirectoryName => _fileInfo.DirectoryName;

        public string FullName => _fileInfo.FullName;

        private readonly FileInfo _fileInfo;

        public LogFile(string name) {
            Date = DateTime.Now;
            Name = name;
            _fileInfo = new FileInfo($"Logs/{name}/{DateTime.Now.ToString("yyyy-MM-dd")}.log");
        }

        public override string ToString() {
            return _fileInfo.ToString();
        }
    }
}
