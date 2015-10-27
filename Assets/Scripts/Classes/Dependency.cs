public class Dependency
{
    private string patternName;
    private int tagIndex;
    private Mastery mastery;

    public Dependency(string patternName, int tagIndex, Mastery mastery)
    {
        this.patternName = patternName;
        this.tagIndex = tagIndex;
        this.mastery = mastery;
    }

    public bool IsResolved()
    {
        PatternInfo patternInfo = PatternManager.instance.patternsInfo[patternName];

        float patternScore = patternInfo.GetScore(tagIndex);
        Mastery patternMastery = Mastery.FromAttempts(patternScore, patternInfo.attempts[tagIndex].Count);

        return patternMastery == mastery;
    }
}
