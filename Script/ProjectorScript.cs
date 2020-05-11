using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ProjectorScript : MonoBehaviour {

    private GameObject blade;
    private RenderTexture renderTexture;
    private CommandBuffer commandBuffer;
    private Renderer renderer;
    private Shader shader;
    private Material material;

    private Vector3 curBladePos;
    private Vector3 curBladeDir;
    private Vector3 preBladePos;
    private Vector3 preBladeDir;

    private int curBladePosID;
    private int curBladeDirID;
    private int preBladePosID;
    private int preBladeDirID;

    void Initialization()
    {
        var mainTex = renderer.material.mainTexture;
        renderTexture = new RenderTexture(mainTex.width, mainTex.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        renderer.material.SetTexture("_GashMap", renderTexture);
        material.mainTexture = renderTexture;

        var c = new CommandBuffer();
        var tempBufferId = Shader.PropertyToID("_TempBuffer");
        c.GetTemporaryRT(tempBufferId, renderTexture.descriptor);
        c.Blit(renderTexture, tempBufferId);
        c.EnableShaderKeyword("BAKE_PAINT");
        c.SetRenderTarget(tempBufferId);
        c.DrawRenderer(renderer, material);
        c.DisableShaderKeyword("BAKE_PAINT");
        c.Blit(tempBufferId, renderTexture);
        c.ReleaseTemporaryRT(tempBufferId);
        commandBuffer = c;
    }
    void Bake()
    {
        Graphics.ExecuteCommandBuffer(commandBuffer);
    }

    void DrawOrbit()
    {
        Vector3[] vertices = {
        preBladePos,
        preBladePos + preBladeDir,
        curBladePos + curBladeDir,
        curBladePos
        };

        int[] triangles = { 0, 1, 2, 0, 2, 3 };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        GameObject obj = new GameObject("Plane");
        Material material = new Material(Shader.Find("Custom/Plane"));
        material.color = new Color(Random.value, Random.value, Random.value, 1);
        obj.AddComponent<MeshRenderer>();
        obj.AddComponent<MeshFilter>();
        obj.GetComponent<MeshRenderer>().material = new Material(material);
        obj.GetComponent<MeshFilter>().sharedMesh = mesh;
        Instantiate(obj, Vector3.zero, Quaternion.identity);
        
    }

    private Renderer FindRenderer()
    {
        Renderer renderer = null;
        foreach(Transform transform in GetComponentInChildren<Transform>())
        {
            renderer = transform.GetComponentInChildren<Renderer>();
            if(renderer != null)
            {
                break;
            }
        }
        return renderer;
    }

	// Use this for initialization
	void Start () {
        blade = GameObject.FindWithTag("Blade");
        renderer = FindRenderer();
        shader = Shader.Find("Unlit/DetectBladeOrbit");
        material = new Material(shader);
        material.SetFloat("BladeLength",  blade.GetComponent<Blade>().GetBladeLength());
        material.SetFloat("BladeRadius",  blade.GetComponent<Blade>().GetBladeRadius());
        preBladePosID = Shader.PropertyToID("_PreBladePos");
        preBladeDirID = Shader.PropertyToID("_PreBladeDir");
        curBladePosID = Shader.PropertyToID("_CurBladePos");
        curBladeDirID = Shader.PropertyToID("_CurBladeDir");
        Initialization();

        curBladePos = blade.transform.position;
        curBladeDir = blade.transform.forward;
        preBladePos = curBladePos;
        preBladeDir = curBladeDir;
    }
	
	// Update is called once per frame
	void Update () {
        if(Time.frameCount%5 == 0)
        {
            curBladePos = blade.transform.position;
            curBladeDir = blade.transform.forward;
            material.SetVector(curBladePosID, curBladePos);
            material.SetVector(curBladeDirID, curBladeDir);
            material.SetVector(preBladePosID, preBladePos);
            material.SetVector(preBladeDirID, preBladeDir);
            //DrawOrbit();
            if ((curBladePos - preBladePos).magnitude > 0.01f || (curBladeDir - preBladeDir).magnitude > 0.01f)
            {
                //DrawOrbit();
                Bake();
            }
            preBladePos = curBladePos;
            preBladeDir = curBladeDir;
        }
	}
}
