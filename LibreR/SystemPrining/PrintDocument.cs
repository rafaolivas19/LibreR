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
    /// <summary>
    /// Represents a printable document.
    /// </summary>
    public class PrintDocument {
        /// <summary>
        /// Gets the internal document object.
        /// </summary>
        /// <value>Internal document object.</value>
        public System.Drawing.Printing.PrintDocument Document => _document ?? (_document = new System.Drawing.Printing.PrintDocument());

        /// <summary>
        /// Gets or sets the document's name.
        /// </summary>
        /// <value>The document's name.</value>
        /// <remarks>This is the name shown on the printing queue.</remarks>
        public string DocumentName
        {
            get => Document.DocumentName;
            set => Document.DocumentName = value;
        }

        private List<Action<PrintPageEventArgs>> PrintingActions => _printingActions ?? (_printingActions = new List<Action<PrintPageEventArgs>>());

        /// <summary>
        /// The default font family to use.
        /// </summary>
        public const string DefaultFontFamily = "Arial";
        /// <summary>
        /// The default horizontal margin.
        /// </summary>
        public const float HorizontalMargin = 20;

        private System.Drawing.Printing.PrintDocument _document;
        private float _positionY = 20;
        private readonly List<int> _pendantColumnsList = new List<int>();
        private List<Action<PrintPageEventArgs>> _printingActions;

        /// <summary>
        /// Creates an empty printing document.
        /// </summary>
        public PrintDocument() { }

        /// <summary>
        /// Creates an empty printing document, attempting to find a specific printer.
        /// </summary>
        /// <param name="printer">The name of the printer to look for.</param>
        public PrintDocument(string printer) {
            var name = GetAvailablePrinters().FirstOrDefault(x => x == printer);
            if (name != null) Document.PrinterSettings.PrinterName = name;
            Document.PrintController = new StandardPrintController();
        }

        /// <summary>
        /// Adds a new line to the document.
        /// </summary>
        /// <param name="line">The linea to add.</param>
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

        /// <summary>
        /// Adds a new line to the document.
        /// </summary>
        /// <param name="text">The line's content.</param>
        /// <param name="size">The font size of the text.</param>
        /// <param name="style">The style of the font.</param>
        /// <param name="type">Te type of font to use.</param>
        /// <param name="alignment">The text alignment to use.</param>
        public void AddLine(string text, float size = 8, FontStyle style = FontStyle.Regular, string type = DefaultFontFamily, TextAlignment alignment = TextAlignment.Left) {
            AddLine(new Line(text, size, style, type, alignment));
        }
               
        /// <summary>
        /// Adds a new line to the document.
        /// </summary>
        /// <param name="text">The line's content.</param>
        /// <param name="alignment">The text aligment to use.</param>
        public void AddLine(string text, TextAlignment alignment) {
            var line = new Line(text) { Alignment = alignment };
            AddLine(line);
        }
        
        /// <summary>
        /// Adds a new line to the document.
        /// </summary>
        /// <param name="text">The line's content.</param>
        /// <param name="style">The style of the font.</param>
        /// <param name="alignment">The text aligment to use.</param>
        public void AddLine(string text, FontStyle style, TextAlignment alignment) {
            var line = new Line(text) { Alignment = alignment, Style = style };
            AddLine(line);
        }

        /// <summary>
        /// Adds a new line to the document.
        /// </summary>
        /// <param name="text">The line's content.</param>
        /// <param name="size">The font size of the text.</param>
        /// <param name="alignment">The text aligment to use.</param>
        public void AddLine(string text, float size, TextAlignment alignment) {
            var line = new Line(text) { Alignment = alignment, Size = size };
            AddLine(line);
        }

        /// <summary>
        /// Adds a new line to the document.
        /// </summary>
        /// <param name="text">The line's content.</param>
        /// <param name="style">The font size of the text.</param>
        public void AddLine(string text, FontStyle style) {
            var line = new Line(text) { Style = style };
            AddLine(line);
        }

        /// <summary>
        /// Adds a new line to the document providing the columns.
        /// </summary>
        /// <param name="columns">The columns forming the line.</param>
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

        /// <summary>
        /// Adds an image to the document.
        /// </summary>
        /// <param name="imgPath">The path where the image is located.</param>
        /// <param name="point">The cooridnate where the image will be added to.</param>
        public void AddImage(string imgPath, System.Drawing.Point point) {
            PrintingActions.Add(args => args.Graphics.DrawImage(new Bitmap(imgPath), point));
        }

        /// <summary>
        /// Adds an image to the document.
        /// </summary>
        /// <param name="img">The image's bitmap.</param>
        /// <param name="width">The image's width.</param>
        /// <param name="height">The image's height. If no height is specified, the value for width will be used.</param>
        /// <param name="alignment">The image's alignment.</param>
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

        /// <summary>
        /// Adds a new, empty line to the document.
        /// </summary>
        /// <param name="height">The height of the line, by default 20.</param>
        public void NewLine(float height = 20) {
            PrintingActions.Add(args => {
                _positionY += height;
            });
        }

        /// <summary>
        /// Adds a separator to the document.
        /// </summary>
        /// <param name="c">The character to use as a separator.</param>
        /// <param name="font">The font used for the separator.</param>
        /// <remarks>A separator is a line spans the entire width of the document, repeating the provided character.</remarks>
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

        /// <summary>
        /// Prints the current document.
        /// </summary>
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
                var sb = new StringBuilder();
                var i = -1;

                while (args.Graphics.MeasureString(sb.ToString(), font).Width < (width != -1 ? width : args.PageBounds.Width - HorizontalMargin * 2)) {
                    aux = sb.ToString();
                    if (++i >= words.Count) break;
                    var word = words[i];
                    sb.Append($"{(sb.ToString() == string.Empty ? string.Empty : " ")}{word}");
                }

                var auxWords = i == 0 ? new List<string>() : Enumerable.Range(i, words.Count - i).Select(j => words[j]).ToList();
                words = auxWords;

                lines.Add(aux == string.Empty ? sb.ToString() : aux);
            }

            return lines;
        }

        /// <summary>
        /// Gets a list of available printers.
        /// </summary>
        /// <returns>The list of available printer names.</returns>
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
