using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LerpImage
{
    public Image image;

    private float data;
    private LerpUIAction action;

    public void SetData(LerpUIAction _action, float _value)
    {
        action = _action;
        data = _value;

        action.Add(Update);

        Update();
    }

    public void Update()
    {
        image.fillAmount = Mathf.Lerp(image.fillAmount, data, 0.2f);

        if (image.fillAmount == data) action.Remove(Update);
    }
}
