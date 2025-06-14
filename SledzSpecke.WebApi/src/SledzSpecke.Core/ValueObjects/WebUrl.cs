using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record WebUrl
{
    public string Value { get; }

    public WebUrl(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidWebUrlException("URL cannot be empty.");
        }

        value = value.Trim();

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            throw new InvalidWebUrlException($"'{value}' is not a valid absolute URL.");
        }

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidWebUrlException("URL must use HTTP or HTTPS protocol.");
        }

        Value = value;
    }

    public static implicit operator string(WebUrl url) => url.Value;
    public static implicit operator WebUrl(string url) => new(url);
    
    public override string ToString() => Value;
    
    public Uri ToUri() => new(Value);
    public string GetDomain() => ToUri().Host;
    public bool IsHttps() => ToUri().Scheme == Uri.UriSchemeHttps;
}