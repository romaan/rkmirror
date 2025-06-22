namespace PsychologistBooking.Domain.Entities;

public class AvailableDate
{
    public int Id { get; set; }
    public Guid PsychologistId { get; set; }
    public DateTime Date { get; set; }
}