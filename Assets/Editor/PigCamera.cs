using UnityEditor;
using UnityEngine;

public class PigCamera {
    [MenuItem("Pig/Assign Camera")]
    static void AssignCamera() {
        var camera = UnityEditor.SceneView.lastActiveSceneView.camera;
        Debug.Log(camera.transform.position);
        
        RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out hit)) {
/*
            Debug.Log(hit.point);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = hit.point;
*/
            var pig = GameObject.FindObjectOfType<PigController>();
            var v = hit.point;
            v.y = 1.5f;
            pig.transform.position = v;
        }
    }
}
