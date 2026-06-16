using UnityEngine;

public enum MusicPhase
{
    None,
    Beginning,
    Endgame
}

public enum TomeAuraPhase
{
    None,
    Foreshadowing,
    SpiritualRoom,
    PickedUp
}

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public MusicPhase currentPhase = MusicPhase.None;
    public TomeAuraPhase tomePhase = TomeAuraPhase.None;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
       /* AkUnitySoundEngine.PostEvent(
            "Play_1_IntroStinger",
            gameObject
        );*/
    }

    public void StartGrotteOgDungeonMusikEvent()
    {
        if (currentPhase != MusicPhase.None)
            return;

        AkUnitySoundEngine.PostEvent(
            "Play_2_Grotte_og_3_Dungeon",
            gameObject
        );
        
        currentPhase = MusicPhase.Beginning;
        
        if (tomePhase != TomeAuraPhase.None)
            return;

        AkUnitySoundEngine.PostEvent(
            "Play_4_TomeAura_3D_Loop",
            gameObject
        );
        tomePhase = TomeAuraPhase.Foreshadowing;
    }
    
    public void StartEndGameMusicEvent()
    {
        if (currentPhase == MusicPhase.Endgame)
            return;
        
        AkUnitySoundEngine.PostEvent(
            "Play_EndGameMusicSwitchContainer",
            gameObject
        );
        
        currentPhase = MusicPhase.Endgame;
    }
}
