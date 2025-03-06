using System.Text.Json;
using SledzSpecke.App.Services.SmkStrategy;

namespace SledzSpecke.Tests.Services.SmkStrategy
{
    public class NewSmkStrategyTests
    {
        private ISmkVersionStrategy strategy;

        [SetUp]
        public void Setup()
        {
            this.strategy = new NewSmkStrategy();
        }

        [Test]
        public void GetVisibleFields_ForAddEditMedicalShift_ReturnsCorrectFields()
        {
            // Act
            var visibleFields = this.strategy.GetVisibleFields("AddEditMedicalShift");

            // Assert
            Assert.That(visibleFields, Is.Not.Null);
            Assert.That(visibleFields.ContainsKey("Date"), Is.True);
            Assert.That(visibleFields.ContainsKey("Hours"), Is.True);
            Assert.That(visibleFields.ContainsKey("Minutes"), Is.True);
            Assert.That(visibleFields.ContainsKey("Location"), Is.True);
            Assert.That(visibleFields.ContainsKey("Year"), Is.True);

            // Old SMK specific fields should be hidden
            Assert.That(visibleFields.ContainsKey("OldSMKField1"), Is.True);
            Assert.That(visibleFields.ContainsKey("OldSMKField2"), Is.True);
            Assert.That(visibleFields["OldSMKField1"], Is.False);
            Assert.That(visibleFields["OldSMKField2"], Is.False);
        }

        [Test]
        public void GetFieldLabels_ForAddEditMedicalShift_ReturnsCorrectLabels()
        {
            // Act
            var fieldLabels = this.strategy.GetFieldLabels("AddEditMedicalShift");

            // Assert
            Assert.That(fieldLabels, Is.Not.Null);
            Assert.That(fieldLabels["Date"], Is.EqualTo("Data dyżuru"));
            Assert.That(fieldLabels["Hours"], Is.EqualTo("Godziny"));
            Assert.That(fieldLabels["Minutes"], Is.EqualTo("Minuty"));
            Assert.That(fieldLabels["Location"], Is.EqualTo("Miejsce dyżuru"));
            Assert.That(fieldLabels["Year"], Is.EqualTo("Rok szkolenia"));
        }

        [Test]
        public void GetRequiredFields_ForAddEditMedicalShift_ReturnsCorrectFields()
        {
            // Act
            var requiredFields = this.strategy.GetRequiredFields("AddEditMedicalShift");

            // Assert
            Assert.That(requiredFields, Is.Not.Null);
            Assert.That(requiredFields, Does.Contain("Date"));
            Assert.That(requiredFields, Does.Contain("Hours"));
            Assert.That(requiredFields, Does.Contain("Location"));

            // Old SMK specific fields should not be required
            Assert.That(requiredFields, Does.Not.Contain("OldSMKField1"));
            Assert.That(requiredFields, Does.Not.Contain("OldSMKField2"));
        }

        [Test]
        public void GetDefaultValues_ForAddEditMedicalShift_ReturnsCorrectDefaults()
        {
            // Act
            var defaultValues = this.strategy.GetDefaultValues("AddEditMedicalShift");

            // Assert
            Assert.That(defaultValues, Is.Not.Null);
            Assert.That(defaultValues.ContainsKey("Date"), Is.True);
            Assert.That(defaultValues.ContainsKey("Hours"), Is.True);
            Assert.That(defaultValues.ContainsKey("Minutes"), Is.True);
            Assert.That(defaultValues.ContainsKey("Year"), Is.True);

            Assert.That(defaultValues["Hours"], Is.EqualTo(10));
            Assert.That(defaultValues["Minutes"], Is.EqualTo(0));
            Assert.That(defaultValues["Year"], Is.EqualTo(1));
        }

        [Test]
        public void FormatAdditionalFields_SerializesToJson()
        {
            // Arrange
            var fields = new Dictionary<string, object>
            {
                { "Field1", "Value1" },
                { "Field2", 123 },
                { "Field3", true },
            };

            // Act
            string json = this.strategy.FormatAdditionalFields(fields);

            // Assert
            Assert.That(json, Is.Not.Null);
            var deserializedFields = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            Assert.That(deserializedFields, Is.Not.Null);
            Assert.That(deserializedFields.Count, Is.EqualTo(3));
            Assert.That(JsonSerializer.Serialize(deserializedFields["Field1"]), Is.EqualTo("\"Value1\""));
            Assert.That(JsonSerializer.Serialize(deserializedFields["Field2"]), Is.EqualTo("123"));
            Assert.That(JsonSerializer.Serialize(deserializedFields["Field3"]), Is.EqualTo("true"));
        }

