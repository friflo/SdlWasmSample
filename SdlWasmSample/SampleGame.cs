using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// ReSharper disable RedundantTypeArgumentsOfMethod
namespace SdlWasmSample;

public class SampleGame : Game
{
    private GraphicsDeviceManager graphics;
//  private SpriteBatch _spriteBatch;
    
	// [Gamefromscratch - MonoGame Tutorial Part Five: 3D Programming - YouTube] https://www.youtube.com/watch?v=OWrBLS7HO0A&list=PLS9MbmO_ssyB_F9AhtJulWkHBCg4Q4tTE&index=8
	private		Vector3		camTarget;
	private		Vector3		camPosition;
	private		Matrix		projectionMatrix;	// Convert 3D to 2d to render on screen (camera lens)
	private		Matrix		viewMatrix;			// camera location & orientation
	private		Matrix		worldMatrix;		// project entity into world
	private		bool		orbit;
	
	private		BasicEffect				basicEffect;
	// geometric info
	private		VertexPositionColor[]	triangleVertices;	// position + color[]
	private		VertexBuffer			vertexBuffer;
    
    public SampleGame()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Console.WriteLine($"Game constructor");
    }
    
    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        base.Initialize();
			
        camTarget   = new Vector3(0, 0,    0);
        camPosition	= new Vector3(0, 0, -100);
			
        projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45),
            GraphicsDevice.DisplayMode.AspectRatio, 1, 1000); // view frustum - near plane 1, far plane 1000
			
        viewMatrix  = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up); // Vector3.Up: (0, 1, 0)
        worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);
			
        // set basicEffect
        basicEffect = new BasicEffect(GraphicsDevice);
        basicEffect.Alpha = 1; // full opaque
        basicEffect.VertexColorEnabled = true; // see colored vertices
        basicEffect.LightingEnabled = false;
			
        // create triangle
        triangleVertices = new VertexPositionColor[3];
        triangleVertices[0] = new VertexPositionColor(new Vector3(  0,  20, 0), Color.Red);
        triangleVertices[1] = new VertexPositionColor(new Vector3(-20, -20, 0), Color.Green);
        triangleVertices[2] = new VertexPositionColor(new Vector3( 20, -20, 0), Color.Blue);
			
        vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
        vertexBuffer.SetData<VertexPositionColor>(triangleVertices);
    }
    
    protected override void LoadContent()
    {
        Console.WriteLine($"Game LoadContent");
        // _spriteBatch = new SpriteBatch(GraphicsDevice);
 
        // TODO: use this.Content to load your game content here
    }
    
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
 
        // TODO: Add your update logic here
        if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
	        camPosition.X	-= 1;
	        camTarget.X		-= 1;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
	        camPosition.X	+= 1;
	        camTarget.X		+= 1;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
	        camPosition.Y	-= 1;
	        camTarget.Y		-= 1;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Down)) {
	        camPosition.Y	+= 1;
	        camTarget.Y		+= 1;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.OemPlus)) {
	        camPosition.Z	-= 1;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.OemMinus)) {
	        camPosition.Z	+= 1;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Space)) {
	        orbit = !orbit;
        }
        if (orbit) {
	        Matrix rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(1));
	        camPosition = Vector3.Transform(camPosition, rotationMatrix);
        }
        viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);
        base.Update(gameTime);
    }
    
    protected override void Draw(GameTime gameTime)
    {
	    basicEffect.Projection	= projectionMatrix;
	    basicEffect.View		= viewMatrix;
	    basicEffect.World		= worldMatrix;
			
	    GraphicsDevice.Clear(Color.CornflowerBlue);
			
	    // TODO: Add your drawing code here
	    GraphicsDevice.SetVertexBuffer(vertexBuffer);
			
	    // Turn off back face culling
	    var rasterizerState = new RasterizerState();
	    rasterizerState.CullMode = CullMode.None;
	    GraphicsDevice.RasterizerState = rasterizerState;

	    foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes) {
		    pass.Apply();
		    GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 3);
	    }
	    base.Draw(gameTime);
    }
}