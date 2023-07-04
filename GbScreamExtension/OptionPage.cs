using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace GbScreamExtension
{
    public class OptionPage : DialogPage
    {
        private string wavFilePathException = @"";
        private string wavFilePathBuildFailed = @"";

        [Category("The Screaming Debugger")]
        [DisplayName("Path to .wav file unhandled exception")]
        [Description("Specifies the path to the .wav file that should be played when an unhandled exception occurs.")]
        public string WavFilePathException
        {
            get { return wavFilePathException; }
            set { wavFilePathException = value; }
        }

        [Category("The Screaming Debugger")]
        [DisplayName("Path to .wav file build failed")]
        [Description("Specifies the path to the .wav file that should be played when a build failed.")]
        public string WavFilePathBuildFailed
        {
            get { return wavFilePathBuildFailed; }
            set { wavFilePathBuildFailed = value; }
        }
    }
}
