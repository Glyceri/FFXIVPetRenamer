using ImGuiNET;
using PetRenamer.Windows.Bonus.FireworkAnim;
using PetRenamer.Windows.Bonus.LoveHeartAnim;
using PetRenamer.Windows.Bonus.RainAnim;
using PetRenamer.Windows.Bonus.RotatingTriangleAnim;
using PetRenamer.Windows.Bonus.SnowAnim;
using System;
using System.Numerics;

namespace PetRenamer.Windows.Bonus;

// Speak about the way I do this and World War III will happen...

[ToolbarAnimation("Event", -1)]
internal class EventAnimation : ToolbarAnimation
{
    ToolbarAnimation activeAnimation = null!;

    internal override void OnInitialize() => SetupAnimation(); 
    internal override void OnClear() => activeAnimation?.OnClear();
    internal override void Update(double deltaTime) => activeAnimation?.Update(deltaTime);
    internal override void Draw(ImDrawListPtr drawListPtr, Vector2 startingPoint, Vector2 endPoint) => activeAnimation?.Draw(drawListPtr, startingPoint, endPoint);
    internal override void OnDispose() => activeAnimation?.OnDispose();

    void SetupAnimation()
    {
        DateTime dateTime = DateTime.Now;
        int month = dateTime.Month;

        // I pass DateTime instead of just day because I am planning on adding things for maybe the first sunday of each month and stuff
        switch (month)
        {
            case 12: December(dateTime); break;
            case 11: November(dateTime); break;
            case 10: October(dateTime); break;
            case 9: September(dateTime); break;
            case 8: August(dateTime); break;
            case 7: July(dateTime); break;
            case 6: June(dateTime); break;
            case 5: May(dateTime); break;
            case 4: April(dateTime); break;
            case 3: March(dateTime); break;
            case 2: February(dateTime); break;
            case 1: January(dateTime); break;
            case 0: January(dateTime); break;
        }

        activeAnimation?.Initialize();
    }

    void January(DateTime dateTime)
    {
        int day = dateTime.Day;

        if (day == 1)   // New Years Day
        {
            activeAnimation = new FireworkAnimation();
            return;
        }
    }

    void February(DateTime dateTime)
    {
        int day = dateTime.Day;

        if (day == 14)  // Valentines day
        {
            activeAnimation = new LoveheartAnimation();
            return;
        }
    }

    void March(DateTime dateTime)
    {
        int day = dateTime.Day;

        if (day == 1)  // First day of Spring
        {
            activeAnimation = new TriangleAnimation();
            return;
        }
    }

    void April(DateTime dateTime) { }
    void May(DateTime dateTime) { }

    void June(DateTime dateTime)
    {
        int day = dateTime.Day;

        if (day == 1) // First day of Summer
        {
            activeAnimation = new TriangleAnimation();
            return;
        }
    }

    void July(DateTime dateTime) { }
    void August(DateTime dateTime) { }

    void September(DateTime dateTime)
    {
        int day = dateTime.Day;

        if (day == 1) // First day of Autumn
        {
            activeAnimation = new RainAnimation();
            return;
        }
    }

    void October(DateTime dateTime) { }
    void November(DateTime dateTime) { }

    void December(DateTime dateTime)
    {
        int day = dateTime.Day;

        if (day == 1) // First day of Winter
        {
            activeAnimation = new SnowAnimation();
            return;
        }

        if (day == 31) // New Years Eve
        {
            activeAnimation = new FireworkAnimation();
            return;
        }
    }
}
