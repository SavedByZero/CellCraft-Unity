using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MRNA : RNA
{

	public override void InitRNA (int i, int count= 1, string pc_id = "")
	{
		base.InitRNA(i, count, pc_id);
		//super(i, count);
	}

}

