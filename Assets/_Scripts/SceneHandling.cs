using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandling : MonoBehaviour
{
    [SerializeField] private GameObject LeftController;
    [SerializeField] private GameObject RightController;
    [SerializeField] private Behaviour RightUIPointer;

    GameObject LeftSaber;
    GameObject LeftShaft;
    GameObject LeftModel;

    GameObject RightSaber;
    GameObject RightShaft;
    GameObject RightModel;

    private void Awake()
    {
        LeftSaber = LeftController.transform.Find("Saber").gameObject;
        LeftShaft = LeftController.transform.Find("Shaft").gameObject;
        LeftModel = LeftController.transform.Find("Model").gameObject;

        RightSaber = RightController.transform.Find("Saber").gameObject;
        RightShaft = RightController.transform.Find("Shaft").gameObject;
        RightModel = RightController.transform.Find("Model").gameObject;
    }

    private void MenuSceneLoaded()
    {
        LeftSaber.SetActive(false);
        LeftShaft.SetActive(false);

        RightSaber.SetActive(false);
        RightShaft.SetActive(false);

        LeftModel.SetActive(true);
        RightModel.SetActive(true);
        RightUIPointer.enabled = true;
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
