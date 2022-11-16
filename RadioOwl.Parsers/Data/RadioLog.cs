using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwl.Parsers.Data
{
    public class RadioLog
    {
        public readonly Guid Id = Guid.NewGuid();

        public  DateTime Date { get; set; }

        public  EventLevel Level { get; set; }

        public char LevelAsString { get { return Level.ToString().FirstOrDefault(); } }

        public string Text { get; set; }

        public int? PartNo { get; set; }

        public string DisplayText { get { return $"{Date:u} {LevelAsString} #{PartNo} {Text}"; } }

        public RadioLog( DateTime dateTime, EventLevel level, string text) 
            : this(dateTime, level, text, null)
        {
        }

        public RadioLog(DateTime dateTime, EventLevel level, string text, int? partNo)
        {
            PartNo = partNo;
            Date = dateTime;
            Level = level;
            Text = text;
        }
    }
}
