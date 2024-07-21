
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bakery.Inputs
{
    public static class RaycastUtilities
    {
        public static bool PointerIsOverUI(Vector2 screenPos, out GameObject hitObject)
        {
            var hitObjects = UIRaycast(ScreenPosToPointerData(screenPos));
            if (hitObjects.Count == 0)
            {
                hitObject = null;
                return false;
            }
            hitObject = hitObjects[0];
            return hitObject.layer == LayerMask.NameToLayer("UI");
        }

        public static List<GameObject> UIRaycast(Vector2 screenPos)
            => UIRaycast(ScreenPosToPointerData(screenPos));

        public static List<GameObject> UIRaycast()
            => UIRaycast(InputServices.GetMousePosition());

        static List<GameObject> UIRaycast(PointerEventData pointerData)
        {
            var results = new List<RaycastResult>();
            if (EventSystem.current == null) Debug.LogError("Add an EventSystem to the scene");
            EventSystem.current.RaycastAll(pointerData, results);

            return results.ConvertAll(result => result.gameObject);
        }



        public static Vector2 GetMouseUIPosition()
            => ScreenPosToPointerData(InputServices.GetMousePosition()).position;

        static PointerEventData ScreenPosToPointerData(Vector2 screenPos)
           => new(EventSystem.current) { position = screenPos };

        internal static bool IsObjectUnderPointer(GameObject gameObject)
        {
            List<GameObject> objList = UIRaycast(InputServices.GetMousePosition());
            if (objList.Count == 0) return false;

            foreach (var o in objList)
                if (o.CompareTag(gameObject.tag) && ReferenceEquals(o, gameObject))
                    return true;

            return false;
        }



        // internal static void GetHitPosition(BagItemComponent itemBeingPlaced, out Vector3 worldPosition)
        // {
        //     var pointerData = ScreenPosToPointerData(InputServices.GetMousePosition());
        //     var results = new List<RaycastResult>();
        //     EventSystem.current.RaycastAll(pointerData, results);

        //     if (results.Count == 0)
        //     {
        //         worldPosition = InputServices.GetMousePosition();
        //         return;
        //     }
        //     worldPosition = results[0].worldPosition;
        // }
    }
}
