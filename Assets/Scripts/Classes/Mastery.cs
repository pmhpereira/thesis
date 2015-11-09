using System.Collections.Generic;

public sealed class Mastery
{
    private readonly string name;
    private readonly int value;

    public static readonly Mastery UNEXERCISED = new Mastery(0, "Unexercised");
    public static readonly Mastery INITIATED = new Mastery(1, "Initiated");
    public static readonly Mastery BURNED_OUT = new Mastery(-1, "Burned Out");
    public static readonly Mastery PARTIALLY_MASTERED = new Mastery(1, "Partially Mastered");
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
            case "P": return PARTIALLY_MASTERED;
            case "M": return MASTERED;

            default: return null;
        }
    }

    public string ToId()
    {
        if (this == UNEXERCISED) return "U";
        if (this == INITIATED) return "I";
        if (this == BURNED_OUT) return "B";
        if (this == PARTIALLY_MASTERED) return "P";
        if (this == MASTERED) return "M";

        return null;
    }

    public static Mastery FromAttempts(float score, int numAttempts)
    {
        if (score >= 0.9 && numAttempts > 8) return MASTERED;
        //if (score >= 0.5 && numAttempts > 6) return PARTIALLY_MASTERED;
        if (score <= 0.1 && numAttempts > 8) return BURNED_OUT;
        if (numAttempts > 0) return INITIATED;
        if (numAttempts == 0) return UNEXERCISED;

        return null;
    }
}
