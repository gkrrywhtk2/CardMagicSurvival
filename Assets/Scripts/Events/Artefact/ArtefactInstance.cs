using UnityEngine;

public class ArtefactInstance 
{
      public int ID;               // 아티팩트 고유 ID
      public int level;            // 아티팩트 레벨

    public ArtefactInstance(int artefactId, int artefactLevel)
    {
        ID = artefactId;
        level = artefactLevel;
    }
}
