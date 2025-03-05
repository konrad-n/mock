using System.Text;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Export
{
    public static class ExportValidator
    {
        /// <summary>
        /// Weryfikuje poprawność danych dyżurów przed eksportem.
        /// </summary>
        /// <param name="shifts">Lista dyżurów do zweryfikowania.</param>
        /// <param name="isOldSmk">Czy eksport jest dla starego SMK..</param>
        /// <returns>Wynik walidacji zawierający informację o błędach.</returns>
        public static ValidationResult ValidateMedicalShifts(List<MedicalShift> shifts, bool isOldSmk)
        {
            var result = new ValidationResult { IsValid = true };
            var errorsBuilder = new StringBuilder();
            int errorCount = 0;

            for (int i = 0; i < shifts.Count; i++)
            {
                var shift = shifts[i];
                bool hasErrors = false;
                var shiftErrors = new StringBuilder();

                // Sprawdzenie wymaganych pól
                if (shift.Date == DateTime.MinValue)
                {
                    shiftErrors.AppendLine($"- Brak daty rozpoczęcia");
                    hasErrors = true;
                }

                if (shift.Hours <= 0)
                {
                    shiftErrors.AppendLine($"- Nieprawidłowa liczba godzin (musi być większa od 0)");
                    hasErrors = true;
                }

                if (isOldSmk && shift.Minutes < 0)
                {
                    shiftErrors.AppendLine($"- Nieprawidłowa liczba minut (nie może być ujemna)");
                    hasErrors = true;
                }

                if (string.IsNullOrWhiteSpace(shift.Location))
                {
                    shiftErrors.AppendLine($"- Brak nazwy komórki organizacyjnej");
                    hasErrors = true;
                }

                // Sprawdzenie dodatkowych pól dla starego SMK
                if (isOldSmk)
                {
                    bool field1Missing = true;
                    bool field2Missing = true;

                    if (!string.IsNullOrEmpty(shift.AdditionalFields))
                    {
                        try
                        {
                            var additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(shift.AdditionalFields);
                            if (additionalFields != null)
                            {
                                if (additionalFields.TryGetValue("OldSMKField1", out var field1) &&
                                    !string.IsNullOrWhiteSpace(field1?.ToString()))
                                {
                                    field1Missing = false;
                                }

                                if (additionalFields.TryGetValue("OldSMKField2", out var field2) &&
                                    !string.IsNullOrWhiteSpace(field2?.ToString()))
                                {
                                    field2Missing = false;
                                }
                            }
                        }
                        catch
                        {
                            // Problemy z deserializacją - traktujemy jakby pola były puste
                        }
                    }

                    if (field1Missing)
                    {
                        shiftErrors.AppendLine($"- Brak osoby nadzorującej");
                        hasErrors = true;
                    }

                    if (field2Missing)
                    {
                        shiftErrors.AppendLine($"- Brak oddziału");
                        hasErrors = true;
                    }
                }

                // Jeśli znaleziono błędy, dodaj je do zbiorczej listy
                if (hasErrors)
                {
                    errorCount++;
                    errorsBuilder.AppendLine($"Dyżur #{i + 1} ({shift.Date:yyyy-MM-dd}) - błędy:");
                    errorsBuilder.Append(shiftErrors);
                    errorsBuilder.AppendLine();
                }
            }

            // Jeśli znaleziono jakiekolwiek błędy, oznacz walidację jako nieudaną
            if (errorCount > 0)
            {
                result.IsValid = false;
                result.ErrorMessage = $"Znaleziono błędy w {errorCount} dyżurach medycznych:\n\n{errorsBuilder}";
            }

            return result;
        }

        // Podobne metody można dodać dla innych typów danych (procedury, staże, kursy itd.)
    }
}