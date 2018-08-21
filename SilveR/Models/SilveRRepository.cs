using Microsoft.EntityFrameworkCore;
using SilveR.ViewModels;
using SilveRModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilveR.Models
{
    public class SilveRRepository : IDisposable
    {
        private readonly SilveRContext context;

        public SilveRRepository(SilveRContext context)
        {
            this.context = context;
        }

        public async Task<Dataset> GetDatasetByID(Nullable<int> datasetID)
        {
            if (datasetID.HasValue)
            {
                return await context.Datasets.SingleAsync(x => x.DatasetID == datasetID.Value);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> HasDatasets()
        {
            return await context.Datasets.AnyAsync();
        }

        public async Task<IList<Dataset>> GetExistingDatasets(string fileName)
        {
            return await context.Datasets.Where(x => x.DatasetName == fileName).ToListAsync();
        }

        public async Task CreateDataset(Dataset dataset)
        {
            context.Datasets.Add(dataset);
            await context.SaveChangesAsync();
        }

        public async Task UpdateDataset(Dataset dataset)
        {
            context.Datasets.Attach(dataset);

            //update certain fields only
            context.Entry(dataset).Property(x => x.TheData).IsModified = true;
            context.Entry(dataset).Property(x => x.DateUpdated).IsModified = true;

            await context.SaveChangesAsync();
        }

        public async Task DeleteDataset(int datasetID)
        {
            Dataset dataset = new Dataset();
            dataset.DatasetID = datasetID;
            context.Entry(dataset).State = EntityState.Deleted;

            //because not cascading, doing manual update on any analyses referening this dataset
            var analyses = context.Analyses.Where(a => a.DatasetID == datasetID);
            foreach (Analysis analysis in analyses)
            {
                analysis.DatasetID = null;
            }

            await context.SaveChangesAsync();
        }

        public async Task<bool> HasAnalyses()
        {
            return await context.Analyses.AnyAsync();
        }

        public async Task<IList<Analysis>> GetAnalyses()
        {
            return await context.Analyses.Include("Script").OrderByDescending(x => x.DateAnalysed).ToListAsync();
        }

        public async Task<IList<DatasetViewModel>> GetDatasetViewModels()
        {
            return await context.Datasets.Select(x => new DatasetViewModel { DatasetID = x.DatasetID, DatasetName = x.DatasetName, VersionNo = x.VersionNo, DateUpdated = x.DateUpdated }).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetScriptNames()
        {
            return await context.Scripts.Select(x => x.ScriptDisplayName).ToListAsync();
        }


        public async Task SaveAnalysis(Analysis newAnalysis)
        {
            context.Analyses.Add(newAnalysis);
            await context.SaveChangesAsync();
        }

        public async Task<Script> GetScriptByName(string scriptFileName)
        {
            return await context.Scripts.SingleAsync(x => x.ScriptFileName == scriptFileName);
        }

        public async Task<bool> HasAnalysisCompleted(string analysisGuid)
        {
            return await context.Analyses.AnyAsync(x => x.AnalysisGuid == analysisGuid && x.RProcessOutput != null);
        }

        public async Task<Analysis> GetAnalysis(string analysisGuid)
        {
            return await context.Analyses.SingleAsync(an => an.AnalysisGuid == analysisGuid);
        }

        public async Task<Analysis> GetAnalysisComplete(string analysisGuid)
        {
            return await context.Analyses.Include("Dataset").Include("Script").Include("Arguments").SingleAsync(an => an.AnalysisGuid == analysisGuid);
        }

        public async Task DeleteAnalysis(Analysis analysis)
        {
            context.Analyses.Remove(analysis);
            await context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }


        public void Dispose()
        {
            context.Dispose();
        }
    }
}