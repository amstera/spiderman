using UnityEngine;
using UnityEngine.UI;

public class ObjectivePanel : MonoBehaviour
{
    public Text CountText;
    public int TotalCount = 4;

    private int _count;

    public void UpdateCount()
    {
        _count++;
        CountText.text = $"{_count}/{TotalCount}";
    }
}
