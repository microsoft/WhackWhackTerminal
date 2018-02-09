using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EmbeddedTerminal
{
    [DataContract]
    public class TerminalTheme
    {
        [DataMember(Name = "foreground")]
        public string Foreground
        {
            get;
            set;
        }

        [DataMember(Name = "background")]
        public string Background
        {
            get;
            set;
        }

        [DataMember(Name = "cursor")]
        public string Cursor
        {
            get;
            set;
        }

        [DataMember(Name = "black")]
        public string Black
        {
            get;
            set;
        }

        [DataMember(Name = "red")]
        public string Red
        {
            get;
            set;
        }

        [DataMember(Name = "green")]
        public string Green
        {
            get;
            set;
        }

        [DataMember(Name = "yellow")]
        public string Yellow
        {
            get;
            set;
        }

        [DataMember(Name = "blue")]
        public string Blue
        {
            get;
            set;
        }

        [DataMember(Name = "magenta")]
        public string Magenta
        {
            get;
            set;
        }

        [DataMember(Name = "cyan")]
        public string Cyan
        {
            get;
            set;
        }

        [DataMember(Name = "white")]
        public string White
        {
            get;
            set;
        }

        [DataMember(Name = "brightBlack")]
        public string BrightBlack
        {
            get;
            set;
        }

        [DataMember(Name = "brightRed")]
        public string BrightRed
        {
            get;
            set;
        }

        [DataMember(Name = "brightGreen")]
        public string BrightGreen
        {
            get;
            set;
        }

        [DataMember(Name = "brightYellow")]
        public string BrightYellow
        {
            get;
            set;
        }

        [DataMember(Name = "brightBlue")]
        public string BrightBlue
        {
            get;
            set;
        }

        [DataMember(Name = "brightMagenta")]
        public string BrightMagenta
        {
            get;
            set;
        }

        [DataMember(Name = "brightCyan")]
        public string BrightCyan
        {
            get;
            set;
        }

        [DataMember(Name = "brightWhite")]
        public string BrightWhite
        {
            get;
            set;
        }
    }
}
