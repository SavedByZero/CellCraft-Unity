using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CellAction
{
	NOTHING = -1,
   MOVE = 0,
 RECYCLE = 1,
 CANCEL_RECYCLE = 2,
 POP = 3,
 REPAIR = 4,
 DIVIDE = 5,

 ACTIVE_TRANSPORT = 16,
 PASSIVE_TRANSPORT = 17,
 MITOSIS = 18,
 MAKE_DEFENSIN = 19,
 TAKE_DEFENSIN = 20,


 EAT = 22,
 TARGET = 23,


 MAKE_TOXIN = 24,
 TAKE_TOXIN = 25,

 MAKE_BASALBODY = 32,
 MAKE_MEMBRANE = 33,
 TAKE_MEMBRANE = 34,
 MAKE_VESICLE = 35,
 UPGRADE_ER = 36,
 ATTACH_ER = 37,
 DETACH_ER = 38,
 MAKE_GOLGI = 39,

 BLEB = 40,
 PSEUDOPOD = 41,
 PSEUDOPOD_PREPARE = 42,

 DISABLE = 50,
 ENABLE = 51,
 TOGGLE = 52,
 MIX_DISABLE = 53,
 TOGGLE_FA = 54,  //Michael: was also 53 before, be careful
 BURN_G = 55,
 BURN_FA = 56,
 TOGGLE_BROWN_FAT = 57,
 ENABLE_BROWN_FAT = 58,
 DISABLE_BROWN_FAT = 59,
 MIX_BROWN_FAT = 60,

 APOPTOSIS = 61,
 NECROSIS = 62,

 BUY_LYSOSOME = 70,
 BUY_PEROXISOME = 71,
 ENABLE_AUTO_PEROXISOME = 72,
 DISABLE_AUTO_PEROXISOME = 73,
 TOGGLE_AUTO_PEROXISOME = 74,
 BUY_SLICER = 75,
 BUY_RIBOSOME = 76,
 BUY_DNAREPAIR = 77,
}
public class Act 
{
	
		
		public static int LYSOSOME_X = 2;   
		public static int DEFENSIN_X = 1;   
		public static int PEROXISOME_X = 1;
		public static int SLICER_X = 5;
		public static int RIBOSOME_X = 5;
		public static int DNAREPAIR_X = 1;
		
		public Act()
		{

		}

