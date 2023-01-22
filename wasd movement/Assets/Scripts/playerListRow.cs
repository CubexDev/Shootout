using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class playerListRow : MonoBehaviour
{
    public RectTransform rect;
    public Image img;
    public TMP_Text indexTxt;
    public TMP_Text nameTxt;
    public TMP_Text killsTxt;
    public TMP_Text deathsTxt;
    public TMP_Text kdTxt;

    static List<playerListRow> rows = new List<playerListRow>();

    Playermanager player;
    bool ownerPlayer;
    bool hostPlayer;

    Color32 lightColor = new Color32( 180, 180, 180, 100);
    Color32 darkColor = new Color32(100, 100, 100, 100);
    Color32 ownerColor = new Color32(96, 104, 211, 100);

    float timer = 1;

    public void setPlayer(Playermanager pPlayer)
    {
        player = pPlayer;
        ownerPlayer = player.IsOwner;
        hostPlayer = player.IsOwnedByServer;
        rows.Add(this);
        recalc();
    }

    public static void removePlayer(Playermanager pPlayer)
    {
        for (int i = 0; i < rows.Count; i++)
        {
            if(rows[i].player == pPlayer)
            {
                Destroy(rows[i].gameObject);
                rows.RemoveAt(i);
            }
        }
    }

    private void Update()
    {
        if (ownerPlayer)
        {
            if (timer >= 1f)
            {
                recalc();
            }
            timer += Time.deltaTime;
        }
    }

    void recalc()
    {
        timer = 0f;

        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].updateData();
        }
        sortList();
        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].setIndex(i);
        }
    }

    void sortList()
    {
        List<playerListRow> sortedRows = new List<playerListRow>();
        while (rows.Count > 0)
        {
            playerListRow bestPlayer = rows[0];
            for (int i = 1; i < rows.Count; i++)
            {
                if (rows[i].player.kills.Value > bestPlayer.player.kills.Value)
                    bestPlayer = rows[i];
                else if (rows[i].player.kills.Value == bestPlayer.player.kills.Value)
                    if (rows[i].player.deaths.Value < bestPlayer.player.deaths.Value)
                        bestPlayer = rows[i];
            }
            rows.Remove(bestPlayer);
            sortedRows.Add(bestPlayer);
        }
        rows = sortedRows;
    }

    void updateData()
    {
        int kills = player.kills.Value;
        int deaths = player.deaths.Value;
        float kd = deaths == 0 ? kills : (float)kills / (float)deaths;

        nameTxt.text = (hostPlayer ? "(host) " : "") + player.name;
        killsTxt.text = kills.ToString();
        deathsTxt.text = deaths.ToString();
        kdTxt.text = kd.ToString();
    }

    void setIndex(int pIndex)
    {
        rect.anchoredPosition = new Vector2(0, -75 - 50 * pIndex);
        indexTxt.text = (pIndex + 1).ToString();
        if (ownerPlayer) img.color = ownerColor;
        else img.color = pIndex % 2 == 0 ? lightColor : darkColor;
    }
}
