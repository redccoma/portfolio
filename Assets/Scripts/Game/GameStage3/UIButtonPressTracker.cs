using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Game.GameStage3
{
    public class UIButtonPressTracker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private TileType tileType;
        
        public UnityEvent<TileType, bool> onButtonPressed;

        public void OnPointerDown(PointerEventData eventData)
        {
            onButtonPressed?.Invoke(tileType, true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onButtonPressed?.Invoke(tileType, false);
        }
    }
}