using UnityEngine;

public class GridTile
{
    public Vector3 Position { get; private set; }
    public bool IsPathTile { get; private set; }
    public bool HasTower { get; private set; }
    public Tower Tower { get; private set; }
    public bool InBurnRange {get; private set; }
    public float BurnTimer { get; private set; }
    public bool IsBurnt { get; private set; }
    private float _burnThreshold;
    private float _burnMultiplier;
    private float _healMultiplier;

    // Burning
    private GridTileFireEffect _fireEffect;

    // Constructor
    public GridTile(Vector3 position, float burnThreshold, float burnMultiplier, float healMultiplier)
    {
        Position = position;
        IsPathTile = false;
        HasTower = false;
        Tower = null;
        InBurnRange = false;
        BurnTimer = 0f;
        IsBurnt = false;
        _burnThreshold = burnThreshold;
        _burnMultiplier = burnMultiplier;
        _healMultiplier = healMultiplier;
    }

    public void UpdateTile(bool inBurnRange)
    {
        InBurnRange = inBurnRange;

        // If the tile is in burn range...
        if (InBurnRange)
        {
            // Increment the timer
            if (BurnTimer < _burnThreshold) BurnTimer += _burnMultiplier * Time.deltaTime;
        }
        else
        {
            // Decrement the timer if not in burn range
            if (BurnTimer > 0f) BurnTimer -= _healMultiplier * Time.deltaTime;
        }

        // If the tile crossed the burn threshold
        if (BurnTimer >= _burnThreshold)
        {
            // Set the tile as burned
            SetTileBurnState(true);

            // Disable the tower on it, if there is one
            if (HasTower && !Tower.IsDisabled) Tower.DisableTower(false);
        }
        else
        {
            // Set the tile as unburned
            SetTileBurnState(false);

            // If less than the burn threshold, enable the tower, if there is one
            if (HasTower && Tower.IsDisabled) Tower.DisableTower(true);
        }

        // DEBUG
        HighlightBurnedTile();
    }

    public bool IsValidTile() => !(IsPathTile || HasTower || IsBurnt);

    public void SetTileAsPath(bool value) => IsPathTile = value;
    public void AddTowerToTile(Tower tower)
    {
        Tower = tower;
        HasTower = true;
    }
    public void RemoveTowerFromTile()
    {
        Tower = null;
        HasTower = false;
    }

    public void IsInBurnRange(bool value) => InBurnRange = value;
    public void SetTileBurnState(bool value) => IsBurnt = value;

    public void ResetTileInfo()
    {
        SetTileAsPath(false);
        RemoveTowerFromTile();
        IsInBurnRange(false);
        BurnTimer = 0f;
        SetTileBurnState(false);
    }


    public void HighlightBurnedTile()
    {
        if (BurnTimer <= 0f)
        {
            // Remove the fire effect, if there is one
            if (_fireEffect != null)
            {
                GridManager.Instance.RemoveFireEffect(_fireEffect);
                _fireEffect = null;
            }

            return;
        }

        // Map burn timer to 0-1
        float percent = Mathf.Clamp01(BurnTimer / _burnThreshold);

        // Lerp the color depending on the burn timer
        Color lerpedColor = Color.Lerp(Color.white, Color.yellow, percent);

        // Make the color red if it is burnt
        if (IsBurnt) lerpedColor = Colors.redOrange;

        lerpedColor.a = percent;

        // DEBUG: Draw a circle with the lerped color
        DebugExtension.DebugCircle(Position, -Vector3.forward, lerpedColor, 0.5f);

        // Update the fire effect
        if (_fireEffect == null)
        {
            // There isn't a fire effect so spawn it
            _fireEffect = GridManager.Instance.SpawnFireEffect(this, lerpedColor);
        }
        else
        {
            // There already is one so update it with the proper color
            _fireEffect.UpdateFireEffect(lerpedColor);
        }
    }

}