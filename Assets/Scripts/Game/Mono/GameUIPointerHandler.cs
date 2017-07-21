using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using LuaInterface;
using UnityEngine.UI;
using System;

public class GameUIPointerHandler :
    MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler
{

    public Action<GameObject> onClick;
    public Action<GameObject> onPointerDown;
    public Action<GameObject> onPointerEnter;
    public Action<GameObject> onPointerExit;
    public Action<GameObject> onPointerUp;

    static public GameUIPointerHandler Get(GameObject go)
    {
        GameUIPointerHandler listener = go.GetComponent<GameUIPointerHandler>();
        if (listener == null) listener = go.AddComponent<GameUIPointerHandler>();
        return listener;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null) onClick(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (onPointerDown != null) onPointerDown(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onPointerEnter != null) onPointerEnter(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (onPointerExit != null) onPointerExit(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (onPointerUp != null) onPointerUp(gameObject);
    }

}