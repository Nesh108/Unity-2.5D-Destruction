using System;
using System.Collections.Generic;
using UnityEngine;

public class Explodable : MonoBehaviour
{
    public Action<List<GameObject>> OnFragmentsGenerated;

    [HideInInspector]
    public bool allowRuntimeFragmentation = false;
    [HideInInspector]
    public float ParentColliderWidth = 1f;
    [HideInInspector]
    public float ChildrenColliderWidth = 1f;
    [HideInInspector]
    public bool DestroyPiecesAfterHit = false;
    [HideInInspector]
    public float DestroyAfterTime = 0f;
    public RangedFloat MassPerFragment;
    public RangedFloat DragPerFragment;
    public RangedFloat AngularDragPerFragment;
    [HideInInspector]
    public bool UseGravityOnFragments = true;
    [HideInInspector]
    public int extraPoints = 0;
    [HideInInspector]
    public int subshatterSteps = 0;
    [HideInInspector]
    public ColliderType ColliderTypeParent = ColliderType.BOX;

    [HideInInspector]
    public string fragmentLayer = "Default";
    [HideInInspector]
    public string sortingLayerName = "Default";
    [HideInInspector]
    public int orderInLayer = 0;

    [HideInInspector]
    public enum ShatterType
    {
        Triangle,
        Voronoi
    };
    [HideInInspector]
    public ShatterType shatterType;
    [HideInInspector]
    public List<GameObject> fragments = new List<GameObject>();
    [HideInInspector]
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
                meshCol.skinWidth = ChildrenColliderWidth;
                Rigidbody rb = p.AddComponent<Rigidbody>();
                rb.mass = UnityEngine.Random.Range(MassPerFragment.minValue, MassPerFragment.maxValue);
                rb.useGravity = UseGravityOnFragments;
                rb.drag = UnityEngine.Random.Range(DragPerFragment.minValue, DragPerFragment.maxValue);
                rb.angularDrag = UnityEngine.Random.Range(AngularDragPerFragment.minValue, AngularDragPerFragment.maxValue);
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                meshCol.sharedMesh = p.GetComponent<MeshFilter>().sharedMesh;
                meshCol.sharedMesh.RecalculateBounds();
                meshCol.sharedMesh.RecalculateNormals();
                meshCol.sharedMesh.RecalculateTangents();
                meshCol.convex = true;
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
                BoxCollider bc = gameObject.AddComponent<BoxCollider>();
                bc.size = new Vector3(bc.size.x, bc.size.y, ParentColliderWidth);
                break;
            case ColliderType.SPHERE:
                gameObject.AddComponent<SphereCollider>().radius = ParentColliderWidth;
                break;
            case ColliderType.CAPSULE:
                gameObject.AddComponent<CapsuleCollider>().radius = ParentColliderWidth;
                break;
            case ColliderType.MESH:
                MeshCollider mc = gameObject.AddComponent<MeshCollider>();
                mc.sharedMesh = SpriteToMesh(gameObject.GetComponent<SpriteRenderer>().sprite);
                mc.convex = true;
                mc.skinWidth = ParentColliderWidth;
                break;
        }
    }

    Mesh SpriteToMesh(Sprite sprite)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Array.ConvertAll(sprite.vertices, i => (Vector3)i);
        mesh.uv = sprite.uv;
        mesh.triangles = Array.ConvertAll(sprite.triangles, i => (int)i);

        return mesh;
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
