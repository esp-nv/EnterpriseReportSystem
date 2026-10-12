using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EnterpriseReportSystem.Services.Interfaces;

public interface IExportService
{
    // Универсални уеб методи, които връщат файловете като готов масив от байтове (Stream/byte[])
    Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string reportTitle);
    Task<byte[]> ExportToCsvAsync<T>(IEnumerable<T> data);
    Task<byte[]> ExportToPdfAsync<T>(IEnumerable<T> data, string reportTitle);
}
