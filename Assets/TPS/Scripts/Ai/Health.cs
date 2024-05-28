using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    [HideInInspector]
    public float currentHealth;
    AIAgent agent;
    UIHealthBar healthBar;
    public GameObject pistol;
    public AIWalkAndDetect aiwalk;
    private CapsuleCollider collider;
    void Start()
    {
        collider=GetComponent<CapsuleCollider>();
        aiwalk = GetComponent<AIWalkAndDetect>();
        agent = GetComponent<AIAgent>();
        currentHealth = maxHealth;
        healthBar=GetComponentInChildren<UIHealthBar>();
    }
    public void TakeDamage(float amount,Vector3 dirction)
    {
        currentHealth -= amount;
        healthBar.SetHealthBarPercentage(currentHealth/maxHealth);
        aiwalk.isChasingPlayer = true;
        if (currentHealth<=0.0f)
        {
            Die(dirction);
        }
    }
    private void Die(Vector3 direction)
    {
       AiDeathState deathState=agent.stateMachine.GetState(AiStateId.Death)as AiDeathState;
        deathState.direction = direction;
        agent.stateMachine.ChangeState(AiStateId.Death);
        pistol.transform.SetParent(null);
        collider.enabled=false;
        pistol.GetComponent<BoxCollider>().enabled = true;
        pistol.AddComponent<Rigidbody>();
        pistol.GetComponent<CubeScript>().enabled = true;
       // pistol.SetActive(false);
        agent.navMeshAgent.enabled = false;
        aiwalk.enabled = false;
        StartCoroutine(RespawnAfterDelay(3.0f));
    }
    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Respawn();
    }
    private void Respawn()
    {
       Destroy(gameObject);
    }
    void Update()
    {
        
    }
}
