using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CellGameObjective 
{
	public string Data_name;
	public float Data_delay;
    public string id; // Internal ID used primarily for activation of other objectives.  "start" is reserved for the opening objective.
	public bool active;
	// Track whether or not this objective is considered to be currently active.
		public bool hidden; // Is thits to be shown to the user as the most recent objective?
		public bool trigger; //Does this fire automatically?
		public string objectiveString; // Description shown to player.  "Collect 400 ATP."
		public int objectiveLink; //id for cross-referencing
		public string objectiveType; // Originally a const ref, but now a String.  // Constant from ObjectiveType class.
		public string targetType; // Originally a const ref, but now a String. // Constant from ObjectiveTargetType class.
		public int targetNum; // Number of items associated with the objective, like 400 (for ATP) or 3 (for mitos).
		public string targetCondition; // Originally a const ref, but now a String. // Constant from ObjectiveCondition.
		public int delay; //Number of frames to delay finishing the objective after the initial success
		public int delayProgress; //how long we've waited
		// Status of the objective
		public int objectiveProgress; // Used to track progress toward objective.  This is handled differently for different objectives.  For HAVE_RESOURCE we use a max, while HAVE_THING we track the number based on deltas.  Starting value must be specified in XML.
		public bool objectiveComplete; // Will be set to true when it is completed
		public bool objectivePending; //is this complete, but just delayed?
		public int soundLevel; //which sound to play? 0=nothing, 1=low, 2=high
		
		// Things to do after completing the objective
		
		public string tutorial_id; //A string for the tutorial heading
		public List<ObjectiveActionParam> tutorials; // A sorted vector of tutorials to be shown, if any.  Broken out from other actions so we can pull these individually as needed.
		public List<ObjectiveAction> actions; // A vector of actions to take after completing the objective.  Can include discoveries and new objectives to activate.
		public List<ObjectiveAction> pre_actions; //A vector of actions to take after completing the objective, but BEFORE the tutorial is shown.


    public CellGameObjective()
    {
        objectiveComplete = false;
        objectivePending = false;
        tutorials = new List< ObjectiveActionParam > ();
        actions = new List< ObjectiveAction > ();
        pre_actions = new List< ObjectiveAction > ();
    }

    public override string ToString() 
	{
			string str = "Objective(id=" + id + " active=" + active + " hidden=" + hidden + "){\n";
			str += "     string="+objectiveString+" type="+objectiveType+" tType=" + targetType + " tNum=" + targetNum + " tCon=" + targetCondition + " progress=" + objectiveProgress + " complete=" + objectiveComplete +  " delay="+delay+"\n}";
			
			return str;
	}
}
