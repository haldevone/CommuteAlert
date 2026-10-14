namespace CommuteAlert.Web.Sl;

public record SlDeparturesResponse(List<SlDeparture> Departures);
public record SlDeparture
(
    string Destination,
    int DirectionCode,
    string State,
    DateTime Scheduled,
    DateTime? Expected,
    SlJourney Journey,
    SlStopArea StopArea,
    SlStopPoint StopPoint,
    SlLine Line
);

public record SlJourney(long Id, string State, string? PredictionState);
public record SlStopArea(int Id, string Name, string Type);
public record SlStopPoint(int Id, string Name, string? Designation);
public record SlLine(int Id, string Designation, string TransportMode, string? GroupOfLines);