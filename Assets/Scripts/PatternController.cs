using UnityEngine;
using System.Collections;

public class PatternController : MonoBehaviour {

    [HideInInspector]
    public int length;

    private MeshRenderer debugRenderer;
    private Material debugRendererMaterial;
    private LineRenderer startLineRenderer, endLineRenderer;

    private BoxCollider2D exitCollider;

    void Awake()
    {
        length = transform.childCount;

        SetupDebugMode();
        SetupExitCollider();
    }

    void Update()
    {
        startLineRenderer.SetVertexCount(0);
        endLineRenderer.SetVertexCount(0);

        DebugPatternLimits();
    }

    void SetupDebugMode()
    {
        GameObject debugChild = new GameObject();
        debugChild.name = "Debug";
        debugChild.transform.SetParent(this.transform);

        debugRendererMaterial = new Material(Shader.Find("Particles/Additive"));

        GameObject leftLineRenderer = new GameObject();
        leftLineRenderer.name = "Left Line Renderer";
        leftLineRenderer.transform.SetParent(debugChild.transform);

        GameObject rightLineRenderer = new GameObject();
        rightLineRenderer.name = "Right Line Renderer";
        rightLineRenderer.transform.SetParent(debugChild.transform);

        startLineRenderer = leftLineRenderer.AddComponent<LineRenderer>();
        startLineRenderer.SetColors(Color.green, Color.green);
        startLineRenderer.SetWidth(0.05f, 0.05f);
        startLineRenderer.material = debugRendererMaterial;

        endLineRenderer = rightLineRenderer.AddComponent<LineRenderer>();
        endLineRenderer.SetColors(Color.red, Color.red);
        endLineRenderer.SetWidth(0.05f, 0.05f);
        endLineRenderer.material = debugRendererMaterial;
    }

    void DebugPatternLimits()
    {
        if (!GameManager.instance.debugMode)
        {
            return;
        }

        startLineRenderer.SetVertexCount(2);
        startLineRenderer.SetPosition(0, transform.position + new Vector3(-0.5f, -1, 0));
        startLineRenderer.SetPosition(1, transform.position + new Vector3(-0.5f, 5, 0));

        endLineRenderer.SetVertexCount(2);
        endLineRenderer.SetPosition(0, transform.position + new Vector3(-0.5f + length, -1, 0));
        endLineRenderer.SetPosition(1, transform.position + new Vector3(-0.5f + length, 5, 0));
    }

    void SetupExitCollider()
    {
        exitCollider = gameObject.AddComponent<BoxCollider2D>();
        exitCollider.isTrigger = true;
        exitCollider.offset = new Vector2(length - 0.5f, 2);
        exitCollider.size = new Vector2(0.1f, 6);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.transform.parent.tag == "Player")
        {
            PatternManager.instance.patternsInfo[gameObject.name].AddAttempt(true);
            Destroy(this.GetComponent<BoxCollider2D>());
        }
    }
}
