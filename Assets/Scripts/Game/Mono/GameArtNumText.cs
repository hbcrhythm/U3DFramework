using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class GameArtNumText : Text
{
    readonly UIVertex[] m_TempVerts = new UIVertex[4];

    protected Vector4 GetOffsetInfo(int textCount,int fontWidth,int fontHeight)
    {
        int maxLineCount = (int)this.rectTransform.rect.width / fontWidth;
        int rowCount = (textCount - 1) / maxLineCount;

        Vector4 info = Vector4.zero;
        switch (this.alignment)
        {
            case TextAnchor.LowerCenter:
                info.x = textCount > maxLineCount ? -fontWidth * maxLineCount / 2.0f : -fontWidth * textCount / 2.0f;
                info.y = 0;
                info.z = fontWidth;
                info.w = fontHeight;
                break;
            case TextAnchor.LowerLeft:
                info.x = 0;
                info.y = 0;
                info.z = fontWidth;
                info.w = fontHeight;
                break;
            case TextAnchor.LowerRight:
                info.x = -fontWidth;
                info.y = 0;
                info.z = -fontWidth;
                info.w = fontHeight;
                break;
            case TextAnchor.MiddleCenter:
                info.x = textCount > maxLineCount ? -fontWidth * maxLineCount / 2.0f : -fontWidth * textCount / 2.0f;
                info.y = textCount > maxLineCount ? fontHeight * (rowCount / 2.0f - 0.5f) : -0.5f * fontHeight;
                info.z = fontWidth;
                info.w = -fontHeight;
                break;
            case TextAnchor.MiddleLeft:
                info.x = 0;
                info.y = textCount > maxLineCount ? fontHeight * (rowCount / 2.0f - 0.5f) : -0.5f * fontHeight;
                info.z = fontWidth;
                info.w = -fontHeight;
                break;
            case TextAnchor.MiddleRight:
                info.x = -fontWidth;
                info.y = textCount > maxLineCount ? fontHeight * (rowCount / 2.0f - 0.5f) : -0.5f * fontHeight;
                info.z = -fontWidth;
                info.w = -fontHeight;
                break;
            case TextAnchor.UpperCenter:
                info.x = textCount > maxLineCount ? -fontWidth * maxLineCount / 2.0f : -fontWidth * textCount / 2.0f;
                info.y = -fontHeight;
                info.z = fontWidth;
                info.w = -fontHeight;
                break;
            case TextAnchor.UpperLeft:
                info.x = 0;
                info.y = -fontHeight;
                info.z = fontWidth;
                info.w = -fontHeight;
                break;
            case TextAnchor.UpperRight:
                info.x = -fontWidth;
                info.y = -fontHeight;
                info.z = -fontWidth;
                info.w = -fontHeight;
                break;
        }
        return info;
    }

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
            int fontWidth = (int)Mathf.Max(this.lineSpacing,1);
            int fontHeight = (int)Mathf.Max(this.fontSize, 1);
            int maxLineCount = (int)inputRect.width / fontWidth;
            Vector4 offsetInfo = this.GetOffsetInfo(vertCount / 4,fontWidth,fontHeight);
            Vector3 offset = new Vector3(offsetInfo.x,offsetInfo.y);
           
            int lineIndex = 0;
            int charIndex = 0;
            Vector3 charOffset = new Vector3(0,0,0);
            string char1 = "1";
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                m_TempVerts[tempVertsIndex].position += offset + charOffset;
                if (tempVertsIndex == 3)
                {
                    toFill.AddUIVertexQuad(m_TempVerts);
                    offset.x += offsetInfo.z;
                    if (++lineIndex %maxLineCount==0)
                    {
                        offset.x = offsetInfo.x;
                        offset.y += offsetInfo.w;

                        charOffset = new Vector3(0, 0, 0);
                    }

                    if (text.Substring(charIndex++, 1) == char1)
                    {
                        charOffset -= new Vector3(4, 0);
                    }
                }
            }
        }
        m_DisableFontTextureRebuiltCallback = false;
    }
}
