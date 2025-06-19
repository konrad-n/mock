#!/usr/bin/env dotnet-script

// Generate valid PESEL
var year = 90; // 1990
var month = 1;
var day = 1;
var serial = 100;
var sex = 1; // male

var peselWithoutChecksum = $"{year:D2}{month:D2}{day:D2}{serial:D3}{sex}";
var weights = new[] { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
var sum = 0;

for (int i = 0; i < 10; i++)
{
    sum += int.Parse(peselWithoutChecksum[i].ToString()) * weights[i];
}

var checksum = (10 - (sum % 10)) % 10;
var validPesel = peselWithoutChecksum + checksum;

Console.WriteLine($"Valid PESEL: {validPesel}");

// Generate valid PWZ
var pwzBase = "123456";
var pwzWeights = new[] { 1, 2, 3, 4, 5, 6 };
var pwzSum = 0;

for (int i = 0; i < 6; i++)
{
    pwzSum += int.Parse(pwzBase[i].ToString()) * pwzWeights[i];
}

var pwzChecksum = pwzSum % 11;
var validPwz = pwzBase + pwzChecksum;

Console.WriteLine($"Valid PWZ: {validPwz}");

// Calculate PESEL sum step by step
Console.WriteLine("\nPESEL calculation:");
for (int i = 0; i < 10; i++)
{
    var digit = int.Parse(peselWithoutChecksum[i].ToString());
    var weight = weights[i];
    var product = digit * weight;
    Console.WriteLine($"Position {i}: {digit} * {weight} = {product}");
}