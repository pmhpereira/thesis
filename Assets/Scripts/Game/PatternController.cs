using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class PatternController : MonoBehaviour
{
    [HideInInspector]
    public float length;

    private MeshRenderer debugRenderer;
    private Material debugRendererMaterial;
    private LineRenderer startLineRenderer, endLineRenderer;

    [HideInInspector]
    public BoxCollider2D startCollider, endCollider;

    private Text text;

    private float lineWidth = 0.05f;
    private float colliderWidth = 0.1f;

    [HideInInspector]
    public bool isRecordingPlayer;
    [HideInInspector]
    public List<PlayerState> playerStates;

    [HideInInspector]
    public bool hasPlayerCollided;

    [HideInInspector]
    public int paceIndex;

    [HideInInspector]
    public int preSpacing, postSpacing;

    void Awake()
    {
        foreach (Transform child in transform)
        {
            float childSize = child.position.x + child.localScale.x;
            if (childSize > length)
            {
                length = childSize;
            }
        }

        playerStates = new List<PlayerState>();
    }

    void Update()
    {
        if (startLineRenderer != null)
        {
            startLineRenderer.SetVertexCount(0);
            endLineRenderer.SetVertexCount(0);

            DebugPatternLimits();
            DebugPatternInfo();
        }

        if (isRecordingPlayer)
        {
            RecordPlayer();
        }
    }

    public void SetupDebugMode()
    {
        GameObject debugChild = new GameObject();
        debugChild.name = "Debug";
        debugChild.transform.SetParent(this.transform);
        debugChild.transform.localPosition = Vector3.zero;

        debugRendererMaterial = new Material(Shader.Find("Particles/Additive"));

        GameObject leftLineRenderer = new GameObject();
        leftLineRenderer.name = "Left Line Renderer";
        leftLineRenderer.transform.SetParent(debugChild.transform);

        GameObject rightLineRenderer = new GameObject();
        rightLineRenderer.name = "Right Line Renderer";
        rightLineRenderer.transform.SetParent(debugChild.transform);

        startLineRenderer = leftLineRenderer.AddComponent<LineRenderer>();
        startLineRenderer.SetColors(Color.green, Color.green);
        startLineRenderer.SetWidth(lineWidth, lineWidth);
        startLineRenderer.material = debugRendererMaterial;

        endLineRenderer = rightLineRenderer.AddComponent<LineRenderer>();
        endLineRenderer.SetColors(Color.red, Color.red);
        endLineRenderer.SetWidth(lineWidth, lineWidth);
        endLineRenderer.material = debugRendererMaterial;

        Canvas canvas = debugChild.AddComponent<Canvas>();
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();

        Vector3 canvasPosition = new Vector3();
        canvasPosition.x = length / 2 - 0.5f;
        canvasPosition.z = -1;
        canvasRectTransform.localPosition = canvasPosition;
        canvasRectTransform.sizeDelta = new Vector2(length * 100, 100);
        canvasRectTransform.localScale /= 100;

        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        text = debugChild.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 40;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
    }

    void DebugPatternLimits()
    {
        if (!GameManager.instance.debugMode)
        {
            return;
        }

        startLineRenderer.SetVertexCount(2);
        startLineRenderer.SetPosition(0, transform.position + new Vector3(-0.5f - lineWidth / 2, -1, 0));
        startLineRenderer.SetPosition(1, transform.position + new Vector3(-0.5f - lineWidth / 2, 5, 0));

        endLineRenderer.SetVertexCount(2);
        endLineRenderer.SetPosition(0, transform.position + new Vector3(length - 0.5f + lineWidth / 2, -1, 0));
        endLineRenderer.SetPosition(1, transform.position + new Vector3(length - 0.5f + lineWidth / 2, 5, 0));
    }

    public void SetupBorderColliders()
    {
        GameObject collider = new GameObject("Start Collider");
        collider.transform.SetParent(this.transform);
        collider.transform.localPosition = Vector3.zero;
        collider.AddComponent<PatternCheckpoint>();

        startCollider = collider.AddComponent<BoxCollider2D>();
        startCollider.isTrigger = true;
        startCollider.size = new Vector2(colliderWidth, 6);
        startCollider.offset = new Vector2(-0.5f - colliderWidth / 2, 2);

        collider = new GameObject("Exit Collider");
        collider.transform.SetParent(this.transform);
        collider.transform.localPosition = Vector3.zero;
        collider.AddComponent<PatternCheckpoint>();

        endCollider = collider.AddComponent<BoxCollider2D>();
        endCollider.isTrigger = true;
        endCollider.size = new Vector2(colliderWidth, 6);
        endCollider.offset = new Vector2(length - 0.5f + colliderWidth / 2, 2);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player" && !hasPlayerCollided)
        {
            hasPlayerCollided = true;

            AddAttempt(false);
        }
    }

    public void AddAttempt(bool attempt)
    {
        PatternManager.instance.patternsInfo[name].AddAttempt(attempt);

        List<string> tags = TagsManager.instance.PlayerStateToTags(playerStates.ToArray());
        foreach (string tag in tags)
        {
            TagsManager.instance.tagsInfo[tag].AddAttempt(attempt);
        }

        if (paceIndex >= 0)
        {
            PaceManager.instance.pacesInfo[((PaceNode)(TreeManager.instance.paceNodes[paceIndex])).paceName].AddAttempt(attempt);
        }
    }

    void DebugPatternInfo()
    {
        if (!GameManager.instance.debugMode)
        {
            text.text = "";
            return;
        }

        string patternName = transform.name;
        string patternScores = PatternManager.instance.patternsInfo[transform.name].GetScoresString();
        text.text = "\n" + patternName + "\n" + patternScores;
    }

    void RecordPlayer()
    {
        PlayerState[] currentPlayerStates = PlayerController.instance.ResolveState();

        foreach (PlayerState state in currentPlayerStates)
        {
            if (!playerStates.Contains(state))
            {
                playerStates.Add(state);
            }
        }
    }

    public void SetPaceIndex(int paceIndex)
    {
        this.paceIndex = paceIndex;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PatternController))]
public class PatternControllerEditor : Editor
{
    private PatternController controller;

    public override void OnInspectorGUI()
    {
        if(controller == null)
        {
            controller = (PatternController) target;
        }

        DrawDefaultInspector();

        if(controller.transform.parent.name == "Generator")
        {
            controller.preSpacing = EditorGUILayout.IntField("Pre-spacing", controller.preSpacing);
            controller.postSpacing = EditorGUILayout.IntField("Post-spacing", controller.postSpacing);

            controller.preSpacing = Mathf.Max(0, controller.preSpacing);
            controller.postSpacing = Mathf.Max(0, controller.postSpacing);
        }
    }
}
#endif