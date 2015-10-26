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
}
