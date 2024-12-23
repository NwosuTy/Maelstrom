using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace Creotly_Studios
{
    public static class GameObjectTools 
    {
        public static void InitializeObject(float delta, Tiles selectedTile, Transform parent, Vector3 position)
        {
            selectedTile.transform.SetParent(parent);
            selectedTile.transform.localPosition = position;

            Vector3 currentScale = selectedTile.transform.localScale;
            selectedTile.transform.localScale = Vector3.zero;
            
            selectedTile.canEnableObjects = true;
            selectedTile.transform.gameObject.SetActive(true);
            selectedTile.transform.DOScale(currentScale, 1 * delta).SetEase(Ease.OutElastic);
        }

        /// <summary>
        /// Deeply Searches For Object by name, if Transform parent is not set, checks entire scene for object, else does deep search on parent
        /// </summary>
        /// <param name="objectName">The Name of Object to Find.</param>
        /// <param name="parent">The Parent Object to Run Deep Search on, searches entire scene if null.</param>
        /// <returns> The Found Object or Null </returns>
        public static GameObject FindChildObject(string objectName, Transform parent = null)
        {
            bool hasParent = (parent != null);
            if(hasParent != true)
            {
                foreach(GameObject rootObject in SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    GameObject foundObject = DeepFindObjectInParent(objectName, rootObject.transform);
                    if(foundObject != null)
                    {
                        return foundObject;
                    }
                }
                return null;
            }
            return DeepFindObjectInParent(objectName, parent);
        }

        private static GameObject DeepFindObjectInParent(string objectName, Transform parent)
        {
            if(parent.name == objectName)
            {
                return parent.gameObject;
            }

            for(int i = 0; i < parent.childCount; i++)
            {
                Transform childObject = parent.GetChild(i);
                GameObject foundObject = DeepFindObjectInParent(objectName, childObject);
                if(foundObject != null)
                {
                    return foundObject;
                }
            }
            return null;
        }
    }
}
