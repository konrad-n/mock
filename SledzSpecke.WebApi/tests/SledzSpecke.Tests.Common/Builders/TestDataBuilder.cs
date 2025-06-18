using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;

namespace SledzSpecke.Tests.Common.Builders;

public abstract class TestDataBuilder<T> where T : class
{
    protected readonly Faker Faker;
    
    protected TestDataBuilder()
    {
        // Use Polish locale for realistic data
        Faker = new Faker("pl");
    }
    
    public abstract T Build();
    
    // Implicit conversion for convenience
    public static implicit operator T(TestDataBuilder<T> builder) => builder.Build();
    
    // Build multiple instances
    public List<T> BuildMany(int count)
    {
        return Enumerable.Range(0, count)
            .Select(_ => Build())
            .ToList();
    }
}