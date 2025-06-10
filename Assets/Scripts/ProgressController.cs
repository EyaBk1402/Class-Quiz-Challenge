using UnityEngine;
using UnityEngine.UI;

public class ProgressController : MonoBehaviour
{
    [Tooltip("Le Slider UI de la ProgressBar")]
    public Slider progressBar;
    private int totalRounds = 10;
    private int currentRound = 0;

   
    public void AdvanceRound()
    {
        currentRound = Mathf.Clamp(currentRound + 1, 0, totalRounds);
        progressBar.value = (currentRound / (float)totalRounds) * progressBar.maxValue;
    }

   
    public void ResetProgress()
    {
        currentRound = 0;
        progressBar.value = 0;
    }
}
