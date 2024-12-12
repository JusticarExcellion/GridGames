using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
    private void
    OnCollisionEnter( Collision other )
    {
        PlayerManager.instance.DestroyPlayerInstance();
        PlayerManager.instance.DestroyedPlayer();
    }
}
