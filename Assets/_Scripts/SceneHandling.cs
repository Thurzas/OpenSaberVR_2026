using System.Collections;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;

public class SceneHandling : MonoBehaviour
{
    [SerializeField] private GameObject LeftSaber;
    [SerializeField] private GameObject LeftShaft;
    [SerializeField] private GameObject LeftModel;

    [SerializeField] private GameObject RightSaber;
    [SerializeField] private GameObject RightShaft;
    [SerializeField] private GameObject RightModel;

    [SerializeField] private Behaviour RightUIPointer;

    [SerializeField] private PostProcessingBehaviour PostProcessing;
    [SerializeField] private PostProcessingProfile MenuProfile;
    [SerializeField] private PostProcessingProfile GameplayProfile;

    private void MenuSceneLoaded()
    {
        LeftSaber.SetActive(false);
        LeftShaft.SetActive(false);

        RightSaber.SetActive(false);
        RightShaft.SetActive(false);

        LeftModel.SetActive(true);
        RightModel.SetActive(true);
        RightUIPointer.enabled = true;
        PostProcessing.profile = MenuProfile;
    }

    private void SaberSceneLoaded()
    {
        LeftSaber.SetActive(true);
        LeftShaft.SetActive(true);

        RightSaber.SetActive(true);
        RightShaft.SetActive(true);

        LeftModel.SetActive(false);
        RightModel.SetActive(false);
        RightUIPointer.enabled = false;
        PostProcessing.profile = GameplayProfile;
    }

    private void Start()
    {
        if (!IsSceneLoaded("Menu"))
        {
            StartCoroutine(LoadScene("Menu", LoadSceneMode.Additive));
        }

        MenuSceneLoaded();
    }

    internal IEnumerator LoadScene(string sceneName, LoadSceneMode mode)
    {
        if (sceneName == "OpenSaber")
        {
            SaberSceneLoaded();
        }
        else if (sceneName == "Menu")
        {
            MenuSceneLoaded();
        }

        yield return SceneManager.LoadSceneAsync(sceneName, mode);
    }

    internal IEnumerator UnloadScene(string sceneName)
    {
        yield return SceneManager.UnloadSceneAsync(sceneName);
    }

    internal bool IsSceneLoaded(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);

        if (scene.name == null)
        {
            return false;
        }

        return true;
    }
}
