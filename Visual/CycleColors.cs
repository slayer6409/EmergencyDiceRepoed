using System.Collections.Generic;
using UnityEngine;

namespace RepoDice.Visual;

public class CycleColors : MonoBehaviour
{
    public List<Color> colors = new List<Color>();
    public float lerpDuration = 1f;

    private int currentIndex = 0;
    private float lerpTime = 0f;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    void Update()
    {
        if (rend == null || colors.Count < 2) return;

        lerpTime += Time.deltaTime / lerpDuration;

        Color startColor = colors[currentIndex];
        Color endColor = colors[(currentIndex + 1) % colors.Count];

        Color currentColor = Color.Lerp(startColor, endColor, lerpTime);
        rend.material.SetColor("_EmissionColor", currentColor);

        if (lerpTime >= 1f)
        {
            lerpTime = 0f;
            currentIndex = (currentIndex + 1) % colors.Count;
        }
    }
}