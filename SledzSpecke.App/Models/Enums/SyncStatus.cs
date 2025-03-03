namespace SledzSpecke.App.Models.Enums
{
    public enum SyncStatus
    {
        NotSynced,  // Nie zsynchronizowane z SMK
        Synced,     // Zsynchronizowane z SMK
        SyncFailed, // Próba synchronizacji nie powiodła się
        Modified,   // Zsynchronizowane, ale później zmodyfikowane
    }
}
