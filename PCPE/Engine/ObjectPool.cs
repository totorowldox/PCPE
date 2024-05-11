using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCPE.Engine
{
    public class ObjectPool<T>
    {
        private List<T> _objects;

        private const int MAXIMUM_OBJECT_COUNT = 30000;
        private int _generatedCount = 0;
        private Func<T> _generateObject;

        public ObjectPool(Func<T> generator)
        {
            _objects = new();
            _generateObject = generator;
        }

        public bool TryGetObject(out T obj)
        {
            if (HasSpareObjects())
            {
                obj = _objects[0];
                _objects.RemoveAt(0);
                return true;
            }
            else if (_generatedCount < MAXIMUM_OBJECT_COUNT)
            {
                obj = _generateObject.Invoke();
                _objects.Add(obj);
                _generatedCount++;
            }
            obj = default;
            return false;
        }

        public bool HasSpareObjects() => _objects.Count > 0;

        public void RecycleObject(T objToRecycle)
        {
            _objects.Add(objToRecycle);
        }
    }
}
