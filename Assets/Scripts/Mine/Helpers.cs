
using System.Collections.Generic;
using UnityEngine;

namespace HelperFuntions {

    public static class Helpers : object {

        public static List<GameObject> AllChilds(GameObject root) {
            List<GameObject> result = new List<GameObject>();
            if(root.transform.childCount > 0) {
                foreach(Transform t in root.transform) {
                    Searcher(result, t.gameObject);
                }
            }
            return result;
        }
        
        static void Searcher(List<GameObject> list, GameObject root) {
            list.Add(root);
            if(root.transform.childCount > 0) {
                foreach(Transform t in root.transform) {
                    Searcher(list, t.gameObject);
                }
            }
        }

    }
}