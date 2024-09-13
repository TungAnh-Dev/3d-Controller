using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    void Start()
    {
        Disable();
    }

    private void OnTriggerEnter(Collider other) 
    {

        IHit hit = other.GetComponent<IHit>();

        if(hit == null) return;

        if(playerController.playerData.level >= hit.GetCurrentLevel())
        {
            hit.OnHit(playerController.GetDamage());
        }
        else
        {
            //TODO: Show level required
        }
    }

    public void Active()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
