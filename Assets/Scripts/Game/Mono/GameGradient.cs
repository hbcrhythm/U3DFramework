using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/GameGradient")]
public class GameGradient : BaseMeshEffect
{
    [SerializeField]
    private Color32 topColor = Color.white;

    [SerializeField]
    private Color32 bottomColor = Color.black;

    private List<UIVertex> mVertexList;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
        {
            return;
        }

        if (mVertexList == null)
        {
            mVertexList = new List<UIVertex>();
        }

        vh.GetUIVertexStream(mVertexList);
        ApplyGradient(mVertexList);

        vh.Clear();
        vh.AddUIVertexTriangleStream(mVertexList);
    }

    private void ApplyGradient(List<UIVertex> vertexList)
    {
        int end = vertexList.Count;
        if (end <= 0)
            return;

        for (int i = 0; i < end; )
        {
            ChangeColor(vertexList, i++, topColor);
            ChangeColor(vertexList, i++, topColor);
            ChangeColor(vertexList, i++, bottomColor);
            ChangeColor(vertexList, i++, bottomColor);
            ChangeColor(vertexList, i++, bottomColor);
            ChangeColor(vertexList, i++, topColor);
        }
    }

    private void ChangeColor(List<UIVertex> vertexList,int index,Color color)
    {
        var temp = vertexList[index];
        temp.color = color;
        vertexList[index] = temp;
    }
}