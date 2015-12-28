using System;
using System.Collections.Generic;
using System.Linq;
using Assets.FlaImporter.Editor.Data.RawData.FrameElements;
using Assets.FlaImporter.Editor.Extentions.FlaExtentionsRaw;
using Assets.FlaImporter.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Assets.FlaImporter.Editor.FlaProcessors
{
    public static class FlaObjectManager
    {
      
        private static readonly Dictionary<string, List<GameObject>> _freeObjects = new Dictionary<string, List<GameObject>>();
        private static readonly Dictionary<string, List<GameObject>> _allObjects = new Dictionary<string, List<GameObject>>();
      
        public static GameObject GetFreeObject(FlaFrameElementRaw elementRaw)
        {
            //Debug.Log(string.Format("{0} - try get object:{1}", DateTime.Now.Ticks, elementRaw.GetName()));
            var elementName = elementRaw.GetName();
            var freeList = default(List<GameObject>);
            if (!_freeObjects.TryGetValue(elementName, out freeList))
            {
                freeList = new List<GameObject>();
                _freeObjects.Add(elementName, freeList);
            }
            var go = freeList.FirstOrDefault();
            if (go != null)
            {
                freeList.Remove(go);
            }
            else
            {
                if (elementRaw is FlaShapeRaw)
                {
                    go = (GameObject)PrefabUtility.InstantiatePrefab(AssetDataBaseUtility.LoadShape(elementName));
                }
                else if (elementRaw is FlaBitmapInstanceRaw)
                {
                    go = (GameObject)PrefabUtility.InstantiatePrefab(AssetDataBaseUtility.LoadBitmapInstance(elementName));
                }
                else if (elementRaw is FlaSymbolInstanceRaw)
                {
                    go = (GameObject)PrefabUtility.InstantiatePrefab(AssetDataBaseUtility.LoadSymbol(elementName));
                }
                else
                {
                    Debug.Log("some element cannot parce " + elementRaw);
                }

                var allList = default(List<GameObject>);
                if (!_allObjects.TryGetValue(elementName, out allList))
                {
                    allList = new List<GameObject>();
                    _allObjects.Add(elementName, allList);
                }
                go.name = elementName + "_" + allList.Count;
                
                allList.Add(go);
            }
            return go;
        }

        public static GameObject GetBusyObject(FlaFrameElementRaw elementRaw)
        {
            return GetBusyObject(elementRaw.GetName());
        }

        public static GameObject GetBusyObject(string name)
        {
            var @object = _allObjects[name].FirstOrDefault(a => _freeObjects[name].All(f => a != f));
            return @object;
        }

        public static List<GameObject> GetAllFreeObjects()
        {
            return _freeObjects.SelectMany(e => e.Value).ToList();
        }

        public static void ReleaseObject(GameObject @object)
        {
            //Debug.Log(string.Format("{0} - release object:{1}",DateTime.Now.Ticks,@object.name));
            if (@object == null)
            {
                return;
            }
            var objectName = _allObjects.Keys.FirstOrDefault(e => @object.name.ToLower().StartsWith(e.ToLower()));
            var freeList = default(List<GameObject>);
            if (!_freeObjects.TryGetValue(objectName, out freeList))
            {
                freeList = new List<GameObject>();
                _freeObjects.Add(objectName, freeList);
            }
            if (!freeList.Contains(@object))
                freeList.Add(@object);
        }
        
        public static void ReleaseObject(FlaFrameElementRaw elementRaw)
        {
           ReleaseObject(elementRaw.GetName());
        }

        public static void ReleaseObject(string objectName)
        {
            var freeList = default(List<GameObject>);
            if (!_freeObjects.TryGetValue(objectName, out freeList))
            {
                freeList = new List<GameObject>();
                _freeObjects.Add(objectName, freeList);
            }
            var @object = _allObjects[objectName].FirstOrDefault(a => _freeObjects[objectName].All(f => a != f));
            if (!freeList.Contains(@object))
                freeList.Add(@object);

        }

        public static void Clear()
        {
            if (_freeObjects != null)
            {
                foreach (var freeObject in _freeObjects)
                {
                    if (freeObject.Value != null)
                    {
                        freeObject.Value.Clear();
                    }
                }
                _freeObjects.Clear();
            }

            if (_allObjects != null)
            {
                foreach (var allObject in _allObjects)
                {
                    if (allObject.Value != null)
                    {
                        allObject.Value.Clear();
                    }
                }
                _allObjects.Clear();
            }
        }

        public static void ReleaseAll()
        {
            foreach (var key in _allObjects.Keys.ToList())
            {
                foreach (var gameObject in _allObjects[key])
                {
                    ReleaseObject(gameObject);
                }
            }
        }

    }
}
