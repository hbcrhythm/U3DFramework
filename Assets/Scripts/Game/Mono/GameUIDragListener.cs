using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using LuaInterface;
using UnityEngine.UI;
using System;

public class GameUIDragListener : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
   
    public Action<GameObject> onBeginDrag;
    public Action<GameObject, float,float> onDrag;
    public Action<GameObject> onEndDrag;

    static public GameUIDragListener Get(GameObject go)
    {
        GameUIDragListener listener = go.GetComponent<GameUIDragListener>();
        if (listener == null) listener = go.AddComponent<GameUIDragListener>();
        return listener;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (onBeginDrag != null) onBeginDrag(gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null) onDrag(gameObject, eventData.delta.x,eventData.delta.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null) onEndDrag(gameObject);
    }
}