using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.EventSystems;
using System;

public class GameUnderlineText : Text
{
    readonly UIVertex[] m_TempVerts = new UIVertex[4];
    readonly UIVertex[] m_underlineVerts = new UIVertex[4];

    public bool showUnderline = false;
    private Color underLineColor;

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
            if (showUnderline)
            {
                for (int i = vertCount - 4; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_underlineVerts[tempVertsIndex] = verts[i];
                    m_underlineVerts[tempVertsIndex].position *= unitsPerPixel;
                    m_underlineVerts[tempVertsIndex].position += new Vector3(0, -3);
                    m_underlineVerts[tempVertsIndex].color = underLineColor;
                }

                for (int i = 0; i < vertCount - 4; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;

                    if (tempVertsIndex == 3)
                    {
                        toFill.AddUIVertexQuad(m_TempVerts);
                    }
                }
                AddUnderline(toFill, (verts[0].position * unitsPerPixel).x, (verts[vertCount - 4].position* unitsPerPixel).x);
            }
            else
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
        }
        m_DisableFontTextureRebuiltCallback = false;
    }

    void AddUnderline(VertexHelper toFill, float startX, float endX)
    {
        float offsetX = m_underlineVerts[0].position.x - startX;
        //float len = m_underlineVerts[1].position.x - m_underlineVerts[0].position.x;

        while (startX < endX)
        {
            for (int i = 0; i < m_underlineVerts.Length;i++ )
            {
                m_underlineVerts[i].position -= new Vector3(offsetX, 0);
            }

            toFill.AddUIVertexQuad(m_underlineVerts);
            startX = m_underlineVerts[1].position.x;
            offsetX = m_underlineVerts[0].position.x - startX + 2f;
        }
    }

    public void SetUnderLineColor(Color color)
    {
        underLineColor = color;
    }
}
