public sealed class PlayerState
{
    public readonly string name;
    private readonly int value;

    public static readonly PlayerState IDLING = new PlayerState(0, "Idling");
    public static readonly PlayerState JUMPING = new PlayerState(1, "Jumping");
    public static readonly PlayerState DOUBLE_JUMPING = new PlayerState(2, "Double Jumping");
    public static readonly PlayerState SLIDING = new PlayerState(3, "Sliding");
    public static readonly PlayerState COLLIDING = new PlayerState(-1, "Colliding");
    public static readonly PlayerState FALLING = new PlayerState(-2, "Falling");

    private PlayerState(int value, string name)
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

    public static PlayerState FromId(string id)
    {
        switch (id)
        {
            case "I": return IDLING;
            case "J": return JUMPING;
            case "DJ": return DOUBLE_JUMPING;
            case "S": return SLIDING;
            case "C": return COLLIDING;
            case "F": return FALLING;

            default: return null;
        }
    }
}
