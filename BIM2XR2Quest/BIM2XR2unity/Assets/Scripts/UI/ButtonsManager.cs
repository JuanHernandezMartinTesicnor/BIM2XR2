using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Oculus.Interaction;
using UnityEngine;

public class ButtonsManager : MonoBehaviour
{
    public bool deactivateOnStart = true;
    public Material activeButtonMaterial;
    public Material inactiveButtonMaterial;
    public GameObject buttonsParent;
    public List<GameObject> buttons;
    public List<bool> toBeDeactivatedUntilRoomChoosed;
    public float zOffset = 0.05f;
    public float yOffset = 0.02f;
    public float xRotation = 10.0f;
    public float animTime = 0.1f;

    private bool buttonHiglighted = false;
    private float buttonOriginalYPos = 0.0f;
    private float butonRelOriginalYPos = 0.0f;

    public ButtonsManager parentButtonsManager;

    [HideInInspector]
    public bool roomChoosed;

    public void Awake()
    {
        roomChoosed = false;
        buttonOriginalYPos = buttons[0].transform.localPosition.y;
        butonRelOriginalYPos = buttonsParent.transform.localPosition.y;

        if (deactivateOnStart)
        {
            DeactivateButtons();
        }
    }

    public void ToggleButtons()
    {

        if (!buttonsParent.activeSelf)
        {
            buttonsParent.SetActive(true);
            ResetButtons();
        }
        else
        {
            TurnOffButtons();
            StartCoroutine(DeactivateButtonParent());
        }
    }

    public void ToggleHighlightButton(GameObject chosenButton)
    {
        if (!buttonHiglighted)
        {
            ChooseButton(chosenButton);
            if (parentButtonsManager != null)
            {
                parentButtonsManager.HiglightButton(this.gameObject, false);
            }
        }
        else
        {
            if (parentButtonsManager != null)
            {
                parentButtonsManager.HiglightButton(this.gameObject, true);
            }
            ResetButtons(chosenButton);
        }
        buttonHiglighted = !buttonHiglighted;
    }


    public void ChooseButton(GameObject chosenButton)
    {
        SlideButtons(chosenButton);

        foreach (GameObject btn in buttons)
        {
            if (btn != chosenButton)
            {
                HiglightButton(btn, false);
            }
            else
            {
                if (toBeDeactivatedUntilRoomChoosed[buttons.IndexOf(btn)] && !roomChoosed)
                {
                    continue;
                }
                btn.GetComponent<PokeInteractable>().enabled = false;
                StartCoroutine(ReEnableChosenButtonColider(btn));
            }
        }
    }

    private IEnumerator ReEnableChosenButtonColider(GameObject chosenButton)
    {
        yield return new WaitForSeconds(animTime);
        chosenButton.GetComponent<PokeInteractable>().enabled = true;
    }

    private IEnumerator DeactivateButtonParent()
    {
        yield return new WaitForSeconds(animTime);
        DeactivateButtons();
    }

    public void ResetButtons(GameObject chosenButton)
    {
        SlideButtons(chosenButton, false);

        foreach (GameObject btn in buttons)
        {
            btn.GetComponent<PokeInteractable>().enabled = false;

            if (toBeDeactivatedUntilRoomChoosed[buttons.IndexOf(btn)] && !roomChoosed)
            {
                continue;
            }

            StartCoroutine(ReEnableChosenButtonColider(btn));
            if (btn != chosenButton)
            {

                HiglightButton(btn);
            }
        }
    }

    public void ResetButtons()
    {
        foreach (GameObject btn in buttons)
        {
            btn.gameObject.SetActive(true);
            if (toBeDeactivatedUntilRoomChoosed[buttons.IndexOf(btn)] && !roomChoosed)
            {
                continue;
            }
            HiglightButton(btn);
        }
    }

    public void TurnOffButtons()
    {
        foreach (GameObject btn in buttons)
        {
            btn.gameObject.SetActive(true);
            if (toBeDeactivatedUntilRoomChoosed[buttons.IndexOf(btn)] && !roomChoosed)
            {
                continue;
            }
            HiglightButton(btn, false);
        }
    }


    private void SlideButtons(GameObject chosenButton, bool slideIn = true)
    {
        if (slideIn)
        {
            //Vector3 paretnsPos = buttonsParent.transform.localPosition;
            Vector3 offset = chosenButton.transform.localPosition;
            //Vector3 offset = chosenButtonPos - paretnsPos;
            offset.z = 0;
            offset.y = -butonRelOriginalYPos;
            buttonsParent.transform.DOLocalMove(-offset, animTime).SetEase(Ease.OutBack);
        }
        else
        {
            Vector3 newButtParentPos = Vector3.zero;
            newButtParentPos.y = butonRelOriginalYPos;
            buttonsParent.transform.DOLocalMove(newButtParentPos, animTime).SetEase(Ease.OutBack);
        }
    }

    public void DeactivateButtons()
    {
        buttonsParent.SetActive(false);
        buttonHiglighted = false;
        foreach (GameObject btn in buttons)
        {
            HiglightButton(btn, false);
        }
    }

    public void HiglightButton(GameObject btn, bool highlight = true)
    {
        Vector3 btnPos = btn.transform.localPosition;
        btn.GetComponent<PokeInteractable>().enabled = highlight;
        int toHiglithAsInt = highlight ? 0 : 1;
        btnPos.y = buttonOriginalYPos - yOffset * toHiglithAsInt;
        btnPos.z = zOffset * toHiglithAsInt;
        btn.transform.DOLocalRotate(new Vector3(-xRotation * toHiglithAsInt, 0, 0), animTime).SetEase(Ease.OutBack);
        btn.transform.DOLocalMove(btnPos, animTime).SetEase(Ease.OutBack);


        Renderer backPlateRenderer = btn.GetComponent<ButtonElements>()?.backPlateRenderer;
        if (backPlateRenderer != null)
        {
            backPlateRenderer.material = highlight ? activeButtonMaterial : inactiveButtonMaterial;
        }

    }
}
