using System;

namespace Model
{
    [Serializable]
    public class Joker
    {
        public JokerData Data;
        public string InstanceID;
        public float DynamicValue; 

        public Joker(JokerData data)
        {
            Data = data;
            InstanceID = Guid.NewGuid().ToString();
            DynamicValue = 0;
        }
    }
}