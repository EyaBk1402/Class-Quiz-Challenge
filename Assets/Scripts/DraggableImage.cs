using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System.Collections.Generic;

public class DraggableImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public UILineRenderer line;

    private Canvas canvas;
    private RectTransform container;
    private RectTransform imageRect;
    private Camera uiCamera;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        container = line.GetComponent<RectTransform>();
        imageRect = GetComponent<RectTransform>();
        uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        line.enabled = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3[] corners = new Vector3[4];
        imageRect.GetWorldCorners(corners);
        Vector3 worldCenter = (corners[0] + corners[2]) * 0.5f;
        Vector2 screenCenter = RectTransformUtility.WorldToScreenPoint(uiCamera, worldCenter);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            container,
            screenCenter,
            uiCamera,
            out Vector2 localStart
        );
        line.Points = new[] { localStart, localStart };
        line.enabled = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            container,
            eventData.position,
            uiCamera,
            out Vector2 localPoint
        );
        var pts = line.Points;
        pts[1] = localPoint;
        line.Points = pts;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var r in results)
        {
            if (r.gameObject.TryGetComponent<Button>(out var btn))
            {
                QuizManager.Instance.OnAnswerSelected(btn);
                break;
            }
        }
        line.enabled = false;
    }
}
