/*
	This class calculates and compares the different values of mastery
*/

using System.Collections.Generic;

public class MasteryComparison
{
    public const string LESS = "<";
    public const string LESS_OR_EQUAL = "<=";
    public const string EQUAL = "=";
    public const string GREATER_OR_EQUAL = ">=";
    public const string GREATER = ">";

    public static readonly List<string> values = new List<string>(new string[] {
        MasteryComparison.LESS,
        MasteryComparison.LESS_OR_EQUAL,
        MasteryComparison.EQUAL,
        MasteryComparison.GREATER_OR_EQUAL,
        MasteryComparison.GREATER
    });

    public static bool Compare(string comparator, string masteryA, string masteryB)
    {
        int masteryAindex = Mastery.values.IndexOf(masteryA);
        int masteryBindex = Mastery.values.IndexOf(masteryB);

        switch(comparator)
        {
            case LESS:
                return masteryAindex < masteryBindex;
            case LESS_OR_EQUAL:
                return masteryAindex <= masteryBindex;
            case EQUAL:
                return masteryAindex == masteryBindex;
            case GREATER_OR_EQUAL:
                return masteryAindex >= masteryBindex;
            case GREATER:
                return masteryAindex > masteryBindex;
            default:
                return false;
        }
    }
}

public class Mastery
{
    public static readonly string FRUSTRATED = "Frustrated";
    public static readonly string UNEXERCISED = "Unexercised";
    public static readonly string INITIATED = "Initiated";
    public static readonly string PARTIALLY_MASTERED = "Partially Mastered";
    public static readonly string MASTERED = "Mastered";
    public static readonly string BURNED_OUT = "Burned Out";

    public static readonly List<string> values = new List<string>(new string[] {
        Mastery.FRUSTRATED,
        Mastery.UNEXERCISED,
        Mastery.INITIATED,
        Mastery.PARTIALLY_MASTERED,
        Mastery.MASTERED,
        Mastery.BURNED_OUT,
    });

    public static string FromId(string id)
    {
        switch(id) {
            case "F": return FRUSTRATED;
            case "U": return UNEXERCISED;
            case "I": return INITIATED;
            case "P": return PARTIALLY_MASTERED;
            case "M": return MASTERED;
            case "B": return BURNED_OUT;

            default: return null;
        }
    }

    public static string ToId(string mastery)
    {
        if (mastery == FRUSTRATED) return "F";
        if (mastery == UNEXERCISED) return "U";
        if (mastery == INITIATED) return "I";
        if (mastery == PARTIALLY_MASTERED) return "P";
        if (mastery == MASTERED) return "M";
        if (mastery == BURNED_OUT) return "B";

        return null;
    }

    public static string FromAttempts(float score, int numAttempts)
    {
        if (score == 1 && numAttempts >= 10) return BURNED_OUT;
        if (score >= 0.9 && numAttempts > 8) return MASTERED;
        if (score >= 0.5 && numAttempts > 6) return PARTIALLY_MASTERED;
        if (score <= 0.1 && numAttempts > 8) return FRUSTRATED;
        if (numAttempts > 0) return INITIATED;
        if (numAttempts == 0) return UNEXERCISED;

        return null;
    }
}
