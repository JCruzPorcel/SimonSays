using UnityEngine;

public class WebBrowserController : MonoBehaviour
{
    private const string URL = "https://linktr.ee/FelicetteStudios";

    public void OpenWebpageInBrowser()
    {
        Application.OpenURL(URL);
    }
}
