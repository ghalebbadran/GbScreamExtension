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
        public DTE2 _dte;
        public DebuggerEvents _debuggerEvents;
        public BuildEvents _buildEvents;

        protected override async Task InitializeAsync(System.Threading.CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            _dte = await GetServiceAsync(typeof(DTE)) as DTE2;

            _debuggerEvents = _dte.Events.DebuggerEvents;
            _debuggerEvents.OnEnterBreakMode += DebuggerEvents_OnEnterBreakMode;

            _buildEvents = _dte.Events.BuildEvents;
            _buildEvents.OnBuildDone += BuildEvents_OnBuildDone;
        }

        private void DebuggerEvents_OnEnterBreakMode(dbgEventReason Reason, ref dbgExecutionAction ExecutionAction)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (Reason == EnvDTE.dbgEventReason.dbgEventReasonExceptionNotHandled)
            {
                PlaySound(EventTypes.Exception);
            }
        }

        private void BuildEvents_OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var solutionBuild = (SolutionBuild2)_dte.Solution.SolutionBuild;
            if (solutionBuild.LastBuildInfo != 0)
            {
                PlaySound(EventTypes.BuildFailed);
            }
        }

        private void PlaySound(EventTypes eventTypes)
        {
            OptionPage options = (OptionPage)GetDialogPage(typeof(OptionPage));

            string wavFilePath = null;
            string defaultWavFileName = null;
            SoundPlayer player = null;

            switch (eventTypes)
            {
                case EventTypes.Exception:
                    wavFilePath = options.WavFilePathException;
                    defaultWavFileName = "GbScreamExtension.scream.wav";
                    break;
                case EventTypes.BuildFailed:
                    wavFilePath = options.WavFilePathBuildFailed;
                    defaultWavFileName = "GbScreamExtension.slap.wav";
                    break;
            }


            if (!string.IsNullOrWhiteSpace(wavFilePath))
            {
                player = new SoundPlayer(wavFilePath);
            }
            else
            {
                System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
                System.IO.Stream s = a.GetManifestResourceStream(defaultWavFileName);

                player = new SoundPlayer(s);
            }

            using (player)
            {
                player.Play();
            }
        }
    }
}
