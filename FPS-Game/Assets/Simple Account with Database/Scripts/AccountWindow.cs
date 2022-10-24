using DatabaseAPI.Account;
using UnityEngine;
using UnityEditor;
using PlayFab;

namespace DatabaseAPI
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class LatestScenes
    {
        private static string currentScene;

        [System.Obsolete]
        static LatestScenes()
        {
            currentScene = EditorApplication.currentScene;
            EditorApplication.hierarchyWindowChanged += hierarchyWindowChanged;
        }
        [System.Obsolete]
        private static void hierarchyWindowChanged()
        {
            if (currentScene != EditorApplication.currentScene)
            {
                currentScene = EditorApplication.currentScene;
                try
                {
                    AccountWindow.aw.RefreshWindow();
                }
                catch { }
            }
        }
    }

    public class AccountWindow : EditorWindow
    {
        public static AccountWindow aw;

        string TitleID = "C9C63"; //DEMO project; Please change this value to your own titleId from Account Manager Window
        bool AuthorizedAnonymousAuth;

        Object accoutManagerPrefab;
        AccountController accoutController;

        private int tab = 0;

        bool SecureDataTransfer = false;

        GameObject ConnectionPanel;

#if UNITY_ANDROID || UNITY_IOS
        GameObject addLoginPanel;
        GameObject recoveryButton;
#endif

        bool prefabError;

        public void OnEnable()
        {
            GetPrefab();

            AuthorizedAnonymousAuth = AccountSharedSettings.AuthorizedAnonymousAuth;
            
            WaitForAccoutManagerObject();
        }

        public void GetPrefab()
        {
            try
            {
                accoutManagerPrefab = AssetDatabase.LoadAssetAtPath("Assets/Simple Account with Database/Prefabs/AccountController.prefab", typeof(GameObject));
                prefabError = false;
            }
            catch
            {
                prefabError = true;
                Debug.LogError("ERROR: Account Controller prefab can not be found ! (error P01)");
            }
        }

        public void WaitForAccoutManagerObject()
        {
            if (accoutController)
            {
#if UNITY_ANDROID || UNITY_IOS
                AuthorizedAnonymousAuth = accoutController.AnonymeLogin;
                addLoginPanel = accoutController.addLoginPanel;
                recoveryButton = accoutController.recoveryButton;
#endif
                ConnectionPanel = accoutController.loginPanel;
            }
        }


        [MenuItem("Database Controller/Settings")]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow<AccountWindow>("Account manager");
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.Show();
        }

        private void OnGUI()
        {
            if (prefabError)
            {
                GUILayout.Label("FATAL ERROR: Looks like you have changed the asset hierarchy...", EditorStyles.boldLabel);
                accoutManagerPrefab = EditorGUILayout.ObjectField("AccountController Prefab", accoutManagerPrefab, typeof(GameObject), false) as GameObject;
                if (accoutManagerPrefab)
                {
                    if (accoutManagerPrefab.name == "AccountController") prefabError = false;
                }
            }
            else
            {
                if (accoutController)
                {
                    tab = GUILayout.Toolbar(tab, new string[] { "Project", "Connection", "Data", "Leaderboard", "Friends", "Multiplayer", "Virtual Currency" });
                    switch (tab)
                    {
                        case 0:
                            RefreshWindow();
                            EditorGUILayout.Space();
                            GUILayout.Label("Project Settings", EditorStyles.boldLabel);
                            EditorGUILayout.Space();
                            ProjectSettingsTab();
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Help", GUILayout.Width(50)))
                            {
                                Application.OpenURL("https://assets.new-line.fr/documentation/simple/#configuration");
                            }
                            GUILayout.EndHorizontal();
                            break;
                        case 1:
                            EditorGUILayout.Space();
                            GUILayout.Label("Connection Settings", EditorStyles.boldLabel);
                            EditorGUILayout.Space();
                            ConnectionTab();
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Help", GUILayout.Width(50)))
                            {
                                Application.OpenURL("https://assets.new-line.fr/documentation/simple/#imp-connection");
                            }
                            GUILayout.EndHorizontal();
                            break;
                        case 2:
                            EditorGUILayout.Space();
                            GUILayout.Label("Data Settings", EditorStyles.boldLabel);
                            EditorGUILayout.Space();
                            DataTab();
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Help", GUILayout.Width(50)))
                            {
                                Application.OpenURL("https://assets.new-line.fr/documentation/simple/#imp-user-stats");
                            }
                            GUILayout.EndHorizontal();
                            break;
                        case 3:
                            EditorGUILayout.Space();
                            GUILayout.Label("Leaderboard Settings", EditorStyles.boldLabel);
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                            GUILayout.Label("This option is only available in the full version of the asset.");
                            EditorGUILayout.Space();
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Show me"))
                            {
                                Application.OpenURL("http://u3d.as/27hK");
                            }
                            GUILayout.EndHorizontal();
                            break;
                        case 4:
                            EditorGUILayout.Space();
                            GUILayout.Label("Friends Settings", EditorStyles.boldLabel);
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                            GUILayout.Label("This option is only available in the full version of the asset.");
                            EditorGUILayout.Space();
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Show me"))
                            {
                                Application.OpenURL("http://u3d.as/27hK");
                            }
                            GUILayout.EndHorizontal();
                            break;
                        case 5:
                            EditorGUILayout.Space();
                            GUILayout.Label("Multiplayer settings", EditorStyles.boldLabel);
                            EditorGUILayout.Space();
                            GUILayout.Label("This section is not available for now...", EditorStyles.largeLabel);
                            if (GUILayout.Button("Learn more"))
                            {
                                Help.BrowseURL("https://assets.new-line.fr/#pricing");
                            }
                            break;
                        case 6:
                            EditorGUILayout.Space();
                            GUILayout.Label("Virtual currency Settings", EditorStyles.boldLabel);
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                            GUILayout.Label("This option is only available in the full version of the asset.");
                            EditorGUILayout.Space();
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Show me"))
                            {
                                Application.OpenURL("http://u3d.as/27hK");
                            }
                            GUILayout.EndHorizontal();
                            break;
                    }
                }
                else
                {
                    RefreshWindow();
                    EditorGUILayout.Space();
                    GUILayout.Label("Project Settings", EditorStyles.boldLabel);
                    EditorGUILayout.Space();
                    ProjectSettingsTab();
                }

                if (EditorApplication.isPlayingOrWillChangePlaymode) RefreshWindow();
            }
        }

        void DataTab()
        {
            SecureDataTransfer = EditorGUILayout.Toggle("User Secure Data Transfer", SecureDataTransfer);
            EditorGUILayout.Space();
            if (GUILayout.Button("Apply"))
            {
                if (SecureDataTransfer)
                {
                    int option = EditorUtility.DisplayDialogComplex("You can't do that !",
                        "Sorry, this option only available in the complete version of this asset.",
                        "See more",  "Buy now", "Close");

                    switch(option){
                        case 0:
                            Application.OpenURL("https://assets.new-line.fr/#pricing");
                            break;

                        case 1:
                            Application.OpenURL("http://u3d.as/27hK");
                            break;

                        default:
                            break;
                    }
                }

                SceneView.RepaintAll();
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.Label("Player Data :", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            if (!EditorApplication.isPlaying)
            {
                GUILayout.Label("You need to be in play mode...");
            }
            else
            {
                if (accoutController.getStatsFinished)
                {
                    foreach (var data in AccountController.playerData)
                        GUILayout.Label(data.Key + ": " + data.Value);
                }
                else { GUILayout.Label("Please Wait..."); }
            }
            EditorGUI.indentLevel--;
        }

        void ProjectSettingsTab()
        {
            TitleID = EditorGUILayout.TextField("PlayFab Project ID", TitleID);
            accoutController = EditorGUILayout.ObjectField("Account controller Object", accoutController, typeof(AccountController), true) as AccountController;

            GameObject[] acobj = GameObject.FindGameObjectsWithTag("GameController");
            foreach (GameObject temp in acobj)
            {
                if (temp.name.StartsWith("AccountController"))
                {
                    accoutController = temp.GetComponent<AccountController>();
                }
            }

            if (!accoutController)
            {
                if (GUILayout.Button("Create this object for me"))
                {
                    GameObject obj = Instantiate(accoutManagerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
                }
            }
            else
            {
                EditorGUILayout.Space();
                GUILayout.Label("This part is fully setup, you can continue in the next tab.", EditorStyles.boldLabel);
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Apply"))
            {
                PlayFabSettings.TitleId = TitleID;

                AccountSharedSettings.TitleID = TitleID;

                Debug.Log("Project settings updated for " + PlayFabSettings.TitleId);
                SceneView.RepaintAll();
            }
        }

        public void ConnectionTab()
        {
            EditorGUILayout.Space();
            ConnectionPanel = EditorGUILayout.ObjectField("Login Panel", ConnectionPanel, typeof(GameObject), true) as GameObject;
#if UNITY_ANDROID || UNITY_IOS
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();


                    AuthorizedAnonymousAuth = EditorGUILayout.BeginToggleGroup("Anonymouse Connection", AuthorizedAnonymousAuth);
                    if (AuthorizedAnonymousAuth)
                    {
                        EditorGUI.indentLevel++;
                        addLoginPanel = EditorGUILayout.ObjectField("Recovery Login Panel", addLoginPanel, typeof(GameObject), true) as GameObject;
                        recoveryButton = EditorGUILayout.ObjectField("Recovery Login Button", recoveryButton, typeof(GameObject), true) as GameObject;
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndToggleGroup();
#endif
            EditorGUILayout.Space();

            if (GUILayout.Button("Apply"))
            {
                PlayFabSettings.TitleId = TitleID;
#if UNITY_ANDROID || UNITY_IOS
                        accoutController.AnonymeLogin = AuthorizedAnonymousAuth;
#endif
                if (AuthorizedAnonymousAuth)
                {
#if UNITY_ANDROID || UNITY_IOS
                            accoutController.addLoginPanel = addLoginPanel;
                            accoutController.recoveryButton = recoveryButton;
#endif
                }

                accoutController.loginPanel = ConnectionPanel;

                AccountSharedSettings.TitleID = TitleID;
                AccountSharedSettings.AuthorizedAnonymousAuth = AuthorizedAnonymousAuth;

                Debug.Log("Settings updated for " + PlayFabSettings.TitleId);
                SceneView.RepaintAll();
            }
        }

        public void RefreshWindow()
        {
            try
            {
                EditorWindow.focusedWindow.Repaint();
                WaitForAccoutManagerObject();
            }
            catch { }
        }

    }

    [CreateAssetMenu(fileName = "Data", menuName = "Database Controller/Manager Object", order = 1)]
    public class AccountSharedSettings : ScriptableObject
    {
        public static string TitleID = "C9C63";
        public static bool AuthorizedAnonymousAuth;
    }
#endif
}

namespace DatabaseAPI.dbHelp
{
    public static class DatabaseHelp
    {
        [MenuItem("Database Controller/Help/Discord")]
        private static void MenuDiscord()
        {
            Application.OpenURL("https://discord.gg/vXnPJMb");
        }

        [MenuItem("Database Controller/Help//Docs")]
        private static void MenuDocumentation()
        {
            Application.OpenURL("https://docs.google.com/document/d/1Kgu_FBcWfWT6p8a0cY0GuLNbpq-msAp7Ahoi2829Axw/edit?usp=sharing");
        }

        [MenuItem("Database Controller/Your Dashboard")]
        private static void MenuDashboard()
        {
            Application.OpenURL("https://developer.playfab.com/");
        }
    }
}