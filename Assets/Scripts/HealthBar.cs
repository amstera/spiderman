using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider Bar;
    public Text Amount;
    public RawImage LowHealthBorder;

    public float CurrentValue = 100;
    private float _updatingValue = 100;

    void Update()
    {
        bool changed = true;
        if (_updatingValue > CurrentValue)
        {
            CurrentValue += 1f;
        }
        else if (_updatingValue < CurrentValue)
        {
            CurrentValue -= 1f;
        }
        else
        {
            changed = false;
        }

        if (changed)
        {
            Amount.text = Mathf.Round(CurrentValue).ToString();
            Bar.value = CurrentValue / 100;

            var lowHealthBorderColor = LowHealthBorder.color;
            lowHealthBorderColor.a = ((1 - Bar.value) * 135) / 255f;
            LowHealthBorder.color = lowHealthBorderColor;
        }
    }

    public void UpdateValue(float amount)
    {
        _updatingValue = Mathf.Clamp(amount, 0, 100);
    }
}