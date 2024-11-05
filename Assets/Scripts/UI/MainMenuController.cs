using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;

    [SerializeField] private GameObject settingsCanvas;

    [SerializeField] private GameObject creditsCanvas;

    [SerializeField] private GameObject loadGameCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CloseAllCanvases();
        mainMenuCanvas.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CloseAllCanvases()
    {
        mainMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
        creditsCanvas.SetActive(false);
        loadGameCanvas.SetActive(false);
    }

    public void OpenCanvas(string name)
    {
        CloseAllCanvases();
        switch (name)
        {
            case "MainMenu":
                mainMenuCanvas.SetActive(true);
                break;
            case "Settings":
                settingsCanvas.SetActive(true);
                break;
            case "Credits":
                creditsCanvas.SetActive(true);
                break;
            case "LoadGame":
                loadGameCanvas.SetActive(true);
                break;
            default:
                Debug.LogWarning("Canvas name not recognized: " + name);
                break;
        }
    }
}
