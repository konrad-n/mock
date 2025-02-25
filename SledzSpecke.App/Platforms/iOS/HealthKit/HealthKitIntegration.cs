using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using HealthKit;
using Microsoft.Extensions.Logging;
using UIKit;

namespace SledzSpecke.App.Platforms.iOS.HealthKit
{
    public class HealthKitIntegration
    {
        private readonly ILogger<HealthKitIntegration> _logger;
        private HKHealthStore _healthStore;
        private bool _isHealthKitAvailable;
        
        public HealthKitIntegration(ILogger<HealthKitIntegration> logger)
        {
            _logger = logger;
            
            // Sprawdź dostępność HealthKit
            _isHealthKitAvailable = HKHealthStore.IsHealthDataAvailable;
            
            if (_isHealthKitAvailable)
            {
                _healthStore = new HKHealthStore();
            }
            else
            {
                _logger.LogWarning("HealthKit is not available on this device");
            }
        }
        
        public bool IsHealthKitAvailable => _isHealthKitAvailable;
        
        /// <summary>
        /// Żąda dostępu do danych HealthKit. Dla profesjonalistów medycznych, często potrzebujemy dostępu do odczytu danych pacjenta.
        /// </summary>
        public async Task<bool> RequestAuthorization()
        {
            if (!_isHealthKitAvailable)
            {
                return false;
            }
            
            try
            {
                // Definiuj typy danych, do których potrzebujemy dostępu
                var typesToRead = new NSSet(
                    HKObjectType.GetQuantityType(HKQuantityTypeIdentifier.HeartRate),
                    HKObjectType.GetQuantityType(HKQuantityTypeIdentifier.BloodPressureSystolic),
                    HKObjectType.GetQuantityType(HKQuantityTypeIdentifier.BloodPressureDiastolic),
                    HKObjectType.GetQuantityType(HKQuantityTypeIdentifier.BodyMass),
                    HKObjectType.GetQuantityType(HKQuantityTypeIdentifier.Height),
                    HKObjectType.GetCategoryType(HKCategoryTypeIdentifier.SleepAnalysis),
                    HKObjectType.GetQuantityType(HKQuantityTypeIdentifier.StepCount),
                    HKObjectType.GetQuantityType(HKQuantityTypeIdentifier.BloodGlucose),
                    HKObjectType.GetQuantityType(HKQuantityTypeIdentifier.OxygenSaturation)
                );
                
                // Definiuj typy danych, które będziemy zapisywać (opcjonalnie)
                var typesToWrite = new NSSet(
                    HKObjectType.GetQuantityType(HKQuantityTypeIdentifier.HeartRate),
                    HKObjectType.GetQuantityType(HKQuantityTypeIdentifier.BloodPressureSystolic),
                    HKObjectType.GetQuantityType(HKQuantityTypeIdentifier.BloodPressureDiastolic),
                    HKObjectType.GetQuantityType(HKQuantityTypeIdentifier.BloodGlucose),
                    HKObjectType.GetQuantityType(HKQuantityTypeIdentifier.OxygenSaturation)
                );
                
                // Poproś o uprawnienia
                var tcs = new TaskCompletionSource<bool>();
                
                _healthStore.RequestAuthorization(typesToRead, typesToWrite, (success, error) =>
                {
                    if (error != null)
                    {
                        _logger.LogError("Error requesting HealthKit authorization: {Error}", error.LocalizedDescription);
                        tcs.SetResult(false);
                        return;
                    }
                    
                    tcs.SetResult(success);
                });
                
                return await tcs.Task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting HealthKit authorization");
                return false;
            }
        }
        
