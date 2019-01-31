using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibreR.SystemPrining.Models {
    /// <summary>
    /// Represents a column in a document line.
    /// </summary>
    public class Column {
        /// <summary>
        /// Gets or sets the line contained in the column.
        /// </summary>
        /// <value>The line contained in the column</value>
        public Line Line { get; set; }

        /// <summary>
        /// Gets or sets the initial boundary.
        /// </summary>
        /// <value>The initial boundary or offset.</value>
        public int InitialBoundary { get; set; }

        /// <summary>
        /// Gets or sets the final boundary.
        /// </summary>
        /// <value>The end boundary or offset.</value>
        public int FinalBoundary { get; set; }

        /// <summary>
        /// Creates a new column.
        /// </summary>
        /// <param name="line">The line contained in the column.</param>
        /// <param name="initialBoundary">The initial boundary of the column.</param>
        /// <param name="finalBoundary">The final boundary of the column.</param>
        public Column(Line line, int initialBoundary, int finalBoundary) {
            Line = line;
            InitialBoundary = initialBoundary;
            FinalBoundary = finalBoundary;
        }
    }
}
