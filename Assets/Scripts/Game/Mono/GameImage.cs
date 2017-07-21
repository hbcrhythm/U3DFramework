using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/GameImage")]
public class GameImage : MonoBehaviour
{
    public Image image;
    public Sprite[] sprites;

    private Dictionary<string,Sprite> mSpritesDic;

    void Init()
    {
        if (mSpritesDic != null)
            return;

        mSpritesDic = new Dictionary<string, Sprite>();

        for (int i = 0, count = sprites.Length; i < count; i++)
        {
            Sprite s = sprites[i];
            mSpritesDic[s.name] = s;
        }
    }

    public void SetSprite(string spriteName)
    {
        Init();
        image.sprite = mSpritesDic[spriteName];
    }

    public void SetImageSprite(Image image,string spriteName)
    {
        Init();
        image.sprite = mSpritesDic[spriteName];
    }
}
