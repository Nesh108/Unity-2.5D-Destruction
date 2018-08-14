using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Explodable), true)]
public class ExplodableEditor : Editor
{

    public override void OnInspectorGUI()
    {
        Explodable myTarget = (Explodable)target;
        myTarget.allowRuntimeFragmentation = EditorGUILayout.Toggle("Allow Runtime Fragmentation", myTarget.allowRuntimeFragmentation);
        myTarget.DestroyPiecesAfterHit = EditorGUILayout.Toggle("Destroy Pieces After Hit", myTarget.DestroyPiecesAfterHit);
        if (myTarget.DestroyPiecesAfterHit)
        {
            myTarget.DestroyAfterTime = EditorGUILayout.FloatField("Destroy After Time", myTarget.DestroyAfterTime);
        }
        this.DrawDefaultInspectorWithoutScriptField();
        myTarget.UseGravityOnFragments = EditorGUILayout.Toggle("Use Gravity On Fragments", myTarget.UseGravityOnFragments);


        myTarget.ColliderTypeParent = (ColliderType)EditorGUILayout.EnumPopup("Collider Type", myTarget.ColliderTypeParent);
        myTarget.ParentColliderWidth = EditorGUILayout.FloatField("Parent Collider Width", myTarget.ParentColliderWidth);
        myTarget.ChildrenColliderWidth = EditorGUILayout.FloatField("Children Collider Width", myTarget.ChildrenColliderWidth);
        myTarget.shatterType = (Explodable.ShatterType)EditorGUILayout.EnumPopup("Shatter Type", myTarget.shatterType);
        myTarget.extraPoints = EditorGUILayout.IntField("Extra Points", myTarget.extraPoints);
        myTarget.subshatterSteps = EditorGUILayout.IntField("Subshatter Steps", myTarget.subshatterSteps);
        if (myTarget.subshatterSteps > 1)
        {
            EditorGUILayout.HelpBox("Use subshatter steps with caution! Too many will break performance!!! Don't recommend more than 1", MessageType.Warning);
        }

        myTarget.fragmentLayer = EditorGUILayout.TextField("Fragment Layer", myTarget.fragmentLayer);
        myTarget.sortingLayerName = EditorGUILayout.TextField("Sorting Layer", myTarget.sortingLayerName);
        myTarget.orderInLayer = EditorGUILayout.IntField("Order In Layer", myTarget.orderInLayer);

        if (GUILayout.Button("Generate Fragments"))
        {
            myTarget.fragmentInEditor();
            EditorUtility.SetDirty(myTarget);
        }

        if (GUILayout.Button("Destroy Fragments"))
        {
            myTarget.deleteFragments();
            EditorUtility.SetDirty(myTarget);
        }
    }
}

public static class DefaultInspector_EditorExtension
{
    public static bool DrawDefaultInspectorWithoutScriptField(this Editor Inspector)
    {
        EditorGUI.BeginChangeCheck();

        Inspector.serializedObject.Update();

        SerializedProperty Iterator = Inspector.serializedObject.GetIterator();

        Iterator.NextVisible(true);

        while (Iterator.NextVisible(false))
        {
            EditorGUILayout.PropertyField(Iterator, true);
        }

        Inspector.serializedObject.ApplyModifiedProperties();

        return (EditorGUI.EndChangeCheck());
    }
}