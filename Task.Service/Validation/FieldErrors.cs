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

    public void Add(string field, string message)
    {
        if (!_errors.TryGetValue(field, out var messages))
            _errors[field] = messages = [];
        messages.Add(message);
    }

    public IReadOnlyDictionary<string, string[]> ToDictionary() =>
        _errors.ToDictionary(e => e.Key, e => e.Value.ToArray());
}
