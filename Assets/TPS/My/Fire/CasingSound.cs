using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasingSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip casingSound;
    void Start()
    {
        StartCoroutine(PlaySound());
        Destroy(this.gameObject,1f);
    }
    private IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(Random.Range(0.25f, 0.85f));
        audioSource.clip = casingSound;
        audioSource.Play();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
