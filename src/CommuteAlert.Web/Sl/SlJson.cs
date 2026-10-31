using System.Text.Json;

namespace CommuteAlert.Web.Sl;

public static class SlJson
{
    public static readonly JsonSerializerOptions Options =
    new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
}
