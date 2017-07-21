using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;


public class GameEliteText : Text
{

    public string symbolStrings;
    public Vector3 charYSpace;
    public Vector3 charXSpace;
    readonly UIVertex[] m_TempVerts = new UIVertex[4];

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        if (font == null)
            return;

        // We don't care if we the font Texture changes while we are doing our Update.
        // The end result of cachedTextGenerator will be valid for this instance.
        // Otherwise we can get issues like Case 619238.
        m_DisableFontTextureRebuiltCallback = true;

        Vector2 extents = rectTransform.rect.size;

        var settings = GetGenerationSettings(extents);
        cachedTextGenerator.Populate(text, settings);

        Rect inputRect = rectTransform.rect;

        // get the text alignment anchor point for the text in local space
        Vector2 textAnchorPivot = GetTextAnchorPivot(alignment);
        Vector2 refPoint = Vector2.zero;
        refPoint.x = (textAnchorPivot.x == 1 ? inputRect.xMax : inputRect.xMin);
        refPoint.y = (textAnchorPivot.y == 0 ? inputRect.yMin : inputRect.yMax);

        // Determine fraction of pixel to offset text mesh.
        Vector2 roundingOffset = PixelAdjustPoint(refPoint) - refPoint;

        // Apply the offset to the vertices
        IList<UIVertex> verts = cachedTextGenerator.verts;
        float unitsPerPixel = 1 / pixelsPerUnit;
        //Last 4 verts are always a new line...
        int vertCount = verts.Count - 4;
        toFill.Clear();


        if (roundingOffset != Vector2.zero)
        {
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                if (tempVertsIndex == 3)
                    toFill.AddUIVertexQuad(m_TempVerts);
            }
        }
        else
        {
            int charIndex = 0;
            Vector3 charOffset = new Vector3(0, 0, 0);
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                m_TempVerts[tempVertsIndex].position += charOffset;
                if (tempVertsIndex == 3)
                {
                    
                    if (symbolStrings.IndexOf(text.Substring(charIndex, 1)) > -1)
                    {
                        m_TempVerts[0].position += charXSpace;
                        m_TempVerts[1].position += charXSpace;
                        m_TempVerts[2].position += charXSpace;
                        m_TempVerts[3].position += charXSpace;
                        charOffset += charYSpace;
                    }

                    toFill.AddUIVertexQuad(m_TempVerts);
                    charIndex = charIndex + 1;
                    //if (text.Substring(charIndex++, 1) == char1)
                    //{
                    //    m_TempVerts[tempVertsIndex].position += new Vector3(0, 10);
                    //    charOffset += new Vector3(0, 10);
                    //}
                }
            }
        }
        m_DisableFontTextureRebuiltCallback = false;
    }
}

