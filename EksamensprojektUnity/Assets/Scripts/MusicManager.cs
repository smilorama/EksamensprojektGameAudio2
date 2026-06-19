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

    public void SetStateTomeAuraNone()
    {
        AkUnitySoundEngine.SetState("TomeAuraStates", "None");
    }

    public void SetStateTomeFire()
    {
        AkUnitySoundEngine.SetState("EndGameMusicStates", "TomeFire");
    }

    public void StartTome2DStereo()
    {
        AkUnitySoundEngine.PostEvent("Play_4_2_TomeAura_STEREO_2D", gameObject);
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

    public void MusicBurnEvent()
    {
        AkUnitySoundEngine.PostEvent("Play_2_5_TomeForeshadowing_SyretFeedbackReverb_BurningScream", gameObject);
    }

    public void PostDelayedVolumeDownTomeAura()
    {
        AkUnitySoundEngine.PostEvent("Delayed_VolumeDown_TomeAura", gameObject);
    }
}
