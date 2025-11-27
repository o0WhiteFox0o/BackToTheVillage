using UnityEngine;

public class CookingStation : MonoBehaviour
{
    public CookingToolType toolType; // Chọn trên Inspector: CuttingBoard / FryingPan...
    private bool playerNear;

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.F))
        {
            CookingManager.Instance.OpenCooking(toolType);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) { if(other.CompareTag("Player")) playerNear = true; }
    private void OnTriggerExit2D(Collider2D other) { if(other.CompareTag("Player")) { playerNear = false; CookingManager.Instance.CloseMainPanel(); } }
}