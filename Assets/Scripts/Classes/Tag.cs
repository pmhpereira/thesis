using System.Collections.Generic;

public class Tag
{
    public static readonly string Jump = "Jump";
    public static readonly string Double_Jump = "Double Jump";
    public static readonly string Slide = "Slide";
    public static readonly string Dash = "Dash";

    public static readonly List<string> values = new List<string>(new string[] {
        Tag.Jump,
        Tag.Double_Jump,
        Tag.Slide,
        Tag.Dash
    });

    public static void Add(string tag)
    {
        if(values.Contains(tag) == false)
        {
            values.Add(tag);
        }
    }
}