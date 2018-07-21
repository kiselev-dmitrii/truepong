using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace TruePong.Utils {
    public static class SceneLoader {
        public static void LoadScene(String sceneName, Action<Scene> onSuccess) {
            SceneManager.LoadScene(sceneName);
            WaitLoading(onSuccess);
        }

        public static void WaitLoading(Action<Scene> onSuccess) {
            UnityAction<Scene, LoadSceneMode> onSceneLoaded = null;
            onSceneLoaded = (scene, mode) => {
                SceneManager.sceneLoaded -= onSceneLoaded;
                onSuccess.TryCall(scene);
            };
            SceneManager.sceneLoaded += onSceneLoaded;
        }

        public static Scene GetActiveScene() {
            return SceneManager.GetActiveScene();
        }
    }
}
