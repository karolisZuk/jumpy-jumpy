using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    bool isJumpAnimating = false;

    public bool GetIsJumpAnimating()
    {
        return isJumpAnimating;
    }

    public void SetIsJumpAnimating(bool value)
    {
        isJumpAnimating = value;
    }
}
