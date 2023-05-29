namespace LanguageModel.Smoothing;

public static class Smoothing
{
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