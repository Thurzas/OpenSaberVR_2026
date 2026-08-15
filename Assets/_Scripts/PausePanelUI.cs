using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class PausePanelUI : MonoBehaviour
{
    [SerializeField] private Button ResumeButton;
    [SerializeField] private Button RestartButton;
    [SerializeField] private Button ConfirmButton;
    [SerializeField] private  GameObject PanelAreYouSure;
    [SerializeField] private  Button ResumeBtn;
    [SerializeField] private  Button RestartBtn;
    [SerializeField] private  Button QuitBtn;
    private void Start()
    {
        var pauseManager = FindAnyObjectByType<PauseManager>(); 
        if (pauseManager == null)
        {
            Debug.LogError("PausePanelUI: no PauseManager found in the scene.");
            return;
        }

        ResumeButton.onClick.AddListener(pauseManager.Resume);
        RestartButton.onClick.AddListener(pauseManager.RestartSong);
    }

    public void AreYouSure()
    {
        ResumeBtn.gameObject.SetActive(false);
        RestartBtn.gameObject.SetActive(false);
        QuitBtn.gameObject.SetActive(false);
        PanelAreYouSure.gameObject.SetActive(true);
    }

    public void No()
    {
        ResumeBtn.gameObject.SetActive(true);
        RestartBtn.gameObject.SetActive(true);
        QuitBtn.gameObject.SetActive(true);
        PanelAreYouSure.gameObject.SetActive(false);        
    }

    public void Yes()
    {
        var pauseManager = FindAnyObjectByType<PauseManager>();
        pauseManager.QuitToMenu();
    }
}
