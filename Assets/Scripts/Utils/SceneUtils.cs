using UnityEngine;
using UnityEngine.SceneManagement;

namespace TruePong.Utils {
    public static class SceneUtils {
        public static T GetComponent<T>(this Scene scene) where T : Component {
            foreach (var rootObject in scene.GetRootGameObjects()) {
                var component = rootObject.GetComponent<T>();
                if (component != null) {
                    return component;
                }
            }

            return null;
        }
    }
}
