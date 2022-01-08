using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Purchasing;


namespace Gismo.Core
{
    public class GameInitalizer : MonoBehaviour
    {
        [SerializeField] private float startTapCooldown;
        [SerializeField] private float startingForce;
        [SerializeField] private float startingCountdownTime;

        [SerializeField] private Tools.BoundaryController ground;
        [SerializeField] private Tools.BoundaryController top;
        [SerializeField] private Tools.BoundaryController left;
        [SerializeField] private Tools.BoundaryController right;

        [SerializeField] private bool developerMode;

        [SerializeField] private Shop.StyleShopController shopController;

        void Start()
        {
            Application.targetFrameRate = 60;
            ground.SetPosition(
                new Vector3(Tools.ScreenBoundaries.GetPoint(
                    new Vector2(.5f, .5f)).x, 
                    Tools.ScreenBoundaries.GetPoint(Tools.PresetPoints.BottomRight).y + .15f, 
                    1f)
                );

            ground.SetBoundarySize(
                new Vector2(
                    Tools.ScreenBoundaries.GetWorldWidth(), .3f)
                );
            
            top.SetPosition(
                new Vector3(Tools.ScreenBoundaries.GetPoint(
                    new Vector2(.5f, .5f)).x,   
                    Tools.ScreenBoundaries.GetPoint(Tools.PresetPoints.TopRight).y,
                    1f)
                );

            top.SetScale(
                new Vector3(
                    Tools.ScreenBoundaries.GetWorldWidth(), 
                    top.GetScale().y, 
                    1f)
                );

            left.SetPosition(
                new Vector3(
                    Tools.ScreenBoundaries.GetPoint(Tools.PresetPoints.TopLeft).x,
                    Tools.ScreenBoundaries.GetPoint(new Vector2(.5f, .5f)).y,
                    1f)
                );

            left.SetBoundarySize(
               new Vector2(
                   .4f, Tools.ScreenBoundaries.GetWorldHeight())
               );

            right.SetPosition(
                new Vector3(
                    Tools.ScreenBoundaries.GetPoint(Tools.PresetPoints.TopRight).x,
                    Tools.ScreenBoundaries.GetPoint(new Vector2(.5f, .5f)).y,
                    1f)
                );

            right.SetBoundarySize(
               new Vector2(
                   .4f, Tools.ScreenBoundaries.GetWorldHeight())
               );

            left.SetCollidability(false);
            right.SetCollidability(false);

            Statics.developerMode = developerMode;

            Statics.currentTapCooldown = startTapCooldown;
            Statics.currentForce = startingForce;
            Statics.startingCountdownTime = startingCountdownTime;

            Statics.stylePoints = PlayerPrefs.GetInt("Style Points");

            shopController.LoadSelected();

            Ads.AdManager.instance.Initalize();

            GameController.instance.ResetGame();
        }
    }
}
