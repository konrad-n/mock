using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Services.SmkStrategy
{
    public static class SmkStrategyFactory
    {
        public static ISmkVersionStrategy CreateStrategy(SmkVersion version)
        {
            return version switch
            {
                SmkVersion.New => new NewSmkStrategy(),
                SmkVersion.Old => new OldSmkStrategy(),
                _ => new NewSmkStrategy(), // Domyślnie nowa wersja
            };
        }
    }
}