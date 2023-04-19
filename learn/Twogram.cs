namespace learn;
public class Twogram
{
    public string Word { get; }
    public string Before { get; }
    public uint OccuranceCount { get; private set; }

    public Twogram(string before, string word) {
        Before = before;
        Word = word;
        OccuranceCount = 0;
    }

    public void IncrementOccurenceCountByOne() {
        OccuranceCount += 1;
    }
}
