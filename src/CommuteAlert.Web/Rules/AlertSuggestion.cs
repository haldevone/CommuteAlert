using CommuteAlert.Web.Models;

namespace CommuteAlert.Web.Rules;

public record AlertSuggestion(AlertType Type, string Key, string Message);
