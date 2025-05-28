using System.Globalization;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

public enum SubmissionType
{
    Complaint,  
    Feedback,  
    Proposal     
}

public class Submission : IHasId, IHasTitle
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public SubmissionType Type { get; set; }
    
    public Guid CreatorId { get; set; }

    public Authority Authority { get; set; } = null!;
    public Guid AuthorityId { get; set; }

    public SubmissionStatus Status { get; set; } = SubmissionStatus.New;
    
    public string Answer { get; set; } = string.Empty;
    
    public GeoPoint? Location { get; set; }
} 

public enum SubmissionStatus
{
    New,
    InProgress,
    Resolved,
    Rejected
}

[Owned]
public class GeoPoint
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public GeoPoint() { }
    
    public GeoPoint(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public override string ToString() => $"({Latitude}, {Longitude})";

    public string To2GisUrl(int zoom = 16) =>
        $"https://2gis.kg/map/{Latitude.ToString(CultureInfo.InvariantCulture)},{Longitude.ToString(CultureInfo.InvariantCulture)}/{zoom}";
}