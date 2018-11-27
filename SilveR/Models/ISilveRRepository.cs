using SilveR.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilveR.Models
{
    public interface ISilveRRepository
    {
        Task CreateDataset(Dataset dataset);
        Task DeleteAnalysis(Analysis analysis);
        Task DeleteDataset(int datasetID);
        void Dispose();
        Task<IList<Analysis>> GetAnalyses();
        Task<Analysis> GetAnalysis(string analysisGuid);
        Task<Analysis> GetAnalysisComplete(string analysisGuid);
        Task<Dataset> GetDatasetByID(int? datasetID);
        Task<IList<DatasetViewModel>> GetDatasetViewModels();
        Task<int> GetLastVersionNumberForDataset(string fileName);
        Task<Script> GetScriptByName(string scriptFileName);
        Task<IEnumerable<string>> GetScriptDisplayNames();
        Task<bool> HasAnalyses();
        Task<bool> HasAnalysisCompleted(string analysisGuid);
        Task<bool> HasDatasets();
        Task AddAnalysis(Analysis analysis);
        Task UpdateDataset(Dataset dataset);
        Task UpdateAnalysis(Analysis analysis);
    }
}