using UnityEngine;
using System.Collections;

public class VRPanelBlockerController : MonoBehaviour
{
    public static VRPanelBlockerController Instance { get; private set; }

    public GameObject panelObject;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowPanel()
    {
        panelObject?.SetActive(true);
    }

    public void HidePanel()
    {
        panelObject?.SetActive(false);
    }

    public void HidePanelAfterDelay(float seconds)
    {
        StartCoroutine(HideWithDelay(seconds));
    }

    private IEnumerator HideWithDelay(float s)
    {
        yield return new WaitForSeconds(s);
        panelObject?.SetActive(false);
    }
}
