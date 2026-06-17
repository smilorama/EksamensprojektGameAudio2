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

    public void SetStateTomePossession()
    {
        AkUnitySoundEngine.SetState("EndGameMusicStates", "TomePossession");
    }

    public void SetStateTomeBraending()
    {
        AkUnitySoundEngine.SetState("EndGameMusicStates", "TomeBraending");
    }

    public void SetStateTomeOutside(bool tomeBurned)
    {
        if (tomeBurned)
            AkUnitySoundEngine.SetState("EndGameMusicStates", "HappyEndning");
        else
            AkUnitySoundEngine.SetState("EndGameMusicStates", "TomeOutside");
    }

    public void SetStatePlayerDeath()
    {
        AkUnitySoundEngine.SetState("EndGameMusicStates", "PlayerDeath");
    }
}
