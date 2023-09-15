using System.Collections.Generic;

namespace PetRenamer.Core.Networking.Structs;

public class Pagination
{
    public int Page { get; set; } = -1;
    public object PageNext { get; set; } = null!;
    public object PagePrev { get; set; } = null!;
    public int PageTotal { get; set; } = -1;
    public int Results { get; set; } = -1;
    public int ResultsPerPage { get; set; } = -1;
    public int ResultsTotal { get; set; } = -1;
}

public class Result
{
    public string Avatar { get; set; } = null!;
    public int FeastMatches { get; set; } = -1;
    public int ID { get; set; } = -1;
    public string Lang { get; set; } = null!;
    public string Name { get; set; } = null!;
    public object Rank { get; set; } = null!;
    public object RankIcon { get; set; } = null!;
    public string Server { get; set; } = null!;
}

public class PaginationRoot
{
    public Pagination Pagination { get; set; } = null!;
    public List<Result> Results { get; set; } = null!;
}