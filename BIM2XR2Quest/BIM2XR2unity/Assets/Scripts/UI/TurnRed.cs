using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class TurnRed : MonoBehaviour
{
    public Material redMat;
    public Renderer render;
    private Material originalMat;

    private bool isActive;

    public void Start()
    {
        originalMat = render.material;
    }

    public void TurnRedNow()
    {
        print("holis");
        isActive = !isActive;
        render.material = isActive ? redMat : originalMat;
    }
}
