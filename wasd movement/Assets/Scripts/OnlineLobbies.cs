using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsToUnity;

public class OnlineLobbies
{
    private static string[] associatedSheet = { "1B9mVvTRAOr9DomFnap_fKH3pbSFmU5onn3SFaGyzDKo",
    "2PACX-1vRST15WtDw-kB7Mf0jjUo-8qKSrUDPZU-ZFSYjlz-RGwg-_QkVnprP2zIGWqhgtAoMxW08lmwH1LMEI",};
    private static string associatedWorksheet = "Lobbies";

    public static void getLobbies()
    {
        for(int i = 0; i < associatedSheet.Length; i++)
        {
            SpreadsheetManager.ReadPublicSpreadsheet(new GSTU_Search(associatedSheet[i], associatedWorksheet, "A0", "E2"), convertLobbies);
        }
    }

    static void convertLobbies(GstuSpreadSheet ss)
    {
        Debug.Log("gfth");
        Debug.Log(ss.Cells.Count);
        foreach (var cell in ss.Cells)
        {
            Debug.Log(cell);
            Debug.Log(cell.Value);
            Debug.Log(cell.Value.value);
        }
        Debug.Log("A2 " + ss.columns["A"][0].value);
    }


}
