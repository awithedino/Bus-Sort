using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ColorPalette
{
    public ColorType colorType;
    public Material carMaterial;
}

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance; 
    
    public List<ColorPalette> colorBank;

    void Awake()
    {
        Instance = this;
    }
    
    public Material GetMaterial(ColorType requestedColor)
    {
        foreach (var palette in colorBank)
        {
            if (palette.colorType == requestedColor)
                return palette.carMaterial;
        }
        return null; 
    }
}