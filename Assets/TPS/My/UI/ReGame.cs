using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public GameObject UI;
   public void Return()
    {
        UI.SetActive(true);
        gameObject.SetActive(false);
    }
    void Update()
    {
        
    }
}