        /// <summary>
        /// Pobiera najnowsze dane tętna
        /// </summary>
        public async Task<(double? value, DateTime? timestamp)> GetLatestHeartRate()
        {
            if (!_isHealthKitAvailable)
            {
                return (null, null);
            }
            
            try
            {
                var heartRateType = HKObjectType.GetQuantityType(HKQuantityTypeIdentifier.HeartRate);
                var sortDescriptor = new NSSortDescriptor(HKSample.SortIdentifierEndDate, false);
                var query = new HKSampleQuery(
                    heartRateType,
                    null,
                    1,
                    new NSSortDescriptor[] { sortDescriptor },
                    (HKSampleQuery sampleQuery, HKSample[] results, NSError error) =>
                    {
                        if (error != null)
                        {
                            _logger.LogError("Error getting heart rate: {Error}", error.LocalizedDescription);
                            return;
                        }
                        
                        if (results != null && results.Length > 0)
                        {
                            var result = results[0] as HKQuantitySample;
                            var heartRate = result.Quantity.GetDoubleValue(HKUnit.Count.UnitDividedBy(HKUnit.Minute));
                            var timestamp = (DateTime)result.EndDate;
                        }
                    });
                
                // Wykonaj zapytanie asynchronicznie
                var tcs = new TaskCompletionSource<(double? value, DateTime? timestamp)>();
                
                _healthStore.ExecuteQuery(query, (completedQuery, results, error) =>
                {
                    if (error != null)
                    {
                        _logger.LogError("Error getting heart rate: {Error}", error.LocalizedDescription);
                        tcs.SetResult((null, null));
                        return;
                    }
                    
                    var samples = results as HKSample[];
                    if (samples != null && samples.Length > 0)
                    {
                        var result = samples[0] as HKQuantitySample;
                        double heartRate = result.Quantity.GetDoubleValue(HKUnit.Count.UnitDividedBy(HKUnit.Minute));
                        DateTime timestamp = (DateTime)result.EndDate;
                        tcs.SetResult((heartRate, timestamp));
                    }
                    else
                    {
                        tcs.SetResult((null, null));
                    }
                });
                
                return await tcs.Task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting heart rate");
                return (null, null);
            }
        }
        
        /// <summary>
        /// Pobiera najnowszy pomiar ciśnienia krwi
        /// </summary>
        public async Task<(double? systolic, double? diastolic, DateTime? timestamp)> GetLatestBloodPressure()
        {
            if (!_isHealthKitAvailable)
            {
                return (null, null, null);
            }
            
            try
            {
                // Implementacja podobna do powyższej, ale dla ciśnienia krwi
                
                // Tutaj dodalibyśmy faktyczną implementację dla ciśnienia krwi
                // Pobieranie zarówno wartości skurczowej, jak i rozkurczowej
                
                return (null, null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting blood pressure");
                return (null, null, null);
            }
        }
        
        /// <summary>
        /// Zapisuje pomiar ciśnienia krwi
        /// </summary>
        public async Task<bool> SaveBloodPressure(double systolic, double diastolic, DateTime timestamp)
        {
            if (!_isHealthKitAvailable)
            {
                return false;
            }
            
            try
            {
                // Konwersja DateTime do NSDate
                var nsDate = (NSDate)timestamp;
                
                // Utworzenie próbki dla ciśnienia skurczowego
                var systolicType = HKObjectType.GetQuantityType(HKQuantityTypeIdentifier.BloodPressureSystolic);
                var systolicQuantity = HKQuantity.FromQuantity(HKUnit.MillimeterOfMercury, systolic);
                var systolicSample = HKQuantitySample.FromType(
                    systolicType,
                    systolicQuantity,
                    nsDate,
                    nsDate,
                    new HKMetadata());
                
                // Utworzenie próbki dla ciśnienia rozkurczowego
                var diastolicType = HKObjectType.GetQuantityType(HKQuantityTypeIdentifier.BloodPressureDiastolic);
                var diastolicQuantity = HKQuantity.FromQuantity(HKUnit.MillimeterOfMercury, diastolic);
                var diastolicSample = HKQuantitySample.FromType(
                    diastolicType,
                    diastolicQuantity,
                    nsDate,
                    nsDate,
                    new HKMetadata());
                
                // Zapisz dane w HealthKit
                var samples = new HKSample[] { systolicSample, diastolicSample };
                
                var tcs = new TaskCompletionSource<bool>();
                
                _healthStore.SaveObjects(samples, (success, error) =>
                {
                    if (error != null)
                    {
                        _logger.LogError("Error saving blood pressure: {Error}", error.LocalizedDescription);
                        tcs.SetResult(false);
                        return;
                    }
                    
                    tcs.SetResult(success);
                });
                
                return await tcs.Task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving blood pressure");
                return false;
            }
        }
    }
}
