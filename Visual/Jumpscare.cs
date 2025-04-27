using System.Collections;
using UnityEngine;

namespace RepoDice.Visual;

public class Jumpscare : MonoBehaviour
{
    
    GameObject NotScaryFace = null;
    GameObject Bald = null;
    bool isScaring = false;
    Vector2 BaseSize = new Vector2(4f, 4f);
    void Start()
    {
        BaseSize = new Vector2(1f, 1f);
        NotScaryFace = transform.GetChild(0).gameObject;
        Bald = transform.GetChild(1).gameObject;
        Bald.SetActive(false);
        NotScaryFace.SetActive(false);
    }

    public void Scare()
    {
        StartCoroutine(DoScare());
    }

    IEnumerator DoScare()
    {
        RepoDice.sounds.TryGetValue(Misc.isGlitchOrConfig() ? Random.value > 0.5f ? "Bald" : "Bald2" : "purr", out AudioClip sfx);
        AudioSource.PlayClipAtPoint(sfx, PlayerAvatar.instance.transform.position, 10f);
        AudioSource.PlayClipAtPoint(sfx, PlayerAvatar.instance.transform.position, 10f);
        AudioSource.PlayClipAtPoint(sfx, PlayerAvatar.instance.transform.position, 10f);
        AudioSource.PlayClipAtPoint(sfx, PlayerAvatar.instance.transform.position, 10f);
        
        GameObject toShow = Misc.isGlitchOrConfig() ? Bald : NotScaryFace;
        isScaring = true;
        toShow.SetActive(true);
        yield return new WaitForSeconds(1.3f);
        isScaring = false;
        toShow.SetActive(false);
    }
}