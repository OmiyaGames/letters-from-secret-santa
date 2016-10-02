using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
	static PauseMenu msInstance = null;

    public enum ClickedAction
    {
        Continue,
        Restart,
        ReturnToMenu
    }

    public GameObject pausePanel;
    public GameObject continueButton;
    System.Action<ClickedAction> onVisibleChanged;

    void Awake()
	{
		msInstance = this;
        OnContinueClicked();
	}

    void OnDestroy()
    {
        msInstance = null;
    }

    public static void Show(System.Action<ClickedAction> visibleChanged)
    {
        //if (Singleton.Get<SceneTransition>().State == SceneTransition.Transition.NotTransitioning)
        //{
            // Store function pointer
            msInstance.onVisibleChanged = visibleChanged;

            // Unlock the cursor
            Screen.lockCursor = false;

            // Make the game object active
            msInstance.pausePanel.SetActive(true);

            // Set the continue button as default
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(msInstance.continueButton, null);

            // Stop time
            Time.timeScale = 0;
        //}
    }

    public static void Hide()
    {
        msInstance.OnContinueClicked();
    }

    public void OnContinueClicked(ClickedAction action)
    {
        // Reset the default button
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null, null);

        // Make time flow again
        Time.timeScale = 1;

        // Lock the cursor
        Screen.lockCursor = true;

        // Hide the panel
        pausePanel.SetActive(false);

        // Indicate change
        if (msInstance.onVisibleChanged != null)
        {
            msInstance.onVisibleChanged(action);
        }
    }

    public void OnContinueClicked()
    {
        OnContinueClicked(ClickedAction.Continue);
    }

    public void OnRestartClicked()
    {
        OnContinueClicked(ClickedAction.Restart);
        Singleton.Get<SceneTransition>().LoadLevel(Application.loadedLevel);
    }

    public void OnReturnToMenuClicked()
    {
        OnContinueClicked(ClickedAction.ReturnToMenu);
        Singleton.Get<SceneTransition>().LoadLevel(GameSettings.MenuLevel);
    }
}
