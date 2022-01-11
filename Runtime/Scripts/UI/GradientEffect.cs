using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientEffect : MonoBehaviour
{
    private RawImage backgroundRawImage;
    private Texture2D backgroundTexture;

    void Awake()
    {
        backgroundRawImage = GetComponent<RawImage>();
        backgroundTexture = new Texture2D(2, 1);
        backgroundTexture.wrapMode = TextureWrapMode.Clamp;
        backgroundTexture.filterMode = FilterMode.Bilinear;
        SetGradientColors(Color.black, Color.white);
    }

    public void SetGradientColors(Color color1, Color color2)
    {
        backgroundTexture.SetPixels(new Color[] { color1, color2 });
        backgroundTexture.Apply();
        backgroundRawImage.texture = backgroundTexture;
    }
}