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
        private static readonly Dictionary<int,FlaObjectHolder> _objectHoldersByLayers = new Dictionary<int, FlaObjectHolder>();
        
        private static FlaObjectHolder GetObjectHolder(int layerIndex)
        {
            var objectHolder = default(FlaObjectHolder);
            if (!_objectHoldersByLayers.TryGetValue(layerIndex, out objectHolder))
            {
                objectHolder = new FlaObjectHolder();
                _objectHoldersByLayers.Add(layerIndex, objectHolder);
            }
            return objectHolder;
        }

        private static int GetCountInstanceByName(string name)
        {
            var holders = _objectHoldersByLayers.Values.ToList();
            var allObjects = holders.SelectMany(e => e.AllObjects).Where(e=>e.Key == name).ToList();
            return allObjects.Count;
        }
        public static GameObject GetFreeObject(FlaFrameElementRaw elementRaw,int layerIndex, Action<GameObject> instaceCallBack = null)
        {
            return GetObjectHolder(layerIndex).GetFreeObject(elementRaw, (instance) =>
            {
                instance.name += "_" + GetCountInstanceByName(elementRaw.GetName());
                if (instaceCallBack != null)
                {
                    instaceCallBack.Invoke(instance);
                }
            });
        }

        public static GameObject GetBusyObject(FlaFrameElementRaw elementRaw, int layerIndex)
        {
            return GetObjectHolder(layerIndex).GetBusyObject(elementRaw);
        }

        public static GameObject GetBusyObject(string name, int layerIndex)
        {
            return GetObjectHolder(layerIndex).GetBusyObject(name);
        }

        public static List<GameObject> GetAllFreeObjects(int layerIndex)
        {
            return GetObjectHolder(layerIndex).GetAllFreeObjects();
        }

        public static void ReleaseObject(GameObject @object, int layerIndex)
        {
            GetObjectHolder(layerIndex).ReleaseObject(@object);
        }

        public static void ReleaseObject(FlaFrameElementRaw elementRaw, int layerIndex)
        {
            GetObjectHolder(layerIndex).ReleaseObject(elementRaw);
        }

        public static void ReleaseObject(string objectName, int layerIndex)
        {
            GetObjectHolder(layerIndex).ReleaseObject(objectName);
        }

        public static void Clear(int layerIndex)
        {
            GetObjectHolder(layerIndex).Clear();
        }

        public static void ReleaseAll(int layerIndex)
        {
            GetObjectHolder(layerIndex).ReleaseAll();
        }

        public static void ReleaseAll()
        {
            foreach (var holder in _objectHoldersByLayers.Values)
            {
                holder.ReleaseAll();
            }
        }

        public static void ClearAll()
        {
            foreach (var holder in _objectHoldersByLayers.Values)
            {
                holder.Clear();
            }
            _objectHoldersByLayers.Clear();
        }


    }

    public class FlaObjectHolder
    {
        private readonly Dictionary<string, List<GameObject>> _freeObjects = new Dictionary<string, List<GameObject>>();
        private readonly Dictionary<string, List<GameObject>> _allObjects = new Dictionary<string, List<GameObject>>();

        public Dictionary<string, List<GameObject>> AllObjects
        {
            get { return _allObjects; }
        }

        public GameObject GetFreeObject(FlaFrameElementRaw elementRaw, Action<GameObject> instaceCallBack = null)
        {
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
                while (instanceNumber.Length <= 3)
                {
                    instanceNumber = "0" + instanceNumber;
                }

                allList.Add(go);
                if (instaceCallBack != null)
                {
                    instaceCallBack(go);
                }
            }
            return go;
        }

        public GameObject GetBusyObject(FlaFrameElementRaw elementRaw)
        {
            return GetBusyObject(elementRaw.GetName());
        }

        public GameObject GetBusyObject(string name)
        {
            var @object = _allObjects[name].OrderBy(e => e.name).FirstOrDefault(a => _freeObjects[name].All(f => a != f));
            return @object;
        }

        public List<GameObject> GetAllFreeObjects()
        {
            return _freeObjects.SelectMany(e => e.Value).ToList();
        }

        public void ReleaseObject(GameObject @object)
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

        public void ReleaseObject(FlaFrameElementRaw elementRaw)
        {
            var @object = GetBusyObject(elementRaw);
            ReleaseObject(@object);
        }

        public void ReleaseObject(string objectName)
        {
            var @object = GetBusyObject(objectName);
            ReleaseObject(@object);
        }

        public void Clear()
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

        public void ReleaseAll()
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
