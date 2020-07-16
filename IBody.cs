using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************
 * IBody
 * The body interface representing objects that should react
 * to getting hit by blasters.
 * *************************************************************/
public interface IBody
{
    void GetHit(GameObject projectileOwner);
}
