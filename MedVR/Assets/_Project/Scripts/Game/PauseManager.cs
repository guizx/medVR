using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    public bool CanPause = true;
    public bool IsPaused;
    public GameObject PausePanel;

    public static Action<bool> OnPausedChanged;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

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
        if (GameManager.Instance.PlatformType == PlatformType.WINDOWS)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
        IsPaused = true;
        PausePanel.SetActive(true);
        OnPausedChanged?.Invoke(IsPaused);
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void Unpause()
    {
        if (GameManager.Instance.PlatformType == PlatformType.WINDOWS)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        IsPaused = false;
        PausePanel.SetActive(false);
        OnPausedChanged?.Invoke(IsPaused);
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void SetCanPause(bool state)
    {
        CanPause = state;
    }

}
