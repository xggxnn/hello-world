using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
using UnityEditor.Animations;
using Games;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class FBXPrefab : EditorWindow
{
    [MenuItem("LYJ/EditorPrefab")]
    public static void Open()
    {
        EditorWindow.GetWindow<FBXPrefab>();
    }

    [MenuItem("LYJ/模型测试场景")]
    static void Go0001()
    {
        EditorApplication.SaveCurrentSceneIfUserWantsTo();
        EditorApplication.OpenScene("Assets/Game/Scenes/Test.unity");
    }

    [MenuItem("LYJ/战斗测试场景")]
    static void Go0002()
    {
        EditorApplication.SaveCurrentSceneIfUserWantsTo();
        EditorApplication.OpenScene("Assets/Game/Scenes/War-Debug.unity");
    }

    [MenuItem("LYJ/主场景入口")]
    static void Go0003()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Game/Scenes/Launch.unity");
        //EditorApplication.SaveCurrentSceneIfUserWantsTo();
        //EditorApplication.OpenScene("Assets/Game/Scenes/Launch.unity");
    }

    //[MenuItem("LYJ/Select/主场景入口")]
    //static void Go0004()
    //{
    //    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
    //    EditorSceneManager.OpenScene("Assets/Game/Scenes/Launch.unity");
    //    //EditorApplication.SaveCurrentSceneIfUserWantsTo();
    //    //EditorApplication.OpenScene("Assets/Game/Scenes/Launch.unity");
    //}

    #region 动态生成基本数据
    private bool PlayOrTown = false;
    private GameObject Root = null;
    private string RootName = "root";
    private GameObject FbxObject = null;
    private Transform[] mAllChildren;
    private string Title = "Prefab生成器";
    #endregion

    //void OnGUI()
    //{
    //    EditorGUILayout.Space();
    //    EditorGUILayout.Space();
    //    EditorGUILayout.BeginHorizontal();
    //    GUI.color = Color.red;
    //    if (Root == null)
    //    {
    //        Title = "Prefab生成器";
    //    }
    //    else
    //    {
    //        if (PlayOrTown)
    //        {
    //            Title = "英雄Prefab生成器";
    //        }
    //        else
    //        {
    //            Title = "机关Prefab生成器";
    //        }
    //    }
    //    GUILayout.Button(Title);
    //    GUI.color = Color.white;
    //    EditorGUILayout.EndHorizontal();
    //    EditorGUILayout.Space();
    //    EditorGUILayout.Space();
    //    if (Root == null)
    //    {
    //        GUILayout.BeginHorizontal();
    //        if (GUILayout.Button("生成英雄根节点"))
    //        {
    //            PlayOrTown = true;
    //            Root = new GameObject();
    //            CharacterController _CC = Root.AddComponent<CharacterController>();
    //            _CC.center = new Vector3(0, 2, 0);
    //            _CC.radius = 1.2F;
    //            _CC.height = 4.2F;
    //            UnityEngine.AI.NavMeshAgent _NMA = Root.AddComponent<UnityEngine.AI.NavMeshAgent>();
    //            _NMA.speed = 12;
    //            _NMA.acceleration = 80;
    //            _NMA.angularSpeed = 720;
    //            _NMA.stoppingDistance = 0.01F;
    //            Object _PFT = Resources.Load("PrefabFx/Other/PlayFowardTip");
    //            GameObject _PFTObj = Instantiate(_PFT) as GameObject;
    //            _PFTObj.transform.SetParent(Root.transform);
    //            _PFTObj.transform.localEulerAngles = Vector3.zero;
    //            _PFTObj.transform.localPosition = Vector3.zero;
    //            _PFTObj.transform.localScale = Vector3.one;
    //            _PFTObj.name = "PlayFowardTip";
    //            Object _Arr = Resources.Load("PrefabFx/Other/Arrow");
    //            GameObject _ArrObj = Instantiate(_Arr) as GameObject;
    //            _ArrObj.transform.SetParent(Root.transform);
    //            _ArrObj.transform.localEulerAngles = Vector3.zero;
    //            _ArrObj.transform.localPosition = Vector3.zero;
    //            _ArrObj.transform.localScale = Vector3.one;
    //            _ArrObj.name = "Arrow";
    //            _ArrObj.SetActive(false);
    //            Root.name = "Root";
    //        }
    //        if (GUILayout.Button("生成机关根节点"))
    //        {
    //            Root = new GameObject();
    //            Root.name = "Root";
    //        }
    //        GUILayout.EndHorizontal();
    //    }
    //    if (Root != null)
    //    {
    //        if (PlayOrTown)
    //        {
    //            PlayDraw();
    //        }
    //    }
    //}

    //private void    PlayDraw()
    //{
    //    GUILayout.BeginHorizontal();
    //    RootName = GUILayout.TextField(RootName);
    //    if (GUILayout.Button("<- 修改名称"))
    //    {
    //        Root.name = RootName;
    //    }
    //    EditorGUILayout.LabelField("Root: " + Root.name);
    //    GUILayout.EndHorizontal();
    //    EditorGUILayout.Space();
    //    EditorGUILayout.Space();
    //    GUILayout.BeginHorizontal();
    //    if (FbxObject == null)
    //    {
    //        if (GUILayout.Button("选择fbx,点击此按钮加载"))
    //        {
    //            Object _o = Selection.activeObject;
    //            if (_o != null)
    //            {
    //                players _players = Root.AddComponent<players>();
    //                FbxObject = Instantiate(_o) as GameObject;
    //                FbxObject.transform.SetParent(Root.transform);
    //                FbxObject.transform.localEulerAngles = Vector3.zero;
    //                FbxObject.transform.localPosition = Vector3.zero;
    //                FbxObject.transform.localScale = Vector3.one;
    //                RootName = _o.name;
    //                FbxObject.name = _o.name;
    //                _players.m_AniMg = FbxObject.AddComponent<AnimatorManager>();
    //                _players.m_charController = Root.GetComponent<CharacterController>();
    //            }
    //        }
    //    }
    //    else
    //    {
    //        if (GUILayout.Button("保存结果"))
    //        {
    //            CreatePrefabObj(Root, FbxObject.name);
    //        }
    //    }

    //    GUILayout.EndHorizontal();
    //}

    private void CreatePrefabObj(GameObject obj, string prefabName)
    {
        string[] str = prefabName.Split('(');
        string path = str[0] + ".prefab";
        if (PlayOrTown)
        {
            path = "Assets/Game/Resources/PrefabFx/Play/" + path;
        }
        else
        {
            path = "Assets/Game/Resources/PrefabFx/Organ/" + path;
        }
        PrefabUtility.CreatePrefab(path, obj);
    }

    private void ModifyUIName(string proName)
    {
        Selection.activeGameObject.name = proName;
    }


    [MenuItem("LYJ/CloseMeshCollider")]
    static void CloseMesh()
    {
        GameObject _o = (GameObject)Selection.activeObject;
        if (_o != null)
        {
            Transform[] children =   _o.GetComponentsInChildren<Transform>(true);
            foreach (Transform item in children)
            {
                MeshCollider meshCollider = item.GetComponent <MeshCollider>();
                if (meshCollider != null)
                {
                    meshCollider.enabled = false;
                }
            }
        }
    }

    static Object[] targetObjs;
    [MenuItem("LYJ/EditSprite &S", false, 100)]
    static void EditTextureToSprite()
    {
        targetObjs = Selection.objects;
        string _Str = "";
        if (targetObjs[0])
        {
            string path = AssetDatabase.GetAssetPath(targetObjs[0]);
            _Str = Path.GetFileName(Path.GetDirectoryName(path));
        }
        for (int i = 0; i < targetObjs.Length; i++)
        {
            if (targetObjs[i] && targetObjs[i] is Texture)
            {
                string path = AssetDatabase.GetAssetPath(targetObjs[i]);
                TextureImporter texture = AssetImporter.GetAtPath(path) as TextureImporter;
                texture.textureType = TextureImporterType.Sprite;
                texture.spriteImportMode = SpriteImportMode.Single;
                texture.spritePixelsPerUnit = 100;
                texture.spritePackingTag = _Str;
                texture.mipmapEnabled = false;
                texture.filterMode = FilterMode.Trilinear;
                AssetDatabase.ImportAsset(path);
            }
        }
    }


    [MenuItem("LYJ/ButtonAni", false, 101)]
    static void AddButtonAnimator()
    {
        GameObject _o = Selection.activeGameObject;
        if (_o != null)
        {
            Transform[] children = _o.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform item in children)
            {
                Button b = item.GetComponent<Button>();
                if (b != null)
                {
                    if (b.transition != Selectable.Transition.SpriteSwap)
                    {
                        b.transition = Selectable.Transition.Animation;
                        Animator _ani = item.GetComponent<Animator>();
                        if (_ani == null)
                        {
                            _ani = item.gameObject.AddComponent<Animator>();
                        }
                        _ani.runtimeAnimatorController = Resources.Load<AnimatorController>("PrefabUI/UI/Button"); ;
                    }
                }
            }
            Loger.Log("Ok");
        }
        else
        {
            Loger.LogError("Please Select a GameObject");
        }
    }

    static Object aniObjs;
    //[MenuItem("LYJ/ChangeAni", false, 102)]
    //static void ChangeAnimator()
    //{
    //    aniObjs = Selection.activeObject;
    //    string fullPath = "";
    //    if (aniObjs)
    //    {
    //        fullPath = Path.GetFullPath(AssetDatabase.GetAssetPath(aniObjs));
    //        string path = AssetDatabase.GetAssetPath(aniObjs);
    //        if (Directory.Exists(fullPath))
    //        {
    //            DirectoryInfo direction = new DirectoryInfo(fullPath);
    //            FileInfo[] files = direction.GetFiles("*",SearchOption.TopDirectoryOnly);
    //            for (int i = 0; i < files.Length; i++)
    //            {
    //                if (files[i].Name.EndsWith(".FBX") || files[i].Name.EndsWith(".fbx"))
    //                {
    //                    string result = files[i].Name.Remove(files[i].Name.LastIndexOf("."));
    //                    if (!result.Contains("_"))
    //                    {
    //                        continue;
    //                    }
    //                    result = result.Remove(0, result.IndexOf("_") + 1);
    //                    result = result.Remove(0, result.IndexOf("_") + 1);
    //                    result = result.Remove(0, result.IndexOf("_") + 1);
    //                    result = result.Substring(0, 1).ToUpper() + result.Substring(1);
    //                    string realPath = path + "/" + files[i].Name;
    //                    ModelImporter modelImporter = AssetImporter.GetAtPath(realPath) as ModelImporter;
    //                    bool loop = false;
    //                    if (result.Equals("Run") || result.Equals("Idle") || result.Equals("Stun") || result.Equals("Idle2") || result.Equals("RunIdle") || result.Equals("RunD") || result.Equals("RundU") || result.Equals("RunL") || result.Equals("RunR"))
    //                    {
    //                        loop = true;
    //                    }
    //                    LLL.LL(files[i].Name + "    修改为：   " + result + (loop ? "   循环播放" : "   一次性"));
    //                    ModelImporterClipAnimation tempClip = modelImporter.defaultClipAnimations[0];
    //                    clipArrayListCreater creater = new clipArrayListCreater();
    //                    creater.addClip(result, (int)tempClip.firstFrame, (int)tempClip.lastFrame, loop, tempClip.wrapMode);
    //                    modelImporter.clipAnimations = creater.getArray();

    //                    AssetDatabase.ImportAsset(realPath);
    //                }
    //            }
    //            LLL.LL("请在死亡动作添加event 555","red");
    //            AssetDatabase.Refresh();
    //        }
    //    }
    //}
}

public class clipArrayListCreater
{
    private List<ModelImporterClipAnimation> clipList = new List<ModelImporterClipAnimation>();
    public void addClip(string name, int firstFrame, int lastFrame, bool loop, WrapMode wrapMode)
    {
        ModelImporterClipAnimation tempClip = new ModelImporterClipAnimation();
        tempClip.name = name;
        tempClip.firstFrame = firstFrame;
        tempClip.lastFrame = lastFrame;
        tempClip.loopTime = loop;
        tempClip.wrapMode = wrapMode;
        //if (name.Equals("Death"))
        //{
        //    AnimationEvent evt = new AnimationEvent();
        //    evt.time = tempClip.lastFrame * 0.85F;
        //    evt.functionName = "NewEvent";
        //    evt.intParameter = 555;
        //    tempClip.events = new AnimationEvent[1];
        //    tempClip.events[0] = evt;
        //}
        clipList.Add(tempClip);
    }

    public ModelImporterClipAnimation[] getArray()
    {
        return clipList.ToArray();
    }
}