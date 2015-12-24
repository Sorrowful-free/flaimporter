using System.Collections;

namespace Assets.FlaImporter.Editor.EditorCoroutine
{
    public class EditorCoroutine
    {
        private IEnumerator _enumerator;
        private EditorCoroutine _waitCoroutine;

        public EditorCoroutine(IEnumerator enumerator)
        {
            _enumerator = enumerator;
        }

        public IEnumerator Enumerator
        {
            get { return _enumerator; }
        }

        public bool UpdateCoroutine()
        {
            if (_enumerator == null)
            {
                return false;
            }
            if (_enumerator.Current is EditorCoroutine && _waitCoroutine == null)
            {
                _waitCoroutine = _enumerator.Current as EditorCoroutine;
            }
            if (_waitCoroutine != null)
            {
                var wait = _waitCoroutine.UpdateCoroutine();
                if (wait)
                {
                    return wait;
                }
                _waitCoroutine = null;
            }
            return _enumerator.MoveNext();
        }

    }
}
