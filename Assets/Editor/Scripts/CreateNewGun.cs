using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class CreateNewGun : EditorWindow {

    static EditorWindow window;


    [MenuItem("Tools/Create new Gun")]
    public static void ShowWindow() {
        window = GetWindow(typeof(CreateNewGun));
    }

    //object
    string gunName;
    GameObject model;
    BulletCasing casing;
    AudioClip shootClip;

    //gun stats
    GunNameID gunID;
    float fireRate, impactForce, damage, minDamage, reloadTime, minRange, maxRange, adsZoom, adsSpeed, hipFireBaseSpread, hipFireMaxSpread, recoveryTime, switchInSpeed;
    int magazineSize, penetration, numReloadShells;
    bool isAuto, isShotgun;
    int shots;
    float headShotMult, torsoShotMult;

    //missing required flags
    bool noName = false, noModel = false, noCasing = false, noSound = false;

    private void OnGUI() {
        GUILayout.Label("\nCreate New Gun", EditorStyles.boldLabel);
        gunName = EditorGUILayout.TextField("Gun Name", gunName);
        model = EditorGUILayout.ObjectField("Gun Model", model, typeof(GameObject), false) as GameObject;
        casing = EditorGUILayout.ObjectField("Casing Model", casing, typeof(BulletCasing), false) as BulletCasing;
        shootClip = EditorGUILayout.ObjectField("Casing Model", casing, typeof(AudioClip), false) as AudioClip;

        GUILayout.Label("\nGun Stats", EditorStyles.boldLabel);
        gunID = (GunNameID) EditorGUILayout.EnumPopup("Gun ID", gunID);
        fireRate = EditorGUILayout.FloatField("Firerate", fireRate);
        damage = EditorGUILayout.FloatField("Damage", damage);
        minDamage = EditorGUILayout.FloatField("Min Damage", minDamage);
        reloadTime = EditorGUILayout.FloatField("Reload", reloadTime);
        minRange = EditorGUILayout.FloatField("Min Range", minRange);
        maxRange = EditorGUILayout.FloatField("Max Range", maxRange);
        adsZoom = EditorGUILayout.FloatField("ADS Zoom", adsZoom);
        adsSpeed = EditorGUILayout.FloatField("ADS Speed", adsSpeed);
        hipFireBaseSpread = EditorGUILayout.FloatField("Hip Fire Base Spread", hipFireBaseSpread);
        hipFireMaxSpread = EditorGUILayout.FloatField("Hip Fire Max Spread", hipFireMaxSpread);
        recoveryTime = EditorGUILayout.FloatField("Recovery Time", recoveryTime);
        switchInSpeed = EditorGUILayout.FloatField("Switch In Speed", switchInSpeed);
        headShotMult = EditorGUILayout.FloatField("Headshot Multiplier", headShotMult);
        torsoShotMult = EditorGUILayout.FloatField("Torso Multiplier", torsoShotMult);

        magazineSize = EditorGUILayout.IntField("Magazine Size", magazineSize);
        penetration = EditorGUILayout.IntField("Penetration", penetration);
        numReloadShells = EditorGUILayout.IntField("Num Shells To Reload", numReloadShells);
        shots = EditorGUILayout.IntField("Pellets Per Shot", shots);

        isAuto = EditorGUILayout.Toggle("Is Auto", isAuto);
        isShotgun = EditorGUILayout.Toggle("Is Shotgun", isShotgun);

        if(noName || noModel || noCasing || noSound) {
            GUI.color = Color.red;
            GUILayout.Label("***************************************************************************************************************", EditorStyles.popup);
            GUI.color = Color.white; 
        }
        if(noName) {
            GUI.color = Color.red;
            GUILayout.Label("**gun name required**");
            GUI.color = Color.white;
        }
        if(noModel) {
            GUI.color = Color.red;
            GUILayout.Label("**gun model required**");
            GUI.color = Color.white;
        }
        if(noCasing) {
            GUI.color = Color.red;
            GUILayout.Label("**casing model required**");
            GUI.color = Color.white;
        }
        if(noSound) {
            GUI.color = Color.red;
            GUILayout.Label("**audio clip required**");
            GUI.color = Color.white;
        }

        if(GUILayout.Button("Create Gun!")) {
            if(string.IsNullOrEmpty(gunName))
                noName = true;
            else
                noName = false;

            if(model == null)
                noModel = true;
            else
                noModel = false;

            if(casing == null)
                noCasing = true;
            else
                noCasing = false;

            if(shootClip == null)
                noSound = true;
            else
                noSound = false;

            if(noName || noModel || noCasing || noSound)
                return;

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
        g.AddComponent<Recoil>();
        Gun gun = g.AddComponent<Gun>();

        //set gun stats
        gun.gunID = gunID;
        gun.SetStats(fireRate, damage, minDamage, reloadTime, minRange, maxRange, adsZoom, adsSpeed, hipFireBaseSpread, hipFireMaxSpread, recoveryTime,
            switchInSpeed, magazineSize, penetration, numReloadShells, isAuto, isShotgun, upgraded, shootClip.name, shots, headShotMult, torsoShotMult, gunID, casing);

        //Create sound in AudioManager
        var am = FindObjectOfType<AudioManager>();
        Sound[] sounds = new Sound[am.sounds.Length + 1];
        for(int i = 0; i < sounds.Length - 1; i++) {
            sounds[i] = am.sounds[i];
        }
        sounds[sounds.Length] = new Sound();
        am.sounds = sounds;

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