using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGiveItems : MonoBehaviour
{
    public void Start()
    {
        if (GetComponentInParent<Fighter>().isPlayer == true && GetComponent<GatitoBlade>() == null)
        {
            ItemCheck();
        }
    }

    public void ItemCheck() // Update on Unarmed too since no weapon
    {
        if (SceneSwitcher.Instance.HasItem("BloodoftheArcher"))
        {
            var botA = gameObject.AddComponent<BloodoftheArcher>();
            botA.stacks = SceneSwitcher.Instance.GetItemCount("BloodoftheArcher");
        }

        if (SceneSwitcher.Instance.HasItem("BloodoftheBandit"))
        {
            var botB = gameObject.AddComponent<BloodoftheBandit>();
            botB.stacks = SceneSwitcher.Instance.GetItemCount("BloodoftheBandit");
        }

        if (SceneSwitcher.Instance.HasItem("BloodoftheKnight"))
        {
            var botK = gameObject.AddComponent<BloodoftheKnight>();
            botK.stacks = SceneSwitcher.Instance.GetItemCount("BloodoftheKnight");
        }

        if (SceneSwitcher.Instance.HasItem("BloodoftheSoldier"))
        {
            var botS = gameObject.AddComponent<BloodoftheSoldier>();
            botS.stacks = SceneSwitcher.Instance.GetItemCount("BloodoftheSoldier");
        }

        if (SceneSwitcher.Instance.HasItem("Food"))
        {
            var food = gameObject.AddComponent<Food>();
            food.stacks = SceneSwitcher.Instance.GetItemCount("Food");
        }

        if (SceneSwitcher.Instance.HasItem("GlassBall"))
        {
            var glassBall = gameObject.AddComponent<GlassBall>();
            glassBall.stacks = SceneSwitcher.Instance.GetItemCount("GlassBall");
        }

        if (SceneSwitcher.Instance.HasItem("RaiseTheRoof"))
        {
            var raisetheRoof = gameObject.AddComponent<RaiseTheRoof>();
            raisetheRoof.stacks = SceneSwitcher.Instance.GetItemCount("RaiseTheRoof");
        }

        if (SceneSwitcher.Instance.HasItem("Training"))
        {
            var training = gameObject.AddComponent<Training>();
            training.stacks = SceneSwitcher.Instance.GetItemCount("Training");
        }

        if (SceneSwitcher.Instance.HasItem("TriTippedDagger"))
        {
            var ttD = gameObject.AddComponent<TriTippedDagger>();
            ttD.stacks = SceneSwitcher.Instance.GetItemCount("TriTippedDagger");
        }
        if (SceneSwitcher.Instance.HasItem("BloodoftheMage"))
        {
            var bloodoftheMage = gameObject.AddComponent<BloodoftheMage>();
            bloodoftheMage.stacks = SceneSwitcher.Instance.GetItemCount("BloodoftheMage");
        }
        if (SceneSwitcher.Instance.HasItem("WindTurbine"))
        {
            var windTurbine = gameObject.AddComponent<WindTurbine>();
            windTurbine.stacks = SceneSwitcher.Instance.GetItemCount("WindTurbine");
        }
        if (SceneSwitcher.Instance.HasItem("TungstonSphere"))
        {
            var tungstonSphere = gameObject.AddComponent<TungstonSphere>();
            tungstonSphere.stacks = SceneSwitcher.Instance.GetItemCount("TungstonSphere");
        }
        if (SceneSwitcher.Instance.HasItem("ShatteredStopwatch"))
        {
            var shatteredStopwatch = gameObject.AddComponent<ShatteredStopwatch>();
            shatteredStopwatch.stacks = SceneSwitcher.Instance.GetItemCount("ShatteredStopwatch");
        }
        if (SceneSwitcher.Instance.HasItem("StandStrong"))
        {
            var standStrong = gameObject.AddComponent<StandStrong>();
            standStrong.stacks = SceneSwitcher.Instance.GetItemCount("StandStrong");
        }
        if (SceneSwitcher.Instance.HasItem("BloodofthePhalanx"))
        {
            var bloodofthePhalanx = gameObject.AddComponent<BloodofthePhalanx>();
            bloodofthePhalanx.stacks = SceneSwitcher.Instance.GetItemCount("BloodofthePhalanx");
        }
        if (SceneSwitcher.Instance.HasItem("BloodoftheReaper"))
        {
            var bloodoftheReaper = gameObject.AddComponent<BloodoftheReaper>();
            bloodoftheReaper.stacks = SceneSwitcher.Instance.GetItemCount("BloodoftheReaper");
        }
        if (SceneSwitcher.Instance.HasItem("TrainingDummy"))
        {
            var trainingDummy = gameObject.AddComponent<TrainingDummy>();
            trainingDummy.stacks = SceneSwitcher.Instance.GetItemCount("TrainingDummy");
        }
        if (SceneSwitcher.Instance.HasItem("PaintedParry"))
        {
            var paintedParry = gameObject.AddComponent<PaintedParry>();
            paintedParry.stacks = SceneSwitcher.Instance.GetItemCount("PaintedParry");
        }
        if (SceneSwitcher.Instance.HasItem("BouncyBall"))
        {
            var bouncyBall = gameObject.AddComponent<BouncyBall>();
            bouncyBall.stacks = SceneSwitcher.Instance.GetItemCount("BouncyBall");
        }
        if (SceneSwitcher.Instance.HasItem("GatitoBlade"))
        {
            var gatitoBlade = gameObject.AddComponent<GatitoBlade>();
            gatitoBlade.stacks = SceneSwitcher.Instance.GetItemCount("GatitoBlade");
        }


    }
}
