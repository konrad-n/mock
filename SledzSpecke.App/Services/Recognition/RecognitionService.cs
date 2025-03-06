using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SledzSpecke.App.Services.Recognition
{
    public class RecognitionService : IRecognitionService
    {
        public Task<bool> AddRecognitionAsync(Models.Recognition recognition)
        {
            throw new NotImplementedException();
        }

        public Task<DateTime> ApplyReductionsToEndDateAsync(DateTime endDate, int specializationId)
        {
            throw new NotImplementedException();
        }

        public Task<int> CalculateTotalReductionDaysAsync(int specializationId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteRecognitionAsync(int recognitionId)
        {
            throw new NotImplementedException();
        }

        public Task<List<RecognitionType>> GetAvailableRecognitionTypesAsync(int specializationId)
        {
            throw new NotImplementedException();
        }

        public Task<Models.Recognition> GetRecognitionAsync(int recognitionId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Models.Recognition>> GetRecognitionsAsync(int specializationId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRecognitionAsync(Models.Recognition recognition)
        {
            throw new NotImplementedException();
        }
    }
}
