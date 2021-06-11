using System;
using DiamondEngine;
using System.Collections.Generic;

public class Level2BossRoom : DiamondComponent
{
    
	public GameObject column01, column02, column03, column04, column05, column06;
    public static List<GameObject> columns = new List<GameObject>();
	private void Awake()
    {
        columns.Clear();
        columns.Add(column01);
        columns.Add(column02);
        columns.Add(column03);
        columns.Add(column04);
        columns.Add(column05);
        columns.Add(column06);
    }

}