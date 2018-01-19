using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Animations;
using System.Collections.Generic;

public class AutoAni : AssetPostprocessor
{
    /// <summary>
    /// 选择文件夹
    /// </summary>
    static Object aniObjs;
    /// <summary>
    /// 替换的材质
    /// </summary>
    static Shader myShader = null;
    [MenuItem("LYJ/AutoAni", false, 103)]
    static void AutoCreateAni()
    {
        aniObjs = null;
        aniObjs = Selection.activeObject;
        if (aniObjs != null)
        {
            myShader = Shader.Find("Custom/PlayerDiffuse");

            string fullPath = Path.GetFullPath(AssetDatabase.GetAssetPath(aniObjs));    //"E:/fdsfd/"
            string path = AssetDatabase.GetAssetPath(aniObjs);                          //"Asset/"
            if (Directory.Exists(fullPath))                                             // 文件存在
            {
                string fbxpath = "";
                List<AnimationClip> clips = new List<AnimationClip>();
                DirectoryInfo direction = new DirectoryInfo(fullPath);
                FileInfo[] files = direction.GetFiles("*",SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    string fileName = files[i].Name.ToLower();
                    if (fileName.EndsWith(".mat"))
                    {
                        //修改材质球shader
                        string fullPath2 = files[i].DirectoryName.Replace('\\', '/');
                        fullPath2 = fullPath2.Remove(0, fullPath2.LastIndexOf("Assets"));
                        string realPath = fullPath2 + "/" + files[i].Name.Remove(files[i].Name.LastIndexOf("."));
                        Material go = AssetDatabase.LoadAssetAtPath(fullPath2 + "/" + files[i].Name, typeof(Material)) as Material;
                        go.shader = myShader;
                        Debug.Log("<color=green>材质球修改完毕</color>");
                    }
                    else if (fileName.EndsWith(".fbx"))
                    {
                        string result = files[i].Name.Remove(files[i].Name.LastIndexOf("."));
                        if (!result.Contains("_"))
                        {
                            fbxpath = files[i].Name;
                            continue;
                        }
                        result = result.Remove(0, result.IndexOf("_") + 1);
                        result = result.Remove(0, result.IndexOf("_") + 1);
                        result = result.Remove(0, result.IndexOf("_") + 1);
                        result = result.Substring(0, 1).ToUpper() + result.Substring(1);
                        string realPath = path + "/" + files[i].Name;
                        ModelImporter modelImporter = AssetImporter.GetAtPath(realPath) as ModelImporter;
                        bool loop = false;
                        if (result.Equals("Run") || result.Equals("Idle") || result.Equals("Idle1") || result.Equals("Stun") || result.Equals("Idle2") || result.Equals("RunIdle") || result.Equals("RunD") || result.Equals("RunT") || result.Equals("RunU") || result.Equals("RunL") || result.Equals("RunR") || result.Equals("Fix"))
                        {
                            loop = true;
                        }
                        ModelImporterClipAnimation tempClip = modelImporter.defaultClipAnimations[0];
                        clipArrayListCreater creater = new clipArrayListCreater();
                        creater.addClip(result, (int)tempClip.firstFrame, (int)tempClip.lastFrame, loop, tempClip.wrapMode);
                        modelImporter.clipAnimations = creater.getArray();
                        clips.Add(AssetDatabase.LoadAssetAtPath(realPath, typeof(AnimationClip)) as AnimationClip);
                        AssetDatabase.ImportAsset(realPath);
                    }
                }
                CreateAniController(path + "/" + aniObjs.name, clips, path + "/" + fbxpath);
                AssetDatabase.Refresh();
                Debug.Log("<color=red>AutoOver</color>");
                aniObjs = null;
            }
        }
        else
        {
            if (EditorUtility.DisplayDialog("", "请选择project面板下fbx文件所在的文件夹", "确定"))
            {
                return;
            }
        }
    }

