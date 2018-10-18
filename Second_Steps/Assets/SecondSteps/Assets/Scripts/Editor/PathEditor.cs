using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    private Path path;
    

    private void Awake()
    {
        path = (Path)target;
        path.Initialize();
    }
    private void OnEnable()
    {
        path = (Path)target;

    }
    public override void OnInspectorGUI()
    {

    }
    private void OnSceneGUI()
    {

    }

    
}