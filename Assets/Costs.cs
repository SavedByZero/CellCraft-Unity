using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Costs
{
	//Building things generally costs ATP, AA, and FA.
	//Digesting things ONLY costs ATP, you get back 75% of the other building materials		
	public static float ENTROPY = 0.75f;

	//You ONLY but positive numbers in the make/sell arrays.
	//This is interpreted in the following ways: 
	//making things always DEBITS the amounts given at face value
	//selling things always CREDITS the amounts given at face value, EXCEPT ATP, which is always debited
	//(an if statement in cost calculations flips this)

	public static int AAX = 10;

	public static int AA_PER_NA = 5 * AAX; //1 NA is enough for an RNA to code 5 AA's

	public static float[] MAKE_EVIL_RNA = { 0, 0.25f, 0, 0, 0 };
	public static float[] SELL_EVIL_RNA = { 0, 0.25f, 0, 0, 0 };

	public static float[] MAKE_RNA = { 0, 1, 0, 0, 0 };
	public static float[] SELL_RNA = { 0, 1, 0, 0, 0 };

	public static float[] MAKE_RIBOSOME = { 2, 1, 1 * AAX, 0, 0 }; //ATP,NA,AA,FA,G
	public static float[] SELL_RIBOSOME = { 1, 1, 1 * AAX, 0, 0 }; //ATP,NA,AA,FA,G

	public static float[] MAKE_VESICLE = { 10, 0, 1 * AAX, 5, 0 };
	public static float[] SELL_VESICLE = { 4, 0, 1 * AAX, 5, 0 };

	public static float[] MAKE_LYSOSOME = { 15, 0, 2.5f * AAX, 1, 0 };
	public static float[] SELL_LYSOSOME = { 8, 0, 2.5f * AAX, 1, 0 };

	public static float[] MAKE_PEROXISOME = { 50, 0, 5 * AAX, 2, 0 };
	public static float[] SELL_PEROXISOME = { 25, 0, 5 * AAX, 2, 0 };

	public static float[] MAKE_DNAREPAIR = { 50, 0, 10 * AAX, 0, 0 };
	public static float[] SELL_DNAREPAIR = { 50, 0, 10 * AAX, 0, 0 };

	public static float[] MAKE_SLICER = { 5, 0, 1 * AAX, 0, 0 };
	public static float[] SELL_SLICER = { 3, 0, 1 * AAX, 0, 0 };

	public static float[] SELL_PROTEIN_GLOB = { 50, 0, 100 * AAX, 0, 0 };

	public static float[] MITOCHONDRION_DIVIDE = { 250, 5, 10 * AAX, 25, 10 };
	public static float[] SELL_MITOCHONDRION = { 100, 5, 10 * AAX, 25, 10 };

	public static float[] CHLOROPLAST_DIVIDE = { 200, 5, 10 * AAX, 25, 10 };
	public static float[] SELL_CHLOROPLAST = { 100, 5, 10 * AAX, 25, 10 };

	public static float[] REWARD_INJECTOR = { 0, 0.02f, 0.25f * AAX, 0.25f, 0 };
	public static float[] REWARD_INVADER = { 0, 0.025f, 0.25f * AAX, 0.25f, 0 };
	public static float[] REWARD_INFESTER = { 0, 0.025f, 0.25f * AAX, 0.25f, 0 };

	public static float[] MAKE_VIRUS_INJECTOR = { 5, 0, 0, 0, 0 };
	public static float[] SELL_VIRUS_INJECTOR = { 2, 0, 0, 0, 0 };

	public static float[] MAKE_VIRUS_INVADER = { 10, 0, 0, 0, 0 };
	public static float[] SELL_VIRUS_INVADER = { 0, 0, 0, 0, 0 };

	public static float[] MAKE_VIRUS_INFESTER = { 20, 0, 0, 0, 0 };
	public static float[] SELL_VIRUS_INFESTER = { 0, 0, 0, 0, 0 };

	//public static const ER_MAKE_GOLGI:Array = [1000, 0, 250, 250, 250];
	//public static const ER_SELL_MEMBRANE:Array = [100, 0, 5, 5, 0];// [100, 0, 10, 50, 0];



	public static float[] ER_MAKE_MEMBRANE = { 100, 0, 5 * AAX, 5, 0 };// [100, 0, 10, 50, 0];
	public static float[] SELL_MEMBRANE = { 50, 0, 5 * AAX, 5, 0 };

	public static float[] SELL_DEFENSIN = { 50, 0, 5 * AAX, 1, 0 };
	public static float[] ER_MAKE_DEFENSIN = { 100, 0, 5 * AAX, 1, 0 };

	public static float[] MAKE_TOXIN = { 50, 0, 2.5f * AAX, 1, 0 };
	public static float[] SELL_TOXIN = { 25, 0, 2.5f * AAX, 1, 0 };

	public static float PIXEL_TO_MICRON = 50f; //pixels to micron ratio
	public static float MOVE_DISTANCE = PIXEL_TO_MICRON * 10f; //how many pixels the below costs get you
	public static float MOVE_DISTANCE2 = MOVE_DISTANCE * MOVE_DISTANCE; //to save sqrts

	public static float[] BLEB = { 10, 0, 0, 0, 0 };
	public static float[] PSEUDOPOD = {10, 0, 0, 0, 0};
		
	public static float RIBOSOME_MOVE = 1;
	public static float LYSOSOME_MOVE = 2.5f;
	public static float PEROXISOME_MOVE = 10;
	public static float SLICER_MOVE = 0.1f;
	public static float MITOCHONDRION_MOVE = 50;
	public static float CHLOROPLAST_MOVE = 50;
	public static float FIX_MEMBRANE = 25;
		

		/**
		 * Returns an array with the cost of a given action and selectable object. This function is fine for an interface 
		 * query, but DON'T YOU DARE USE THIS IN A LOOP! This function was designed to be cheap and easy, so long as you 
		 * name the costs consistently, it justs work as you add new costs and actions. If you need speed, you'll have 
		 * to write a hack that looks things up more directly.
		 * @param	s
		 * @param	action
		 * @return the cost [atp, na, aa, fa, g]
		 */

	/*   //TODO
	public static float[] getActionCostByIndex(Selectable s, CellAction action):Array {
			//IF YOU USE THIS IN A LOOP YOU ARE ASKING FOR IT!
			return Costs[s.getTextID().toUpperCase() + "_" + Act.getS(action).toUpperCase()];
		}*/

	public static float getMoveCostByString(string s)
	{
		return moveCostResolve(s);
	}

	private static float[] recycleCostResolve(string s)
    {
		switch (s.ToUpper())
        {
			case "CHLOROPLAST":
				return SELL_CHLOROPLAST;
			case "DEFENSIN":
				return SELL_DEFENSIN;
			case "DNAREPAIR":
				return SELL_DNAREPAIR;
			case "EVIL_RNA":
				return SELL_EVIL_RNA;
			case "LYSOSOME":
				return SELL_LYSOSOME;
			case "MEMBRANE":
				return SELL_MEMBRANE;
			case "MITOCHONDRION":
				return SELL_MITOCHONDRION;
			case "PEROXISOME":
				return SELL_PEROXISOME;
			case "PROTEIN_GLOB":
				return SELL_PROTEIN_GLOB;
			case "RIBOSOME":
				return SELL_RIBOSOME;
			case "RNA":
				return SELL_RNA;
			case "SLICER":
				return SELL_SLICER;
			case "TOXIN":
				return SELL_TOXIN;
			case "VESICLE":
				return SELL_VESICLE;
			case "VIRUS_INFESTER":
				return SELL_VIRUS_INFESTER;
			case "VIRUS_INJECTOR":
				return SELL_VIRUS_INJECTOR;
			case "VIRUS_INVADER":
				return SELL_VIRUS_INVADER;
        }

		return null;
    }

	private static float moveCostResolve(string s)
    {
		switch (s.ToUpper())
		{
			case "RIBOSOME":
				return RIBOSOME_MOVE;
			case "LYSOSOME":
				return LYSOSOME_MOVE;
			case "PEROXISOME":
				return PEROXISOME_MOVE;
			case "SLICER":
				return SLICER_MOVE;
			case "MITOCHONDRION":
				return MITOCHONDRION_MOVE;
			case "CHLOROPLAST":
				return CHLOROPLAST_MOVE;
		

		}

		return -10000; //if you gain this, you know something's wrong!
	}

	public static float[] getRecycleCostByString(string s)
	{
		return recycleCostResolve(s);
	}
      //TODO
	/*public static float[] getRecycleCostByName(Selectable s)
	{
		
		return Costs["SELL_" + s.getTextID().toUpperCase()];
	}*/

/*  //TODO
	public static function getMoveCostByName(s:Selectable):Number
	{
		return moveCostResolve(s.getTextID());
	}*/

	//TODO
	/*
	public static function getActionCostByName(s:Selectable, actionName: String):Array
	{
		return Costs[s.getTextID().toUpperCase() + "_" + actionName.toUpperCase()];
	}*/

	public static int getRNACost(float[] a)
	{
		
		int i = Mathf.CeilToInt(a[2] / AA_PER_NA);
		i *= (int)MAKE_RNA[1];
		return i;
		//if AA = 0, i = 0;
		//if AA = 4, i = 1;
		//if AA = 5, i = 1;
		//if AA = 9, i = 2;
		//if AA = 10, i = 2;
		//if AA = 11, i = 3;
	}

	//Lysosomes, peroxisomes, and slicer enzymes need RNA to spawn, so this adds that into the cost
	//You automatically get it back when the RNA degrades, so it's not in the sell price

	public static float[] getMAKE_TOXIN(int i = 1)
	{
		 float[] a = new float[MAKE_TOXIN.Length];
		Array.Copy(MAKE_TOXIN, a, MAKE_TOXIN.Length);
		a[1] += getRNACost(a);

		for (int j = 0; j < a.Length; j++) {
			a[j] *= i;
		}
		return a;
	}

	public static float[] getMAKE_DEFENSIN(int i = 1)
	{
		  float[] a = new float[ER_MAKE_DEFENSIN.Length];
		Array.Copy(ER_MAKE_DEFENSIN,a,ER_MAKE_DEFENSIN.Length);
		a[1] += getRNACost(a);

		for (int j = 0; j < a.Length; j++) {
			a[j] *= i;
		}
		return a;
	}

	public static float[] getMAKE_MEMBRANE(int i = 1)
	{
		float[] a = new float[ER_MAKE_MEMBRANE.Length];
		Array.Copy(ER_MAKE_MEMBRANE, a, ER_MAKE_MEMBRANE.Length);
		a[1] += getRNACost(a);

		for (int j = 0; j < a.Length; j++) {
			a[j] *= i;
		}
		return a;
	}

	public static float[] getMAKE_LYSOSOME(int i= 1)
	{
		float[] a = new float[MAKE_LYSOSOME.Length];
		Array.Copy(MAKE_LYSOSOME, a, MAKE_LYSOSOME.Length);

		a[1] += getRNACost(a); //add the NA cost of an RNA

		for (int j = 0; j < a.Length; j++) {    //multiply times i, the number of lysosomes ordered
			a[j] *= i;
		}
		return a;
	}

	public static float[] getMAKE_PEROXISOME(int i= 1)
	{
		float[] a = new float[MAKE_PEROXISOME.Length];
		Array.Copy(MAKE_PEROXISOME, a, MAKE_PEROXISOME.Length);
		a[1] += getRNACost(a);
		for (int j = 0; j < a.Length; j++) {
			a[j] *= i;
		}
		return a;
	}

	public static float[] getMAKE_RIBOSOME(int i = 1)
	{
		float[] a = new float[MAKE_RIBOSOME.Length];
		Array.Copy(MAKE_RIBOSOME, a, MAKE_RIBOSOME.Length);
		for (int j = 0; j < a.Length; j++) {
			a[j] *= i;
		}
		return a;
	}

	/*public static float[] getMAKE_VESICLE():Array {
		 float[] a = MAKE_VESICLE.concat();

	}*/

	public static float[] getMAKE_SLICER(int i= 1)
	{
		float[] a = new float[MAKE_SLICER.Length];
		Array.Copy(MAKE_SLICER, a, MAKE_SLICER.Length);
		//a[1] += MAKE_RNA[1];
		a[1] += getRNACost(a);
		for (int j = 0; j < a.Length; j++) {
			a[j] *= i;
		}
		return a;
	}

	public static float[] getMAKE_DNAREPAIR(int i = 1)
	{
		  float[] a = new float[MAKE_DNAREPAIR.Length];
	Array.Copy(MAKE_DNAREPAIR, a, MAKE_DNAREPAIR.Length);
		//a[1] += MAKE_RNA[1];
		a[1] += getRNACost(a);
		for (int j = 0; j < a.Length; j++) {
			a[j] *= i;
		}
		return a;
	}

	public static float getMoveDistInMicrons()
	{
		return MOVE_DISTANCE / PIXEL_TO_MICRON;
	}
}
