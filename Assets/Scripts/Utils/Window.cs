using System;
using EZData;
using UnityEngine;

namespace TruePong.Utils {
    public class Window : Context {
        public String Path { get; private set; }
        private GameObject go;

        public Window(String prefabPath) {
            Path = prefabPath;
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            go = GameObject.Instantiate(prefab, null);
            go.SetActive(false);

            var dataContext = go.AddComponent<ItemDataContext>();
            dataContext.SetContext(this);
        }

        public bool IsActive() {
            return go.activeSelf;
        }

        public void SetActive(bool isActive) {
            go.SetActive(isActive);
        }

        public void Activate() {
            go.SetActive(true);
        }

        public void Deactivate() {
            go.SetActive(false);
        }

        public void Destroy() {
            GameObject.Destroy(go);
        }
    }
}
