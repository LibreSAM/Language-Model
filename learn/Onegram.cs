namespace learn;
public class Onegram
{
    public string Word { get; }
    public uint OccuranceCount { get; private set; }

    public Onegram(string word) {
        Word = word;
        OccuranceCount = 0;
    }

    public void IncrementOccurenceCountByOne() {
        OccuranceCount += 1;
    }
}
