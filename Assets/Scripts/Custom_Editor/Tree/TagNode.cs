using UnityEngine;
using NodeEditorFramework;
#if UNITY_EDITOR
using UnityEditor;
#endif
[Node(false, "Game Node/Mechanic", false)]
public class TagNode : BaseNode
{
    public const string ID = "tagNode";
    public override string GetID { get { return ID; } }
    private Color nodeColor = new Color(1f, 0f, 0f);

    [HideInInspector]
    public string tag;
    [HideInInspector]
    public int tagIndex;

    private bool oldValue;

    public override Node Create(Vector2 pos)
    {
        TagNode node = CreateInstance<TagNode>();
        node.rect = new Rect(pos.x, pos.y, 175, 50);
        node.name = "Mechanic";

        tagIndex = 0;

        node.CreateInput("", "Bool");
        node.CreateOutput("", "Bool");

        return node;
    }

    public override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = nodeColor;

        if(rect.width != 175)
        {
            rect.width = 175;
            rect.height = 50;
        }

        base.DrawOutlinedNode();

        GUI.backgroundColor = oldColor;
    }

    public override void NodeGUI()
    {
        base.NodeGUI();

        tag = Tag.values[tagIndex];

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        for(int i = 0; i < Inputs.Count; i++)
        {
            Inputs[i].DisplayLayout();
        }
        GUILayout.EndVertical();

        #if UNITY_EDITOR
            tagIndex = EditorGUILayout.Popup("", tagIndex, Tag.values.ToArray(), GUILayout.MaxWidth(rect.width * .8f));
        #else
            GUILayout.Label(tag);
        #endif
        GUILayout.Label(Mastery.ToId(TreeManager.instance.GetTagMastery(tag)));

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
        if(Inputs[0].connection != null)
        {
            value = Inputs[0].GetValue<bool>();
        }
        else
        {
            value = true;
        }

        Outputs[0].SetValue<bool>(value);
        
        if(value != oldValue)
        {
            oldValue = value;
            UpdateTreeManager();
        }

        return true;
    }

    void UpdateTreeManager()
    {
        if(Application.isPlaying)
        {
            TreeManager.instance.UpdateNode(this);
        }
    }
}
