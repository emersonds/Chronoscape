using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Knight : PlayerController
{
    // May be helpful to reference this documentation while designing the characters:
    // https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/object-oriented/polymorphism

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void GetInput()
    {
        if (Input.GetButtonDown("BasicAbility"))
            ActivateBasicAbility();
        if (Input.GetButtonDown("UltimateAbility"))
            ActivateUltimateAbility();

        base.GetInput();
    }

    protected override void ActivateBasicAbility()
    {
        Debug.Log("Basic ability cast!");
    }

    protected override void ActivateUltimateAbility()
    {
        Debug.Log("Ultimate ability cast!");
    }
}
