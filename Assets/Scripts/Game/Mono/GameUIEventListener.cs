using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using LuaInterface;
using UnityEngine.UI;
using System;

public class GameUIEventListener : EventTrigger
{
   
    public Action<GameObject> onBeginDrag;
    public Action<GameObject> onCancel;
    public Action<GameObject> onDeselect;
    public Action<GameObject, PointerEventData> onDrag;
    public Action<GameObject> onDrop;
    public Action<GameObject> onEndDrag;
    public Action<GameObject> onInitializePotentialDrag;
    public Action<GameObject> onMove;
    public Action<GameObject> onClick;
    public Action<GameObject> onPointerDown;
    public Action<GameObject> onPointerEnter;
    public Action<GameObject> onPointerExit;
    public Action<GameObject> onPointerUp;
    public Action<GameObject> onScroll;
    public Action<GameObject> onSelect;
    public Action<GameObject> onSubmit;
    public Action<GameObject> onUpdateSelected;

    static public GameUIEventListener Get(GameObject go)
    {
        GameUIEventListener listener = go.GetComponent<GameUIEventListener>();
        if (listener == null) listener = go.AddComponent<GameUIEventListener>();
        return listener;
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (onBeginDrag != null) onBeginDrag(gameObject);
    }
    public override void OnCancel(BaseEventData eventData)
    {
        if (onCancel != null) onCancel(gameObject);
    }
    public override void OnDeselect(BaseEventData eventData)
    {
        if (onDeselect != null) onDeselect(gameObject);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null) onDrag(gameObject, eventData);
    }
    public override void OnDrop(PointerEventData eventData)
    {
        if (onDrop != null) onDrop(gameObject);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null) onEndDrag(gameObject);
    }
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (onInitializePotentialDrag != null) onInitializePotentialDrag(gameObject);
    }
    public override void OnMove(AxisEventData eventData)
    {
        if (onMove != null) onMove(gameObject);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null) onClick(gameObject);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (onPointerDown != null) onPointerDown(gameObject);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onPointerEnter != null) onPointerEnter(gameObject);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (onPointerExit != null) onPointerExit(gameObject);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onPointerUp != null) onPointerUp(gameObject);
    }
    public override void OnScroll(PointerEventData eventData)
    {
        if (onScroll != null) onPointerUp(gameObject);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect(gameObject);
    }
    public override void OnSubmit(BaseEventData eventData)
    {
        if (onSelect != null) onSubmit(gameObject);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelected != null) onUpdateSelected(gameObject);
    }

}