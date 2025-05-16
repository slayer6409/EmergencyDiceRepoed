using System.Collections;
using UnityEngine;

namespace RepoDice.Visual;

public class Jumpscare : MonoBehaviour
{
    
    GameObject NotScaryFace = null;
    GameObject cat1 = null;
    GameObject cat2 = null;
    GameObject cat3 = null;
    GameObject cat4 = null;
    GameObject tree = null;
    GameObject Bald = null;
    bool isScaring = false;
    Vector2 BaseSize = new Vector2(4f, 4f);
    void Start()
    {
        BaseSize = new Vector2(1f, 1f);
        NotScaryFace = transform.GetChild(0).gameObject;
        Bald = transform.GetChild(1).gameObject;
        cat1 = transform.GetChild(2).gameObject;
        cat2 = transform.GetChild(3).gameObject;
        cat3 = transform.GetChild(4).gameObject;
        cat4 = transform.GetChild(5).gameObject;
        tree = transform.GetChild(6).gameObject;
        Bald.SetActive(false);
        NotScaryFace.SetActive(false);
        cat1.SetActive(false);
        cat2.SetActive(false);
        cat3.SetActive(false);
        cat4.SetActive(false);
        tree.SetActive(false);
    }

    public void Scare()
    {
        StartCoroutine(DoScare());
    }

    IEnumerator DoScare()
    {
       
        
        GameObject toShow = Misc.isGlitchOrConfig() ? Bald : NotScaryFace;
        var rnd = Random.value;
        if (!Misc.isGlitchOrConfig())
        {
            if (rnd < 0.16666) toShow = cat1;
            else if (rnd < 0.333333) toShow = cat2;
            else if (rnd < 0.499999) toShow = cat3;
            else if (rnd < 0.699999) toShow = cat4;
            else if (rnd < 0.8199999) toShow = tree;
            else if (rnd > 0.8199999) toShow = NotScaryFace;
            else toShow = NotScaryFace;
        }
        
        RepoDice.sounds.TryGetValue(Misc.isGlitchOrConfig() ? Random.value > 0.5f ? "Bald" : "Bald2" : "purr", out AudioClip sfx);
        if(toShow.name == "tree") RepoDice.sounds.TryGetValue("boom", out sfx);
        
        
        GameObject tempAudio = new GameObject("TempAudio");
        tempAudio.transform.position = PlayerAvatar.instance.transform.position;
        AudioSource source = tempAudio.AddComponent<AudioSource>();
        source.clip = sfx;
    
        source.spatialBlend = 0f; 
        source.minDistance = 5f;
        source.maxDistance = 15f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.dopplerLevel = 0f;

        float volumeToUse = RepoDice.Volume.Value;
        source.volume = volumeToUse;
        source.playOnAwake = false;
        if(!Misc.isGlitchOrConfig()) source.pitch = Random.Range(0.8f, 1.2f);
        source.Play();
        GameObject.Destroy(tempAudio, sfx.length);
        // AudioSource.PlayClipAtPoint(sfx, PlayerAvatar.instance.transform.position, RepoDice.Volume.Value);
        // AudioSource.PlayClipAtPoint(sfx, PlayerAvatar.instance.transform.position, RepoDice.Volume.Value);
        // AudioSource.PlayClipAtPoint(sfx, PlayerAvatar.instance.transform.position, RepoDice.Volume.Value);
        // AudioSource.PlayClipAtPoint(sfx, PlayerAvatar.instance.transform.position, RepoDice.Volume.Value);
        isScaring = true;
        toShow.SetActive(true);
        yield return new WaitForSeconds(1.3f);
        isScaring = false;
        toShow.SetActive(false);
    }
}