using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellPointer : MonoBehaviour
{
    public enum PointerState { None, Active, Deactive, Selected }
    protected Animator mAnimator;

    private void Awake() => Initialize();

    protected virtual void Initialize()
    {
        mAnimator = GetComponent<Animator>();
    }

    public void ChangePointerState(PointerState state)
    {
        if (state == PointerState.Active) gameObject.SetActive(true);
        else if (state == PointerState.Deactive) gameObject.SetActive(false);
        else if (state == PointerState.Selected) mAnimator.SetTrigger("Select");
    }
}
