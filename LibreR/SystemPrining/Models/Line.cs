using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using FontStyle = System.Drawing.FontStyle;

namespace LibreR.SystemPrining.Models {
    public class Line {
        public string Text { get; set; }

        public float Size { get; set; }

        public FontStyle Style { get; set; }

        public string Type { get; set; }

        public TextAlignment Alignment { get; set; }

        private const string DefaultFontType = "Arial";

        public Line(string text, float size = 8, FontStyle style = FontStyle.Regular, string type = DefaultFontType, TextAlignment alignment = TextAlignment.Left) {
            Text = text;
            Size = size;
            Style = style;
            Type = type;
            Alignment = alignment;
        }
    }
}
