using UnityEngine;

namespace DudeRescueSquad.UI
{
    public class TestUIPopup : MonoBehaviour
    {
#if UNITY_EDITOR
        public void OpenPopup()
        {
            UIPopupManager.Instance.ShowPopup<UIPopupTest>(PopupType.Test)
                .Setup("TEST", "Testing popups...", 
                () => { Debug.Log("Check Accept button action"); }, 
                () => { Debug.Log("Popup was closed"); }
            );
        }
#endif
    }
}