        [Test]
        public void ParseAdditionalFields_WithValidJson_DeserializesCorrectly()
        {
            // Arrange
            string json = "{\"Field1\":\"Value1\",\"Field2\":123,\"Field3\":true}";

            // Act
            var fields = this.strategy.ParseAdditionalFields(json);

            // Assert
            Assert.That(fields, Is.Not.Null);
            Assert.That(fields.Count, Is.EqualTo(3));
            Assert.That(JsonSerializer.Serialize(fields["Field1"]), Is.EqualTo("\"Value1\""));
            Assert.That(JsonSerializer.Serialize(fields["Field2"]), Is.EqualTo("123"));
            Assert.That(JsonSerializer.Serialize(fields["Field3"]), Is.EqualTo("true"));
        }

        [Test]
        public void ParseAdditionalFields_WithEmptyJson_ReturnsEmptyDictionary()
        {
            // Arrange
            string json = string.Empty;

            // Act
            var fields = this.strategy.ParseAdditionalFields(json);

            // Assert
            Assert.That(fields, Is.Not.Null);
            Assert.That(fields.Count, Is.EqualTo(0));
        }

        [Test]
        public void IsFieldSupported_ForSupportedField_ReturnsTrue()
        {
            // Act & Assert
            Assert.That(this.strategy.IsFieldSupported("AddEditMedicalShift", "Date"), Is.True);
            Assert.That(this.strategy.IsFieldSupported("AddEditMedicalShift", "Hours"), Is.True);
            Assert.That(this.strategy.IsFieldSupported("AddEditMedicalShift", "Location"), Is.True);
        }

        [Test]
        public void IsFieldSupported_ForUnsupportedField_ReturnsFalse()
        {
            // Act & Assert
            Assert.That(this.strategy.IsFieldSupported("AddEditMedicalShift", "OldSMKField1"), Is.False);
            Assert.That(this.strategy.IsFieldSupported("AddEditMedicalShift", "OldSMKField2"), Is.False);
            Assert.That(this.strategy.IsFieldSupported("AddEditMedicalShift", "NonExistentField"), Is.False);
        }

        [Test]
        public void GetValidationMessage_ForField_ReturnsMessage()
        {
            // Act & Assert
            Assert.That(this.strategy.GetValidationMessage("AddEditMedicalShift", "Date"), Is.EqualTo("Data dyżuru jest wymagana"));
            Assert.That(this.strategy.GetValidationMessage("AddEditMedicalShift", "Hours"), Is.EqualTo("Liczba godzin musi być większa od 0"));
            Assert.That(this.strategy.GetValidationMessage("AddEditMedicalShift", "Location"), Is.EqualTo("Miejsce dyżuru jest wymagane"));
        }

        [Test]
        public void GetViewTitle_ForViews_ReturnsCorrectTitles()
        {
            // Act & Assert
            Assert.That(this.strategy.GetViewTitle("AddEditMedicalShift"), Is.EqualTo("Dodaj/Edytuj dyżur medyczny"));
            Assert.That(this.strategy.GetViewTitle("AddEditProcedure"), Is.EqualTo("Dodaj/Edytuj procedurę"));
            Assert.That(this.strategy.GetViewTitle("AddEditInternship"), Is.EqualTo("Dodaj/Edytuj staż"));
        }

        [Test]
        public void GetPickerOptions_ForOperatorCode_ReturnsCorrectOptions()
        {
            // Act
            var options = this.strategy.GetPickerOptions("AddEditProcedure", "OperatorCode");

            // Assert
            Assert.That(options, Is.Not.Null);
            Assert.That(options.Count, Is.EqualTo(2));
            Assert.That(options["A"], Is.EqualTo("Operator (A)"));
            Assert.That(options["B"], Is.EqualTo("Asysta (B)"));
        }
    }

