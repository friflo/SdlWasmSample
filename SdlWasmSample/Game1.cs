using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// ReSharper disable RedundantTypeArgumentsOfMethod
namespace MonoGame;

public class Game1 : Game
{
	private readonly GraphicsDeviceManager graphics;
		
	// [Gamefromscratch - MonoGame Tutorial Part Five: 3D Programming - YouTube] https://www.youtube.com/watch?v=OWrBLS7HO0A&list=PLS9MbmO_ssyB_F9AhtJulWkHBCg4Q4tTE&index=8
	internal			Matrix		worldMatrix;		// project entity into world
	private				Camera		camera;			
		
	private readonly	Triangle	triangle;
	private				Model		cube;
	private readonly	Drones		drones;
	
    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Console.WriteLine($"Game constructor");
        triangle = new Triangle();
        drones	= new Drones();
    }
    
    protected override void Initialize()
    {
	    // Add your initialization logic here
	    graphics.IsFullScreen = false;
	    graphics.PreferredBackBufferWidth  = 640;
	    graphics.PreferredBackBufferHeight = 480;
	    graphics.ApplyChanges();

	    base.Initialize();
			
	    camera.Initialize(GraphicsDevice);
	    camera.orbit = true;
	    worldMatrix = Matrix.CreateWorld(camera.target, Vector3.Forward, Vector3.Up);
			
	    triangle.Initialize(this);
	    drones.Initialize();
	    drones.SetGrid();
	    // drones.SetCube();
    }
    
    protected override void LoadContent()
    {
	    // cube = Content.Load<Model>("Cube");
	    // use this.Content to load your game content here
    }
    
    protected override void Update(GameTime gameTime)
    {
	    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
		    Exit();

	    // Add your update logic here
	    drones.UpdateTransforms(gameTime.TotalGameTime.TotalMilliseconds, worldMatrix);
	    camera.UpdateCamera();
	    base.Update(gameTime);
    }
    
    protected override void Draw(GameTime gameTime)
    {
	    GraphicsDevice.Clear(Color.CornflowerBlue);
			
	    // Add your drawing code here
	    triangle.Draw(this, camera);

	    // DrawModel(cube, worldMatrix, camera);
			
	    var world = worldMatrix + Matrix.CreateTranslation(new Vector3(5,0,0));
	    // DrawModel(cube, world, camera);
	    // drones.Draw(cube, camera);
			
	    base.Draw(gameTime);
    }
    
    private static void DrawModel(Model model, in Matrix world, in Camera camera)
    {
	    foreach (ModelMesh mesh in model.Meshes) {
		    foreach (BasicEffect effect in mesh.Effects) {
			    effect.EnableDefaultLighting();
			    effect.View			= camera.viewMatrix;
			    effect.World		= world;
			    effect.Projection	= camera.projectionMatrix;
			    mesh.Draw();
		    }
	    }
    }
}