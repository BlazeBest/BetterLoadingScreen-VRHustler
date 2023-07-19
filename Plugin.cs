using _IL2CPP;
using BManager.Attributes;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;

namespace BetterLoadingScreen
{
    [BManager.Attributes.ModuleInfo("BetterLoadingScreen", "1.0", "Grummus | Remake for VRHustler: BlazeBest")]
    unsafe public class Plugin : BManager.VRModule
    {
        public static GameObject gameObject = null;
        public static void Main()
        {

            gameObject = new GameObject("BLC");
            gameObject.RegisterCustomComponent<BetterLoadingScreen>();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
        }

    }

    public class BetterLoadingScreen : MonoBehaviour
    {
        public BetterLoadingScreen() : base(IntPtr.Zero) { }

        public static bool isDownloading = false;

        public void Update()
        {
            if (!File.Exists("BlazeEngine/loading.assetbundle"))
            {
                if (isDownloading) return;
                using (WebClient webClient = new WebClient())
                {
                    isDownloading = true;
                    webClient.BaseAddress = "https://www.icefrag.ru";
                    webClient.DownloadFile("/openfiles/loading.assetbundle", "BlazeEngine/loading.assetbundle");
                }
                isDownloading = false;
                if (!File.Exists("BlazeEngine/loading.assetbundle"))
                {
                    Destroy();
                    return;
                }
            }

            // --- Find Objects
            Transform UserInterface = VRCUiManager.Instance?.transform;
            if (UserInterface == null) return;
            // Console.WriteLine(UserInterface.gameObject.name);

            Transform transform2 = UserInterface.Find("MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/SkyCube_Baked");
            Transform transform3 = UserInterface.Find("LoadingBackground_TealGradient_Music/SkyCube_Baked");
            if (transform2 == null || transform3 == null) return;
            // --- Loading Asset
            assets = AssetBundle.LoadFromFile("BlazeEngine/loading.assetbundle");
            assets.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            /*
            string[] texts = assets.GetAllScenePaths();
            foreach (string text in texts)
            {
                Console.WriteLine("- " + text);
            }
            */
            // Destroy();

            /*
            - assets/bundle/loadingbackground.prefab
            - assets/bundle/login.prefab
             */
            loadScreenPrefab = assets.LoadAsset<GameObject>("assets/bundle/loadingbackground.prefab");
            loadScreenPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;

            loginPrefab = assets.LoadAsset<GameObject>("assets/bundle/login.prefab");
            loginPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;

            // OldLoadingScreenSettings.RegisterSettings();
            try
            {
                CreateGameObjects();
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            Destroy();
        }


        private void CreateGameObjects()
        {
            Transform UserInterface = VRCUiManager.Instance.transform;
            if (UserInterface == null) return;
            Console.WriteLine("Finding original GameObjects");
            // First Error
            var SkyCube = UserInterface.Find("MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/SkyCube_Baked").gameObject;
            var bubbles = UserInterface.Find("MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles").gameObject;
            var loginBubbles = UserInterface.Find("LoadingBackground_TealGradient_Music/_FX_ParticleBubbles").gameObject;
            var originalStartScreenAudio = UserInterface.Find("LoadingBackground_TealGradient_Music/LoadingSound").gameObject;
            var originalStartScreenSkyCube = UserInterface.Find("LoadingBackground_TealGradient_Music/SkyCube_Baked").gameObject;
            var originalLoadingAudio = UserInterface.Find("MenuContent/Popups/LoadingPopup/LoadingSound").gameObject;

            Console.WriteLine("Creating new GameObjects");
            loadScreenPrefab = CreateGameObject(loadScreenPrefab, new Vector3(400, 400, 400), UserInterface.Find("MenuContent/Popups/"), "LoadingPopup");
            loginPrefab = CreateGameObject(loginPrefab, new Vector3(0.5f, 0.5f, 0.5f), UserInterface, "LoadingBackground_TealGradient_Music");
            // newCube = CreateGameObject(newCube, new Vector3(0.5f, 0.5f, 0.5f), "UserInterface/", "LoadingBackground_TealGradient_Music");

            Console.WriteLine("Disabling original GameObjects");

            // Disable original objects from loading screen
            SkyCube.active = false;
            bubbles.active = false;
            originalLoadingAudio.active = false;

            // Disable original objects from login screen
            originalStartScreenAudio.active = false;
            originalStartScreenSkyCube.active = false;
            loginBubbles.active = false;

            // Apply any preferences (yes ik this is lazy)

            Console.WriteLine("Applying Preferences");

            loadScreenPrefab = UserInterface.Find("/MenuContent/Popups/LoadingPopup/LoadingBackground(Clone)")?.gameObject;
            if (loadScreenPrefab == null) return;

            var music = loadScreenPrefab.transform.Find("MenuMusic");
            var spaceSound = loadScreenPrefab.transform.Find("SpaceSound");
            var warpTunnel = loadScreenPrefab.transform.Find("Tunnel");
            var logo = loadScreenPrefab.transform.Find("VRCLogo");
            var InfoPanel = UserInterface.Find("/MenuContent/Popups/LoadingPopup/LoadingInfoPanel");
            // var originalLoadingAudio = UserInterface.Find("/MenuContent/Popups/LoadingPopup/LoadingSound").gameObject;
            var aprfools = loadScreenPrefab.transform.Find("meme");

            if (music != null)
                music.gameObject.SetActive(true);

            if (spaceSound != null)
                spaceSound.gameObject.SetActive(true);

            if (warpTunnel != null)
                warpTunnel.gameObject.SetActive(true);

            if (logo != null)
                logo.gameObject.SetActive(true);

            if (InfoPanel != null)
                InfoPanel.gameObject.SetActive(true);

            if (DateTime.Today.Month == 4 && DateTime.Now.Day == 1)
            {
                if (logo != null)
                    logo.gameObject.SetActive(false);

                if (aprfools != null)
                    aprfools.gameObject.SetActive(true);
            }


        }


        private GameObject CreateGameObject(GameObject obj, Vector3 scale, Transform rootDest, string parent)
        {
            Console.WriteLine("Creating " + obj.name);
            var UIRoot = rootDest.gameObject;
            var requestedParent = UIRoot.transform.Find(parent);
            var newObject = Instantiate(obj, requestedParent, false).GetValue<GameObject>();
            newObject.transform.parent = requestedParent;
            newObject.transform.localScale = scale;
            return newObject;
        }

        private static GameObject loadScreenPrefab;
        private static GameObject loginPrefab;

        private static AssetBundle assets;
    }
}

public class VRCUiManager : MonoBehaviour
{
    public VRCUiManager(IntPtr ptr) : base(ptr) { }

    public static VRCUiManager Instance
    {
        get
        {
            IL2Property property = Instance_Class.GetProperty(nameof(Instance));
            if (property == null)
                (property = Instance_Class.GetProperty(x => x.Instance)).Name = nameof(Instance);
            return property?.GetGetMethod().Invoke()?.GetValue<VRCUiManager>();
        }
    }
    /*
    public static T GetPage<T>(string screenPath) where T : VRCUiPage
    {
        return GameObject.Find(screenPath)?.GetComponent<T>();
    }
    */

    public static new IL2Class Instance_Class = IL2CPP.AssemblyList["Assembly-CSharp"].GetClasses().FirstOrDefault(x => x.GetField("_dragMenuPanel") != null);
}
