namespace CommuteAlert.Web.Sl.Models;

public record SlDeviation
(
    long DeviationCaseId,
    SlPublish Publish,
    List<SlMessageVariant> MessageVariants,
    SlScope Scope
);

public record SlPublish(DateTimeOffset From, DateTimeOffset Upto);
public record SlMessageVariant(string Header, string Details, string? ScopeAlias, string Language);
public record SlScope(List<SlScopeStopArea>? StopAreas, List<SlScopeLine>? Lines);
public record SlScopeStopArea(int Id, string Name, string Type);
public record SlScopeLine(int Id, string Designation, string TransportMode, string? GroupOfLines);