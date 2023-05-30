using UnityEngine.EventSystems;
using IButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace UnityEngine.UI
{
    public sealed class MBInfoButton : Selectable, IPointerClickHandler
    {
        public Button.ButtonClickedEvent onLeftClick = new();
        public Button.ButtonClickedEvent onRightClick = new();
        public Button.ButtonClickedEvent onMiddleClick = new();


        public void OnPointerClick(PointerEventData data)
            => (data.button switch {
                IButton.Left => onLeftClick,
                IButton.Right => onRightClick,
                IButton.Middle => onMiddleClick,
                _ => null
            })?.Invoke();
    }
}