using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using TaskManager.Core.Validation;

namespace TaskManager.Service.Validation;

// FN-00-R2: trims input and re-checks it in the Service; messages per GN-07-R1
public sealed class FieldErrors
{
    private readonly Dictionary<string, List<string>> _errors = [];

    public bool HasErrors => _errors.Count > 0;

    // Required text: returns the trimmed value
    public string Text(string field, string? value, int min, int max)
    {
        var trimmed = value?.Trim() ?? string.Empty;
        if (trimmed.Length == 0)
            Add(field, ValidationMessages.Required);
        else if (trimmed.Length < min || trimmed.Length > max)
            Add(field, string.Format(ValidationMessages.Length, field, max, min));
        return trimmed;
    }

    // Optional text: returns the trimmed value, or null when empty
    public string? OptionalText(string field, string? value, int max)
    {
        var trimmed = value?.Trim();
        if (string.IsNullOrEmpty(trimmed))
            return null;
        if (trimmed.Length > max)
            Add(field, string.Format(ValidationMessages.MaxLength, field, max));
        return trimmed;
    }

    // FN-GU-01: trimmed and lowercased; max 256 and email format as in DT-01
    public string Email(string field, string? value)
    {
        var email = value?.Trim().ToLowerInvariant() ?? string.Empty;
        if (email.Length == 0)
            Add(field, ValidationMessages.Required);
        else if (email.Length > 256)
            Add(field, string.Format(ValidationMessages.MaxLength, field, 256));
        else if (!EmailFormat.IsValid(email))
            Add(field, ValidationMessages.Email);
        return email;
    }

    // EN-01-R1: 8–64 characters with a letter and a digit. Not trimmed: spaces are part of the password.
    public void Password(string field, string? value)
    {
        if (string.IsNullOrEmpty(value))
            Add(field, ValidationMessages.Required);
        else if (value.Length is < 8 or > 64)
            Add(field, string.Format(ValidationMessages.Length, field, 64, 8));
        else if (!PasswordFormat.IsMatch(value))
            Add(field, ValidationMessages.PasswordComplexity);
    }

    private static readonly EmailAddressAttribute EmailFormat = new();
    private static readonly Regex PasswordFormat = new(ValidationMessages.PasswordPattern, RegexOptions.Compiled);

    public void Add(string field, string message)
    {
        if (!_errors.TryGetValue(field, out var messages))
            _errors[field] = messages = [];
        messages.Add(message);
    }

    public IReadOnlyDictionary<string, string[]> ToDictionary() =>
        _errors.ToDictionary(e => e.Key, e => e.Value.ToArray());
}
