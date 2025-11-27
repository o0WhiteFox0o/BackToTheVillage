using UnityEngine;
using UnityEngine.Events;

public abstract class BaseCookingMinigame : MonoBehaviour
{
    public UnityAction OnWin;
    public UnityAction OnLose;

    protected bool isPlaying = false;

    public virtual void StartMinigame(float difficulty, float timeLimit)
    {
        gameObject.SetActive(true);
        isPlaying = true;
    }

    protected void EndGame(bool win)
    {
        isPlaying = false;
        gameObject.SetActive(false);
        if (win) OnWin?.Invoke(); else OnLose?.Invoke();
    }
}