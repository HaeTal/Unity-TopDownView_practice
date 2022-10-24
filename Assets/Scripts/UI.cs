using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public void ButtonPointerEnter(Text text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 255 / 255f);
    }

    public void ButtonPointerEnter(Button button)
    {
        Color buttonColor = button.GetComponent<Image>().color;

        button.GetComponent<Image>().color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, 255 / 255f);
    }

    public void ButtonPointerExit(Text text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 120 / 255f);
    }

    public void ButtonPointerExit(Button button)
    {
        Color buttonColor = button.GetComponent<Image>().color;

        button.GetComponent<Image>().color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, 120 / 255f);
    }

    /*public void ButtonPointerEnter<T>(T t)
    {
        if(typeof(T) == typeof(Button))
        {
            Color buttonColor = t.GetComponent<Image>().color;

            button.GetComponent<Image>().color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, 255 / 255f);
        }

        else if (typeof(T) == typeof(Text))
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 255 / 255f);
        }
    }*/
}
