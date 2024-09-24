using UnityEngine;

public class LightTrailCollisions : MonoBehaviour
{

    //TODO: Need to create the process for detecting if someone has hit a light trail
    //This will be very perfomance critical since we have more than one object going to be running this

    //NOTE: Need to think about the maximum amount of AI Enemies in a level to still be performant without any significant frame drops

    /* NOTE: This seems to be a decent solution for this needs a lot of testing:
     * https://www.reddit.com/r/Unity3D/comments/14hpici/detecting_collisions_on_trail_renderer_in_3d/
     */

    void Start()
    {
    }

    void Update()
    {
    }
}
