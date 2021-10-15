using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider Bar;
    public Text Amount;

    public float CurrentValue = 100;
    private float _updatingValue = 100;

    void Update()
    {
        if (_updatingValue > CurrentValue)
        {
            CurrentValue += 1f;
        }
        else if (_updatingValue < CurrentValue)
        {
            CurrentValue -= 1f;
        }

        Amount.text = Mathf.Round(CurrentValue).ToString();
        Bar.value = CurrentValue / 100;
    }

    public void UpdateValue(float amount)
    {
        _updatingValue = amount;
    }
}