using System.Runtime.Serialization;
using System.Windows.Media;

namespace Microsoft.VisualStudio.Terminal
{
    [DataContract]
    public class TerminalTheme
    {
        [DataMember(Name = "foreground")]
        public Color Foreground
        {
            get;
            set;
        }

        [DataMember(Name = "background")]
        public Color Background
        {
            get;
            set;
        }

        [DataMember(Name = "cursor")]
        public Color Cursor
        {
            get;
            set;
        }

        [DataMember(Name = "black")]
        public Color Black
        {
            get;
            set;
        }

        [DataMember(Name = "red")]
        public Color Red
        {
            get;
            set;
        }

        [DataMember(Name = "green")]
        public Color Green
        {
            get;
            set;
        }

        [DataMember(Name = "yellow")]
        public Color Yellow
        {
            get;
            set;
        }

        [DataMember(Name = "blue")]
        public Color Blue
        {
            get;
            set;
        }

        [DataMember(Name = "magenta")]
        public Color Magenta
        {
            get;
            set;
        }

        [DataMember(Name = "cyan")]
        public Color Cyan
        {
            get;
            set;
        }

        [DataMember(Name = "white")]
        public Color White
        {
            get;
            set;
        }

        [DataMember(Name = "brightBlack")]
        public Color BrightBlack
        {
            get;
            set;
        }

        [DataMember(Name = "brightRed")]
        public Color BrightRed
        {
            get;
            set;
        }

        [DataMember(Name = "brightGreen")]
        public Color BrightGreen
        {
            get;
            set;
        }

        [DataMember(Name = "brightYellow")]
        public Color BrightYellow
        {
            get;
            set;
        }

        [DataMember(Name = "brightBlue")]
        public Color BrightBlue
        {
            get;
            set;
        }

        [DataMember(Name = "brightMagenta")]
        public Color BrightMagenta
        {
            get;
            set;
        }

        [DataMember(Name = "brightCyan")]
        public Color BrightCyan
        {
            get;
            set;
        }

        [DataMember(Name = "brightWhite")]
        public Color BrightWhite
        {
            get;
            set;
        }

        [DataMember(Name = "border")]
        public Color Border
        {
            get;
            set;
        }
    }
}
