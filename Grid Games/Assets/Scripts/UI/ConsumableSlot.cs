using UnityEngine;

public class ConsumableSlot : MonoBehaviour
{
    [SerializeField] private GameObject ConsumableImgaeObject;

    public void
    Deactivate()
    {
        ConsumableImgaeObject.SetActive( false );
    }

    public void
    Activate()
    {
        ConsumableImgaeObject.SetActive( true );
    }
}

