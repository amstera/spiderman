using UnityEngine;
using UnityEngine.UI;

public class ObjectivePanel : MonoBehaviour
{
    public Text CountText;
    public int TotalCount = 4;
    public int Count;
    public CutScenePlayer CutScenePlayer;

    public void UpdateCount()
    {
        Count++;
        CountText.text = $"{Count}/{TotalCount}";

        if (Count == 4)
        {
            CutScenePlayer.PlayEndClip();
            gameObject.SetActive(false);
        }
    }
}
