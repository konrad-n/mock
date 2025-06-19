#!/usr/bin/env dotnet-script
#r "nuget: Microsoft.AspNetCore.Cryptography.KeyDerivation, 8.0.0"

using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

var password = "Test123";
const int SaltSize = 128 / 8;
const int HashSize = 256 / 8;
const int Iterations = 100000;

// Use a fixed salt for testing
byte[] salt = Convert.FromBase64String("4m+bQ6bnHBV78o35ezYs/Q==");

byte[] hash = KeyDerivation.Pbkdf2(
    password: password,
    salt: salt,
    prf: KeyDerivationPrf.HMACSHA256,
    iterationCount: Iterations,
    numBytesRequested: HashSize);

var saltBase64 = Convert.ToBase64String(salt);
var hashBase64 = Convert.ToBase64String(hash);

Console.WriteLine($"Password: {password}");
Console.WriteLine($"Salt: {saltBase64}");
Console.WriteLine($"Hash: {hashBase64}");
Console.WriteLine($"Full: {saltBase64}.{hashBase64}");