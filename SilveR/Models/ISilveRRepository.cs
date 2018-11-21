using System.Collections.Generic;
using System.Threading.Tasks;
using SilveR.ViewModels;

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
        Task<IList<Dataset>> GetExistingDatasets(string fileName);
        Task<Script> GetScriptByName(string scriptFileName);
        Task<IEnumerable<string>> GetScriptNames();
        Task<bool> HasAnalyses();
        Task<bool> HasAnalysisCompleted(string analysisGuid);
        Task<bool> HasDatasets();
        Task SaveAnalysis(Analysis newAnalysis);
        Task SaveChangesAsync();
        Task UpdateDataset(Dataset dataset);
    }
}