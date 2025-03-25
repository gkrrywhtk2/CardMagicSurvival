using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Object/WeaponsData")]
public class WeaponsData : ScriptableObject
{
    //프론트엔드 웨폰 데이터
    public int weaponId;                   // 무기 고유번호
    public int materialCost_Upgrade;       // 강화 소모량
    public string weaponName_KOR;          // 한글 이름
    public WeaponGrade weaponGrade;        // 무기 등급
    public Sprite weaponMainSprite;        // 메인 스프라이트
    public enum EffectTag { ATK, CRI }
    public EffectTag[] tags;

    [Header("# ATK")]
    public float equippeBase_ATK;          // 장착 기본 공격력
    public float ownedBase_ATK;            // 보유 효과 기본 공격력

    // GetEuipedATK 로직 수정: 레벨마다 equippeBase_ATK의 10%씩 상승
    public float GetEuipedATK(int upgrade)
    {
        return equippeBase_ATK * (1 + 0.1f * upgrade); // 기본 공격력의 10%씩 증가
    }

    // GetOwnedATK 로직 수정: 레벨마다 ownedBase_ATK의 50%씩 상승
    public float GetOwnedATK(int level)
    {
        return ownedBase_ATK * (1 + 0.5f * level); // 기본 공격력의 50%씩 증가
    }

    [Header("# CRI")] // 크리티컬 데미지
    public float ownedBase_CRI;             // 보유 효과 치명타 공격력

    public float GetOwnedCRI(int level)
    {
        return ownedBase_CRI * (1 + 0.5f * level); // 기본 치명타 공격력의의 50%씩 증가
    }
}
