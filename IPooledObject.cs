using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************
 * IPooledObject
 * The interface representing objects that are pooled using the
 * object pooler.
 * *************************************************************/
public interface IPooledObject
{
    // OnObjectSpawn is like the start method when
    // the object is spawned.
    void OnObjectSpawn();
}
