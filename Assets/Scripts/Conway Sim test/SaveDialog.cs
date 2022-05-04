using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SaveDialog : MonoBehaviour
{

    public InputField patternName;

    public HUD hud;
    public void savePattern()
    {
        EventManager.TriggerEvent("SavePattern");

        hud.isActive = false;
        gameObject.SetActive(false);
    }

    public void quitDialog()
    {
        hud.isActive = false;
        gameObject.SetActive(false);
    }
}
