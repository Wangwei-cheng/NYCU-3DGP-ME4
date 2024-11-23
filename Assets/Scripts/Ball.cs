using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        if(string.Equals(other.gameObject.name, "Player"))
        {
            Destroy(this.gameObject);
        }
    }
}
