public class Dependency
{
    private string patternName;
    private int tagIndex;
    private string mastery;

    public Dependency(string patternName, int tagIndex, string mastery)
    {
        this.patternName = patternName;
        this.tagIndex = tagIndex;
        this.mastery = mastery;
    }

    public bool IsResolved()
    {
        PatternInfo patternInfo = PatternManager.instance.patternsInfo[patternName];

        float patternScore = patternInfo.GetScore(tagIndex);
        string patternMastery = Mastery.FromAttempts(patternScore, patternInfo.attempts[tagIndex].Count);

        return patternMastery == mastery;
    }
}
