using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.EventSystems;
using System;

public class EmojiAtlasInfo
{
    public string pattern;
    public bool placeholder;
}

public class GameEmojiText : Text, IPointerClickHandler
{
    public class EmojiInfo
    {
        public float x;
        public float y;
        public float width;
        public float height;
    }

    public class HrefInfo
    {
        public string link;
        public string text;
        public int startIndex;
        public int maxIndex;
        public List<Rect> rects = new List<Rect>();
    }

    static Dictionary<string, Dictionary<string, EmojiInfo>> atlasDataDic = new Dictionary<string, Dictionary<string, EmojiInfo>>();
    static Dictionary<string, EmojiAtlasInfo> atlasInfoDic = new Dictionary<string, EmojiAtlasInfo>();

    public static void InitFaceConfig(string atlasName, string atlasData, EmojiAtlasInfo atlasInfo)
    {
        Dictionary<string, EmojiInfo> atlasDatas = null;

        if (atlasDataDic.TryGetValue(atlasName, out atlasDatas) == false)
        {
            atlasDatas = new Dictionary<string, EmojiInfo>();
            atlasDataDic[atlasName] = atlasDatas;
            atlasInfoDic[atlasName] = atlasInfo;

            var lineList = atlasData.Split('\n');
            var firstLine = lineList[0].Split(',');
            float width = float.Parse(firstLine[0]);
            float height = float.Parse(firstLine[1]);

            for (int i = 1, count = lineList.Length; i < count; i++)
            {
                var line = lineList[i];
                var rowValues = line.Split(',');
                if (rowValues.Length >= 5)
                {
                    string key = rowValues[0];

                    EmojiInfo info = new EmojiInfo();
                    info.x = float.Parse(rowValues[1]) / width;
                    info.y = (height - float.Parse(rowValues[2])) / height;//ngui打包的图集，（0，0）点在左上角，uv的（0，0）点应该在左下角
                    info.width = float.Parse(rowValues[3]) / width;
                    info.height = float.Parse(rowValues[4]) / height;

                    atlasDatas[key] = info;
                }
            }
        }
    }

    public float GetPreferredWidth(float scaleFactor)
    {
        var settings = GetGenerationSettings(Vector2.zero);
        settings.scaleFactor = scaleFactor;
        return cachedTextGeneratorForLayout.GetPreferredWidth(m_Text, settings) / scaleFactor;
    }

    void Awake()
    {
        var name = gameObject.name;
        if (atlasDataDic.TryGetValue(name,out mEmojiIndex) == false)
        {
            return;
        }

        //mEmojiIndex = atlasDataDic[name];
        mEmojiAtlasInfo = atlasInfoDic[name];
        mEmojiDic = new Dictionary<int, EmojiInfo>();
        mHrefInfoDic = new Dictionary<int, HrefInfo>();

        //SetEmojiText("[03]这是一个支持表情系统[03]的Emo[12]，运行时支持动态表情");
    }

