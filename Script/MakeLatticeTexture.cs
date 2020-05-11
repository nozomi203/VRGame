using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeLatticeTexture : MonoBehaviour {

    [SerializeField]
    private string fileName = "Assets/Texture/lattice.asset";
    [SerializeField]
    private int textureSize = 256;
    [SerializeField]
    private float lineInterval = 16;
    [SerializeField]
    private float lineWidth = 1;
    [SerializeField]
    private Color baseColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField]
    private Color lineColor = Color.white;


    private Texture2D latticeTexture;
    private Color[] buffer;
   
	// Use this for initialization
	void Start () {
        latticeTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, true);
        latticeTexture.filterMode = FilterMode.Bilinear;

        buffer = new Color[textureSize * textureSize];

        MakeTexture();

        UnityEditor.AssetDatabase.CreateAsset(latticeTexture, fileName);
    }

    private void MakeTexture()
    {
        for(int i = 0; i < textureSize; i++)
        {
            for(int j = 0; j < textureSize; j++)
            {
                if(i%lineInterval <= lineWidth || j%lineInterval <= lineWidth)
                {
                    buffer[i * 256 + j] = lineColor;
                }
                else
                {
                    buffer[i * 256 + j] = baseColor;
                }
            }
        }

        latticeTexture.SetPixels(buffer);
        latticeTexture.Apply();
    }
}
