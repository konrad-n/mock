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
                        bool typedDefault = defaultValue != null ? (bool)(object)defaultValue : default;
                        return (T)(object)Preferences.Get(key, typedDefault);
                    }
                    else if (typeof(T) == typeof(string))
                    {
                        string? typedDefault = defaultValue != null ? (string?)(object)defaultValue : default;
                        return (T)(object)(Preferences.Get(key, typedDefault) ?? string.Empty);
                    }
                    else if (typeof(T) == typeof(int))
                    {
                        int typedDefault = defaultValue != null ? (int)(object)defaultValue : default;
                        return (T)(object)Preferences.Get(key, typedDefault);
                    }
                    else if (typeof(T) == typeof(double))
                    {
                        double typedDefault = defaultValue != null ? (double)(object)defaultValue : default;
                        return (T)(object)Preferences.Get(key, typedDefault);
                    }
                    else if (typeof(T) == typeof(float))
                    {
                        float typedDefault = defaultValue != null ? (float)(object)defaultValue : default;
                        return (T)(object)Preferences.Get(key, typedDefault);
                    }
                    else if (typeof(T) == typeof(long))
                    {
                        long typedDefault = defaultValue != null ? (long)(object)defaultValue : default;
                        return (T)(object)Preferences.Get(key, typedDefault);
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
                if (value == null)
                {
                    _logger.LogWarning("Attempted to set null value for key {Key}", key);
                    return;
                }

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

        public Task<bool> ClearAllSettingsAsync()
        {
            try
            {
                Preferences.Clear();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing all settings");
                return Task.FromResult(false);
            }
        }
    }
}
