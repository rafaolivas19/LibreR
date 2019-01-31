using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using FontStyle = System.Drawing.FontStyle;

namespace LibreR.SystemPrining.Models {
    /// <summary>
    /// Represents a document line.
    /// </summary>
    public class Line {
        /// <summary>
        /// Gets or sets the line's content.
        /// </summary>
        /// <value>The line's content.</value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the line's font size.
        /// </summary>
        /// <value>The fon't size of the line.</value>
        public float Size { get; set; }

        /// <summary>
        /// Gets or sets the line's font style.
        /// </summary>
        /// <value>The line's font style.</value>
        public FontStyle Style { get; set; }

        /// <summary>
        /// Gets or sets the line's font type.
        /// </summary>
        /// <value>The line's font's family.</value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the line's text alignment.
        /// </summary>
        /// <value>The text aligment of the line.</value>
        public TextAlignment Alignment { get; set; }

        private const string DefaultFontType = "Arial";

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="text">The line's content.</param>
        /// <param name="size">The font size of the line, by default 8.</param>
        /// <param name="style">The font style of the line.</param>
        /// <param name="type">The line's font's family.</param>
        /// <param name="alignment">The text aligment type of the line. Left by default.</param>
        public Line(string text, float size = 8, FontStyle style = FontStyle.Regular, string type = DefaultFontType, TextAlignment alignment = TextAlignment.Left) {
            Text = text;
            Size = size;
            Style = style;
            Type = type;
            Alignment = alignment;
        }
    }
}
