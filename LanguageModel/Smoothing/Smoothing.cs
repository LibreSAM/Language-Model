namespace LanguageModel.Smoothing;

/// <summary>
/// Provides general functions for calculating smoothed ngram probabilities.
/// </summary>
public static class Smoothing
{
    /// <summary>
    /// Creates an <see cref="ISmoothing"/> object implementing the specified smoothing type.
    /// </summary>
    /// <param name="type">The smoothing type.</param>
    /// <returns>An object implementing <see cref="ISmoothing"/> that implements the specified smoothing type.</returns>
    /// <exception cref="ArgumentException">An unknown smoothing type was provided.</exception>
    public static ISmoothing Get(SmoothingType type)
    {
        return type switch
        {
            SmoothingType.Regular => new RegularSmoothing(),
            SmoothingType.KneserNey => new KneserNeySmoothing(),
            _ => throw new ArgumentException($"Invalid value: \"{type}\"")
        };
    }
}