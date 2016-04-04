using UnityEngine;

public static class Constants
{
    public static class Colors
    {
        public static class Nodes
        {
            // Boolean nodes
            public static readonly Color Input = new Color(1f, 1f, 1f);

            public static readonly Color Not = new Color(0f, .50f, 1f);
            public static readonly Color Or = new Color(0f, .75f, 1f);
            public static readonly Color And = new Color(0f, 1f, 1f);

            public static readonly Color Memory = new Color(1f, .75f, 1f);

            // Game nodes
            public static readonly Color Mastery = new Color(1f, 0f, 0f);
            public static readonly Color Timer = new Color(1f, 0f, 0f);

            public static readonly Color Tag = new Color(.75f, .25f, .75f);
            public static readonly Color TagSpawner = new Color(.5f, .0f, .5f);

            public static readonly Color Pattern = new Color(.2f, .7f, .2f);
            public static readonly Color PatternSpawner = new Color(0f, .5f, 0f);

            public static readonly Color Pace = new Color(1f, .4f, 0f);
            public static readonly Color PaceSpawner = new Color(.9f, .3f, 0f);

            public static readonly Color Pointer = new Color(.3f, .3f, .3f);
        }
    }
}
