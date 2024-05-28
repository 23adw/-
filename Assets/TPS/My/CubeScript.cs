using UnityEngine;
using Photon.Pun;

public class CubeScript : MonoBehaviour
{
    private float timer = 0f;
    private float deleteInterval = 10f;//É¾³ý¼ä¸ô
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Damage>().HandleCubeCollision();
            Destroy(this.gameObject);
        }
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer>=deleteInterval)
        {
            Destroy(this.gameObject);
        }
    }
}
