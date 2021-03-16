using UnityEditor;
using UnityEngine;
using DearFuture.Object;

[CustomEditor(typeof(MergeMeshes))]
public class MeshMergeEditor : Editor
{
    	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		MergeMeshes meshMerger = (MergeMeshes)target;
        if(GUILayout.Button("Merge"))
        {
            meshMerger.Combine();
        }

        if(GUILayout.Button("Merge Multi Mats"))
        {
            meshMerger.CombineMeshesWithMultiMatsDelegate();
        }
        
	}
}


