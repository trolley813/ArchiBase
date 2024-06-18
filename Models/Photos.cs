using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Models;

public class License : IAuditable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Link { get; set; }
}

[Owned]
public class BindMarkup
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class BuildingBind
{
    public Guid Id { get; set; }
    public Photo Photo { get; set; }
    public Building Building { get; set; }
    public BindMarkup? Markup { get; set; }
}

public class Gallery : IAuditable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Photo> Photos { get; set; }
}

public class Photo : IAuditable
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public bool NonAuthor { get; set; }

    public string Extension { get; set; } = "jpg";

    public string? Description { get; set; }
    public ImpreciseDate ShootingDate { get; set; } = new();
    public DateTime PublicationDate { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Direction { get; set; }
    public License? License { get; set; }

    public List<BuildingBind> BuildingBinds { get; set; } = [];

    public List<Gallery> Galleries { get; set; } = [];

    public List<PhotoVote> Votes { get; set; } = [];

    public int VotesCount => Votes.Select(v => v.Vote).Sum();
    public int UpvotesCount => Votes.Where(v => v.Vote > 0).Count();
    public int DownvotesCount => Votes.Where(v => v.Vote < 0).Count();


    public string PhotoLink => $"/data/photos/{Id.ToString()[..8]}/{Id}.{Extension}";

    public bool IsHidden { get; set; }
}

public class PhotoVote
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public Photo Photo { get; set; }
    public int Vote { get; set; }
}