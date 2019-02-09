using Microsoft.EntityFrameworkCore;
using SilveR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilveR.Models
{
    public sealed class SilveRRepository : IDisposable, ISilveRRepository
    {
        private readonly SilveRContext context;

        public SilveRRepository(SilveRContext context)
        {
            this.context = context;
        }

        public async Task<Dataset> GetDatasetByID(int datasetID)
        {
            return await context.Datasets.SingleAsync(x => x.DatasetID == datasetID);
        }

        public async Task<bool> HasDatasets()
        {
            return await context.Datasets.AnyAsync();
        }

        public async Task<int> GetLastVersionNumberForDataset(string fileName)
        {
            var versionNumbers = await context.Datasets.Where(x => x.DatasetName == fileName).Select(x => x.VersionNo).ToListAsync();
            if (versionNumbers.Any())
            {
                return versionNumbers.Max();
            }
            else
            {
                return 0;
            }
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

            //because not cascading, doing manual update on any analyses referencing this dataset
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

        public async Task<IEnumerable<Analysis>> GetAnalyses()
        {
            return await context.Analyses.Include("Script").OrderByDescending(x => x.DateAnalysed).ToListAsync();
        }

        public async Task<IEnumerable<DatasetViewModel>> GetDatasetViewModels()
        {
            return await context.Datasets.Select(x => new DatasetViewModel { DatasetID = x.DatasetID, DatasetName = x.DatasetName, VersionNo = x.VersionNo, DateUpdated = x.DateUpdated }).ToListAsync();
        }

        public async Task<IEnumerable<Script>> GetScripts()
        {
            return await context.Scripts.ToListAsync();
        }


        public async Task AddAnalysis(Analysis analysis)
        {
            context.Analyses.Add(analysis);
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

        public async Task UpdateAnalysis(Analysis analysis)
        {
            context.Entry(analysis).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }


        public async Task<UserOption> GetUserOptions()
        {
            UserOption userOption = await context.UserOptions.SingleOrDefaultAsync();
            if (userOption == null)
            {
                userOption = new UserOption();
            }

            return userOption;
        }

        public async Task UpdateUserOptions(UserOption userOption)
        {
            UserOption existingUserOption = context.UserOptions.Find(userOption.UserOptionID);
            if (existingUserOption == null)
            {
                context.Add(userOption);
            }
            else
            {
                //context.Entry(existingUserOption).CurrentValues.SetValues(userOption);
                context.Update(userOption);
            }

            await context.SaveChangesAsync();
        }


        public void Dispose()
        {
            context.Dispose();
        }
    }
}