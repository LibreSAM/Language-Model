namespace learn;
public class Threegram
{
    public string Word { get; }
    public string Before { get; }
    public string BeforeBefore { get; }
    public uint OccuranceCount { get; private set; }

    public Threegram(string beforebefore, string before, string word) {
        BeforeBefore = beforebefore;
        Before = before;
        Word = word;
        OccuranceCount = 0;
    }

    public void IncrementOccurenceCountByOne() {
        OccuranceCount += 1;
    }
}
