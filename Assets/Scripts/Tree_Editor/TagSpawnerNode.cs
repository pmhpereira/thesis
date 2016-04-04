using UnityEngine;
using NodeEditorFramework;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Node(false, "Game Node/Spawner/Mechanic")]
public class TagSpawnerNode : BaseNode
{
    public const string ID = "tagSpawnerNode";
    public override string GetID { get { return ID; } }

    [SerializeField][HideInInspector]
    public List<int> tagsIndices;
    public List<float> tagsMasteryWeights;

    private int rectDefaultHeight = 70;

    public override Node Create(Vector2 pos)
    {
        TagSpawnerNode node = CreateInstance<TagSpawnerNode>();
        node.creationId = GetNextId();
        node.rect = new Rect(pos.x, pos.y, 210, rectDefaultHeight);
        node.name = "Mechanic Spawner";

        node.CreateInput("", "Bool");
        node.CreateOutput("", "Bool");

        return node;
    }

    protected override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = Constants.Colors.Nodes.TagSpawner;

        if(tagsIndices == null)
        {
            tagsIndices = new List<int>();
            tagsMasteryWeights = new List<float>();
        }
        rect.height = rectDefaultHeight + tagsIndices.Count * 20;

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

        GUILayout.BeginVertical();
        if(tagsIndices.Count > 0)
        {
            GUILayout.Label("      Mechanic                 mW");
        }
        #if UNITY_EDITOR
        {
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 25;
            for(int i = 0; i < tagsIndices.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if(GUILayout.Button("x", GUILayout.MaxWidth(25)))
                {
                    tagsIndices.RemoveAt(i);
                    tagsMasteryWeights.RemoveAt(i);

                    if(i == tagsIndices.Count)
                    {
                        break;
                    }
                }

                if(tagsIndices.Count > 0)
                {
                    tagsIndices[i] = EditorGUILayout.Popup("", tagsIndices[i], TagsManager.instance.tagsName.ToArray(), GUILayout.MaxWidth(rect.width - 40));
                    tagsMasteryWeights[i] = EditorGUILayout.FloatField("", tagsMasteryWeights[i], GUILayout.MaxWidth(40));
                    tagsMasteryWeights[i] = Mathf.Max(0, tagsMasteryWeights[i]);
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Mechanic"))
            {
                tagsIndices.Add(0);
                tagsMasteryWeights.Add(1);
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
        #else
        {
            for(int i = 0; i < tagsIndices.Count; i++)
            {
                GUILayout.Label("".PadRight(4) + TagsManager.instance.tagsName[tagsIndices[i]].PadRight(19) + tagsMasteryWeights[i]);
            }
        }
        #endif
        GUILayout.EndVertical();

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
        value = Inputs[0].GetValue<bool>();
        Outputs[0].SetValue<bool>(value);

        return true;
    }
}
