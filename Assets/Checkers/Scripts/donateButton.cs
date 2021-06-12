using UnityEngine;

public class donateButton : MonoBehaviour
{
    public string url;

    public void OpenLink()
    {
        Application.OpenURL(url);
    }
}
