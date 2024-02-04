using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Food : MonoBehaviour, IUsable
{
    [field: SerializeField]
    public UnityEvent onUse { get; private set; }

    private float HealthBoost = 1;

    public void Use(GameObject actor)
    {
        actor.GetComponent<Player>().AddHealth(HealthBoost);
        onUse?.Invoke();
        Destroy(gameObject);
    }
}
