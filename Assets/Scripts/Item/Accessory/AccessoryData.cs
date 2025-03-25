using UnityEngine;
[CreateAssetMenu(fileName = "Accessory", menuName = "Scriptable Object/AccessoryData")]

public class AccessoryData : ScriptableObject
{
    //프론트엔드 웨폰 데이터
    public int accessoryId; // 고유번호
    public int UpgradeCost;       // 강화 소모량
    public string Name_KOR;          // 한글 이름
    public Sprite MainSprite;        // 메인 스프라이트
    public enum AccessoryGrade { Common, Rare, Epic, Legendary, Mythic, Primordial }
    public AccessoryGrade rank; //등급

    public enum EffectTag {HP,VIT,CRI,LUK}
    public EffectTag equipedTag;//장착 효과 태그
    public EffectTag[] ownedTags;//보유 효과 태그s

    [Header("# HP")]
    public float equippeBase_HP;          // 장착 기본 최대 체력
    public float ownedBase_HP;            // 보유 효과  최대 체력

    // 강화 수치마다 10%씩 상승
    public float GetEuipedHP(int upgrade)
    {
        return equippeBase_HP * (1 + 0.1f * upgrade); //10%씩 증가
    }
     // 로직 수정: 레벨마다 50%씩 상승
    public float GetOwnedHP(int level)
    {
        return ownedBase_HP * (1 + 0.5f * level); //50%씩 증가
    }

    [Header("# VIT")] // 초당 체력 회복
    public float equipBase_VIT;             // 장착 초당 회복 
    public float ownedBase_VIT;             // 보유 초당 회복 
    public float GetEuipedVIT(int upgrade)
    {
        return equipBase_VIT * (1 + 0.1f * upgrade); //10%씩 증가
    }

    public float GetOwnedVIT(int level)
    {
        return ownedBase_VIT * (1 + 0.5f * level); //50%씩 증가
    }

    [Header("# CRI")] // 치명타 데미지
    public float equipBase_CRI;             // 장착 초당 회복 
    public float ownedBase_CRI;             // 보유 초당 회복 
    public float GetEuipedCRI(int upgrade)
    {
        return equipBase_CRI * (1 + 0.1f * upgrade); //10%씩 증가
    }

    public float GetOwnedCRI(int level)
    {
        return ownedBase_CRI * (1 + 0.5f * level); //50%씩 증가
    }

     [Header("# LUK")] // 추가 골드 획득
    public float equipBase_LUK;             // 장착
    public float ownedBase_LUK;             // 보유
    public float GetEuipedLUK(int upgrade)
    {
        return equipBase_LUK * (1 + 0.1f * upgrade); //10%씩 증가
    }

    public float GetOwnedLUK(int level)
    {
        return ownedBase_LUK * (1 + 0.5f * level); //50%씩 증가
    }


    
}
