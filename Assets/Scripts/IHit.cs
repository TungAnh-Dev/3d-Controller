using UnityEngine;

public interface  IHit 
{
    int GetCurrentLevel();
    void OnHit(float damage);
}