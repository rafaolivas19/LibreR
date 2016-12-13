using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibreR.SystemPrining.Models {
    public class Column {
        public Line Line { get; set; }
        public int InitialBoundary { get; set; }
        public int FinalBoundary { get; set; }

        public Column(Line line, int initialBoundary, int finalBoundary) {
            Line = line;
            InitialBoundary = initialBoundary;
            FinalBoundary = finalBoundary;
        }
    }
}
