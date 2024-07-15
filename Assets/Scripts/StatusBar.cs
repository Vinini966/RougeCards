using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class StatusBar : MonoBehaviour
{
    public Slider Bar;
    public TMP_Text StatusText;

    public abstract void InitilizeBar(Actor actor);

    public abstract void UpdateBar(Actor actor);


}
