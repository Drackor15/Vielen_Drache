using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    string buttonSound = "MouseHover";

    public void OnPointerEnter(PointerEventData eventData) {
        AudioManager.instanceAudioManager.PlaySound(buttonSound);
    }

    public void OnPointerExit(PointerEventData eventData) {
    }
}
