/* Name: Sarah Huang
 * Date: 2/19/23
 * Program: etg.cs
 * Purpose: A text-adventure game in object-oriented style where the player must find a key to escape the graveyard.
 *			The player will type commands to examine locations and move around the level.
 */

using System;
using System.IO;
using System.Collections.Generic;


struct Coords {
	public int x;
	public int y;
	public Coords(int x, int y) {
		this.x = x;
		this.y = y;
	}
	public override bool Equals(Object obj) { return obj is Coords c && this == c; }
	public override int GetHashCode() { return this.x.GetHashCode() ^ this.y.GetHashCode(); }
	public static bool operator ==(Coords a, Coords b) { return a.x == b.x && a.y == b.y; }
	public static bool operator !=(Coords a, Coords b) { return !(a == b); }
}


/* Contains a grid of locations. */
class Level {
	Location[,] grid;
	int x, y;

	public Level() { 
		this.grid = null; 
		this.x = 0;
		this.y = 0;
	}
	public void createGrid(int x, int y){ 
		this.grid = new Location[x, y]; 
		for(int row = 0; row < x; row++){
			for(int col = 0; col < y; col++){
				this.grid[row, col] = new Location();
			}
		}
		this.x = x;
		this.y = y;
	}
	public Location[,] getGrid() { return this.grid; }
	public int getXBoundary()	 { return this.x; }
	public int getYBoundary()	 { return this.y; }
}



/* Contains zero or more Entity objects and handles each interaction. */
class Location {
	bool isExit;		//Whether the Location object is the exit
	Key key;			//Key Entity found in Location 
	bool keyLocation;	//Whether the Key is in the Location
	bool keyTaken;		//Whether the Player picked up the Key (no longer in Location)
	List<Loot> loot;	//List of Loot Entities since in case multiple chests in Location
	bool lootLocation;	//Whether the Location object has at least one Loot entity
	bool lootTaken;		//Whether the Player picked up the Loot (chests are now empty)
	Skeleton skeleton;	//Skeleton Entity found in Location
	bool skeletonLocation;	//Whether a Skeleton is in the Location

	public Location(){
		this.isExit = false;			
		this.key = null;				
		this.keyLocation = false;
		this.keyTaken = false;
		this.loot = null;
		this.lootLocation = false;
		this.lootTaken = false;
		this.skeleton = null;
		this.skeletonLocation = false;
	}

	//Exit
	public void becomeExit(){ this.isExit = true; }
	public bool isExitLocation(){ return this.isExit; }

	//Key
	public void addKey(){
		this.key = new Key();
		this.keyLocation = true;
	}
	public void removeKey(){ this.keyTaken = true; }
	public bool isKeyLocation(){ return this.keyLocation; }
	public bool isKeyTaken(){ return this.keyTaken; }
	public Key getKey(){ return this.key; }

	//Loot
	public void createLootList(){
		this.loot = new List<Loot>();
		this.lootLocation = true;
	}
	public void addLoot(int count){
		this.loot.Add(new Loot() { numCoins = count, chestOpened = false }); 
	}
	public void emptyChest(){
		this.lootTaken = true;
		foreach (Loot numChests in loot){
			numChests.chestOpened = true;
		}
	}
	public bool isLootLocation(){ return this.lootLocation; }
	public bool isLootTaken(){ return this.lootTaken; }
	public List<Loot> getLoot(){ return this.loot; }

	//Skeleton
	public void addSkeleton(){
		this.skeleton = new Skeleton();
		this.skeletonLocation = true;
	}
	public bool isSkeletonLocation(){ return this.skeletonLocation; }
	public Skeleton getSkeleton(){ return this.skeleton; }
}


/* Special dialog lines for the Player encountering the exit. */
class Exit : Location {
	public void look(){
		Console.WriteLine("That looks like the gate out of this spooky place!");
	}
	public void interact(bool haveKey){
		if(haveKey){ 
			Console.WriteLine("You open the gate with your key!"); 
		}else{ 
			Console.WriteLine("You try to open the gate, but it's locked. Must need a key..."); }
	}
}


/* Base class for game objects. */
abstract class Entity {
	//describe the entity (if it can be seen) when player looks
	public virtual  void look() {}
	//define what happens when player goes to location containing entity
	public abstract void interact(Player player);
}


/* Special dialog lines for the Player encountering a key. */
class Key : Entity {
	public override void look() {
		Console.WriteLine("You see a key on the ground! Might need that to get out of here...");	
	}
	public override void interact(Player player){
		Console.WriteLine("You picked up a key!");
		player.set_key_status(true);
	}
}


/* Special dialog lines for the Player encountering loot. */
class Loot : Entity {
	public int numCoins { get; set; }
	public bool chestOpened { get; set; }

	public override void look() {
		if(this.chestOpened){
			Console.WriteLine("A treasure chest sits already opened.");
		}else{
			Console.WriteLine("You see what looks like the corner of a treasure chest poking out of the ground.");	
		}
	}
	public override void interact(Player player){
		if(this.chestOpened){
			Console.WriteLine("The chest is empty...");
		}else{
			Console.WriteLine($"You open the chest and find {this.numCoins} coins!");
			player.coinInventory += numCoins;
		}
	}
}


