using System.Collections.Generic;

public class Tag
{
    public static readonly string None = "None";
    public static readonly string Jump = "Jump";
    public static readonly string Double_Jump = "Double Jump";

    public static readonly List<string> values = new List<string>(new string[] {
        Tag.None,
        Tag.Jump,
        Tag.Double_Jump
    });

    public static void Add(string tag)
    {
        if(values.Contains(tag) == false)
        {
            values.Add(tag);
        }
    }
}