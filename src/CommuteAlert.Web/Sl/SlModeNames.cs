namespace CommuteAlert.Web.Sl;

public static class SlModeNames
{
    public static string Singular(string mode) => mode switch
    {
        "BUS" => "Buss",
        "METRO" => "Tunnelbana",
        "TRAM" => "Spårvagn",
        "TRAIN" => "Tåg",
        "SHIP" => "Båt",
        "FERRY" => "Färja",
        "TAXI" => "Taxi",
        _ => mode
    };

    public static string Plural(string mode) => mode switch
    {
        "BUS" => "bussar",
        "METRO" => "tunnelbanetåg",
        "TRAM" => "spårvagnar",
        "TRAIN" => "tåg",
        "SHIP" => "båtar",
        "FERRY" => "färjor",
        "TAXI" => "taxi",
        _ => mode
    };
}