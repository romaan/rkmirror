using PsychologistBooking.Domain.Enums;

namespace PsychologistBooking.Domain.Entities;

public class Psychologist
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public PsychologistType PsychologistType { get; private set; }
    public string ShortDescription { get; private set; }
    public List<AvailableDate> AvailableDates { get; private set; } = new();

    private Psychologist() { }

    public static PsychologistBuilder Builder() => new PsychologistBuilder();

    public class PsychologistBuilder
    {
        private readonly Psychologist _psych = new Psychologist();

        public PsychologistBuilder WithName(string first, string last)
        {
            _psych.FirstName = first;
            _psych.LastName = last;
            return this;
        }

        public PsychologistBuilder WithType(PsychologistType type)
        {
            _psych.PsychologistType = type;
            return this;
        }

        public PsychologistBuilder WithDescription(string desc)
        {
            _psych.ShortDescription = desc;
            return this;
        }

        public PsychologistBuilder AddAvailability(DateTime date)
        {
            _psych.AvailableDates.Add(new AvailableDate { Date = date });
            return this;
        }

        public Psychologist Build()
        {
            _psych.Id = Guid.NewGuid();
            return _psych;
        }
    }
}
