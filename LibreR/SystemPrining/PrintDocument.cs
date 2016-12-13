using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Management;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using LibreR.SystemPrining.Models;
using FontStyle = System.Drawing.FontStyle;

namespace LibreR.SystemPrining {
    public class PrintDocument {
        public System.Drawing.Printing.PrintDocument Document => _document ?? (_document = new System.Drawing.Printing.PrintDocument());

        private List<Action<PrintPageEventArgs>> PrintingActions => _printingActions ?? (_printingActions = new List<Action<PrintPageEventArgs>>());

        public const string DefaultFontFamily = "Arial";
        public const float HorizontalMargin = 20;

        private System.Drawing.Printing.PrintDocument _document;
        private float _positionY = 20;
        private readonly List<int> _pendantColumnsList = new List<int>();
        private List<Action<PrintPageEventArgs>> _printingActions;

        public PrintDocument() { }

        public PrintDocument(string printer) {
            var name = GetAvailablePrinters().FirstOrDefault(x => x == printer);
            if (name != null) Document.PrinterSettings.PrinterName = name;
            Document.PrintController = new StandardPrintController();
        }

        public void AddLine(Line line) {
            if (line.Text == null) return;
            var font = new Font(line.Type, line.Size, line.Style);

            PrintingActions.Add(args => {
                var pageWidth = args.PageBounds.Width - HorizontalMargin * 2;

                foreach (var y in SplitString(line.Text, font, args)) {
                    var dimension = args.Graphics.MeasureString(y, font);
                    var x = HorizontalMargin;

                    switch (line.Alignment) {
                        case TextAlignment.Center:
                            x = (pageWidth - dimension.Width) / 2 + HorizontalMargin;
                            break;

                        case TextAlignment.Right:
                            x = pageWidth - dimension.Width + HorizontalMargin;
                            break;
                    }

                    args.Graphics.DrawString(y, font, Brushes.Black, x, _positionY,
                        new StringFormat());

                    _positionY += dimension.Height + 4;
                }
            });
        }

        public void AddLine(string text, float size = 8, FontStyle style = FontStyle.Regular, string type = DefaultFontFamily, TextAlignment alignment = TextAlignment.Left) {
            AddLine(new Line(text, size, style, type, alignment));
        }

        public void AddLine(string text, TextAlignment alignment) {
            var line = new Line(text) { Alignment = alignment };
            AddLine(line);
        }

        public void AddLine(string text, FontStyle style, TextAlignment alignment) {
            var line = new Line(text) { Alignment = alignment, Style = style };
            AddLine(line);
        }

        public void AddLine(string text, float size, TextAlignment alignment) {
            var line = new Line(text) { Alignment = alignment, Size = size };
            AddLine(line);
        }

        public void AddLine(string text, FontStyle style) {
            var line = new Line(text) { Style = style };
            AddLine(line);
        }

        public void AddLine(params Column[] columns) {
            _pendantColumnsList.Add(-columns.Length);
            var auxY = 0f;

            foreach (var column in columns) {
                var font = new Font(column.Line.Type, column.Line.Size, column.Line.Style);

                PrintingActions.Add(args => {
                    float width = column.FinalBoundary - column.InitialBoundary;

                    if (_pendantColumnsList.Count > 0 && _pendantColumnsList[0] < 0) {
                        auxY = _positionY;
                        _pendantColumnsList[0] = Math.Abs(_pendantColumnsList[0]);
                    }

                    var y = auxY;

                    foreach (var line in SplitString(column.Line.Text, font, args, width)) {
                        var dimension = args.Graphics.MeasureString(line, font);
                        var x = HorizontalMargin + column.InitialBoundary;

                        switch (column.Line.Alignment) {
                            case TextAlignment.Center:
                                x = x + width / 2 - dimension.Width / 2;
                                break;

                            case TextAlignment.Right:
                                x = x + width - dimension.Width;
                                break;
                        }

                        args.Graphics.DrawString(line, font, Brushes.Black, x, y,
                            new StringFormat());

                        if (_pendantColumnsList.Count > 0) {

                            _pendantColumnsList[0]--;

                            if (_pendantColumnsList[0] < 1)
                                _pendantColumnsList.RemoveAt(0);
                        }

                        y += dimension.Height + 4;
                        if (y > _positionY) _positionY = y;
                    }
                });
            }
        }

        public void AddImage(string imgPath, System.Drawing.Point point) {
            PrintingActions.Add(args => args.Graphics.DrawImage(new Bitmap(imgPath), point));
        }

        public void AddImage(Bitmap img, int width, int height = -1, TextAlignment alignment = TextAlignment.Left) {
            if (height == -1) height = width;

            var x = 0f;

            PrintingActions.Add(args => {
                var pageWidth = args.PageBounds.Width - HorizontalMargin * 2;

                switch (alignment) {
                    case TextAlignment.Left:
                        x = HorizontalMargin;
                        break;
                    case TextAlignment.Right:
                        x = pageWidth - HorizontalMargin * 2 - width;
                        break;
                    case TextAlignment.Center:
                        x = (pageWidth - width) / 2;
                        break;
                }

                args.Graphics.DrawImage(img, new Rectangle((int)Math.Ceiling(x), (int)Math.Ceiling(_positionY), width, height));
            });
        }

        public void NewLine(float height = 20) {
            PrintingActions.Add(args => {
                _positionY += height;
            });
        }

        public void Separator(char c, Font font = null) {
            PrintingActions.Add(args => {
                if (font == null) font = new Font(DefaultFontFamily, 8);
                var aux = string.Empty;
                var sb = new StringBuilder();
                var pageWidth = args.PageBounds.Width + 100;

                while (args.Graphics.MeasureString(sb.ToString(), font).Width < pageWidth) {
                    aux = sb.ToString();
                    sb.Append(c);
                }

                var dimension = args.Graphics.MeasureString(aux, font);

                args.Graphics.DrawString(aux, font, Brushes.Black, 0, _positionY,
                    new StringFormat());

                _positionY += dimension.Height + 4;
            });
        }

        public void Print() {
            Document.PrintPage += (sender, args) => { foreach (var x in PrintingActions) x.Invoke(args); };

            Document.Print();
            Document.Dispose();
        }

        private List<string> SplitString(string s, Font font, PrintPageEventArgs args, float width = -1) {
            var words = s.Split(' ').ToList();
            var lines = new List<string>();

            while (words.Count != 0) {
                var aux = string.Empty;
                var auxWords = new List<string>();
                var sb = new StringBuilder();
                var i = -1;

                while (args.Graphics.MeasureString(sb.ToString(), font).Width < (width != -1 ? width : args.PageBounds.Width - HorizontalMargin * 2)) {
                    aux = sb.ToString();
                    if (++i >= words.Count) break;
                    var word = words[i];
                    sb.Append($"{(sb.ToString() == string.Empty ? string.Empty : " ")}{word}");
                }

                auxWords = i == 0 ? new List<string>() : Enumerable.Range(i, words.Count - i).Select(j => words[j]).ToList();
                words = auxWords;

                lines.Add(aux == string.Empty ? sb.ToString() : aux);
            }

            return lines;
        }

        public static string[] GetAvailablePrinters() {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
            return (from ManagementBaseObject printer
                     in searcher.Get()
                    where !printer["WorkOffline"].ToString().ToLower().Equals("true")
                    select printer["DeviceId"].ToString())
                     .ToArray();
        }
    }
}
