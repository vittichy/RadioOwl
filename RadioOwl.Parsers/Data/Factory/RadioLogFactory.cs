using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwl.Parsers.Data.Factory
{
public    class RadioLogFactory
    {

        public RadioLog Create(string text, EventLevel level = EventLevel.Informational)
        {
            return new RadioLog(DateTime.Now, level, text);
        }

        public RadioLog Create(string text, int? partNo, EventLevel level = EventLevel.Informational)
        {
            return new RadioLog(DateTime.Now, level, text, partNo);
        }
    }
}
