using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/GameGraphic")]
public class GameGraphic : MonoBehaviour
{
    public Graphic graphic;
    public Graphic[] graphices;
    public Object[] objects;
 
    public void SetFontByIndex(int index)
    {
        int len = this.objects.Length;
        if (index < len)
        {
            Text text = this.graphic as Text;
            if (text)
            {
                text.font = this.objects[index] as Font;
            }
        }
    }

    private void SetSprites(Sprite sprite, bool setNativeSize)
    {
        int len = this.graphices.Length;
        if (len > 0)
        {
            for (int i = 0; i < len; i++)
            {
                this.SetSprite(this.graphices[i] as Image, sprite, setNativeSize);
            }
        }

    }

    private void SetSprite(Image image, Sprite sprite, bool setNativeSize)
    {
        image.sprite = sprite;
        if (setNativeSize)
        {
            image.SetNativeSize();
        }
    }

    public void SetSpriteByIndex(int index, bool setNativeSize)
    {
        int len = this.objects.Length;
        if (index < len)
        {
            if (this.graphic)
            {
                this.SetSprite(this.graphic as Image,this.objects[index] as Sprite,setNativeSize);
            }
            else
            {
                this.SetSprites(this.objects[index] as Sprite,setNativeSize);
            }

        }
    }
}
