using UnityEngine;

[RequireComponent(typeof(Explodable))]
public class ExplodeOnClick : MonoBehaviour
{

    private Explodable _explodable;

    void Start()
    {
        _explodable = GetComponent<Explodable>();
    }
    void OnMouseDown()
    {
        _explodable.explode();
        ExplosionForce ef = FindObjectOfType<ExplosionForce>();
        ef.doExplosion(transform.position);
    }
}
