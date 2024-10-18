﻿using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Giantnodes.Infrastructure;

/// <summary>
/// Represents a property that has failed validation or requires attention in a fault scenario.
/// </summary>
public sealed record FaultProperty
{
    /// <summary>
    /// Represents a specific validation failure for a property.
    /// </summary>
    /// <param name="Rule">The validation rule that failed.</param>
    /// <param name="Reason">The reason for the validation failure.</param>
    public readonly record struct ValidationInfo(string Rule, string Reason);

    /// <summary>
    /// Initializes a new instance of the <see cref="FaultProperty"/> class.
    /// </summary>
    /// <param name="property">The name of the property.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="validation">A collection of validation failures for the property.</param>
    [JsonConstructor]
    public FaultProperty(string property, string? value, IReadOnlyCollection<ValidationInfo>? validation = null)
    {
        Property = property;
        Value = value;
        Validation = validation ?? Array.Empty<ValidationInfo>();
    }

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    public string Property { get; init; }

    /// <summary>
    /// Gets the value of the property.
    /// </summary>
    public string? Value { get; init; }

    /// <summary>
    /// Gets a collection of validation failures associated with the property.
    /// </summary>
    public IReadOnlyCollection<ValidationInfo> Validation { get; init; } = [];

    /// <summary>
    /// Creates a new <see cref="FaultProperty"/> instance with the specified property name and value.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="property">The name of the property.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>A new <see cref="FaultProperty"/> instance.</returns>
    public static FaultProperty Create<T>(string property, T? value)
        => new(property, value?.ToString());

    /// <summary>
    /// Creates a new <see cref="FaultProperty"/> instance with the specified property name and a null value.
    /// </summary>
    /// <param name="property">The name of the property.</param>
    /// <returns>A new <see cref="FaultProperty"/> instance.</returns>
    public static FaultProperty Create(string property)
        => new(property, null);

    /// <summary>
    /// Creates a new <see cref="FaultProperty"/> instance using the caller's argument expression to determine the
    /// property name.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="property">The property value.</param>
    /// <param name="parameterName">The name of the property, automatically extracted from the caller's argument expression.</param>
    /// <returns>A new <see cref="FaultProperty"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the extracted parameter name is null.</exception>
    public static FaultProperty Create<T>(
        T property,
        [CallerArgumentExpression("property")] string? parameterName = null) where T : notnull
        => Create(parameterName ?? throw new ArgumentNullException(nameof(parameterName)), property);
}