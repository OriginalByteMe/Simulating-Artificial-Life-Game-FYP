//using system.collections;
//using system.collections.generic;
//using unityengine;
//using system.io;
//using system.xml.serialization;
//using unityengine.tilemaps;

//public class game : monobehaviour
//{
//    private static int screen_width = screen.width;
//    private static int screen_height = screen.height;

//    //private static int screen_width = 64; //-1024 pixels / 16 units (16x16 image)
//    //private static int screen_height = 48; //-768 pixels / 16 units

//    public hud hud;

//    public int[] kill = new int[] { 2, 3 };
//    public int create = 3;
//    public float speed = 0.1f;
//    public bool probability_enabled = false;
//    public float probabilitycreate = 0.1f;
//    public float probabilitykill = 0.1f;
//    private float timer = 0;

//    public bool start = false;

//    public sprite deathcell, newcell, staticcell;
//    private spriterenderer spriterenderer;

//    public cell mygameobject;
//    cell[,] grid = new cell[screen_width, screen_height];

//    // start is called before the first frame update
//    void start()
//    {
//        eventmanager.startlistening("savepattern", savepattern);
//        eventmanager.startlistening("loadpattern", loadpattern);
//        //spriterenderer = getcomponent<spriterenderer>();
//        placecells();
//    }

//    private void onmouseover()
//    {
//        debug.log("mouse is over gameobject.");
//    }
//    // update is called once per frame
//    void update()
//    {
//        if (start)
//        {
//            if (timer >= speed)
//            {
//                timer = 0f;
//                countneighbors();
//                if (probability_enabled)
//                {
//                    probabilisticpopulationcontrol();
//                }
//                populationcontrol();
//            }
//            else
//            {
//                timer += time.deltatime;
//            }
//        }


//        userinput();

//    }

//    private void loadpattern()
//    {
//        string path = "patterns";

//        if (!directory.exists(path))
//        {
//            debug.log("no directory found");
//            return;
//        }

//        xmlserializer serializer = new xmlserializer(typeof(pattern));
//        string patternname = hud.loaddialog.patternname.options[hud.loaddialog.patternname.value].text;
//        path += "/" + patternname + ".xml";

//        streamreader reader = new streamreader(path);
//        pattern pattern = (pattern)serializer.deserialize(reader.basestream);
//        reader.close();

//        bool isalive;
//        int x = 0, y = 0;

//        foreach (char c in pattern.patternstring)
//        {
//            if (c.tostring() == "1")
//            {
//                isalive = true;
//            }
//            else
//            {
//                isalive = false;
//            }

//            grid[x, y].setalive(isalive);

//            x++;
//            if (x == screen_width)
//            {
//                x = 0;
//                y++;
//            }
//        }

//    }

//    private void savepattern()
//    {
//        // path of save files
//        string path = "patterns";
//        // if directory is not present create directory
//        if (!directory.exists(path))
//        {
//            directory.createdirectory(path);
//        }

//        // create object of pattern class
//        pattern pattern = new pattern();

//        string patternstring = null;

//        // appending grid to string variable to be exported later
//        for (int y = 0; y < screen_height; y++)
//        {
//            for (int x = 0; x < screen_width; x++)
//            {
//                if (grid[x, y].isalive == false)
//                {
//                    patternstring += "0";
//                }
//                else
//                {
//                    patternstring += "1";
//                }

//            }
//        }

//        pattern.patternstring = patternstring;

//        // now serialize the string into a xml file in the given path directory
//        xmlserializer serializer = new xmlserializer(typeof(pattern));

//        streamwriter writer = new streamwriter(path + "/" + hud.savedialog.patternname.text + ".xml");
//        serializer.serialize(writer.basestream, pattern);
//        writer.close();


//        debug.log(pattern.patternstring);
//    }

//    void userinput()
//    {
//        if (!hud.isactive)
//        {
//            if (input.getmousebuttondown(0))
//            {
//                vector2 mousepoint = camera.main.screentoworldpoint(input.mouseposition);

//                int x = mathf.roundtoint(mousepoint.x);
//                int y = mathf.roundtoint(mousepoint.y);

//                // check if mouse is in bounds
//                if (x >= 0 && y >= 0 && x < screen_width && y < screen_height)
//                {
//                    grid[x, y].setalive(!grid[x, y].isalive);
//                }
//            }

//            if (input.getkeyup(keycode.space))
//            {
//                if (start == false)
//                {
//                    start = true;
//                }
//                else
//                {
//                    start = false;
//                }
//            }

//            if (input.getkeyup(keycode.r))
//            {
//                randomalivecell();
//            }

//            if (input.getkeyup(keycode.s))
//            {
//                // save current pattern
//                //savepattern();
//                hud.showsavedialog();
//            }

//            if (input.getkeyup(keycode.l))
//            {
//                // load a saved pattern
//                hud.showloaddialog();
//            }
//        }




//    }

//    void placecells()
//    {
//        for (int y = 0; y < screen_height; y++)
//        {
//            for (int x = 0; x < screen_width; x++)
//            {

