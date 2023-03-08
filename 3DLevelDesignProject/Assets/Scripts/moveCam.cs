using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCam : MonoBehaviour
{
   [SerializeField] Transform camPos;

   private void Update() {
    transform.position = camPos.position;
   }
}
