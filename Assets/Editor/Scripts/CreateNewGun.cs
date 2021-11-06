using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class CreateNewGun : EditorWindow {

    static EditorWindow window;


    [MenuItem("Tools/Create new Gun")]
    public static void ShowWindow() {
        window = GetWindow(typeof(CreateNewGun));
    }

    string gunName;
    GameObject model;

    bool noName = false, noModel = false;

    private void OnGUI() {
        GUILayout.Label("Create New Gun", EditorStyles.boldLabel);
        gunName = EditorGUILayout.TextField("Gun Name", gunName);
        model = EditorGUILayout.ObjectField("Gun Model", model, typeof(GameObject), false) as GameObject;

        if(noName) {
            GUI.color = Color.red;
            GUILayout.Label("**gun name required**\n");
            GUI.color = Color.white;
        }
        if(noModel) {
            GUI.color = Color.red;
            GUILayout.Label("**gun model required**\n");
            GUI.color = Color.white;
        }

        if(GUILayout.Button("Create Gun!")) {
            if(string.IsNullOrEmpty(gunName)) {
                noName = true;
                if(model != null) {
                    return;
                }
            }
            else
                noName = false;

            if(model == null) {
                noModel = true;
                return;
            }
            else
                noModel = false;

            Debug.LogWarning("TODO:\n" +
                "Add firing position with muzzle flash and link\n" +
                "Add eject port and link");

            Finish();
        }
    }

    void Finish() {
        window.Close();
        GameObject g = CreateGun(false);
        CreateGun(true);
        CreateWallGun(g);
    }


    AnimatorController controller;
    GameObject CreateGun(bool upgraded) {
        string gName = gunName;
        if(upgraded)
            gName += "_Upgraded";

        //Create Base Object with Gun Component
        var g = new GameObject(gName);
        g.AddComponent<Gun>();
        g.AddComponent<Recoil>();

        //Create Holder Transform and link with base object
        var g_holder = new GameObject(gName);
        g_holder.transform.SetParent(g.transform);
        Animator anim = g_holder.AddComponent<Animator>();

        //Create new Animator and store in Assets ***if gun is upgraded type, just copy from non upgraded version of gun***
        if(!upgraded) {
            AssetDatabase.CreateFolder("Assets/Animations", gName);
            controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Animations/" + gName + "/" + gName + ".controller");
            controller.AddParameter("Reload", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Fire", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Melee", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Switch", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("IsRunning", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        }
        anim.runtimeAnimatorController = controller;

        //Create Model holder Transform and link with holder
        var g_model = new GameObject("Model");
        g_model.transform.SetParent(g_holder.transform);
        g_model.transform.rotation = Quaternion.Euler(0, 90, 0);

        //Insert actual gun model into model holder
        var m = Instantiate(model, g_model.transform);
        m.transform.localScale = Vector3.one;

        //Store new gun as asset
        if(!upgraded)
            return PrefabUtility.SaveAsPrefabAsset(g, "Assets/Prefabs/Guns/" + gName + ".prefab");
        else
            return PrefabUtility.SaveAsPrefabAsset(g, "Assets/Prefabs/Upgraded Guns/" + gName + ".prefab");
    }

    void CreateWallGun(GameObject gun) {
        //Create Wallgun object of gunName name and link under wallgun manager object in scene
        var wgm = FindObjectOfType<WallGunManager>();
        string gName = gunName + "_WallGun";
        GameObject g = new GameObject(gName);
        g.transform.SetParent(wgm.transform);

        //Add wallgun comp and set known variables
        var wg = g.AddComponent<WallGun>();
        wg.gunName = gunName;
        wg.gunModel = gun;

        //Add and link gun model
        var g_model = Instantiate(model, g.transform);
        g_model.transform.localScale = Vector3.one;
        g_model.transform.rotation = Quaternion.Euler(0, 90, 0);

        //Save wallgun as prefab
        PrefabUtility.SaveAsPrefabAsset(g, "Assets/Prefabs/Wall Guns/" + gName + ".prefab");
    }
}