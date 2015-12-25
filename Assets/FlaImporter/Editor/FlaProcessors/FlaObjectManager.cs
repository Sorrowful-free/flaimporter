using System;
using System.Collections.Generic;
using System.Linq;
using Assets.FlaImporter.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Assets.FlaImporter.Editor.FlaProcessors
{
    public static class FlaObjectManager
    {
        public readonly static FlaObjectsHolder Symbols = new FlaObjectsHolder(AssetDataBaseUtility.LoadSymbol);
        public readonly static FlaObjectsHolder BitmapInstance = new FlaObjectsHolder(AssetDataBaseUtility.LoadBitmapInstance);
        public readonly static FlaObjectsHolder Shapes = new FlaObjectsHolder(AssetDataBaseUtility.LoadShape);

        public static void Clear()
        {
            Symbols.Clear();
            BitmapInstance.Clear();
            Shapes.Clear();
        }

        public static void ReleaseAll()
        {
            Symbols.ReleaseAll();
            BitmapInstance.ReleaseAll();
            Shapes.ReleaseAll();
        }
    }

    public class FlaObjectsHolder
    {
        private Dictionary<string, List<GameObject>> _freeObjects = new Dictionary<string, List<GameObject>>();
        private Dictionary<string, List<GameObject>> _allObjects = new Dictionary<string, List<GameObject>>();
        private Func<string,GameObject> _loadDelegate;

        public FlaObjectsHolder(Func<string,GameObject> loadDelegate)
        {
            _loadDelegate = loadDelegate;
        }

        public GameObject GetFreeObject(string name)
        {
            var freeList = default(List<GameObject>);
            if (!_freeObjects.TryGetValue(name, out freeList))
            {
                freeList = new List<GameObject>();
                _freeObjects.Add(name, freeList);
            }
            var go = freeList.FirstOrDefault();
            if (go != null)
            {
                freeList.Remove(go);
            }
            else
            {
                go = (GameObject)PrefabUtility.InstantiatePrefab(_loadDelegate(name));
                var allList = default(List<GameObject>);
                if (!_allObjects.TryGetValue(name, out allList))
                {
                    allList = new List<GameObject>();
                    _allObjects.Add(name, allList);
                }
                go.name = name + "_" + allList.Count;
                allList.Add(go);
            }
            return go;
        }

        public void ReleaseObject(GameObject @object)
        {
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
