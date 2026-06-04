using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    public void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
    }
}