//                cell cell = instantiate(resources.load("prefabs/cell", typeof(cell)), new vector2(x, y), quaternion.identity) as cell;
//                cell.setalive(false);
//                grid[x, y] = cell;

//                //grid[x, y].setalive(randomalivecell());
//            }
//        }
//    }



//    void countneighbors()
//    {
//        for (int y = 0; y < screen_height; y++)
//        {
//            for (int x = 0; x < screen_width; x++)
//            {
//                int numneighbors = 0;

//                // first check for out of bounds condition, if not out of bounds then execute 

//                // north
//                if (y + 1 < screen_height)
//                {
//                    if (grid[x, y + 1].isalive)
//                    {
//                        numneighbors++;
//                    }
//                }

//                // south
//                if (y - 1 >= 0)
//                {
//                    if (grid[x, y - 1].isalive)
//                    {
//                        numneighbors++;
//                    }
//                }

//                // east
//                if (x + 1 < screen_width)
//                {
//                    if (grid[x + 1, y].isalive)
//                    {
//                        numneighbors++;
//                    }
//                }

//                // west
//                if (x - 1 >= 0)
//                {
//                    if (grid[x - 1, y].isalive)
//                    {
//                        numneighbors++;
//                    }
//                }

//                // north east
//                if (x + 1 < screen_width && y + 1 < screen_height)
//                {
//                    if (grid[x + 1, y + 1].isalive)
//                    {
//                        numneighbors++;
//                    }
//                }

//                // north west
//                if (x - 1 >= 0 && y + 1 < screen_height)
//                {
//                    if (grid[x - 1, y + 1].isalive)
//                    {
//                        numneighbors++;
//                    }
//                }

//                // south east
//                if (y - 1 >= 0 && x + 1 < screen_width)
//                {
//                    if (grid[x + 1, y - 1].isalive)
//                    {
//                        numneighbors++;
//                    }
//                }

//                // south west
//                if (y - 1 >= 0 && x - 1 >= 0)
//                {
//                    if (grid[x - 1, y - 1].isalive)
//                    {
//                        numneighbors++;
//                    }
//                }

//                grid[x, y].numneighbors = numneighbors;
//            }
//        }
//    }


//    void populationcontrol()
//    {
//        for (int y = 0; y < screen_height; y++)
//        {
//            for (int x = 0; x < screen_width; x++)
//            {
//                // - rules
//                // - any live cell with 2 or 3 live neighbours survives
//                // - any dead cells with 3 live neighbors become a live cell
//                // - all other live cells die in the next generation and all other dead cells stay dead

//                // alive cell
//                if (grid[x, y].isalive)
//                {

//                    if (grid[x, y].numneighbors == kill[0] || grid[x, y].numneighbors == kill[1])
//                    {
//                        grid[x, y].gameobject.getcomponent<spriterenderer>().sprite = staticcell;
//                    }

//                    if (grid[x, y].numneighbors != kill[0] && grid[x, y].numneighbors != kill[1])
//                    {
//                        grid[x, y].gameobject.getcomponent<spriterenderer>().sprite = deathcell;
//                        grid[x, y].setalive(false);
//                    }
//                }
//                // dead cell
//                else
//                {

//                    if (grid[x, y].numneighbors == create)
//                    {
//                        grid[x, y].gameobject.getcomponent<spriterenderer>().sprite = newcell;
//                        grid[x, y].setalive(true);

//                    }
//                }
//            }
//        }
//    }
//    void probabilisticpopulationcontrol()
//    {
//        int rand = unityengine.random.range(0, 100);
//        for (int y = 0; y < screen_height; y++)
//        {
//            for (int x = 0; x < screen_width; x++)
//            {
//                // - rules
//                // - any live cell with 2 or 3 live neighbours survives
//                // - any dead cells with 3 live neighbors become a live cell
//                // - all other live cells die in the next generation and all other dead cells stay dead
//                if (grid[x, y].isalive)
//                {
//                    // alive cell
//                    if (grid[x, y].numneighbors != kill[0] && grid[x, y].numneighbors != kill[1])
//                    {
//                        if (rand <= probabilitykill * 10)
//                        {
//                            grid[x, y].setalive(false);
//                        }

//                        //gameobject.getcomponent<spriterenderer>().sprite = resources.load("prefabs/cell", typeof(cell))
//                    }
//                }
//                else
//                {
//                    // dead cell
//                    if (grid[x, y].numneighbors == create)
//                    {
//                        if (rand <= probabilitycreate * 10)
//                        {
//                            grid[x, y].setalive(true);
//                        }
//                    }

//                }
//            }
//        }
//    }
//    // randomly determine which cells are alive or not when creating the board
//    void randomalivecell()
//    {


//        for (int y = 0; y < screen_height; y++)
//        {

//            for (int x = 0; x < screen_width; x++)
//            {
//                int rand = unityengine.random.range(0, 100);
//                if (rand > 85)
//                {
//                    grid[x, y].setalive(true);
//                }
//            }
//        }
//    }
//}