    public Action<string> mClickHref;
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 lp;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out lp);

        foreach (var kvp in mHrefInfoDic)
        {
            var hrefInfo = kvp.Value;
            var boxes = hrefInfo.rects;
            for (var i = 0; i < boxes.Count; ++i)
            {
                if (boxes[i].Contains(lp))
                {
                    if (mClickHref != null)
                    {
                        mClickHref(hrefInfo.link);
                    }
                    return;
                }
            }
        }
    }

    //超链正则
    private static readonly Regex s_HrefRegex = new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);

    EmojiAtlasInfo mEmojiAtlasInfo;
    Dictionary<string, EmojiInfo> mEmojiIndex;
    Dictionary<int, EmojiInfo> mEmojiDic;
    Dictionary<int, HrefInfo> mHrefInfoDic;
    const string cPlaceholderChar = "正 ";
    public void SetEmojiText(string emojiText)
    {
        mHrefInfoDic.Clear();
        mEmojiDic.Clear();
        int lastSubIndex = 0;

        StringBuilder result = new StringBuilder();
        if (emojiText.IndexOf("href") != -1)
        {
            var hrefMatches = s_HrefRegex.Matches(emojiText);
            Match hrefMatchItem = null;
            for (int i = 0; i < hrefMatches.Count; i++)
            {
                hrefMatchItem = hrefMatches[i];

                HrefInfo info = new HrefInfo();
                info.link = hrefMatchItem.Groups[1].Value;
                info.text = hrefMatchItem.Groups[2].Value;

                if (lastSubIndex != hrefMatchItem.Index)
                {
                    result.Append(emojiText.Substring(lastSubIndex, hrefMatchItem.Index - lastSubIndex));
                }

                info.startIndex = result.Length;
                info.maxIndex = info.startIndex + info.text.Length - 1;
                mHrefInfoDic[info.startIndex] = info;
                result.Append(info.text);
                lastSubIndex = hrefMatchItem.Index + hrefMatchItem.Length;
            }

            if (hrefMatchItem == null)
            {
                text = emojiText;
            }
            else if (lastSubIndex != emojiText.Length - hrefMatchItem.Length)
            {
                result.Append(emojiText.Substring(lastSubIndex));
                emojiText = result.ToString();
                result = new StringBuilder();
            }

            lastSubIndex = 0;
        }

        MatchCollection matches = Regex.Matches(emojiText, mEmojiAtlasInfo.pattern);

        Match matchItem = null;
        for (int i = 0; i < matches.Count; i++)
        {
            EmojiInfo info;
            if (mEmojiIndex.TryGetValue(matches[i].Value, out info))
            {
                matchItem = matches[i];

                if (mEmojiAtlasInfo.placeholder)
                {
                    if (lastSubIndex != matchItem.Index)
                    {
                        result.Append(emojiText.Substring(lastSubIndex, matchItem.Index - lastSubIndex));
                    }
                    mEmojiDic.Add(result.Length, info);
                    result.Append(cPlaceholderChar);
                    lastSubIndex = matchItem.Index + matchItem.Length;
                }
                else
                {
                    mEmojiDic.Add(matchItem.Index, info);
                    matchItem = null;
                }
            }
        }

        if (matchItem == null)
        {
            text = emojiText;
            return;
        }
        else if (lastSubIndex != emojiText.Length - matchItem.Length)
        {
            result.Append(emojiText.Substring(lastSubIndex));
        }

        text = result.ToString();
    }

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
        refPoint.x = Mathf.Lerp(inputRect.xMin, inputRect.xMax, textAnchorPivot.x);
        refPoint.y = Mathf.Lerp(inputRect.yMin, inputRect.yMax, textAnchorPivot.y);

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
            EmojiInfo info;
            int hrefIndex = 0;
            HrefInfo hrefInfo = null;

            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                if (tempVertsIndex == 3)
                {
                    int index = i / 4;

                    if (hrefInfo != null || mHrefInfoDic.TryGetValue(hrefIndex, out hrefInfo))
                    {
                        if (hrefInfo.startIndex == hrefIndex)
                        {
                            hrefInfo.rects.Clear();
                        }

                        var bound = new Bounds(m_TempVerts[0].position, Vector3.zero);
                        bound.Encapsulate(m_TempVerts[1].position);
                        bound.Encapsulate(m_TempVerts[2].position);
                        bound.Encapsulate(m_TempVerts[3].position);

                        hrefInfo.rects.Add(new Rect(bound.min, bound.size));

                        if (hrefIndex >= hrefInfo.maxIndex)
                        {
                            hrefInfo = null;
                        }
                    }

                    if (mEmojiDic.TryGetValue(index, out info))
                    {
                        hrefIndex += 3;
                        //float offset = 4;

                        //m_TempVerts[0].position = m_TempVerts[0].position + new Vector3(-offset, offset);
                        //m_TempVerts[1].position = m_TempVerts[1].position + new Vector3(offset, offset);
                        //m_TempVerts[2].position = m_TempVerts[2].position + new Vector3(offset, -offset);
                        //m_TempVerts[3].position = m_TempVerts[3].position + new Vector3(-offset, -offset);

                        //m_TempVerts[0].uv0 = new Vector2(4, 4);
                        //m_TempVerts[1].uv0 = new Vector2(4, 4);
                        //m_TempVerts[2].uv0 = new Vector2(4, 4);
                        //m_TempVerts[3].uv0 = new Vector2(4, 4);

                        m_TempVerts[0].uv1 = new Vector2(info.x, info.y);
                        m_TempVerts[1].uv1 = new Vector2(info.x + info.width, info.y);
                        m_TempVerts[2].uv1 = new Vector2(info.x + info.width, info.y - info.height);
                        m_TempVerts[3].uv1 = new Vector2(info.x, info.y - info.height);

                        var offsetY = m_TempVerts[1].position.y + m_TempVerts[3].position.x - m_TempVerts[2].position.x;

                        int scaleOffset = 2;
                        int xOffset = 3;
                        m_TempVerts[0].position += new Vector3(-scaleOffset + xOffset, scaleOffset);
                        m_TempVerts[1].position += new Vector3(scaleOffset + xOffset, scaleOffset);
                        m_TempVerts[2].position = new Vector3(m_TempVerts[2].position.x, offsetY) + new Vector3(scaleOffset + xOffset, -scaleOffset);
                        m_TempVerts[3].position = new Vector3(m_TempVerts[3].position.x, offsetY) + new Vector3(-scaleOffset + xOffset, -scaleOffset);

                        //Debug.LogError(string.Format("{0},{1},{2},{3}", m_TempVerts[0].position, m_TempVerts[1].position, m_TempVerts[2].position, m_TempVerts[3].position));

                        //m_TempVerts[0].uv1 = new Vector2(info.x, info.y + info.height);
                        //m_TempVerts[1].uv1 = new Vector2(info.x + info.width, info.y + info.height);
                        //m_TempVerts[2].uv1 = new Vector2(info.x + info.width, info.y);
                        //m_TempVerts[3].uv1 = new Vector2(info.x, info.y);

                    }

                    toFill.AddUIVertexQuad(m_TempVerts);

                    hrefIndex++;
                }
            }

        }
        m_DisableFontTextureRebuiltCallback = false;
    }
}
