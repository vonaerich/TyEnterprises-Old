
namespace TY.SPIMS.Client.Helper.Export
{
    public class ReportExporter
    {
        public IExportStrategy ExportStrategy { get; set; }

        public ReportExporter(IExportStrategy strategy)
        {
            this.ExportStrategy = strategy;
        }

        public void ExportReport()
        {
            ExportStrategy.PerformExport();
        }
    }
}
