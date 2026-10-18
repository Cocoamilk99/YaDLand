using UnityEngine;
using UnityEditor;

public class AutoAddMeshCollider : EditorWindow
{
    [MenuItem("Tools/Add Mesh Collider to All Meshes in Scene")]
    public static void AddMeshColliders()
    {
        // ค้นหา GameObject ทั้งหมดที่มี MeshFilter ในฉาก
        MeshFilter[] meshFilters = FindObjectsByType<MeshFilter>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int addedCount = 0;

        foreach (MeshFilter mf in meshFilters)
        {
            // ตรวจสอบว่าวัตถุนั้นมี Collider หรือยัง
            if (mf.GetComponent<Collider>() == null)
            {
                // เพิ่ม MeshCollider เข้าไป
                MeshCollider mc = mf.gameObject.AddComponent<MeshCollider>();
                mc.sharedMesh = mf.sharedMesh;
                addedCount++;
            }
        }

        // แสดงกล่องข้อความแจ้งเตือนผลลัพธ์
        EditorUtility.DisplayDialog("Complete", $"เพิ่ม Mesh Collider เรียบร้อยแล้ว ทั้งหมด {addedCount} ชิ้น!", "OK");
    }
}