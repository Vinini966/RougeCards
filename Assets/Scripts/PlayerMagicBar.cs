using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMagicBar : StatusBar
{

    // Start is called before the first frame update
    void Start()
    {
        HeroFactory.HeroInitilized += InitilizeBar;
    }

    public override void InitilizeBar(Actor actor)
    {
        actor.MPChanged += UpdateBar;
        UpdateBar(actor);
    }

    public override void UpdateBar(Actor actor)
    {
        Bar.value = actor.CurrentMagic / actor.Magic;
        StatusText.text = $"{actor.CurrentMagic}/{actor.Magic}";
    }

}