	public static CellAction getI(string s) {
		s = s.ToLower();
			if (s == "move") return CellAction.MOVE;
			else if (s == "recycle") return CellAction.RECYCLE;
			else if (s == "cancel_recycle") return CellAction.CANCEL_RECYCLE;
			else if (s == "pop") return CellAction.POP;
			else if (s == "repair") return CellAction.REPAIR;
			else if (s == "divide") return CellAction.DIVIDE;
			else if (s == "apoptosis") return CellAction.APOPTOSIS;
			else if (s == "necrosis") return CellAction.NECROSIS;
			else if (s == "active_transport") return CellAction.ACTIVE_TRANSPORT;
			else if (s == "passive_transport") return CellAction.PASSIVE_TRANSPORT;
			else if (s == "mitosis") return CellAction.MITOSIS;
			else if (s == "make_defensin") return CellAction.MAKE_DEFENSIN;
			else if (s == "take_defensin") return CellAction.TAKE_DEFENSIN;
			else if (s == "make_golgi") return CellAction.MAKE_GOLGI;
			else if (s == "eat") return CellAction.EAT;
			else if (s == "target") return CellAction.TARGET;
			else if (s == "make_basalbody") return CellAction.MAKE_BASALBODY;
			else if (s == "make_membrane") return CellAction.MAKE_MEMBRANE;
			else if (s == "take_membrane") return CellAction.TAKE_MEMBRANE;
			else if (s == "make_vesicle") return CellAction.MAKE_VESICLE;
			else if (s == "upgrade_er") return CellAction.UPGRADE_ER;
			else if (s == "attach_er") return CellAction.ATTACH_ER;
			else if (s == "detach_er") return CellAction.DETACH_ER;
			else if (s == "bleb") return CellAction.BLEB;
			else if (s == "pseudopod") return CellAction.PSEUDOPOD;
			else if (s == "pseudopod_prepare") return CellAction.PSEUDOPOD_PREPARE;
			else if (s == "enable") return CellAction.ENABLE;
			else if (s == "disable") return CellAction.DISABLE;
			else if (s == "toggle") return CellAction.TOGGLE;
			else if (s == "toggle_fa") return CellAction.TOGGLE_FA;
			else if (s == "burn_fa") return CellAction.BURN_FA;
			else if (s == "burn_g") return CellAction.BURN_G;
			else if (s == "enable_brown_fat") return CellAction.ENABLE_BROWN_FAT;
			else if (s == "disable_brown_fat") return CellAction.DISABLE_BROWN_FAT;
			else if (s == "toggle_brown_fat") return CellAction.TOGGLE_BROWN_FAT;
			else if (s == "enable_auto_peroxisome") return CellAction.ENABLE_AUTO_PEROXISOME;
			else if (s == "disable_auto_peroxisome") return CellAction.DISABLE_AUTO_PEROXISOME;
			else if (s == "toggle_auto_peroxisome") return CellAction.TOGGLE_AUTO_PEROXISOME;
			else if (s == "mix_disable") return CellAction.MIX_DISABLE;
			else if (s == "mix_brown_fat") return CellAction.MIX_BROWN_FAT;
			else if (s == "buy_lysosome") return CellAction.BUY_LYSOSOME;
			else if (s == "buy_peroxisome") return CellAction.BUY_PEROXISOME;
			else if (s == "buy_ribosome") return CellAction.BUY_RIBOSOME;
			else if (s == "buy_slicer") return CellAction.BUY_SLICER;
			else if (s == "buy_dnarepair") return CellAction.BUY_DNAREPAIR;
			else if (s == "make_toxin") return CellAction.MAKE_TOXIN;
			else if (s == "take_toxin") return CellAction.TAKE_TOXIN;
			return CellAction.NOTHING;
		}

public static string getS(CellAction i)
{
		
	switch (i)
	{
		case CellAction.NOTHING: return ""; 
		case CellAction.MOVE: return "move"; 
		case CellAction.RECYCLE: return "recycle"; 
		case CellAction.CANCEL_RECYCLE: return "cancel_recycle"; 
		case CellAction.POP: return "pop"; 
		case CellAction.REPAIR: return "repair"; 
		case CellAction.DIVIDE: return "divide"; 
		case CellAction.APOPTOSIS: return "apoptosis"; 
		case CellAction.NECROSIS: return "necrosis"; 
		case CellAction.ACTIVE_TRANSPORT: return "active_transport"; 
		case CellAction.PASSIVE_TRANSPORT: return "passive_transport"; 
		case CellAction.MITOSIS: return "mitosis"; 
		case CellAction.MAKE_GOLGI: return "make_golgi"; 
		case CellAction.MAKE_DEFENSIN: return "make_defensin"; 
		case CellAction.TAKE_DEFENSIN: return "take_defensin"; 
		case CellAction.EAT: return "eat"; 
		case CellAction.TARGET: return "target"; 
		case CellAction.MAKE_BASALBODY: return "make_basalbody"; 
		case CellAction.MAKE_MEMBRANE: return "make_membrane"; 
		case CellAction.TAKE_MEMBRANE: return "take_membrane"; 
		case CellAction.MAKE_VESICLE: return "make_vesicle"; 
		case CellAction.UPGRADE_ER: return "upgrade_er"; 
		case CellAction.ATTACH_ER: return "attach_er"; 
		case CellAction.DETACH_ER: return "detach_er"; 
		case CellAction.BLEB: return "bleb"; 
		case CellAction.PSEUDOPOD: return "pseudopod"; 
		case CellAction.PSEUDOPOD_PREPARE: return "pseudopod_prepare"; 
		case CellAction.ENABLE: return "enable"; 
		case CellAction.DISABLE: return "disable"; 
		case CellAction.TOGGLE: return "toggle"; 
		case CellAction.TOGGLE_FA: return "toggle_fa"; 
		case CellAction.BURN_FA: return "burn_fa"; 
		case CellAction.BURN_G: return "burn_g"; 
		case CellAction.TOGGLE_BROWN_FAT: return "toggle_brown_fat"; 
		case CellAction.ENABLE_BROWN_FAT: return "enable_brown_fat"; 
		case CellAction.DISABLE_BROWN_FAT: return "disable_brown_fat"; 
		case CellAction.MIX_DISABLE: return "mix_disable"; 
		case CellAction.MIX_BROWN_FAT: return "mix_brown_fat"; 
		case CellAction.BUY_LYSOSOME: return "buy_lysosome"; 
		case CellAction.BUY_PEROXISOME: return "buy_peroxisome"; 
		case CellAction.ENABLE_AUTO_PEROXISOME: return "enable_auto_peroxisome"; 
		case CellAction.TOGGLE_AUTO_PEROXISOME: return "toggle_auto_peroxisome"; 
		case CellAction.DISABLE_AUTO_PEROXISOME: return "disable_auto_peroxisome"; 
		case CellAction.BUY_SLICER: return "buy_slicer"; 
		case CellAction.BUY_RIBOSOME: return "buy_ribosome"; 
		case CellAction.BUY_DNAREPAIR: return "buy_dnarepair"; 
		case CellAction.MAKE_TOXIN: return "make_toxin"; 
		case CellAction.TAKE_TOXIN: return "take_toxin"; 
	}
	return "";
}

public static string getTxt(CellAction i)
{
	string s = "";
	switch (i)
	{
		case CellAction.NOTHING: return ""; 
		case CellAction.MOVE: return ("Move"); 
		case CellAction.POP: return ("Pop"); 
		case CellAction.RECYCLE: return ("Recycle"); 
		case CellAction.CANCEL_RECYCLE: return ("Cancel Recycle"); 
		case CellAction.REPAIR: return ("Repair"); 
		case CellAction.DIVIDE: return ("Divide"); 
		case CellAction.APOPTOSIS: return ("Apoptosis"); 
		case CellAction.NECROSIS: return ("Necrosis"); 
		case CellAction.MITOSIS: return ("Mitosis"); 
		case CellAction.MAKE_GOLGI: return ("Make Golgi"); 
		case CellAction.MAKE_BASALBODY: return ("Make Basal Body"); 
		case CellAction.MAKE_MEMBRANE: return ("Buy Membrane"); 
		case CellAction.TAKE_MEMBRANE: return ("Recycle Membrane"); 
		case CellAction.MAKE_VESICLE: return ("Make Vesicle"); 
		case CellAction.UPGRADE_ER: return ("Upgrade ER"); 
		case CellAction.ATTACH_ER: return ("Attach ER"); 
		case CellAction.DETACH_ER: return ("Detach ER"); 
		case CellAction.BLEB: return ("Bleb"); 
		case CellAction.PSEUDOPOD: return ("Pseudopod"); 
		case CellAction.PSEUDOPOD_PREPARE: return ("Click & Drag"); 
		case CellAction.ENABLE: return ("Enable"); 
		case CellAction.DISABLE: return ("Disable"); 
		case CellAction.TOGGLE: return ("Toggle"); 
		case CellAction.TOGGLE_FA: return ("Toggle FA"); 
		case CellAction.BURN_G: return ("Use Glucose"); 
		case CellAction.BURN_FA: return ("Use Fatty Acids"); 
		//case CellAction.BURN_
		case CellAction.TOGGLE_BROWN_FAT: return ("Toggle Brown Fat"); 
		case CellAction.ENABLE_BROWN_FAT: return ("Enable Brown Fat"); 
		case CellAction.DISABLE_BROWN_FAT: return ("Disable Brown Fat"); 
		case CellAction.TOGGLE_AUTO_PEROXISOME: return ("Toggle Auto-build peroxisomes"); 
		case CellAction.ENABLE_AUTO_PEROXISOME: return ("Auto-build peroxisomes"); 
		case CellAction.DISABLE_AUTO_PEROXISOME: return ("Don't auto-build peroxisomes"); 

		case CellAction.MIX_DISABLE: return ("Disable"); 
		case CellAction.MIX_BROWN_FAT: return ("Disable Brown Fat"); 
		case CellAction.MAKE_TOXIN: return ("Buy Toxin"); 
		case CellAction.TAKE_TOXIN: return ("Recycle Toxin"); 
		case CellAction.TAKE_DEFENSIN: return ("Recycle Defensin");
		case CellAction.MAKE_DEFENSIN: return ("Buy Defensin"); 
		/*if (DEFENSIN_X > 1) s = "Buy " + DEFENSIN_X + s + "s";
		else s = "Buy a " + s;
		return s;
		*/
		case CellAction.BUY_LYSOSOME:
			s = " Lysosome";
			if (LYSOSOME_X > 1) s = "Buy " + LYSOSOME_X + s + "s";
			else s = "Buy a " + s;
			return s;
			
		case CellAction.BUY_PEROXISOME:
			s = " Peroxisome";
			if (PEROXISOME_X > 1) s = "Buy " + PEROXISOME_X + s + "s";
			else s = "Buy a " + s;
			return s;
			
		case CellAction.BUY_SLICER:
			s = " Slicer Enzyme";
			if (SLICER_X > 1) s = "Buy " + SLICER_X + s + "s";
			else s = "Buy a " + s;
			return s;
			
		case CellAction.BUY_RIBOSOME:
			s = " Ribosome";
			if (RIBOSOME_X > 1) s = "Buy " + RIBOSOME_X + s + "s";
			else s = "Buy a " + s;
			return s;
			
		case CellAction.BUY_DNAREPAIR:
			s = " DNA Repair Enzyme";
			if (DNAREPAIR_X > 1) s = "Buy " + DNAREPAIR_X + s + "s";
			else s = "Buy a " + s;
			return s;
			

	}
	return "";
}
		
	}  