    static void CreateAniController(string path, List<AnimationClip> clips, string fbxPath)
    {
        // 创建动画控制器
        AnimatorController   animatorController = AnimatorController.CreateAnimatorControllerAtPath(path + "Controller.controller");
        AnimatorControllerLayer layer = null;
        if (animatorController.layers.Length > 0)
        {
            layer = animatorController.layers[0];
        }
        AnimatorStateMachine layMachine = layer.stateMachine;
        Vector3 anyStatePosition = layMachine.anyStatePosition;

        float OFFSET_X = 220;
        float OFFSET_Y = 60;
        float ITEM_PER_LINE = 4;
        float originX = anyStatePosition.x - OFFSET_X * (ITEM_PER_LINE / 2.5f);
        float originY = -20;
        float x = originX;
        float y = originY;

        layMachine.anyStatePosition = new Vector3(-150, -150, 0);
        layMachine.entryPosition = new Vector3(150, -150, 0);

        UnityEditor.Animations.AnimatorState defAni = null;
        // 设置默认动作
        AnimationClip defClip = clips.Find(m => m.name.Equals("Idle"));
        if (defClip != null)
        {
            UnityEditor.Animations.AnimatorState state = layMachine.AddState(defClip.name,new Vector3(0, -100, 0));
            state.motion = defClip;
            layMachine.defaultState = state;
            defAni = state;
        }
        else
        {
            if (EditorUtility.DisplayDialog("", "未发现idle动作，请检查", "确定"))
            {
                /*return*/
            }
        }

        foreach (AnimationClip newClip in clips)
        {
            if (newClip.name.Equals("Idle"))
            {
                continue;
            }
            UnityEditor.Animations.AnimatorState state = layMachine.AddState(newClip.name,new Vector3(x, y, 0));
            state.motion = newClip;

            if (defAni != null)
            {
                if (newClip.name.Equals("Hit") || newClip.name.Equals("Attack") || newClip.name.Equals("Stun") || newClip.name.Equals("Skill1") || newClip.name.Equals("Skill2") || newClip.name.Equals("Skill3") || newClip.name.Equals("Skill4") || newClip.name.Equals("Skill5") || newClip.name.Equals("Skill6") || newClip.name.Equals("Skill7") || newClip.name.Equals("Attack1") || newClip.name.Equals("Attack2") || newClip.name.Equals("Attack3") || newClip.name.Equals("Attack4") || newClip.name.Equals("Attack5") || newClip.name.Equals("Attack6"))
                {
                    AnimatorStateTransition ast = state.AddTransition(defAni);
                    ast.hasExitTime = true;
                    ast.exitTime = 0.99F;
                    ast.duration = 0.1F;
                    ast.offset = 0;
                }
            }
            x += OFFSET_X;
            if (x >= originX + OFFSET_X * ITEM_PER_LINE)
            {
                x = originX;
                y += OFFSET_Y;
            }
        }
        // 创建prefab
        string name = aniObjs.name;
        string prefabPath = "Assets/Game/Resources/PrefabUnit/" + aniObjs.name + ".prefab";
        GameObject go = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath(fbxPath, typeof(GameObject)) as GameObject);
        if (go == null) return;


        go.name = name;
        Animator animator = go.GetComponent<Animator>();
        if (animator == null)
        {
            animator = go.AddComponent<Animator>();
        }
        AnimatorManager aniManager = go.AddComponent<AnimatorManager>();
        // 攻击点
        GameObject shotPos = new GameObject();
        shotPos.name = "ShotPos";
        shotPos.transform.SetParent(go.transform);
        shotPos.transform.localEulerAngles = Vector3.zero;
        shotPos.transform.localPosition = Vector3.zero;
        shotPos.transform.localScale = Vector3.one;
        aniManager.ShotPos = shotPos.transform;

        // 攻击点
        GameObject bloodPos = new GameObject();
        bloodPos.name = "BloodPos";
        bloodPos.transform.SetParent(go.transform);
        bloodPos.transform.localEulerAngles = Vector3.zero;
        bloodPos.transform.localPosition = new Vector3(0, 4, 0);
        bloodPos.transform.localScale = Vector3.one;
        // 受击点
        GameObject anchorShotBy = new GameObject();
        anchorShotBy.name = "AnchorShotBy";
        anchorShotBy.transform.SetParent(go.transform);
        anchorShotBy.transform.localEulerAngles = Vector3.zero;
        anchorShotBy.transform.localPosition = new Vector3(0, 4, 0);
        anchorShotBy.transform.localScale = Vector3.one;

        animator.applyRootMotion = false;
        animator.runtimeAnimatorController = animatorController;
        GameObject resObj = PrefabUtility.CreatePrefab(prefabPath, go);
        GameObject.DestroyImmediate(go);
        AssetDatabase.SaveAssets();

        if (EditorUtility.DisplayDialog("", "请调整攻击点，血条，受击点位置，prefab位置：" + prefabPath, "确定"))
        {
            /*return*/
            Debug.Log("<color=red>" + prefabPath + "</color>");
        }
    }
}