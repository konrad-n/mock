using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Interfaces.Services;

namespace SledzSpecke.App.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly ILogger<SettingsService> _logger;

        public SettingsService(ILogger<SettingsService> logger)
        {
            _logger = logger;
        }

        public T GetSetting<T>(string key, T defaultValue)
        {
            try
            {
                if (Preferences.ContainsKey(key))
                {
                    if (typeof(T) == typeof(bool))
                    {
                        return (T)(object)Preferences.Get(key, (bool)(object)defaultValue);
                    }
                    else if (typeof(T) == typeof(string))
                    {
                        return (T)(object)Preferences.Get(key, (string)(object)defaultValue);
                    }
                    else if (typeof(T) == typeof(int))
                    {
                        return (T)(object)Preferences.Get(key, (int)(object)defaultValue);
                    }
                    else if (typeof(T) == typeof(double))
                    {
                        return (T)(object)Preferences.Get(key, (double)(object)defaultValue);
                    }
                    else if (typeof(T) == typeof(float))
                    {
                        return (T)(object)Preferences.Get(key, (float)(object)defaultValue);
                    }
                    else if (typeof(T) == typeof(long))
                    {
                        return (T)(object)Preferences.Get(key, (long)(object)defaultValue);
                    }
                    else if (typeof(T) == typeof(DateTime))
                    {
                        var ticks = Preferences.Get(key, 0L);
                        return ticks == 0 ? defaultValue : (T)(object)new DateTime(ticks);
                    }
                }

                return defaultValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting setting {Key}", key);
                return defaultValue;
            }
        }

        public void SetSetting<T>(string key, T value)
        {
            try
            {
                if (typeof(T) == typeof(bool))
                {
                    Preferences.Set(key, (bool)(object)value);
                }
                else if (typeof(T) == typeof(string))
                {
                    Preferences.Set(key, (string)(object)value);
                }
                else if (typeof(T) == typeof(int))
                {
                    Preferences.Set(key, (int)(object)value);
                }
                else if (typeof(T) == typeof(double))
                {
                    Preferences.Set(key, (double)(object)value);
                }
                else if (typeof(T) == typeof(float))
                {
                    Preferences.Set(key, (float)(object)value);
                }
                else if (typeof(T) == typeof(long))
                {
                    Preferences.Set(key, (long)(object)value);
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    Preferences.Set(key, ((DateTime)(object)value).Ticks);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting {Key} to {Value}", key, value);
            }
        }

        public async Task<bool> ClearAllSettingsAsync()
        {
            try
            {
                Preferences.Clear();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing all settings");
                return false;
            }
        }
    }
}
