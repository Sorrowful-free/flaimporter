using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Assets.FlaExporter.Editor.EditorCoroutine
{
    
    public static class EditorCoroutineExtention
    {
        private static Dictionary<IEnumerator,EditorCoroutine> _editorCoroutines = new Dictionary<IEnumerator, EditorCoroutine>();
        private static bool _isNeedListening = true;
       
        public static EditorCoroutine StartAsEditorCoroutine(this IEnumerator coroutine)
        {
            return StartEditorCoroutine(coroutine);
        }

        public static void StopAsEditorCoroutine(this IEnumerator coroutine)
        {
            StopEditorCoroutine(coroutine);
        }

        public static EditorCoroutine StartEditorCoroutine(IEnumerator coroutine)
        {
            var corutineNode = new EditorCoroutine(coroutine);
            _editorCoroutines.Add(coroutine, corutineNode);
            if (_isNeedListening && _editorCoroutines.Count > 0)
            {
                EditorApplication.update+=EditorUpdateHandler;
                _isNeedListening = false;
            }
            return corutineNode;
        }

        public static void StopEditorCoroutine(IEnumerator coroutine)
        {
            _editorCoroutines.Remove(coroutine);
            if (!_isNeedListening && _editorCoroutines.Count <= 0)
            {
                EditorApplication.update -= EditorUpdateHandler;
                _isNeedListening = true;
            }
        }

        private static void EditorUpdateHandler()
        {
            if (_editorCoroutines.Count <= 0)
            {
                return;
            }
            foreach (var coroutine in _editorCoroutines.Values.ToList())
            {
                if (!coroutine.UpdateCoroutine())
                {
                    StopEditorCoroutine(coroutine.Enumerator);
                }
            }
        }

        public static void StopAllEditorCoroutines()
        {
            foreach (var coroutine in _editorCoroutines.ToList())
            {
                StopEditorCoroutine(coroutine.Key);
            }
        }
    }

    
}