/* Special dialog lines for the Player encountering a skeleton. */
class Skeleton : Entity {
   public override void look(){
		Console.WriteLine("Not much to see here.");
   }
   public override void interact(Player player){
		Console.WriteLine("A bony arm juts out of the ground and grabs your ankle!");
		Console.WriteLine("You've been dragged six feet under by a skeleton.");
		player.set_alive_status(false);
   }
}


/* Contains information about Player. */
class Player {
	public Coords coords { get; set; }			
	bool alive_status;							
	bool key_status;
	public int coinInventory { get; set; }
	public Location currentLocation { get; set;}

	public Player() {
		this.coords = new Coords(0, 0);
		this.alive_status = true;
		this.key_status = false;
		this.coinInventory = 0;
	}

	public bool is_at(Coords xy) { return this.coords == xy; }
	public bool is_alive() { return this.alive_status; }
	public void set_alive_status(bool val){ this.alive_status = val; }
	public bool has_key() { return this.key_status; }
	public void set_key_status(bool val){ this.key_status = val; }
	public void print_stats() {
		Console.WriteLine(String.Format("{0,-12}{1}", "  LOCATION:", $"{this.coords.x}, {this.coords.y}"));
		Console.WriteLine(String.Format("{0,-12}{1}", "  COINS:", $"{this.coinInventory}"));
		Console.WriteLine(String.Format("{0,-12}{1}", "  KEY:", $"{this.key_status}"));
		Console.WriteLine(String.Format("{0,-12}{1}", "  DEAD:", $"{!this.alive_status}"));
	}
}


/* The game's main driver class. Contains Player and Level objects. Also handles input. */
class Game {
	int    num_turns;
	Level  level;
	public Player player { get; }
	public Game() { this.player = new Player(); }

	public void load(string path) {
		this.level = new Level();
		string line;
		using (StreamReader reader = new StreamReader(path)) {
			while ((line = reader.ReadLine()) != null) {
				if (line == "") { continue; }

				string[] split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (split.Length < 3) {
					Console.WriteLine($"Bad command in level file: '{line}'");
					Environment.Exit(1);
				}
				int x     = int.Parse(split[1]);
				int y     = int.Parse(split[2]);
				int count = 0;
				if (split.Length > 3) {
					count = int.Parse(split[3]);
				}

				switch (split[0]) {
					//Set the level's size to x by y
					case "size":
						level.createGrid(x,y);
						break;
					//Set the level's exit location to be x, y
					case "exit":
						Location exitSpot = new Location();
						exitSpot.becomeExit();
						level.getGrid()[x, y] = exitSpot;
						break;
					//Add a key to location x, y
					case "key":
						if(level.getGrid()[x, y] == null){
							Location keySpot = new Location();
							keySpot.addKey();
							level.getGrid()[x, y] = keySpot;
						}
						//If there is already a Location in the grid, just add on to it
						else{
							level.getGrid()[x, y].addKey();
						}
						break;
					//Add loot to location x, y with count coins
					case "loot":
						if(level.getGrid()[x, y] == null){
							Location chestSpot = new Location();
							chestSpot.createLootList();
							chestSpot.addLoot(count);
							level.getGrid()[x, y] = chestSpot;
						}else{
							if(level.getGrid()[x, y].getLoot() == null){
								level.getGrid()[x, y].createLootList();
							}
							level.getGrid()[x, y].addLoot(count);
						}
						break;
					//Add a skeleton to location x, y
					case "skeleton":
						if(level.getGrid()[x, y] == null){
							Location skeletonSpot = new Location();
							skeletonSpot.addSkeleton();
							level.getGrid()[x, y] = skeletonSpot;
						}else{
							level.getGrid()[x, y].addSkeleton();
						}
						break;
					default:
						Console.WriteLine($"Bad command in level file: '{line}'");
						Environment.Exit(1);
						break;
				}
			}
		}
	}


