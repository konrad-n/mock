using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record Address
{
    public string Street { get; }
    public string HouseNumber { get; }
    public string? ApartmentNumber { get; }
    public string PostalCode { get; }
    public string City { get; }
    public string Province { get; }

    public Address(
        string street,
        string houseNumber,
        string? apartmentNumber,
        string postalCode,
        string city,
        string province)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Street cannot be empty.");
        if (string.IsNullOrWhiteSpace(houseNumber))
            throw new DomainException("House number cannot be empty.");
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new DomainException("Postal code cannot be empty.");
        if (!IsValidPostalCode(postalCode))
            throw new DomainException("Invalid postal code format. Use XX-XXX format.");
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City cannot be empty.");
        if (string.IsNullOrWhiteSpace(province))
            throw new DomainException("Province cannot be empty.");

        Street = street.Trim();
        HouseNumber = houseNumber.Trim();
        ApartmentNumber = string.IsNullOrWhiteSpace(apartmentNumber) ? null : apartmentNumber.Trim();
        PostalCode = postalCode.Trim();
        City = city.Trim();
        Province = province.Trim();
    }

    private static bool IsValidPostalCode(string postalCode)
    {
        // Polish postal code format: XX-XXX
        return System.Text.RegularExpressions.Regex.IsMatch(postalCode, @"^\d{2}-\d{3}$");
    }

    public string GetFullAddress()
    {
        var apartment = string.IsNullOrEmpty(ApartmentNumber) ? "" : $"/{ApartmentNumber}";
        return $"{Street} {HouseNumber}{apartment}, {PostalCode} {City}, {Province}";
    }

    public override string ToString() => GetFullAddress();
}