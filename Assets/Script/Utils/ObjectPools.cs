using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectPoolType {Arrow=0,Hpbar,Provoke,Shield,Sleep,mon1,mon2 }
public class ObjectPools : MonoBehaviour
{
    [SerializeField]
    ObjectPooling[] objectPoolings;

    public ObjectPooling GetObjectPool(ObjectPoolType type)
    {
        return objectPoolings[(int)type];
    }
}
