using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : StatusBar
{

    // Start is called before the first frame update
    void Start()
    {
        HeroFactory.HeroInitilized += InitilizeBar;
    }

    public override void InitilizeBar(Actor actor)
    {
        actor.HealthChanged += UpdateBar;
        UpdateBar(actor);
    }

    public override void UpdateBar(Actor actor)
    {
        Bar.value = actor.CurrentHealth / actor.Health;
        StatusText.text = $"{actor.CurrentHealth}/{actor.Health}";
    }

}
