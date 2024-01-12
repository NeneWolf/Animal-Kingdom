using System;

public class PlayerData 
{
    public string uID;
    public string username;
    public int bestScore;
    public string bestScoreDate;
    public int totalPlayersInGame;
    public string roomName;

    public PlayerData()
    {
        uID = Guid.NewGuid().ToString();
    }
}
