namespace Infrastructure.AI.Vectors;

public class CommonProblemDto
{
    public string RepresentativeDescription { get; set; } = "";
    public int Occurrences { get; set; }
    public List<SimilarSubmission> SimilarSubmissions { get; set; } = [];
}

public class SimilarSubmission
{
    public Guid SubmissionId { get; set; }
    public string ShortDescription { get; set; } = string.Empty;
}