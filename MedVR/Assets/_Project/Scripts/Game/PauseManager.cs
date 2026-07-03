using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public bool CanPause = true;
    public bool IsPaused;
    public GameObject PausePanel;

    public static Action<bool> OnPausedChanged;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && CanPause)
        {
            if (IsPaused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        IsPaused = true;
        PausePanel.SetActive(true);
        OnPausedChanged?.Invoke(IsPaused);
        Time.timeScale = 0f;
    }

    public void Unpause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsPaused = false;
        PausePanel.SetActive(false);
        OnPausedChanged?.Invoke(IsPaused);
        Time.timeScale = 1f;
    }

    public void SetCanPause(bool state)
    {
        CanPause = state;
    }

}
