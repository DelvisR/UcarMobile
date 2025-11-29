using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace UcarMobileApi.Application.Validators.Common;

/// <summary>
/// Provides collection-based reusable validation rules for FluentValidation.
/// Includes rules for validating uniqueness and single-occurrence constraints
/// in list properties.
/// </summary>
public static class CollectionValidationExtensions
{
    /// <summary>
    /// Validates that a collection contains items with unique values according to a key selector.
    /// </summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TItem">The item type within the collection.</typeparam>
    /// <typeparam name="TKey">The type of the key used for uniqueness comparison.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="keySelector">A function selecting the key whose uniqueness must be enforced.</param>
    /// <returns>The FluentValidation rule builder.</returns>
    /// <example>
    /// RuleFor(x => x.Contacts)
    ///     .MustHaveUnique(c => c.Email);
    ///
    /// RuleFor(x => x.ServiceZones)
    ///     .MustHaveUnique(z => z.ServiceZone.Id);
    ///
    /// RuleFor(x => x.WorkShifts)
    ///     .MustHaveUnique(s => new { s.Day, s.StartTime });
    /// </example>
    public static IRuleBuilderOptions<T, IEnumerable<TItem>> MustHaveUnique<T, TItem, TKey>(
        this IRuleBuilder<T, IEnumerable<TItem>> ruleBuilder,
        Func<TItem, TKey> keySelector)
    {
        return ruleBuilder
            .Must(list =>
                list == null ||
                !list.Any() ||
                list.Select(keySelector)
                    .GroupBy(k => k)
                    .All(g => g.Count() == 1))
            .WithMessage("Duplicate values are not allowed.");
    }

    /// <summary>
    /// Validates that exactly one item in the collection satisfies the given predicate.
    /// Useful for ensuring that only one item is marked as a primary or default element.
    /// </summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TItem">The item type within the collection.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="predicate">A predicate selecting the item that must occur exactly once.</param>
    /// <returns>The FluentValidation rule builder.</returns>
    /// <example>
    /// RuleFor(x => x.ServiceZones)
    ///     .MustHaveSingle(z => z.IsPrimaryZone);
    ///
    /// RuleFor(x => x.Addresses)
    ///     .MustHaveSingle(a => a.IsDefaultAddress);
    /// </example>
    public static IRuleBuilderOptions<T, List<TItem>> MustHaveSingle<T, TItem>(
        this IRuleBuilder<T, List<TItem>> ruleBuilder,
        Func<TItem, bool> predicate)
    {
        return ruleBuilder
            .Must(list =>
                list == null ||
                list.Count == 0 ||
                list.Count(predicate) == 1)
            .WithMessage("Exactly one item must match the condition.");
    }
}
