﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : ScriptableObject{

    // 0 Heart; 1 Spade; 2 Club; 3 Diamond; 
    public static int MajorAttune = -1;
    public static int MinorAttune = -1;

    public static int Attack;
    public static float Speed;
    public static float ValkMult;
    public static int ValkCount = 0;

    public static string nextBoss = "";
    public static int currentDay = 0;

    public static int DeltaAttack;
    public static float deltaSpeed;


    public static void SetUp()
    {
        Attack = 3;
        Speed = 5f;
        ValkMult = 1f;

        if (MajorAttune == 0 || MinorAttune == 0)
        {
            if (MajorAttune == 0)
            {
                Speed -= 1f;
            }
        }
        if (MajorAttune == 1 || MinorAttune == 1)
        {
            if (MajorAttune == 1)
            {
                ValkMult -= 0.5f;
            }
            Attack += 1;
        }
        if (MajorAttune == 2 || MinorAttune == 2)
        {
            if (MajorAttune == 2)
            {
                Attack -= 1;
            }
            Speed += 2f;

        }
        if (MajorAttune == 3 || MinorAttune == 3)
        {
            if (MajorAttune == 3)
            {
                Attack -= 1;
            }
            ValkMult += 0.5f;

        }
    }

    public static void DeTune()
    {
        MajorAttune = -1;
        MinorAttune = -1;
        Attack = 5;
        Speed = 5f;
        ValkMult = 1f;
    }

    public static void DetermineSoup()
    {
        nextBoss = "Portobeyo";
    }
    
}
