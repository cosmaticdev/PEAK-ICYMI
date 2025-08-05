#nullable enable
namespace MedalPeakPlugin;

public class PlayerModel
{
    public string playerId;
    public string playerName;

    public PlayerModel(string playerId, string playerName)
    {
        this.playerId = playerId;
        this.playerName = playerName;
    }
}
