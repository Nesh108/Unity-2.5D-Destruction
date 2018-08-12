using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float Time;

    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, Time);
    }
}
