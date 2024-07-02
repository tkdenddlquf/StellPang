using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour, IPointerClickHandler
{
    [Header("Main")]
    public GameObject main_TimeAttack;
    public GameObject main_Stage;
    public GameObject main_Exit;

    [Header("Stage")]
    public GameObject[] stage_rounds;

    [Header("Game")]
    public GameObject game_TimeAttack;
    public GameObject game_Stage;
    public TMP_Text[] game_score_Text;
    public GameObject[] diretionButton;

    [Header("Result")]
    public GameObject result_Replay;
    public GameObject result_Main;

    [Header("ESC")]
    public GameObject esc_Continue;
    public GameObject esc_Replay;
    public GameObject esc_Main;

    private readonly Dictionary<GameObject, System.Action> clickEvent = new();

    public void Start()
    {
        // MAIN
        clickEvent.Add(main_TimeAttack, () =>
        {
            GameManager._instance.PlayGame("TimeAttack");

            game_TimeAttack.SetActive(true);
            game_Stage.SetActive(false);

            GameManager._instance.score_Text = game_score_Text[0];
        });

        clickEvent.Add(main_Stage, () =>
        {
            GameManager._instance.PlayGame($"Stage_{0}");

            game_TimeAttack.SetActive(false);
            game_Stage.SetActive(true);

            GameManager._instance.score_Text = game_score_Text[1];
        });

        clickEvent.Add(main_Exit, () =>
        {
            Application.Quit();
        });

        // GAME
        clickEvent.Add(diretionButton[0], () =>
        {
            GameManager._instance.SetDirection(3);
        });

        clickEvent.Add(diretionButton[1], () =>
        {
            GameManager._instance.SetDirection(2);
        });

        clickEvent.Add(diretionButton[2], () =>
        {
            GameManager._instance.SetDirection(1);
        });

        clickEvent.Add(diretionButton[3], () =>
        {
            GameManager._instance.SetDirection(0);
        });

        // RESULT
        clickEvent.Add(result_Replay, () =>
        {
            GameManager._instance.PlayGame("");
        });

        clickEvent.Add(result_Main, () =>
        {
            GameManager._instance.GoMain();
        });

        // ESC
        clickEvent.Add(esc_Continue, () =>
        {
            GameManager._instance.ESC();
        });

        clickEvent.Add(esc_Replay, () =>
        {
            GameManager._instance.ESC();
            GameManager._instance.PlayGame("");
        });

        clickEvent.Add(esc_Main, () =>
        {
            GameManager._instance.ESC();
            GameManager._instance.GoMain();
        });
    }

    public void OnPointerClick(PointerEventData _eventData)
    {
        if (clickEvent.ContainsKey(_eventData.pointerEnter))
        {
            clickEvent[_eventData.pointerEnter]();
        }
    }
}
