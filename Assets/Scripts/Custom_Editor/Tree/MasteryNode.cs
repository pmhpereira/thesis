using UnityEngine;
using NodeEditorFramework;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Node(false, "Game Node/Mastery", false)]
public class MasteryNode : BaseNode
{
    public const string ID = "masteryNode";
    public override string GetID { get { return ID; } }
    private Color nodeColor = new Color(1f, .4f, 0f);

    [HideInInspector]
    public string tag;
    [HideInInspector]
    public TagNode tagNode;
    [HideInInspector]
    public int masteryIndex;
    [HideInInspector]
    public PatternNode patternNode;
    [HideInInspector]
    public string mastery;

    public override Node Create(Vector2 pos)
    {
        MasteryNode node = CreateInstance<MasteryNode>();
        node.rect = new Rect(pos.x, pos.y, 150, 50);
        node.name = "Mastery";

        node.CreateInput("", "Bool");
        node.CreateOutput("", "Bool");

        return node;
    }

    public override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = nodeColor;

        base.DrawNode();

        GUI.backgroundColor = oldColor;
    }

    public override void NodeGUI()
    {
        base.NodeGUI();

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        for (int i = 0; i < Inputs.Count; i++)
        {
            Inputs[i].DisplayLayout();
        }
        GUILayout.EndVertical();

        if (Inputs[0].connection == null)
        {
            patternNode = null;
            tagNode = null;
            masteryIndex = 0;
            mastery = null;
            GUILayout.Label("Game node missing");
        }
        else if (Inputs[0].connection.body.GetID == PatternNode.ID || Inputs[0].connection.body.GetID == TagNode.ID)
        {
            if(Inputs[0].connection.body.GetID == PatternNode.ID)
            {
                patternNode = (PatternNode)Inputs[0].connection.body;
                tagNode = null;
            }
            else
            {
                tagNode = (TagNode)Inputs[0].connection.body;
                patternNode = null;
            }

            #if UNITY_EDITOR
                masteryIndex = EditorGUILayout.Popup("", masteryIndex, Mastery.values.ToArray(), GUILayout.MaxWidth(rect.width * .8f));
                mastery = Mastery.values[masteryIndex];
            #else
                GUILayout.Label(Mastery.values[masteryIndex]);
            #endif
        }
        else
        {
            RemoveConnection(Inputs[0].connection.connections[0]);
        }

        GUILayout.BeginVertical();
        for (int i = 0; i < Outputs.Count; i++)
        {
            Outputs[i].DisplayLayout();
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
    }

    public override bool Calculate()
    {
        if (Inputs[0].connection != null)
        {
            value = Inputs[0].GetValue<bool>();
        }

        bool realValue = value && CalculateMastery();
        Outputs[0].SetValue<bool>(realValue);

        return true;
    }

    public bool CalculateMastery()
    {
        if(tagNode != null)
        {
            return TreeManager.instance.IsMasteryResolved(this, tagNode);
        }
        else if(patternNode != null)
        {
            return TreeManager.instance.IsMasteryResolved(this, patternNode);
        }

        return false;
    }
}
