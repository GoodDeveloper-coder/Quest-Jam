using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{

    IEnumerator Move();

    void SetGhostFields(Transform suckUpPosition);

    void InitializeGhost();

    void MoveToVacumCleaner();
}
