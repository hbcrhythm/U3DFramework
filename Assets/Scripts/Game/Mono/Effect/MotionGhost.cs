using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MotionGhost : MonoBehaviour
{
    public int interval = 1;
    public Renderer ghostRenderer;
    public Material realTimeRendererMaterial;
    private List<Vector3> mOffsets = new List<Vector3>();
    private Material[] mMaterials;
    private int mLastFrame = 0;
    void OnEnable()
    {
        var offsets = this.mOffsets;
        offsets.Clear();
        Vector3 pos = this.transform.position;
        for (int i = 0; i < 4; i++)
        {
            offsets.Add(pos);
        }
        this.mLastFrame = Time.frameCount;
    }
    void LateUpdate()
    {
        if (Time.frameCount - this.mLastFrame < this.interval)
        {
            return;
        }

        this.mLastFrame = Time.frameCount;
        var materials = this.mMaterials;
        if (materials == null || materials.Length == 0)
        {
            return;
        }

        var offsets = this.mOffsets;
        Vector3 pos = this.transform.position;
        foreach (var mat in materials)
        {
            mat.SetVector("_Offset0", offsets[3] - pos);
            mat.SetVector("_Offset1", offsets[2] - pos);
            mat.SetVector("_Offset2", offsets[1] - pos);
            mat.SetVector("_Offset3", offsets[0] - pos);
        }
        offsets.Add(pos);
        offsets.RemoveAt(0);
    }

    private Stack<Material> mMaterialPool = new Stack<Material>();

    private Material PopRealTimeRenderMaterial()
    {
        if (this.realTimeRendererMaterial == null) return null;
        if (this.mMaterialPool.Count == 0)
        {
            return new Material(this.realTimeRendererMaterial);
        }
        return this.mMaterialPool.Pop();
    }

    private void PushRealTimeRenderMaterial(Material mtl)
    {
        if (mtl)
        {
            this.mMaterialPool.Push(mtl);
        }
    }

    public void SetRealTimeRenderer(SkinnedMeshRenderer smr)
    {
        if (this.realTimeRendererMaterial == null) return;
        var materials = smr.materials;
        if (materials == null) return;
        int len = materials.Length;
        if (len == 0) return;

        var myMaterials = this.mMaterials;
        if (myMaterials != null)
        {
            for (int i = 0; i < myMaterials.Length; i++)
            {
                this.PushRealTimeRenderMaterial(myMaterials[i]);
            }
        }
        myMaterials = new Material[len];
        for (int i = 0; i < len; i++)
        {
            myMaterials[i] = this.PopRealTimeRenderMaterial();
            myMaterials[i].mainTexture = materials[i].mainTexture;
        }

        SkinnedMeshRenderer myRenderer = null;
        if (this.ghostRenderer)
        {
            myRenderer = (SkinnedMeshRenderer)this.ghostRenderer;
        }
        else
        {
            myRenderer = this.gameObject.AddComponent<SkinnedMeshRenderer>();
            this.ghostRenderer = myRenderer;
        }

        myRenderer.sharedMesh = smr.sharedMesh;
        myRenderer.bones = smr.bones;
        myRenderer.rootBone = smr.rootBone;
        myRenderer.materials = myMaterials;
        this.mMaterials = myMaterials;
    }
}
