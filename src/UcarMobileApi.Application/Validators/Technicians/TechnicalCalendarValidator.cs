using System.Collections.Generic;
using FluentValidation;
using UcarMobileApi.Application.DTOs.Technicians;
using UcarMobileApi.Application.Validators.Common;

namespace UcarMobileApi.Application.Validators.Technicians;

/// <summary>Validator for TechnicalWorkScheduleDto</summary>
public class TechnicalWorkScheduleValidator : AbstractValidator<TechnicalWorkScheduleDto>
{
    public TechnicalWorkScheduleValidator()
    {
        RuleFor(x => x.TechnicianId).GreaterThan(0);
        RuleFor(x => x.StartTime).NotNull();
        RuleFor(x => x.EndTime).NotNull();
        RuleFor(x => x).Must(x => x.StartTime < x.EndTime)
            .WithMessage("StartTime must be less than EndTime.");
        RuleFor(x => x.Day).IsInEnum().WithMessage(ValidatorErrors.InvalidValue);
    }
}

/// <summary>
/// Validator for a list of TechnicalWorkScheduleDto items.
/// Ensures each entry is valid and that there are no duplicated days.
/// </summary>
public class TechnicalWorkScheduleListValidator : AbstractValidator<IEnumerable<TechnicalWorkScheduleDto>>
{
    public TechnicalWorkScheduleListValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .WithMessage("The schedule list cannot be empty.");

        RuleForEach(x => x)
            .SetValidator(new TechnicalWorkScheduleValidator());

        // Ensure no overlapping ranges within the same Day
        RuleFor(x => x)
            .MustHaveNoOverlappingTimeRanges(
                groupKeySelector: item => item.Day,
                startSelector: item => item.StartTime,
                endSelector: item => item.EndTime,
                allowTouching: true) // allow ranges that touch (end == start)
            .WithMessage("Schedules for the same day must not overlap.");
    }
}

/// <summary>Validator for TechnicalCalendarBlockDto</summary>
public class TechnicalCalendarBlockValidator : AbstractValidator<TechnicalCalendarBlockDto>
{
    public TechnicalCalendarBlockValidator()
    {
        RuleFor(x => x.TechnicianId).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty();

        RuleFor(x => x).Must(HasDateOrWeeklyDay)
            .WithMessage("Either SpecificDate or WeeklyDay must be provided.");

        When(x => !x.AllDay, () =>
        {
            RuleFor(x => x.StartTime).NotNull();
            RuleFor(x => x.EndTime).NotNull();
            RuleFor(x => x).Must(x => x.StartTime < x.EndTime)
                .WithMessage("StartTime must be less than EndTime for partial-day blocks.");
        });
    }

    private static bool HasDateOrWeeklyDay(TechnicalCalendarBlockDto dto)
    {
        return dto.SpecificDate.HasValue || dto.WeeklyDay.HasValue;
    }
}