    public class OldSmkStrategyTests
    {
        private ISmkVersionStrategy strategy;

        [SetUp]
        public void Setup()
        {
            this.strategy = new OldSmkStrategy();
        }

        [Test]
        public void GetVisibleFields_ForAddEditMedicalShift_ReturnsCorrectFields()
        {
            // Act
            var visibleFields = this.strategy.GetVisibleFields("AddEditMedicalShift");

            // Assert
            Assert.That(visibleFields, Is.Not.Null);
            Assert.That(visibleFields.ContainsKey("Date"), Is.True);
            Assert.That(visibleFields.ContainsKey("Hours"), Is.True);
            Assert.That(visibleFields.ContainsKey("Minutes"), Is.True);
            Assert.That(visibleFields.ContainsKey("Location"), Is.True);
            Assert.That(visibleFields.ContainsKey("Year"), Is.True);

            // Old SMK specific fields should be visible
            Assert.That(visibleFields.ContainsKey("OldSMKField1"), Is.True);
            Assert.That(visibleFields.ContainsKey("OldSMKField2"), Is.True);
            Assert.That(visibleFields["OldSMKField1"], Is.True);
            Assert.That(visibleFields["OldSMKField2"], Is.True);
        }

        [Test]
        public void GetRequiredFields_ForAddEditMedicalShift_ReturnsCorrectFields()
        {
            // Act
            var requiredFields = this.strategy.GetRequiredFields("AddEditMedicalShift");

            // Assert
            Assert.That(requiredFields, Is.Not.Null);
            Assert.That(requiredFields, Does.Contain("Date"));
            Assert.That(requiredFields, Does.Contain("Hours"));
            Assert.That(requiredFields, Does.Contain("Location"));

            // Old SMK specific fields should be required
            Assert.That(requiredFields, Does.Contain("OldSMKField1"));
            Assert.That(requiredFields, Does.Contain("OldSMKField2"));
        }

        [Test]
        public void GetDefaultValues_ForAddEditMedicalShift_ReturnsCorrectDefaults()
        {
            // Act
            var defaultValues = this.strategy.GetDefaultValues("AddEditMedicalShift");

            // Assert
            Assert.That(defaultValues, Is.Not.Null);
            Assert.That(defaultValues.ContainsKey("Date"), Is.True);
            Assert.That(defaultValues.ContainsKey("Hours"), Is.True);
            Assert.That(defaultValues.ContainsKey("Minutes"), Is.True);
            Assert.That(defaultValues.ContainsKey("Year"), Is.True);
            Assert.That(defaultValues.ContainsKey("OldSMKField1"), Is.True);
            Assert.That(defaultValues.ContainsKey("OldSMKField2"), Is.True);

            Assert.That(defaultValues["Hours"], Is.EqualTo(10));
            Assert.That(defaultValues["Minutes"], Is.EqualTo(0));
            Assert.That(defaultValues["Year"], Is.EqualTo(1));
            Assert.That(defaultValues["OldSMKField1"], Is.EqualTo(string.Empty));
            Assert.That(defaultValues["OldSMKField2"], Is.EqualTo(string.Empty));
        }

        [Test]
        public void GetValidationMessage_ForOldSmkFields_ReturnsMessage()
        {
            // Act & Assert
            Assert.That(this.strategy.GetValidationMessage("AddEditMedicalShift", "OldSMKField1"), Is.EqualTo("Osoba nadzorująca jest wymagana"));
            Assert.That(this.strategy.GetValidationMessage("AddEditMedicalShift", "OldSMKField2"), Is.EqualTo("Oddział jest wymagany"));
        }

        [Test]
        public void GetViewTitle_ForViews_ReturnsCorrectTitles()
        {
            // Act & Assert
            Assert.That(this.strategy.GetViewTitle("AddEditMedicalShift"), Is.EqualTo("Dodaj/Edytuj dyżur medyczny (Stary SMK)"));
            Assert.That(this.strategy.GetViewTitle("AddEditProcedure"), Is.EqualTo("Dodaj/Edytuj procedurę (Stary SMK)"));
            Assert.That(this.strategy.GetViewTitle("AddEditInternship"), Is.EqualTo("Dodaj/Edytuj staż (Stary SMK)"));
        }
    }
}