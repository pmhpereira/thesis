using UnityEngine;
using System;
using System.Collections.Generic;
using NodeEditorFramework;
using NodeEditorFramework.Resources;

public class RuntimeNodeEditor : MonoBehaviour 
{
	public string CanvasString;
	public NodeCanvas canvas;
	public NodeEditorState state;

	public void Start () 
	{
		if ((canvas == null || state == null))
		{
			if (!string.IsNullOrEmpty (CanvasString))
				LoadNodeCanvas (CanvasString);
			else
				Debug.LogWarning ("Please use one option to select a canvas!");
		}
		else
			NodeEditor.RecalculateAll (canvas);

		NodeEditor.initiated = false;
	}

    public enum Splitscreen
    {
        None,
        Vertical,
    }

    public Splitscreen splitscreen = Splitscreen.None;

    public void ToogleSplitscreen()
    {
        switch(splitscreen)
        {
            case Splitscreen.None:
                splitscreen = Splitscreen.Vertical;
                break;
            case Splitscreen.Vertical:
                splitscreen = Splitscreen.None;
                break;
        }

        UpdateSplitscreen();
    }

    void UpdateSplitscreen()
    {
        Rect rect = new Rect();

        if(splitscreen == Splitscreen.None)
        {
            rect.width = 0;
            rect.height = 0;
        }
        else if(splitscreen == Splitscreen.Vertical)
        {
            rect.width = Screen.width;
            rect.height = Screen.height / 2;
        }

        rect.x = Screen.width - rect.width;
        rect.y = Screen.height - rect.height;

        if(state != null)
        {
		    state.canvasRect = rect;
        }
    }

	public void OnGUI ()
	{
		if (canvas != null && state != null) 
		{
			NodeEditor.checkInit ();
			if (NodeEditor.InitiationError) 
			{
				GUILayout.Label ("Initiation failed! Check console for more information!");
				return;
			}

			try
			{
				//GUI.BeginGroup (rootRect, NodeEditorGUI.nodeSkin.box);

				//GUILayout.FlexibleSpace ();

				//GUI.BeginGroup (subRootRect, NodeEditorGUI.nodeSkin.box);
                UpdateSplitscreen();
				NodeEditor.DrawCanvas (canvas, state);

				//GUI.EndGroup ();

				//GUI.EndGroup ();
			}
			catch (UnityException e)
			{ // on exceptions in drawing flush the canvas to avoid locking the ui.
				NewNodeCanvas ();
				Debug.LogError ("Unloaded Canvas due to exception in Draw!");
				Debug.LogException (e);
			}
		}
	}

	public void LoadNodeCanvas (string path) 
	{
        // Load the NodeCanvas
        ResourceManager.Init(NodeEditor.editorPath + "Resources/");

        canvas = NodeEditor.LoadNodeCanvas (path);
		if (canvas == null)
			canvas = ScriptableObject.CreateInstance<NodeCanvas> ();
		
		// Load the associated MainEditorState
		List<NodeEditorState> editorStates = NodeEditor.LoadEditorStates (path);
		if (editorStates.Count == 0)
			state = ScriptableObject.CreateInstance<NodeEditorState> ();
		else 
		{
			state = editorStates.Find (x => x.name == "MainEditorState");
			if (state == null)
				state = editorStates[0];
		}
		
		NodeEditor.RecalculateAll (canvas);
	}

	public void NewNodeCanvas () 
	{
		// New NodeCanvas
		canvas = ScriptableObject.CreateInstance<NodeCanvas> ();;
		// New NodeEditorState
		state = ScriptableObject.CreateInstance<NodeEditorState> ();
		state.canvas = canvas;
		state.name = "MainEditorState";
	}
}
