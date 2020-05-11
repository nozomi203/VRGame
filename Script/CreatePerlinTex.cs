using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreatePerlinTex : MonoBehaviour
{
    [SerializeField]
    private float scale = 1f;
    // Start is called before the first frame update
    void Start()
    {
        CreateTexture(scale);
    }

    private void CreateTexture(float scale = 1f, int size = 256)
    {
        Texture2D tex = new Texture2D(size, size);

        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                float val = Mathf.PerlinNoise((float)i / size * scale, (float)j / size * scale);
                Color col = new Color(val, val, val);
                tex.SetPixel(i, j, col);
            }
        }

        AssetDatabase.CreateAsset(tex, "Assets/Image/perlintex.asset");
    }
}
