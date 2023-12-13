namespace Netsphere.Server.Game
{
    public enum CharacterCreateResult
    {
        Success,
        LimitReached,
        InvalidGender,
        InvalidSlot,
        InvalidDefaultItem,
        SlotInUse
    }

    public enum CharacterInventoryError
    {
        OK,
        InvalidSlot,
        SlotAlreadyInUse,
        ItemNotAllowed,
        ItemAlreadyInUse
    }

    public enum ChannelJoinError
    {
        OK,
        AlreadyInChannel,
        ChannelFull
    }

    public enum RoomJoinError
    {
        OK,
        AlreadyInRoom,
        RoomFull,
        KickedPreviously,
        ChangingRules,
        NoIntrusion
    }

    public enum TeamJoinError
    {
        OK,
        AlreadyInTeam,
        TeamFull
    }

    public enum TeamChangeError
    {
        OK,
        WrongRoom,
        GameIsRunning,
        NotInTeam,
        AlreadyInTeam,
        PlayerIsReady,
        InvalidTeam,
        Full
    }

    public enum TeamChangeModeError
    {
        OK,
        WrongRoom,
        PlayerIsPlaying,
        NotInTeam,
        AlreadyInMode,
        PlayerIsReady,
        InvalidMode,
        Full
    }

    public enum RoomCreateError
    {
        OK,
        InvalidGameRule,
        InvalidMap
    }

    public enum RoomChangeRulesError
    {
        OK,
        AlreadyChangingRules,
        InvalidMap,
        InvalidGameRule,
        PlayerLimitTooLow
    }

    public enum GameRuleState
    {
        Waiting,
        Loading,
        Starting,
        Playing,
        EnteringResult,
        Result,

        FirstHalf,
        EnteringHalfTime,
        HalfTime,
        SecondHalf
    }

    public enum GameRuleStateTrigger
    {
        StartGame,
        EndGame,
        StartResult,
        StartHalfTime,
        StartSecondHalf
    }

    public enum ClanCreateError
    {
        None,
        NameAlreadyExists,
        AlreadyInClan
    }

    public enum ClanEvent
    {
        Register,
        Join,
        Leave,
        Approve,
        Decline,
        Kick,
        Ban,
        Unban
    }
}
