// ================================================================================================================================
// File:        ExportNavMesh.cs
// Description: Exports the navmesh as a .obj file, after converting that the navmesh can be loaded in the server
//              Obj exporter component based on: http://wiki.unity3d.com/index.php?title=ObjExporter
// ================================================================================================================================

using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

//For some reason the project will not build when this script is included, so ive added this if clause to only compile this script in the unity editor
#if (UNITY_EDITOR)
public class ExportNavMesh : MonoBehaviour
{
    [MenuItem("Custom/Export NavMesh to mesh")]
    static void Export()
    {
        NavMeshTriangulation triangulatedNavMesh = NavMesh.CalculateTriangulation();

        Mesh mesh = new Mesh();
        mesh.name = "ExportedNavMesh";
        mesh.vertices = triangulatedNavMesh.vertices;
        mesh.triangles = triangulatedNavMesh.indices;
        string filename = Application.dataPath + "/" + Path.GetFileNameWithoutExtension(EditorApplication.currentScene) + " Exported NavMesh.obj";
        MeshToFile(mesh, filename);
        print("NavMesh exported as '" + filename + "'");
        AssetDatabase.Refresh();
    }

    static string MeshToString(Mesh mesh)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("g ").Append(mesh.name).Append("\n");
        foreach (Vector3 v in mesh.vertices)
        {
            sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in mesh.normals)
        {
            sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in mesh.uv)
        {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }
        for (int material = 0; material < mesh.subMeshCount; material++)
        {
            sb.Append("\n");

            int[] triangles = mesh.GetTriangles(material);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
            }
        }
        return sb.ToString();
    }

    static void MeshToFile(Mesh mesh, string filename)
    {
        using (StreamWriter sw = new StreamWriter(filename))
        {
            sw.Write(MeshToString(mesh));
        }
    }
}
#endif