using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoadDialog : MonoBehaviour
{
    public Dropdown patternName;
    public HUD hud;
    // Start is called before the first frame update
    void Start()
    {
        ReloadOptions();
    }

    private void OnEnable()
    {
        ReloadOptions();
    }

    void ReloadOptions()
    {

        List<string> options = new List<string>();

        string[] filePaths = Directory.GetFiles(@"patterns/");

        for(int i =0; i< filePaths.Length; i++)
        {
            string filename = filePaths[i].Substring(filePaths[i].LastIndexOf('/') + 1);
            string extension = System.IO.Path.GetExtension(filename);

            filename = filename.Substring(0, filename.Length - extension.Length);

            options.Add(filename);

        }
        patternName.ClearOptions();
        patternName.AddOptions(options);
    }

    public void quitDialog()
    {
        hud.isActive = false;
        gameObject.SetActive(false);
    }

    public void loadPattern()
    {

        EventManager.TriggerEvent("LoadPattern");

        hud.isActive = false;
        gameObject.SetActive(false);
    }
}
