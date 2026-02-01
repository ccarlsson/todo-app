using System.Net.Mail;

namespace TodoApp.Domain.ValueObjects;

public sealed class Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email is required.", nameof(value));
        }

        var trimmed = value.Trim();
        _ = new MailAddress(trimmed);

        return new Email(trimmed.ToLowerInvariant());
    }

    public override string ToString() => Value;
}
