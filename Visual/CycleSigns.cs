using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CycleSigns : MonoBehaviour
{
    class DiceVisuals
    {
        public Sprite Sprite;
        public Color ModelColor;
        public Color EmissionColor;
        public float Emission;

        public DiceVisuals(Sprite sprite, Color color, Color emissionColor, float emission)
        {
            Sprite = sprite;
            ModelColor = color;
            EmissionColor = emissionColor;
            Emission = emission;
        }
    }

    public float CycleTime = 1f;

    private float CurrentTimer = 0f;
    private int CurrentSprite = 0;
    private bool Stop = false;

    private SpriteRenderer SignSpriteRenderer;
    private SpriteRenderer SignSpriteRenderer2;
    private SpriteRenderer SignSpriteRenderer3;
    private Renderer DiceRenderer;

    List<DiceVisuals> Visuals = new List<DiceVisuals>();
    void Start()
    {
        Visuals.Add(new DiceVisuals(RepoDice.RepoDice.WarningExclamation, Color.yellow, Color.yellow,40f));
        //Visuals.Add(new DiceVisuals(RepoDice.RepoDice.WarningExclamation, Color.yellow, Color.yellow,100f));
        Visuals.Add(new DiceVisuals(RepoDice.RepoDice.WarningDeath, Color.red, Color.red,100f));
        Visuals.Add(new DiceVisuals(RepoDice.RepoDice.WarningLuck, Color.green, Color.green,300f));
        if(RepoDice.Misc.isGlitchOrConfig()) Visuals.Add(new DiceVisuals(RepoDice.RepoDice.WarningGlitch, Color.magenta, Color.magenta,300f));

        SignSpriteRenderer = transform.Find("Emergency Sign").gameObject.GetComponent<SpriteRenderer>();
        SignSpriteRenderer2 = transform.Find("Emergency Sign2").gameObject.GetComponent<SpriteRenderer>();
        DiceRenderer = gameObject.GetComponent<Renderer>();
    }
    
    
    void Update()
    {
        if (Stop) return;
        CurrentTimer -=  Time.deltaTime;
        if(CurrentTimer <= 0f)
        {
            CurrentTimer = CycleTime;
            CycleSprite();
        }
    }

    void CycleSprite()
    {
        CurrentSprite++;
        if (CurrentSprite >= Visuals.Count)
            CurrentSprite = 0;

        SignSpriteRenderer.sprite = Visuals[CurrentSprite].Sprite;
        SignSpriteRenderer2.sprite = Visuals[CurrentSprite].Sprite;
        DiceRenderer.material.SetColor("_Color", Visuals[CurrentSprite].ModelColor);
        DiceRenderer.material.SetColor("_EmissionColor", Visuals[CurrentSprite].EmissionColor * Visuals[CurrentSprite].Emission);
    }

    public void HideSigns()
    {
        Stop = true;
        SignSpriteRenderer.GetComponent<SpriteRenderer>().enabled = false;
        SignSpriteRenderer2.GetComponent<SpriteRenderer>().enabled = false;
    }
}
