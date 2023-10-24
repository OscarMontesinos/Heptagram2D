using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Orel : PjBase
{
    public GameObject desertFeather;
    public float a1Damage;
    public float a1Spd;
    public float a1Range;
    public float a1detour;
    public float a1Modifier;


    public float a2Damage;
    public float a2Spd;
    public float a2Range;
    public float a2detour;
    public override void MainAttack()
    {
        if (!IsCasting())
        {
            StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd)));
            DesertFeather feather = Instantiate(desertFeather, transform.position, controller.pointer.transform.rotation).GetComponent<DesertFeather>();
            feather.SetUp(this, a1Spd, a1Range, CalculateSinergy(a1Damage));
            if(CharacterManager.Instance.data[1].convergence >= 6)
            {
                DesertFeather feather2 = Instantiate(desertFeather, transform.position, controller.pointer.transform.rotation).GetComponent<DesertFeather>();
                feather2.SetUp(this, Random.Range(a1Spd-20,a1Spd), a1Range, CalculateSinergy(a1Damage / a1Modifier));
                feather2.transform.localEulerAngles = new Vector3(feather2.transform.localEulerAngles.x, feather2.transform.localEulerAngles.y, feather2.transform.localEulerAngles.z + Random.Range(1,a1detour));
                DesertFeather feather3 = Instantiate(desertFeather, transform.position, controller.pointer.transform.rotation).GetComponent<DesertFeather>();
                feather3.SetUp(this, Random.Range(a1Spd - 20, a1Spd), a1Range, CalculateSinergy(a1Damage / a1Modifier));
                feather3.transform.localEulerAngles = new Vector3(feather3.transform.localEulerAngles.x, feather3.transform.localEulerAngles.y, feather3.transform.localEulerAngles.z - Random.Range(1, a1detour));
            }
        }
        base.MainAttack();
    }

    public override void StrongAttack()
    {
        if (!IsCasting())
        {
            if (CharacterManager.Instance.data[1].convergence >= 1)
            {

            }

            StartCoroutine(FeatherDischarge());
        }
        base.StrongAttack();
    }

    IEnumerator FeatherDischarge()
    {
        controller.LockPointer(true);
        yield return StartCoroutine(Cast(CalculateAtSpd(stats.atSpd) * strongAtSpdMultiplier));

        DesertFeather feather = Instantiate(desertFeather, transform.position, controller.pointer.transform.rotation).GetComponent<DesertFeather>();
        feather.SetUp(this, Random.Range(a2Spd - 30, a2Spd), a2Range, CalculateSinergy(a2Damage));
        feather.transform.localEulerAngles = new Vector3(feather.transform.localEulerAngles.x, feather.transform.localEulerAngles.y, feather.transform.localEulerAngles.z - Random.Range(-a2detour, a2detour));

        DesertFeather feather2 = Instantiate(desertFeather, transform.position, controller.pointer.transform.rotation).GetComponent<DesertFeather>();
        feather2.SetUp(this, Random.Range(a2Spd - 30, a2Spd), a2Range, CalculateSinergy(a2Damage));
        feather2.transform.localEulerAngles = new Vector3(feather2.transform.localEulerAngles.x, feather2.transform.localEulerAngles.y, feather2.transform.localEulerAngles.z - Random.Range(-a2detour, a2detour));

        DesertFeather feather3 = Instantiate(desertFeather, transform.position, controller.pointer.transform.rotation).GetComponent<DesertFeather>();
        feather3.SetUp(this, Random.Range(a2Spd - 30, a2Spd), a2Range, CalculateSinergy(a2Damage));
        feather3.transform.localEulerAngles = new Vector3(feather3.transform.localEulerAngles.x, feather3.transform.localEulerAngles.y, feather3.transform.localEulerAngles.z - Random.Range(-a2detour, a2detour));

        DesertFeather feather4 = Instantiate(desertFeather, transform.position, controller.pointer.transform.rotation).GetComponent<DesertFeather>();
        feather4.SetUp(this, Random.Range(a2Spd - 30, a2Spd), a2Range, CalculateSinergy(a2Damage));
        feather4.transform.localEulerAngles = new Vector3(feather4.transform.localEulerAngles.x, feather4.transform.localEulerAngles.y, feather4.transform.localEulerAngles.z - Random.Range(-a2detour, a2detour));

        DesertFeather feather5 = Instantiate(desertFeather, transform.position, controller.pointer.transform.rotation).GetComponent<DesertFeather>();
        feather5.SetUp(this, Random.Range(a2Spd - 30, a2Spd), a2Range, CalculateSinergy(a2Damage));
        feather5.transform.localEulerAngles = new Vector3(feather5.transform.localEulerAngles.x, feather5.transform.localEulerAngles.y, feather5.transform.localEulerAngles.z - Random.Range(-a2detour, a2detour));

        controller.LockPointer(false);
        yield return StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd)));
    }
}
