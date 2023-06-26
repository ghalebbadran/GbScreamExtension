using EnvDTE;
using EnvDTE80;
using GbScreamExtension;
using Microsoft.VisualStudio.Shell;
using System;
using System.Media;
using System.Runtime.InteropServices;
using Task = System.Threading.Tasks.Task;

namespace Gb
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(GbScreamExtension.PackageGuidString)]
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideOptionPage(typeof(OptionPage), "The Screaming Debugger", "General", 0, 0, true)]
    public sealed class GbScreamExtension : AsyncPackage
    {
        public const string PackageGuidString = "bc59ccca-3a4f-418d-935c-e455c90f0c44";
        public DebuggerEvents _debuggerEvents;
        protected override async Task InitializeAsync(System.Threading.CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var dte = await GetServiceAsync(typeof(DTE)) as DTE2;

            _debuggerEvents = dte.Events.DebuggerEvents;
            _debuggerEvents.OnEnterBreakMode += DebuggerEvents_OnEnterBreakMode;
        }


        private void DebuggerEvents_OnEnterBreakMode(dbgEventReason Reason, ref dbgExecutionAction ExecutionAction)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (Reason == EnvDTE.dbgEventReason.dbgEventReasonExceptionNotHandled)
            {
                OptionPage options = (OptionPage)GetDialogPage(typeof(OptionPage));
                string wavFilePath = options.WavFilePath;

                SoundPlayer player = null;

                if (!string.IsNullOrWhiteSpace(wavFilePath))
                {
                    player = new SoundPlayer(wavFilePath);
                }
                else
                {
                    System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
                    System.IO.Stream s = a.GetManifestResourceStream("GbScreamExtension.scream.wav");

                    player = new SoundPlayer(s);
                }

                using (player)
                {
                    player.Play();
                }
            }
        }
    }
}
