using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{

    IEnumerator Move();

    void SetGhostFields(Transform suckUpPosition, float speedOfSuckUpGhostInVacumCleaner);

    void InitializeGhost();

    void MoveToVacumCleaner();
}
