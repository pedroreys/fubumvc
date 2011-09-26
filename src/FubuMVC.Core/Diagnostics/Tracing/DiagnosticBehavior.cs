using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class DiagnosticBehavior : IActionBehavior
    {
        private readonly IDebugDetector _detector;
        private readonly IDebugReport _report;
        private readonly IDebugCallHandler _debugCallHandler;

        public DiagnosticBehavior(IDebugReport report, IDebugDetector detector, IRequestHistoryCache history, IDebugCallHandler debugCallHandler, IFubuRequest request)
        {
            _report = report;
            _debugCallHandler = debugCallHandler;
            _detector = detector;

            history.AddReport(report, request.Get<CurrentRequest>());
        }

        public IActionBehavior Inner { get; set; }

        public void Invoke()
        {
            Inner.Invoke();

            write();
        }

        public void InvokePartial()
        {
            Inner.InvokePartial();

            write();
        }

        private void write()
        {
            _report.MarkFinished();

            if (!_detector.IsDebugCall()) return;

            _debugCallHandler.Handle();
        }
    }
}