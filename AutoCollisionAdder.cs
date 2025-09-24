// warning 
// if you have to many objects in the scen unity may freeze for a while when calculating the preview list
//
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Linq;

public class AutoCollisionAdder : EditorWindow
{
    private enum ColliderType { BoxCollider, MeshCollider, SphereCollider, CapsuleCollider }
    private ColliderType selectedCollider = ColliderType.MeshCollider;

    private bool useNameFilter = false;
    private string nameFilter = "";

    private bool useTagFilter = false;
    private string tagFilter = "Untagged";

    private bool useLayerFilter = false;
    private int layerFilter = 0;

    private bool useSectionFilter = false;
    private BoxBoundsHandle sectionHandle = new BoxBoundsHandle();

    private bool includeChildren = false;

    private bool useAllFilter = false;

    private static readonly Color highlightColor = new Color(0f, 1f, 0.5f, 1f);

    [MenuItem("Tools/Auto Collision Adder")]
    public static void ShowWindow()
    {
        GetWindow<AutoCollisionAdder>("Auto Collision Adder");
    }

    void OnGUI()
    {
        GUILayout.Space(8);
        GUILayout.Label("Auto Collision Adder", new GUIStyle(EditorStyles.boldLabel) { fontSize = 16, alignment = TextAnchor.MiddleCenter });
        GUILayout.Space(4);
        DrawLine();

        GUILayout.Label("Collider Type", EditorStyles.boldLabel);
        selectedCollider = (ColliderType)EditorGUILayout.EnumPopup(selectedCollider);

        GUILayout.Space(8);
        DrawLine();

        GUILayout.Label("Filters", EditorStyles.boldLabel);

        useAllFilter = EditorGUILayout.Toggle("All Objects (no filters)", useAllFilter);

        if (useAllFilter)
        {
            useNameFilter = false;
            useTagFilter = false;
            useLayerFilter = false;
            useSectionFilter = false;
            GUI.enabled = false;
        }

        useNameFilter = EditorGUILayout.Toggle("Use Name Filter", useNameFilter);
        if (useNameFilter)
        {
            nameFilter = EditorGUILayout.TextField("Name Contains", nameFilter);
        }

        useTagFilter = EditorGUILayout.Toggle("Use Tag Filter", useTagFilter);
        if (useTagFilter)
        {
            tagFilter = EditorGUILayout.TagField("Tag", tagFilter);
        }

        useLayerFilter = EditorGUILayout.Toggle("Use Layer Filter", useLayerFilter);
        if (useLayerFilter)
        {
            layerFilter = EditorGUILayout.LayerField("Layer", layerFilter);
        }

        useSectionFilter = EditorGUILayout.Toggle("Filter by Section (3D Box)", useSectionFilter);
        if (useSectionFilter)
        {
            EditorGUILayout.HelpBox(
                "A 3D transparent box will appear in the Scene View. " +
                "Only objects inside this box will be affected.",
                MessageType.Info
            );
        }

        if (useAllFilter)
        {
            GUI.enabled = true;
        }

        GUILayout.Space(8);
        DrawLine();

        GUILayout.Label("Options", EditorStyles.boldLabel);
        includeChildren = EditorGUILayout.Toggle("Include Children", includeChildren);

        GUILayout.Space(12);

        if (GUILayout.Button("Add Colliders to Objects", GUILayout.Height(30)))
        {
            GameObject[] targets = GetFilteredObjects();

            if (targets.Length == 0)
            {
                EditorUtility.DisplayDialog(
                    "No Objects Found",
                    "No objects match the current filters, or they already have a collider.",
                    "OK"
                );
                return;
            }

            bool confirm = EditorUtility.DisplayDialog(
                "Confirm Collider Addition",
                $"{targets.Length} objects will receive {selectedCollider}.\n\nDo you want to continue?",
                "Add Colliders",
                "Cancel"
            );

            if (confirm)
            {
                AddColliders(targets);
            }
        }

        GUILayout.Space(8);
        DrawLine();
        GUILayout.Label("Objects matching filters:", EditorStyles.boldLabel);

        GameObject[] previewTargets = GetFilteredObjects();
        if (previewTargets.Length == 0)
        {
            GUILayout.Label("No objects found.", EditorStyles.miniLabel);
        }
        else
        {
            var scrollStyle = new GUIStyle(EditorStyles.helpBox) { fontSize = 11 };
            using (var scroll = new EditorGUILayout.ScrollViewScope(Vector2.zero, GUILayout.Height(120)))
            {
                foreach (var obj in previewTargets)
                {
                    GUILayout.Label(obj.name, scrollStyle);
                }
            }
            GUILayout.Label($"Total: {previewTargets.Length}", EditorStyles.miniBoldLabel);
        }

        GUILayout.FlexibleSpace();
        DrawLine();
        GUILayout.Label("Autor: ganduspl", new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleRight, fontStyle = FontStyle.Italic }); //credits
    }

    private void DrawLine(int thickness = 1, int padding = 4)
    {
        Rect r = EditorGUILayout.GetControlRect(false, thickness + padding);
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, new Color(0.5f, 0.5f, 0.5f, 1));
    }

    private GameObject[] GetFilteredObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        if (!useAllFilter && !useNameFilter && !useTagFilter && !useLayerFilter && !useSectionFilter)
            return new GameObject[0];

        var filtered = allObjects.Where(obj =>
        {
            if (obj.GetComponent<Collider>() != null) return false;

            if (!useAllFilter)
            {
                if (useNameFilter && !obj.name.Contains(nameFilter)) return false;
                if (useTagFilter && obj.tag != tagFilter) return false;
                if (useLayerFilter && obj.layer != layerFilter) return false;

                if (selectedCollider == ColliderType.MeshCollider)
                {
                    MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                    if (meshFilter == null || meshFilter.sharedMesh == null) return false;
                }

                if (useSectionFilter)
                {
                    Bounds filterBounds = new Bounds(sectionHandle.center, sectionHandle.size);
                    if (!filterBounds.Contains(obj.transform.position)) return false;
                }
            }
            else
            {
                if (selectedCollider == ColliderType.MeshCollider)
                {
                    MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                    if (meshFilter == null || meshFilter.sharedMesh == null) return false;
                }
            }

            return true;
        }).ToList();

        if (includeChildren)
        {
            var children = filtered
                .SelectMany(obj => obj.GetComponentsInChildren<Transform>(true))
                .Select(t => t.gameObject)
                .Where(obj => obj.GetComponent<Collider>() == null)
                .Distinct()
                .ToList();

            filtered.AddRange(children.Where(obj =>
            {
                if (filtered.Contains(obj)) return false;
                if (!useAllFilter)
                {
                    if (useNameFilter && !obj.name.Contains(nameFilter)) return false;
                    if (useTagFilter && obj.tag != tagFilter) return false;
                    if (useLayerFilter && obj.layer != layerFilter) return false;
                    if (selectedCollider == ColliderType.MeshCollider)
                    {
                        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                        if (meshFilter == null || meshFilter.sharedMesh == null) return false;
                    }
                    if (useSectionFilter)
                    {
                        Bounds filterBounds = new Bounds(sectionHandle.center, sectionHandle.size);
                        if (!filterBounds.Contains(obj.transform.position)) return false;
                    }
                }
                else
                {
                    if (selectedCollider == ColliderType.MeshCollider)
                    {
                        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                        if (meshFilter == null || meshFilter.sharedMesh == null) return false;
                    }
                }
                return true;
            }));
        }

        return filtered.Distinct().ToArray();
    }

    private void AddColliders(GameObject[] targets)
    {
        int addedCount = 0;

        foreach (GameObject obj in targets)
        {
            Undo.RecordObject(obj, "Add Collider");
            switch (selectedCollider)
            {
                case ColliderType.BoxCollider:
                    obj.AddComponent<BoxCollider>();
                    addedCount++;
                    break;
                case ColliderType.MeshCollider:
                    obj.AddComponent<MeshCollider>();
                    addedCount++;
                    break;
                case ColliderType.SphereCollider:
                    obj.AddComponent<SphereCollider>();
                    addedCount++;
                    break;
                case ColliderType.CapsuleCollider:
                    obj.AddComponent<CapsuleCollider>();
                    addedCount++;
                    break;
            }
        }

        Debug.Log($"added {addedCount} {selectedCollider}(s).");
        EditorUtility.DisplayDialog("Success", $"Added {addedCount} {selectedCollider}(s).", "OK");
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (useSectionFilter)
        {
            Handles.color = new Color(0f, 1f, 1f, 0.5f);

            using (new Handles.DrawingScope(Matrix4x4.identity))
            {
                if (sectionHandle.size == Vector3.zero) sectionHandle.size = new Vector3(5, 5, 5);
                sectionHandle.DrawHandle();
            }
        }

        GameObject[] previewTargets = GetFilteredObjects();
        foreach (var obj in previewTargets)
        {
            if (obj == null) continue;
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null)
            {
                Handles.color = highlightColor;
                Bounds b = rend.bounds;
                Handles.DrawWireCube(b.center, b.size);
            }
            else
            {
                Handles.color = highlightColor;
                Handles.DrawWireDisc(obj.transform.position, Vector3.up, 0.2f);
            }
        }
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
}
