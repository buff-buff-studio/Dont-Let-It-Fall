using System;
using UnityEngine;
using UnityEngine.Events;

namespace DLIFR.Data
{
    public class Variable : ScriptableObject
    {
        public Action onChange;
        public UnityEvent onChangeUE;

        public virtual string AsString(string format = null) => null;
    }

    public class Variable<T> : Variable
    {
        [SerializeField]
        private T _value;

        public T value
        {
            get => _value;

            set 
            {
                if(value.Equals(_value))
                    return;

                _value = value;

                onChange?.Invoke();
                onChangeUE.Invoke();
            }
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public override string AsString(string format = null)
        {
            if(format == null)
                return _value.ToString();

            object o = _value;

            if(format == "HOUR")
            {
                float time = ((float) o)%24;
                int hour = Mathf.FloorToInt(time);
                int minutes = Mathf.FloorToInt(60 * (time - hour));
            
                return $"{hour:00}:{minutes:00}";
            }

            if(format == "DAY")
            {
                int day = Mathf.FloorToInt(((float) o)/24);
                return $"{day + 1}";
            }


            if(_value is float)
                return ((float) o).ToString(format);

            if(_value is int)
                return ((int) o).ToString(format);

            if(_value is double)
                return ((double) o).ToString(format);

            return _value.ToString();
        }
    }
}