public sealed class Mastery
{
    private readonly string name;
    private readonly int value;

    public static readonly Mastery UNEXERCISED = new Mastery(0, "Unexercised");
    public static readonly Mastery INITIATED = new Mastery(1, "Initiated");
    public static readonly Mastery BURNED_OUT = new Mastery(-1, "Burned Out");
    public static readonly Mastery MASTERED = new Mastery(2, "Mastered");

    private Mastery(int value, string name)
    {
        this.name = name;
        this.value = value;
    }

    public int Value()
    {
        return value;
    }

    public override string ToString()
    {
        return name;
    }

    public static Mastery FromId(string id)
    {
        switch(id) {
            case "U": return UNEXERCISED;
            case "I": return INITIATED;
            case "B": return BURNED_OUT;
            case "M": return MASTERED;
        
            default: return null;
        }
    }
}
