using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;

namespace Structure
{
    public abstract class Entity : MonoBehaviour
    {
        [HideInInspector][SerializeField] private GameObject[] childObjects;


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

        protected TMP_Text FindTextObject(string objectName)
        {
            var targetObject = FindObjectByName(objectName);
            if (targetObject != null)
            {
                return targetObject.GetComponent<TMP_Text>();
            }

            return null;
        }
        protected void SetText(string objectName, string text)
        {
            var textObject = FindTextObject(objectName);
            if (textObject != null)
            {
                textObject.text = text;
            }
        }

        protected void StylizeColorFlash(string objectName, Color flashColor, Color finalColor, float flashDuration = 0.2f, float returnDuration = 0.3f)
        {
            var textObject = FindTextObject(objectName);
            if (textObject != null)
            {
                var seq = DOTween.Sequence();
                seq.Append(textObject.DOColor(flashColor, flashDuration))
                   .Append(textObject.DOColor(finalColor, returnDuration));
            }
        }

    }
}
