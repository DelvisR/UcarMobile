using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace UcarMobileApi.Application.Validators.Technicians;

/// <summary>
/// Collection validation helpers for time-range scenarios.
/// </summary>
public static class TimeRangeValidationExtensions
{
    /// <summary>
    /// Validates that, for items grouped by a key (e.g. DayOfWeek), their time ranges do not overlap.
    /// </summary>
    /// <typeparam name="T">The root model type being validated.</typeparam>
    /// <typeparam name="TItem">The collection item type.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="groupKeySelector">Select grouping key (e.g. day).</param>
    /// <param name="startSelector">Select start TimeSpan.</param>
    /// <param name="endSelector">Select end TimeSpan.</param>
    /// <param name="allowTouching">
    /// If true, adjacent ranges where previous.End == next.Start are allowed.
    /// If false, equality is considered overlap and will fail validation.
    /// </param>
    /// <returns>Rule builder options.</returns>
    public static IRuleBuilderOptions<T, IEnumerable<TItem>> MustHaveNoOverlappingTimeRanges<T, TItem>(
        this IRuleBuilder<T, IEnumerable<TItem>> ruleBuilder,
        Func<TItem, object> groupKeySelector,
        Func<TItem, TimeSpan?> startSelector,
        Func<TItem, TimeSpan?> endSelector,
        bool allowTouching = true)
    {
        return ruleBuilder
            .Must(list =>
            {
                if (list == null) return true;
                var items = list.ToList();
                if (items.Count == 0) return true;

                // Validate each item has sensible times
                foreach (var it in items)
                {
                    var s = startSelector(it);
                    var e = endSelector(it);
                    if (!s.HasValue || !e.HasValue) return false; // invalid item
                    if (s.Value >= e.Value) return false; // start must be strictly < end
                }

                // Group by the key (e.g., day)
                var groups = items.GroupBy(groupKeySelector);
                foreach (var g in groups)
                {
                    var ranges = g
                        .Select(it => (Start: startSelector(it)!.Value, End: endSelector(it)!.Value))
                        .OrderBy(r => r.Start)
                        .ToList();

                    TimeSpan? prevEnd = null;
                    foreach (var (start, end) in ranges)
                    {
                        if (prevEnd == null)
                        {
                            prevEnd = end;
                            continue;
                        }

                        // If touching is allowed, only strictly greater means overlap.
                        // If touching not allowed, greater-or-equal means overlap.
                        if (allowTouching)
                        {
                            if (prevEnd > start) return false; // overlap
                        }
                        else
                        {
                            if (prevEnd >= start) return false; // overlap or touching
                        }

                        prevEnd = (end > prevEnd) ? end : prevEnd;
                    }
                }

                return true;
            })
            .WithMessage("Time ranges overlap within the same group (e.g., same day).");
    }
}
