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

    [HideInInspector]
    public string tag;
    [HideInInspector]
    public int masteryIndex;
    [HideInInspector]
    public int masteryComparisonIndex;
    [HideInInspector]
    public string mastery;

    private BaseNode parentNode;
    private bool isConnected;

    public override Node Create(Vector2 pos)
    {
        MasteryNode node = CreateInstance<MasteryNode>();
        node.creationId = GetNextId();
        node.rect = new Rect(pos.x, pos.y, 180, 50);
        node.name = "Mastery";

        node.CreateInput("", "Bool");
        node.CreateOutput("", "Bool");

        return node;
    }

    public override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = Constants.Colors.Nodes.Mastery;

        if(rect.width != 180)
        {
            rect.width = 180;
            rect.height = 50;
        }

        base.DrawOutlinedNode();

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
            masteryIndex = Mastery.values.IndexOf(Mastery.INITIATED);
            masteryComparisonIndex = MasteryComparison.values.IndexOf(MasteryComparison.EQUAL);
            mastery = null;

            parentNode = null;
            isConnected = false;
        }
        else
        {
            switch (Inputs[0].connection.body.GetID)
            {
                case TagNode.ID:
                case PatternNode.ID:
                case PaceNode.ID:
                case PatternSpawnerNode.ID:
                case PaceSpawnerNode.ID:
                    isConnected = true;
                    parentNode = (BaseNode) Inputs[0].connection.body;
                    break;
                default:
                    isConnected = false;
                    parentNode = null;
                    RemoveConnection(Inputs[0].connection.connections[0]);
                    break;
            }
        }

        if(isConnected)
        {
            #if UNITY_EDITOR
                masteryComparisonIndex = EditorGUILayout.Popup("", masteryComparisonIndex, MasteryComparison.values.ToArray(), GUILayout.MaxWidth(rect.width * .175f));
                masteryIndex = EditorGUILayout.Popup("", masteryIndex, Mastery.values.ToArray(), GUILayout.MaxWidth(rect.width * .66f));
                mastery = Mastery.values[masteryIndex];
            #else
                GUILayout.Label(MasteryComparison.values[masteryComparisonIndex]);
                GUILayout.Label(Mastery.values[masteryIndex]);
            #endif
        }
        else
        {
            GUILayout.Label("Game node missing");
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

        value = value && CalculateMastery();
        Outputs[0].SetValue<bool>(value);

        return true;
    }

    public bool CalculateMastery()
    {
        if(parentNode != null)
        {
            return TreeManager.instance.IsMasteryResolved(this, parentNode);
        }

        return false;
    }
}
