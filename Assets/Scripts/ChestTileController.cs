using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using System;
using System.Diagnostics;
using UnityEngine;
using System.Collections;

public class ChestTileController : MonoBehaviour
{
    public Tilemap chestTilemap;
    public TileBase closedChestTile;
    public TileBase openChestTile;

    [Header("UI Prompt")]
    public TextMeshProUGUI interactionText;
    public UnityEngine.UI.Image backgroundImage;
    public float fadeDuration = 1f;
    public float interactionRange = 1.5f;

    public List<Vector3Int> chestTilePositions = new List<Vector3Int>();
    private Vector3Int? currentNearbyChest = null;
    private Coroutine fadeCoroutine;
    private ChestState currentState;
    private bool isPromptVisible = false;

    void Start()
    {
        chestTilePositions = new List<Vector3Int>
    {
        new Vector3Int(503, 322, 0),
        new Vector3Int(523, 325, 0),
        new Vector3Int(541, 320, 0),
        new Vector3Int(528, 317, 0),
        new Vector3Int(517, 312, 0),
        new Vector3Int(497, 307, 0),
        new Vector3Int(503, 313, 0),
        new Vector3Int(524, 300, 0),
        new Vector3Int(495, 297, 0),
        new Vector3Int(543, 300, 0),
        new Vector3Int(515, 292, 0),
        new Vector3Int(502, 287, 0),
        new Vector3Int(520, 285, 0),
        new Vector3Int(524, 287, 0),
        new Vector3Int(530, 290, 0),
        new Vector3Int(538, 293, 0)
    };

        SetState(new ClosedChestState(this));
        SetAlpha(0f);
    }


    void Update()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        Vector3Int? nearest = null;
        float minDist = float.MaxValue;

        foreach (var chestPos in chestTilePositions)
        {
            Vector3 worldPos = chestTilemap.GetCellCenterWorld(chestPos);
            float dist = Vector3.Distance(player.transform.position, worldPos);

            if (dist < interactionRange && dist < minDist)
            {
                nearest = chestPos;
                minDist = dist;
            }
        }

        if (nearest.HasValue)
        {
            if (!currentNearbyChest.HasValue || currentNearbyChest.Value != nearest.Value)
            {
                currentNearbyChest = nearest;
                SetState(new ClosedChestState(this));
                StartFadeInPrompt();
                isPromptVisible = true;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                SetState(new OpenChestState(this));
                SetTile(currentNearbyChest.Value, openChestTile);
                chestTilePositions.Remove(currentNearbyChest.Value);
                currentNearbyChest = null;
                StartFadeOutPrompt();
                isPromptVisible = false;
            }
        }
        else if (isPromptVisible)
        {
            currentNearbyChest = null;
            StartFadeOutPrompt();
            isPromptVisible = false;
        }

        currentState?.HandleInput();
    }

    public void SetState(ChestState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void SetTile(Vector3Int tilePos, TileBase newTile)
    {
        chestTilemap.SetTile(tilePos, newTile);

    }

    public void GiveReward()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        if (player.currentHealth < player.maxHealth)
        {
            player.TakeDamage(-1);
        }
        else
        {
            player.saltShots += 5;
        }

        player.UpdateBulletCount();
    }

  


    void StartFadeInPrompt()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadePrompt(1f));
    }

    void StartFadeOutPrompt()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadePrompt(0f));
    }

    IEnumerator FadePrompt(float targetAlpha)
    {
        float startAlpha = interactionText.color.a;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / fadeDuration);
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, blend);
            SetAlpha(newAlpha);
            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    void SetAlpha(float alpha)
    {
        var textColor = interactionText.color;
        textColor.a = alpha;
        interactionText.color = textColor;

        var bgColor = backgroundImage.color;
        bgColor.a = alpha;
        backgroundImage.color = bgColor;
    }

    public Vector3Int? GetCurrentChestPosition() => currentNearbyChest;
}