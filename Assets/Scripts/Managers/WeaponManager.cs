using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    public Image[] waeaponIcons;
    public TMP_Text[] upgradeText;//강화 수치
    public TMP_Text[] stackCountText;//중첩 수치
    public Image[] fills;//게이지 수치 fill 오브젝트
    public int[] weaponCountText;//0/5, 아이템 보유 수치

    //Weapon List
    public List<Weapon> weaponList = new List<Weapon>();//웨폰 데이터 저장

    public void LoadWeaponList(List<Weapon> weaponlists){
        weaponList = weaponlists;

    }

    public void PrintWeaponList()
{
    foreach (Weapon weapon in weaponList)
    {
        Debug.Log("Weapon ID: " + weapon.weaponId + 
                  ", Upgrade Level: " + weapon.upgradeLevel + 
                  ", Grade: " + weapon.grade + 
                  ", Stack Count: " + weapon.stackCount + 
                  ", Is Equipped: " + weapon.isEquipped);
    }
}


}
