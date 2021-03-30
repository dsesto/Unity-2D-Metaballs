/// Author: David Sesto (GitHub: @dsesto)
/// Digging Dinosaurs Games (Twitter @digging_dinos)

using UnityEngine;

/*
 * Representation of a metaball, which includes its definition and some basic functionalities
 */
public class Metaball {
    public Vector2 position;
    public Vector2 velocity;
    public float radius;

    private float startPositionMargin = 0.1f; // 10% margin
    private float speedRange = 0.5f; // +- 50%
    private float radiusRange = 0.5f; // +- 50%

    /// Instantiate a Metaball, with a random starting position, velocity and radius
    public Metaball() {
        float startX = Random.Range(MetaballManager.instance.boundaries.xMin * (1 - startPositionMargin),
                                    MetaballManager.instance.boundaries.xMax * (1 - startPositionMargin));
        float startY = Random.Range(MetaballManager.instance.boundaries.yMin * (1 - startPositionMargin),
                                    MetaballManager.instance.boundaries.yMax * (1 - startPositionMargin));
        position = new Vector2(startX, startY);

        float velocityMagnitude = MetaballManager.instance.metaballSpeed * Random.Range(1 - speedRange, 1 + speedRange);
        velocity = new Vector2(RandomSign(), RandomSign()) * velocityMagnitude;

        radius = MetaballManager.instance.metaballRadius * Random.Range(1 - radiusRange, 1 + radiusRange);
    }

    /// Moves the Metaball when called, on each frame update
    public void Move() {
        position += velocity * Time.deltaTime;

        // Change direction of ball movement if it is out ouf bounds
        if (position.x >= MetaballManager.instance.boundaries.xMax)
            velocity.x = Mathf.Abs(velocity.x) * -1;
        else if (position.x <= MetaballManager.instance.boundaries.xMin)
            velocity.x = Mathf.Abs(velocity.x);

        if (position.y >= MetaballManager.instance.boundaries.yMax)
            velocity.y = Mathf.Abs(velocity.y) * -1;
        else if (position.y <= MetaballManager.instance.boundaries.yMin)
            velocity.y = Mathf.Abs(velocity.y);
    }

    #region Helpers
    private int RandomSign() {
        if (Random.value > 0.5f)
            return 1;
        else
            return -1;
    }
    #endregion
}
