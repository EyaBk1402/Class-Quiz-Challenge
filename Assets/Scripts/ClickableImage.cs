using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableImage : MonoBehaviour, IPointerClickHandler
{
    public int slotIndex;
    void IPointerClickHandler.OnPointerClick(PointerEventData e)
    {
        QuizManager.Instance.SelectImage(slotIndex);
    }
}
