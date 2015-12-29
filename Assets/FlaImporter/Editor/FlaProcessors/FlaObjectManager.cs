using System;
using System.Collections.Generic;
using System.Linq;
using Assets.FlaImporter.Editor.Data.RawData.FrameElements;
using Assets.FlaImporter.Editor.Extentions.FlaExtentionsRaw;
using Assets.FlaImporter.Editor.Utils;
using Assets.FlaImporter.FlaImporter.ColorAndFilersHolder;
using Assets.FlaImporter.FlaImporter.Transorm;
using Assets.FlaImporter.FlaImporter.Transorm.Enums;
using UnityEditor;
using UnityEngine;

namespace Assets.FlaImporter.Editor.FlaProcessors
{
    public static class FlaObjectManager
    {
      
        private static readonly Dictionary<string, List<GameObject>> _freeObjects = new Dictionary<string, List<GameObject>>();
        private static readonly Dictionary<string, List<GameObject>> _allObjects = new Dictionary<string, List<GameObject>>();
      
        public static GameObject GetFreeObject(FlaFrameElementRaw elementRaw,Action<GameObject> instaceCallBack = null)
        {
            //Debug.Log(string.Format("{0} - try get object:{1}", DateTime.Now.Ticks, elementRaw.GetName()));
            var elementName = elementRaw.GetName();
            var freeList = default(List<GameObject>);
            if (!_freeObjects.TryGetValue(elementName, out freeList))
            {
                freeList = new List<GameObject>();
                _freeObjects.Add(elementName, freeList);
            }
            var go = freeList.OrderBy(e => e.name).FirstOrDefault();
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
                var instanceNumber = allList.Count.ToString();
                while (instanceNumber.Length<=3)
                {
                    instanceNumber = "0" + instanceNumber;
                }
                go.name = elementName + "_" + instanceNumber;
                
                allList.Add(go);
                if (instaceCallBack != null)
                {
                    instaceCallBack(go);
                }
            }
            return go;
        }

        public static GameObject GetBusyObject(FlaFrameElementRaw elementRaw)
        {
            return GetBusyObject(elementRaw.GetName());
        }

        public static GameObject GetBusyObject(string name)
        {
            var @object = _allObjects[name].OrderBy(e=>e.name).FirstOrDefault(a => _freeObjects[name].All(f => a != f));
            return @object;
        }

        public static List<GameObject> GetAllFreeObjects()
        {
            return _freeObjects.SelectMany(e => e.Value).ToList();
        }

        public static void ReleaseObject(GameObject @object)
        {
          //  Debug.Log(string.Format("{0} - release object:{1}",DateTime.Now.Ticks,@object == null?"null!":@object.name));
            if (@object == null)
            {
                return;
            }

            var objectName = _allObjects.Keys.FirstOrDefault(key =>
            {
                var subObjectName = @object.name.ToLower();
                var lastUnderlineIndex = subObjectName.LastIndexOf("_");
                var substring = subObjectName.Substring(0, lastUnderlineIndex);
                //Debug.Log("sub string is "+substring +" and key is "+key );
                return substring == key.ToLower();
            });
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
            var @object = GetBusyObject(elementRaw);
            ReleaseObject(@object);
        }

        public static void ReleaseObject(string objectName)
        {
            var @object = GetBusyObject(objectName);
            ReleaseObject(@object);
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
