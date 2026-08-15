using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private XRNode hand = XRNode.LeftHand;

    private SceneHandling SceneHandling;
    private bool isPaused = false;
    private bool wasMenuButtonPressed = false;

    private void Awake()
    {
        SceneHandling = GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>();
    }

    private void Update()
    {
        var device = InputDevices.GetDeviceAtXRNode(hand);
        if (!device.isValid || !device.TryGetFeatureValue(CommonUsages.primaryButton, out var pressed))
        {
            return;
        }

        if (pressed && !wasMenuButtonPressed)
        {
            OnPausePressed();
        }

        wasMenuButtonPressed = pressed;
    }

    private void OnPausePressed()
    {
        if (!SceneHandling.IsSceneLoaded("OpenSaber"))
        {
            return;
        }

        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        SetSongAudioPaused(true);
        StartCoroutine(LoadPauseSceneRoutine());
    }

    private IEnumerator LoadPauseSceneRoutine()
    {
        yield return SceneManager.LoadSceneAsync("Pause_Menu", LoadSceneMode.Additive);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SetSongAudioPaused(false);
        StartCoroutine(UnloadPauseSceneRoutine());
    }

    private IEnumerator UnloadPauseSceneRoutine()
    {
        yield return SceneManager.UnloadSceneAsync("Pause_Menu");
    }

    public void RestartSong()
    {
        isPaused = false;
        Time.timeScale = 1f;
        StartCoroutine(RestartSongRoutine());
    }

    private IEnumerator RestartSongRoutine()
    {
        yield return SceneManager.UnloadSceneAsync("Pause_Menu");
        yield return SceneManager.UnloadSceneAsync("OpenSaber");
        yield return SceneManager.LoadSceneAsync("OpenSaber", LoadSceneMode.Additive);
    }

    public void QuitToMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;
        StartCoroutine(QuitToMenuRoutine());
    }

    private IEnumerator QuitToMenuRoutine()
    {
        yield return SceneManager.UnloadSceneAsync("Pause_Menu");
        yield return SceneHandling.LoadScene("Menu", LoadSceneMode.Additive);
        yield return SceneHandling.UnloadScene("OpenSaber");
    }

    private void SetSongAudioPaused(bool paused)
    {
        var spawner = FindAnyObjectByType<NotesSpawner>();
        if (spawner == null || spawner.AudioSource == null)
        {
            return;
        }

        spawner.IsPaused = paused;

        if (paused)
        {
            spawner.AudioSource.Pause();
        }
        else
        {
            spawner.AudioSource.UnPause();
        }
    }
}