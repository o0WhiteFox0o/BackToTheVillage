using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] public GameObject bagUI;
    [SerializeField] public GameObject generalUI;
    [SerializeField] public GameObject npcUI;
    [SerializeField] public GameObject settingUI;


    private void Start()
    {
        InputManager.OnOpenBagPress += ToggleBagUI;
        InputManager.OnGeneralUIPress += ToggleGeneralUI;
    }


    private void OnDisable()
    {
        InputManager.OnOpenBagPress -= ToggleBagUI;
        InputManager.OnGeneralUIPress -= ToggleGeneralUI;
    }


    private void ToggleBagUI()
    {
        if (bagUI.activeSelf)
        {
            bagUI.SetActive(false);
        }
        else
        {
            bagUI.SetActive(true);
        }
    }


    private void ToggleGeneralUI()
    {
        if (generalUI.activeInHierarchy)
        {
            generalUI.SetActive(false);
        }
        else
        {
            generalUI.SetActive(true);
        }
    }


    /// <summary>
    /// Hiển thị UI con trong general UI.
    /// </summary>
    public void EnableGeneralSubUI(Transform subUI)
    {
        subUI.SetAsLastSibling();
    }
}
