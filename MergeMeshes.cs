using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

#if UNITY_EDITOR
namespace DearFuture.Object
{
    [RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class MergeMeshes : MonoBehaviour
    {
        [SerializeField] private Material material;
        public void Combine()
        {
            Quaternion oldRot = transform.rotation;
            Vector3 oldPos = transform.position;

            transform.rotation = Quaternion.identity;
            transform.position = Vector3.zero;

            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(true);
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            for (int i = 1; i < meshFilters.Length; i++)
            {
                combine[i].subMeshIndex = 0;
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
            }

            Mesh finalMesh = new Mesh();
            finalMesh.CombineMeshes(combine);
            GetComponent<MeshFilter>().sharedMesh = finalMesh;
            if (TryGetComponent<MeshCollider>(out MeshCollider mc))
            {
                mc.sharedMesh = finalMesh;

            }
            GetComponent<MeshRenderer>().sharedMaterial = material;

            transform.rotation = oldRot;
            transform.position = oldPos;
            string path = "Assets/Resources/Models/Objects/CombinedMeshes/";
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            AssetDatabase.CreateAsset(finalMesh, path + Selection.activeGameObject.name + ".asset" );
            AssetDatabase.SaveAssets();
        }

        public void CombineMeshesWithMultiMatsDelegate()
        {
             Quaternion oldRot = transform.rotation;
            Vector3 oldPos = transform.position;

            transform.rotation = Quaternion.identity;
			transform.position = Vector3.zero;

            List<Material> mList = new List<Material>();
			HashSet<Material> mSet = new HashSet<Material>();
			MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
					
            for(int i = 1; i<meshRenderers.Length; i++)
			{
				var mr = meshRenderers[i];
                if(mr.gameObject.layer.Equals(LayerMask.NameToLayer("GroundTrigger")))
                {
                    continue;
                }
				foreach(var m in mr.sharedMaterials)
				{
					mSet.Add(m);
				}
			}

            GetComponent<MeshRenderer>().sharedMaterials = new Material[mList.Count];

			MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
			Dictionary<int, List<CombineInstance>> helper = new Dictionary<int, List<CombineInstance>>();
			
			for(int i = 1; i<meshFilters.Length; i++)
			{
				var mf = meshFilters[i];
                if(mf.gameObject.layer.Equals(LayerMask.NameToLayer("GroundTrigger")))
                {
                    continue;
                }
				Material localMaterial = mf.GetComponent<MeshRenderer>().sharedMaterial;
				List<CombineInstance> tmp;
				if(!helper.TryGetValue(localMaterial.GetInstanceID(), out tmp))
				{
					tmp = new List<CombineInstance>();
					helper.Add(localMaterial.GetInstanceID(), tmp);
				}

				var ci = new CombineInstance();
				ci.mesh = mf.sharedMesh;
				ci.transform = mf.transform.localToWorldMatrix;
				tmp.Add(ci);
			}

            var list = new List<CombineInstance>();
			foreach (var e in helper) {
				var m = new Mesh();
				m.CombineMeshes(e.Value.ToArray());
				var ci = new CombineInstance();
				ci.mesh = m;
				list.Add(ci);
			}

            var result = new Mesh();
			result.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
			result.CombineMeshes(list.ToArray(), false, false);
			GetComponent<MeshRenderer>().sharedMaterials = mSet.ToArray();
			GetComponent<MeshFilter>().sharedMesh = result;
			GetComponent<MeshCollider>().sharedMesh = result;

			transform.rotation = oldRot;
            transform.position = oldPos;

            string path = "Assets/Resources/Models/Objects/CombinedMeshes/";
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            AssetDatabase.CreateAsset(result, path + "Mesh_" + Selection.activeGameObject.transform.parent.name + ".asset" );
            AssetDatabase.SaveAssets();
        }
    }
}
#endif