using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrid : MonoBehaviour
{
	private float cell_w; //how big is a cell? (assume square)
	private float cell_h;
	private int grid_w; //how many cells wide
	private int grid_h; //how many cells tall
	private float half_w;
	private float half_h;
	private List<List<List<GameDataObject>>> grid;
	

	private List<GameDataObject> v;
	public GameObject grid_shape;
	private int t = 0;
	private int testTime = 30;
	private int frameCount = 0;
	private List<object> tests;
	private float memNum;
	private int calcCount = 0;
	// Start is called before the first frame update
	void Start()
	{
		setup();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void destruct()
	{
		wipeGrid();
		//removeChild(grid_shape);
	}


	public void clearGrid()
	{
		for (int xx = grid_w - 1; xx >= 0; xx--)
		{
			for (int yy = grid_h - 1; yy >= 0; yy--)
			{
				int length = grid[xx][yy].Count;
				for (int i = length - 1; i >= 0; i--)
				{
					grid[xx][yy][i] = null;
					grid[xx][yy].RemoveAt(i);
				}
			}
		}
		displayGrid();
	}

	public void wipeGrid()
	{
		for (int xx = grid_w - 1; xx >= 0; xx--)
		{
			for (int yy = grid_h - 1; yy >= 0; yy--)
			{
				int length = grid[xx][yy].Count;
				for (int i = length - 1; i >= 0; i--)
				{
					//removeChild(grid[xx][yy][i]);
					grid[xx][yy][i] = null;
					grid[xx][yy].RemoveAt(i);//.splice(i, 1);
				}
				grid[xx][yy] = null;
				grid[xx].RemoveAt(yy);
			}
			grid[xx] = null;
			grid.RemoveAt(xx);
		}
		grid = null;
		//System.gc();  //TODO?


	}

	private void setup()
	{
		//grid_shape = new Shape();
		grid_shape.transform.SetParent(this.transform, false);


		grid_shape.transform.SetSiblingIndex(this.transform.childCount - 1);

	}

	public float getCellW()
	{
		return cell_w;
	}

	public float getCellH()
	{
		return cell_h;
	}


	public float getGridW()
	{
		return grid_w;
	}

	public float getGridH()
	{
		return grid_h;
	}

	public float getSpanW()
	{
		return grid_w * cell_w;
	}

	public float getSpanH()
	{
		return grid_h * cell_h;
	}

	public void makeGrid(int w, int h, float spanX, float spanY)
	{

		grid_w = w;
		grid_h = h;
		cell_w = spanX / grid_w;
		cell_h = spanY / grid_h;
		half_w = cell_w / 2;
		half_h = cell_h / 2;
		makeNewGrid(grid_w, grid_h);
	}

	public void putIn(int xx, int yy, GameDataObject thing)
	{
		//before we can put the object in we have to transform to grid space, whose 0-0 is at upper left

		if (xx >= grid_w) xx = grid_w - 1;
		if (yy >= grid_h) yy = grid_h - 1;
		if (xx <= 0) xx = 0;
		if (yy <= 0) yy = 0;
		grid[xx][yy].Add(thing);

		//DEBUG
		/*
		if (Cell.SHOW_GRID)  //TODO
		{
			displayGrid();
		}*/
	}

	public void takeOut(int xx, int yy, GameDataObject thing = null)
	{
		int i = 0;
		if (xx >= grid_w) xx = grid_w - 1;
		if (yy >= grid_h) yy = grid_h - 1;
		if (xx < 0) xx = 0;
		if (yy < 0) yy = 0;

		bool success = false;
		int length = grid[xx][yy].Count;

		for (int j = length - 1; j >= 0; j--)
		{
			if (grid[xx][yy][j].ptr == thing.ptr || thing == null || thing.ptr == null)
			{
				grid[xx][yy].RemoveAt(j);

				success = true;
				//break;
			}
			i++;
		}
	}

	private void makeNewGrid(int w, int h)
	{
		grid = new List<List<List<GameDataObject>>>();
		for (int ww = 0; ww < w; ww++) 
		{
			grid.Add(new List< List< GameDataObject >>());
			for (int hh = 0; hh < h; hh++) 
			{
				grid[ww].Add(new List<GameDataObject>());
			}
		}
	}

	private void drawGridLines(bool isCanvas)
	{
		//TODO:  yeah, I don't think so, not in Unity. 

		/*                   
		grid_shape.graphics.clear();
		if (isCanvas)
		{
			grid_shape.graphics.lineStyle(1, 0xFF0000);
		}
		else
		{
			grid_shape.graphics.lineStyle(1, 0x000000);
		}
		grid_shape.graphics.moveTo(0, 0);
		for (var w:int = 0; w <= grid_w; w++){
			grid_shape.graphics.moveTo(w * cell_w, 0);
			grid_shape.graphics.lineTo(w * cell_w, cell_h * grid_h);
			for (var h:int = 0; h <= grid_h; h++){
				grid_shape.graphics.moveTo(0, h * cell_h);
				grid_shape.graphics.lineTo(cell_w * grid_w, h * cell_h);
			}
		}*/

	}

	private void drawContents()
	{
		for (int w = 0; w < grid_w; w++)
		{
			for (int h = 0; h < grid_h; h++) 
			{
				int length = grid[w][h].Count;
				float sizeRatio = cell_w / 16;
				if (length > 0)
				{
					//TODO: nope
					//grid_shape.graphics.drawCircle(w * cell_w + half_w, h * cell_h + half_h, length * sizeRatio);
				}
			}
		}
	}

	/*
	 * This looks like it was supposed to test the collision between a list and some neighbors, but not sure it was ever finished, as the 
	 * conditional is left empty.
	 * */
	private int testCollision(List<GameDataObject> list, List<GameDataObject> neighbors)
	{
			int calcs = 0;
			for (int i=0; i < list.Count; i++)
			{
				GameDataObject g = list[i];
				for (int j=0; j < neighbors.Count; j++)
				{
					GameDataObject n = neighbors[j];
					calcs++;
					if (g.ptr != n.ptr)
					{
						float dx = g.x - n.x;
						float dy = g.y - n.y;
						float d2 = (dx * dx) + (dy * dy);
						if (d2 < (g.radius * g.radius + n.radius * n.radius))
						{
							//collision
						}
					}
				}
			}
			return calcs;
	}

	public List<GameDataObject> getNeighbors(int w,int h)
	{
		List < GameDataObject > list = new List< GameDataObject >();
		for (int xx = -1; xx <= 1; xx++)
		{
			for (int yy = -1; yy <= 1; yy++)
			{
				if (w + xx >= 0 && w + xx < grid_w)
				{
					if (h + yy >= 0 && h + yy < grid_h)
					{
						for (int k=0; k < grid[w + xx][h + yy].Count; k++ )
						{
							GameDataObject g = grid[w + xx][h + yy][k];
							list.Add(g);
						}
					}
				}
			}
		}
		return list;
	}

	private void updateGrid()
	{
		displayGrid();
	}

	private float getMemory()
	{
			
		float mem = (float)(SystemInfo.systemMemorySize / 1025/1024); //Do we still need this modifier in unity?
			return(mem );
	}



	public void displayGrid(bool isCanvas = false) 
	{
		drawGridLines(isCanvas);
		drawContents();
	}

	public void setTestTime(int s)
	{
		testTime = s * 30; //how many seconds you want
	}


}
