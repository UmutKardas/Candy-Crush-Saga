using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Structure
{
    public abstract class Entity : MonoBehaviour
    {
        [HideInInspector] [SerializeField] private GameObject[] childObjects;

#if UNITY_EDITOR
        private void OnValidate()
        {
            childObjects = RetrieveAllChildObjects(gameObject).ToArray();
        }

        private List<GameObject> RetrieveAllChildObjects(GameObject parent)
        {
            return parent.GetComponentsInChildren<Transform>(true)
                .Where(child => !GameObjectUtility.GetStaticEditorFlags(child.gameObject)
                    .HasFlag(StaticEditorFlags.BatchingStatic)).Select(child => child.gameObject).ToList();
        }
#endif

        protected void ToggleObject(string objectName, bool isActive)
        {
            var targetObject = FindObjectByName(objectName);
            if (targetObject != null)
            {
                targetObject.SetActive(isActive);
            }
        }

        protected GameObject FindObjectByName(string objectName)
        {
            return childObjects.FirstOrDefault(item => item.name == objectName);
        }
    }
}
