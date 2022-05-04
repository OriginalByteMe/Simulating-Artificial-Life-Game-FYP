using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridOverlay : MonoBehaviour
{
    private Material lineMaterial;

    public bool showMain = true;
    public bool showSub = false;

    public int gridSizeX;
    public int gridSizeY;

    public float startX;
    public float startY;
    public float startZ;

    public Color mainColour = new Color(0f, 1f, 0f, 1f);
    public Color subColour = new Color(0f, 0.5f, 0f, 1f);

    // Small division
    public float smallStep;
    // Large division
    public float largeStep;

    void CreateLineMaterial()
    {
        if(!lineMaterial)
        {
            var shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);

            lineMaterial.hideFlags = HideFlags.HideAndDontSave;

            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlen", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstAlpha);

            // Turn off depth writing
            lineMaterial.SetInt("_ZWrite", 0);

            // Turn off backface culling
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        }
    }

    private void OnDisable()
    {
        DestroyImmediate(lineMaterial);
    }

    private void OnPostRender()
    {
        CreateLineMaterial();

        lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);

        if(showSub)
        {
            GL.Color(subColour);

            for(float y = 0; y<= gridSizeY; y+= smallStep)
            {
                GL.Vertex3(startX, startY + y, startZ);
                GL.Vertex3(startX + gridSizeX, startY + y, startZ);
            }

            for(float x = 0; x<= gridSizeX; x+= smallStep)
            {
                GL.Vertex3(startX + x, startY, startZ);
                GL.Vertex3(startX + x, startY + gridSizeY, startZ);
            }
        }

        if(showMain)
        {
            GL.Color(mainColour);
            for (float y = 0; y <= gridSizeY; y += largeStep)
            {
                GL.Vertex3(startX, startY + y, startZ);
                GL.Vertex3(startX + gridSizeX, startY + y, startZ);
            }

            for (float x = 0; x <= gridSizeX; x += largeStep)
            {
                GL.Vertex3(startX + x, startY, startZ);
                GL.Vertex3(startX + x, startY + gridSizeY, startZ);
            }
        }

        GL.End();
    }
}