	public void input(string line) {
		this.num_turns += 1;

		//End game if player hits exhaustion
		if(this.num_turns > (2 * level.getGrid().Length)){
			Console.WriteLine("You have died from exhaustion.");
			this.player.set_alive_status(false);
			this.exit();
		}

		Console.WriteLine("================================================================");
		string[] split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		if (split.Length != 2) {
			Console.WriteLine($"Bad command in input: '{line}'");
			return;
		}
		//Update player coordinates
		Coords new_coords = this.player.coords;
		switch (split[1]) {
			case "north":
				new_coords.y += 1;
				break;
			case "south":
				new_coords.y -= 1;
				break;
			case "east":
				new_coords.x += 1;
				break;
			case "west":
				new_coords.x -= 1;
				break;
			default:
				Console.WriteLine($"Bad command in input: '{line}'");
				return;
		}

		//Error check new coordinates are within grid
		bool withinX = new_coords.x >= 0 && new_coords.x < this.level.getXBoundary();
		bool withinY = new_coords.y >= 0 && new_coords.y < this.level.getYBoundary();
		if(!withinX || !withinY){
			Console.WriteLine("A towering wall is before you. This must be the edge of the graveyard.");
		}
		else{
			switch (split[0]) {
				case "go":
					this.player.coords = new_coords;
					Location currentSpot = level.getGrid()[this.player.coords.x, this.player.coords.y];
					this.player.currentLocation = currentSpot;
					
					//Player reaches the exit (look + interact)
					if(currentSpot.isExitLocation()){
						Exit exit = new Exit();
						exit.look();
						exit.interact(this.is_over());
						this.exit_if_over();
					}
					//Player reaches the key (only looking)
					if(currentSpot.isKeyLocation() && !currentSpot.isKeyTaken()){
						currentSpot.getKey().look();
					}
					//Player reaches the loot (only looking)
					if(currentSpot.isLootLocation()){
						foreach(Loot numChest in currentSpot.getLoot()){
							numChest.look();
						}	
					}
					//Player INTERACTS with key
					if(currentSpot.isKeyLocation() && !currentSpot.isKeyTaken()){
						currentSpot.getKey().interact(this.player);
						currentSpot.removeKey();
					}
					//Player INTERACTS with loot
					if(currentSpot.isLootLocation()){
						foreach(Loot numChest in currentSpot.getLoot()){
							numChest.interact(this.player);
						}
						if(!currentSpot.isLootTaken()){ currentSpot.emptyChest(); }
					}
					//Player finds nothing (including the skeleton's skeleton)
					if(!currentSpot.isExitLocation() && !currentSpot.isKeyLocation() && !currentSpot.isLootLocation()){
						Console.WriteLine("Not much to see here.");
					}
					//Skeleton interacts with Player
					if(currentSpot.isSkeletonLocation()){
						currentSpot.getSkeleton().interact(this.player);
						this.exit();
					}
					break;

				case "look":
					Location nextSpot = level.getGrid()[new_coords.x, new_coords.y];
					//Player spots exit
					if(nextSpot.isExitLocation()){ 
						Exit exit = new Exit();
						exit.look(); 
					}
					//Player spots key
					if(nextSpot.isKeyLocation() && !nextSpot.isKeyTaken()){
						nextSpot.getKey().look();
					}
					//Player spots loot
					if(nextSpot.isLootLocation()){ 
						foreach(Loot numChest in nextSpot.getLoot()){
							numChest.look();
						}
					}
					//Player spots nothing (including skeleton's location)
					if(!nextSpot.isExitLocation() && !nextSpot.isKeyLocation() && !nextSpot.isLootLocation()){
						Console.WriteLine("Not much to see here.");
					}
					break;

				default:
					Console.WriteLine($"Bad command in input: '{line}'");
					return;
			}
		}
	}

	//Exit conditions to win the game
	bool is_over() {
		if(this.player.has_key() && this.player.currentLocation.isExitLocation()){
			return true;
		}
		return false;
	}

	void print_stats() {
		if (this.is_over() && player.is_alive()) {
			Console.WriteLine("You successfully escaped the graveyard!");
		} else {
			Console.WriteLine("You did not escape the graveyard. GAME OVER");
		}
		Console.WriteLine($"Game ended after {this.num_turns} turn(s).");
		player.print_stats();
	}

	public void exit() {
		Console.WriteLine("================================================================");
		this.print_stats();
		Environment.Exit(0);
	}

	public void exit_if_over() {
		if (this.is_over()) { this.exit(); }
	}

	public void intro() {
		Console.WriteLine("You awake in a daze to find yourself alone in the dead of night, surrounded by headstones...");
		Console.WriteLine("You must escape this graveyard.");
		Console.WriteLine("================================================================");
		// Look at the current location.
		Location spot = this.level.getGrid()[this.player.coords.x, this.player.coords.y];		
		if(!spot.isExitLocation() && !spot.isKeyLocation() && !spot.isLootLocation() && !spot.isSkeletonLocation()){
			Console.WriteLine("Not much to see here.");
		}else{
			//Just in case the level had someting at the start
			if(spot.isExitLocation()){
				Exit exit = new Exit();
				exit.look(); 
			}
			if(spot.isKeyLocation()){ spot.getKey().look(); }
			if(spot.isLootLocation()){ 
				foreach(Loot numChest in spot.getLoot()){
					numChest.look();
				}
			}
			if(spot.isSkeletonLocation()){ spot.getSkeleton().look(); }
		}	
		Console.Write($"{this.player.coords.x}, {this.player.coords.y}> ");
	}
}


/* The program's class. Will contain the Main() method. */
class ETG {
	static void Main(string[] args) {
		if (args.Length != 1) {
			Console.WriteLine("ERROR: expected a single argument (the level file)");
			Environment.Exit(1);
		}

		Game game = new Game();
		game.load(args[0]);
		game.intro();
		game.exit_if_over();

		string line;
		while ((line = Console.ReadLine()) != null) {
			if (line == "") { continue; }
			game.input(line);
			game.exit_if_over();
			Console.Write($"{game.player.coords.x}, {game.player.coords.y}> ");
		}
		game.exit();
	}
}
