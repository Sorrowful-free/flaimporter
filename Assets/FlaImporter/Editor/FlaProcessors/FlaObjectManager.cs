using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.FlaImporter.Editor.Data.RawData.FrameElements;
using Assets.FlaImporter.Editor.EditorCoroutine;
using Assets.FlaImporter.Editor.Extentions.FlaExtentionsRaw;
using Assets.FlaImporter.FlaImporter.Transorm;
using UnityEngine;

namespace Assets.FlaImporter.Editor.FlaProcessors
{
    public static class FlaObjectManager
    {
       public static FlaObjectsHolder Objects = new FlaObjectsHolder(FlaFrameElementProcessor.ProcessFlaElement);
    }

    public class FlaObjectsHolder
    {
        private Dictionary<string, List<GameObject>> _freeObjects = new Dictionary<string, List<GameObject>>();
        private Dictionary<string, List<GameObject>> _allObjects = new Dictionary<string, List<GameObject>>();
        private Func<FlaFrameElementRaw, Action<GameObject>, IEnumerator> _loadDelegate;

        public FlaObjectsHolder(Func<FlaFrameElementRaw,Action<GameObject>, IEnumerator> loadDelegate)
        {
            _loadDelegate = loadDelegate;
        }

        public IEnumerator GetFreeObject(FlaFrameElementRaw element,Action<GameObject> callback)
        {
            var freeList = default(List<GameObject>);
            if (!_freeObjects.TryGetValue(element.GetName(), out freeList))
            {
                freeList = new List<GameObject>();
                _freeObjects.Add(element.GetName(), freeList);
            }
            var go = freeList.FirstOrDefault();
            if (go != null)
            {
                freeList.Remove(go);
            }
            else
            {
                yield return _loadDelegate(element, (goResources) =>
                {
                    go = goResources;//GameObject.Instantiate(goResources);
                    var allList = default(List<GameObject>);
                    if (!_allObjects.TryGetValue(element.GetName(), out allList))
                    {
                        allList = new List<GameObject>();
                        _allObjects.Add(element.GetName().ToLower(), allList);
                    }
                    go.name = element.GetName()+"_"+allList.Count;
                    allList.Add(go);
                    go.GetComponent<FlaTransform>();
                    if (callback != null)
                    {
                        callback(go);
                    }
                }).StartAsEditorCoroutine();
            }
        }

        public void ReleaseObject(GameObject @object)
        {
            var objectName = _allObjects.Keys.FirstOrDefault(e => @object.name.ToLower().StartsWith(e.ToLower())).ToLower();
            Debug.Log("release object : "+objectName+" :"+@object.name);
            var freeList = default(List<GameObject>);
            if (!_freeObjects.TryGetValue(objectName, out freeList))
            {
                freeList = new List<GameObject>();
                _freeObjects.Add(objectName, freeList);
            }
            freeList.Add(@object);
        }

    }
}
