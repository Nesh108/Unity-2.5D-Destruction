using System;
using System.Collections.Generic;
using UnityEngine;

public class Explodable : MonoBehaviour
{
    public Action<List<GameObject>> OnFragmentsGenerated;

    public bool allowRuntimeFragmentation = false;
    public bool DestroyPiecesAfterHit = false;
    public float DestroyAfterTime = 0f;
    public int extraPoints = 0;
    public int subshatterSteps = 0;
    public ColliderType ColliderTypeParent = ColliderType.BOX;

    public string fragmentLayer = "Default";
    public string sortingLayerName = "Default";
    public int orderInLayer = 0;


    public enum ShatterType
    {
        Triangle,
        Voronoi
    };
    public ShatterType shatterType;
    public List<GameObject> fragments = new List<GameObject>();
    private List<List<Vector2>> polygons = new List<List<Vector2>>();

    /// <summary>
    /// Creates fragments if necessary and destroys original gameobject
    /// </summary>
    public void explode()
    {
        //if fragments were not created before runtime then create them now
        if (fragments.Count == 0 && allowRuntimeFragmentation)
        {
            generateFragments();
        }
        //otherwise unparent and activate them
        else
        {
            foreach (GameObject frag in fragments)
            {
                frag.transform.parent = null;
                frag.SetActive(true);
            }
        }
        //if fragments exist destroy the original
        if (fragments.Count > 0)
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// Creates fragments and then disables them
    /// </summary>
    public void fragmentInEditor()
    {
        if (fragments.Count > 0)
        {
            deleteFragments();
        }
        generateFragments();
        foreach (GameObject frag in fragments)
        {
            frag.transform.parent = transform;
            frag.SetActive(false);
        }
    }
    public void deleteFragments()
    {
        foreach (GameObject frag in fragments)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(frag);
            }
            else
            {
                Destroy(frag);
            }
        }
        fragments.Clear();
        polygons.Clear();
    }
    /// <summary>
    /// Turns Gameobject into multiple fragments
    /// </summary>
    private void generateFragments()
    {

        ClearCollider();
        fragments = new List<GameObject>();
        switch (shatterType)
        {
            case ShatterType.Triangle:
                fragments = SpriteExploder.GenerateTriangularPieces(gameObject, extraPoints, subshatterSteps);
                break;
            case ShatterType.Voronoi:
                fragments = SpriteExploder.GenerateVoronoiPieces(gameObject, extraPoints, subshatterSteps);
                break;
            default:
                Debug.Log("invalid choice");
                break;
        }

        SetCollider();

        //sets additional aspects of the fragments
        foreach (GameObject p in fragments)
        {
            if (p != null)
            {
                p.layer = LayerMask.NameToLayer(fragmentLayer);
                p.GetComponent<Renderer>().sortingLayerName = sortingLayerName;
                p.GetComponent<Renderer>().sortingOrder = orderInLayer;
                MeshCollider meshCol = p.AddComponent<MeshCollider>();
                Rigidbody rb = p.AddComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                meshCol.sharedMesh = p.GetComponent<MeshFilter>().sharedMesh;
                meshCol.convex = true;
                meshCol.inflateMesh = true;
                if (DestroyPiecesAfterHit)
                {
                    p.AddComponent<DestroyAfter>().Time = DestroyAfterTime;
                }
            }
        }

        foreach (ExplodableAddon addon in GetComponents<ExplodableAddon>())
        {
            if (addon.enabled)
            {
                addon.OnFragmentsGenerated(fragments);
            }
        }
    }

    private void ClearCollider()
    {
        BoxCollider bc = gameObject.GetComponent<BoxCollider>();
        SphereCollider sc = gameObject.GetComponent<SphereCollider>();
        CapsuleCollider cc = gameObject.GetComponent<CapsuleCollider>();
        MeshCollider mc = gameObject.GetComponent<MeshCollider>();
        if (bc != null)
        {
            DestroyImmediate(bc);
        }
        else if (sc != null)
        {
            DestroyImmediate(sc);
        }
        else if (cc != null)
        {
            DestroyImmediate(cc);
        }
        else if (mc != null)
        {
            DestroyImmediate(mc);
        }
    }

    private void SetCollider()
    {

        DestroyImmediate(gameObject.GetComponent<PolygonCollider2D>());
        DestroyImmediate(gameObject.GetComponent<BoxCollider2D>());

        switch (ColliderTypeParent)
        {
            case ColliderType.BOX:
                gameObject.AddComponent<BoxCollider>();
                break;
            case ColliderType.SPHERE:
                gameObject.AddComponent<SphereCollider>();
                break;
            case ColliderType.CAPSULE:
                gameObject.AddComponent<CapsuleCollider>();
                break;
            case ColliderType.MESH:
                gameObject.AddComponent<MeshCollider>();
                break;
        }
    }
}

public enum ColliderType
{
    NONE,
    BOX,
    SPHERE,
    CAPSULE,
    MESH
}
