namespace PsychologistBooking.Application.Dtos;

public class PsychologistDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string PsychologistType { get; set; } = default!;
    public string PictureUrl { get; set; } = default!;
    public List<DateTime> NextAvailable { get; set; } = new();
}