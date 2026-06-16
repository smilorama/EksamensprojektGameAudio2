/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID ATTACK_HITENEMY = 3927008369U;
        static const AkUniqueID ATTACK_HITNOTHING = 726189174U;
        static const AkUniqueID ATTACK_HITTERRAIN = 1224938770U;
        static const AkUniqueID DRINK_POTION = 2541729425U;
        static const AkUniqueID PLAY_2_GROTTE_OG_3_DUNGEON = 3932489860U;
        static const AkUniqueID PLAY_4_TOMEAURA_3D_LOOP = 2272618794U;
        static const AkUniqueID PLAY_ATTACK_ENEMY_HIT = 486256373U;
        static const AkUniqueID PLAY_ATTACK_ENEMY_MISS = 1284475082U;
        static const AkUniqueID PLAY_BRAZIER = 2521551685U;
        static const AkUniqueID PLAY_CAVE = 3602995889U;
        static const AkUniqueID PLAY_DEATH_CREATURE = 795377228U;
        static const AkUniqueID PLAY_DEATH_CULTIST = 3622272533U;
        static const AkUniqueID PLAY_ENDGAMEMUSICSWITCHCONTAINER = 3311667097U;
        static const AkUniqueID PLAY_FOOTSTEP = 1602358412U;
        static const AkUniqueID PLAY_FOOTSTEP_ENEMY = 1095021605U;
        static const AkUniqueID PLAY_IDLE_CULTIST = 1329754913U;
        static const AkUniqueID PLAY_LINJE1 = 743624797U;
        static const AkUniqueID PLAY_LINJE2 = 743624798U;
        static const AkUniqueID PLAY_LOWHEALTH = 1279037710U;
        static const AkUniqueID PLAY_OUTSIDE = 473294595U;
        static const AkUniqueID PLAY_ROOMTONE_TEMPLE = 3699433833U;
        static const AkUniqueID PLAY_WATER_LOOP = 1564978696U;
        static const AkUniqueID PLAY_WEIRD_ROOMTONE = 2387658637U;
        static const AkUniqueID STOP_IDLE_CULTIST = 1199249995U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace BEGINNINGMUSICROOMSTATES
        {
            static const AkUniqueID GROUP = 831006884U;

            namespace STATE
            {
                static const AkUniqueID DUNGEONBEGINNING = 4167468798U;
                static const AkUniqueID GROTTEBEGINNING = 87758831U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace BEGINNINGMUSICROOMSTATES

        namespace ENDGAMEMUSICSTATES
        {
            static const AkUniqueID GROUP = 538938061U;

            namespace STATE
            {
                static const AkUniqueID HAPPYENDNING = 376991726U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID PLAYERDEATH = 1656947812U;
                static const AkUniqueID TOMEBRAENDING = 1119949180U;
                static const AkUniqueID TOMEOUTSIDE = 1776398501U;
                static const AkUniqueID TOMEPICKUP = 2158032164U;
                static const AkUniqueID TOMEPOSSESSION = 1212775290U;
            } // namespace STATE
        } // namespace ENDGAMEMUSICSTATES

        namespace TOMEAURASTATES
        {
            static const AkUniqueID GROUP = 3042705799U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID TOMEFORESHADOWING = 1866109078U;
                static const AkUniqueID TOMEPICKUP = 2158032164U;
                static const AkUniqueID TOMESPIRITUALROOM = 1167319226U;
            } // namespace STATE
        } // namespace TOMEAURASTATES

    } // namespace STATES

    namespace SWITCHES
    {
        namespace MATERIALS
        {
            static const AkUniqueID GROUP = 4050929301U;

            namespace SWITCH
            {
                static const AkUniqueID DIRT = 2195636714U;
                static const AkUniqueID GRASS = 4248645337U;
                static const AkUniqueID STONE = 1216965916U;
                static const AkUniqueID TILE = 2637588553U;
                static const AkUniqueID WATER = 2654748154U;
            } // namespace SWITCH
        } // namespace MATERIALS

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID PLAYERHEALTH_PARAM = 2337577746U;
        static const AkUniqueID TOMEPOSSESSIONDELAY = 3059304411U;
        static const AkUniqueID TOMEPOSSESSIONMODULATION = 614263488U;
        static const AkUniqueID TOMEPOSSESSIONTIMESTRETCH = 828022604U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID MAIN = 3161908922U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID MAIN_AUDIO_BUS = 2246998526U;
        static const AkUniqueID MUSIC_BUS = 3127962312U;
    } // namespace BUSSES

    namespace AUX_BUSSES
    {
        static const AkUniqueID LILLERUM_REV = 2986432809U;
        static const AkUniqueID MEDIUMRUM_REV = 3454183034U;
        static const AkUniqueID PARKERINGREV_001_BAELTE1 = 288340792U;
        static const AkUniqueID PARKERINGREV_002_BAELTE2 = 1020492676U;
        static const AkUniqueID PARKERINGREV_003_KLAP1 = 230976631U;
        static const AkUniqueID PARKERINGREV_004_KLAP2 = 3950004767U;
        static const AkUniqueID PARKERINGREV_005_KLAP3 = 903063735U;
        static const AkUniqueID PARKERINGREV_006_KLAP4 = 2665109655U;
        static const AkUniqueID PARKERINGREV_007_KLAP5 = 2767839671U;
        static const AkUniqueID PARKERINGREV_008_KLAP6 = 3813691439U;
        static const AkUniqueID STORTRUM1_REV = 3943616538U;
        static const AkUniqueID STORTRUM2_REV = 2730326357U;
    } // namespace AUX_BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
