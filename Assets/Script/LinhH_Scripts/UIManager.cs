using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] public GameObject bagUI;

    private void Start()
    {
        InputManager.OnOpenBagPress += EnableBagUI;
    }
    

    private void OnDisable() {
        InputManager.OnOpenBagPress -= EnableBagUI;
    }
    

    private void EnableBagUI()
    {
        bagUI.SetActive(true);
    }
}
