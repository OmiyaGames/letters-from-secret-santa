using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
    public UnityEngine.UI.Button[] allButtons;

    bool isClicked = false;

    void Start()
    {
        Screen.lockCursor = false;
        for (int index = GameSettings.NumLevels; index < allButtons.Length; ++index)
        {
            if (allButtons[index] != null)
            {
                allButtons[index].gameObject.SetActive(false);
            }
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (isClicked == false)
        {
            isClicked = Singleton.Get<SceneTransition>().LoadLevel(levelIndex);
            if(isClicked == true)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void LoadCredits()
    {
        LoadLevel(GameSettings.CreditsLevel);
    }

    public void Quit()
    {
        if (isClicked == false)
        {
            Application.Quit();
            isClicked = true;
        }
    }

#if UNITY_EDITOR
    public SceneTransition transition;

    [ContextMenu("Update Button Text")]
    public void SetupButtons()
    {
        UnityEngine.UI.Text[] allTexts = null;
        for(int index = 0; ((index < transition.levelNames.Length) && (index < allButtons.Length)); ++index)
        {
            if((allButtons[index] != null) && (string.IsNullOrEmpty(transition.levelNames[index]) == false))
            {
                allTexts = allButtons[index].GetComponentsInChildren<UnityEngine.UI.Text>();
                foreach(UnityEngine.UI.Text label in allTexts)
                {
                    label.text = transition.levelNames[index];
                }
            }
        }
    }
#endif
}
