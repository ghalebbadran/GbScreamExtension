using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace GbScreamExtension
{
    public class OptionPage : DialogPage
    {
        private string wavFilePath = @"";

        [Category("The Screaming Debugger")]
        [DisplayName("Path to .wav file")]
        [Description("Specifies the path to the .wav file that should be played when an unhandled exception occurs.")]
        public string WavFilePath
        {
            get { return wavFilePath; }
            set { wavFilePath = value; }
        }
    }

}
