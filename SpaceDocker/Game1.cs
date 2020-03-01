using BEPUphysics;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SpaceDocker
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Model ship;
        float aspectRatio;
        BEPUutilities.Vector3 modelVelocity= BEPUutilities.Vector3.Zero;
        Skybox skybox;
        List<Asteroid> asteroids = new List<Asteroid>();


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        public static Vector3 CameraPosition { get; private set; }
        public static Vector3 CameraDirection { get; private set; }
        public static Vector3 modelPosition { get; private set; }
        public static Matrix ModelRotation { get; private set; }
        public static BEPUphysics.Entities.Prefabs.Capsule physCapsule { get; private set; }
        public static BEPUutilities.Matrix modelRotationBepu { get; private set; }
        public static BEPUutilities.Vector3 cameraPositionBepu { get; private set; }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Services.AddService<Space>(new Space());
            modelPosition = new Vector3(0, 0, -5);

            skybox = new Skybox(this, graphics.GraphicsDevice.Viewport.AspectRatio);
            new Asteroid(this, new Vector3(-2, 1.5f, -5), "A", 2, new Vector3(0.2f, 0, 0), new Vector3(0.3f, 0.5f, 0.5f));
            new Asteroid(this, new Vector3(2, 1.5f, -5), "B", 3, new Vector3(-0.2f, 0, 0), new Vector3(-0.5f, -0.6f, 0.2f));
            CameraPosition = new Vector3(0, 0, 0);
            cameraPositionBepu = new BEPUutilities.Vector3(0, 0, 0);
            CameraDirection = Vector3.Forward;
            ModelRotation = Matrix.Identity;
            modelRotationBepu = new BEPUutilities.Matrix(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ship = Content.Load<Model>("Models\\p1_wedge");
            physCapsule = new BEPUphysics.Entities.Prefabs.Capsule(MathConverter.Convert(modelPosition), 1.2f, 0.9f, 1f);
            this.Services.GetService<Space>().Add(physCapsule);
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            //background = Content.Load<Texture2D>("starsky");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            KeyboardState keyState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyState.IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            Services.GetService<Space>().Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            if (keyState.IsKeyDown(Keys.W))
            {
                modelVelocity += BEPUutilities.Vector3.Normalize(modelRotationBepu.Forward) * 0.001f;
            }

            if (keyState.IsKeyDown(Keys.S))
            {
                modelVelocity += BEPUutilities.Vector3.Normalize(modelRotationBepu.Backward) * 0.001f;
            }

            if (keyState.IsKeyDown(Keys.X))
            {
                modelVelocity = modelVelocity * new BEPUutilities.Vector3(0.99f, 0.99f, 0.99f);
            }


            float angularChange = 0.02f;
            if (keyState.IsKeyDown(Keys.A))
            {
                modelRotationBepu=BEPUutilities.Matrix.Multiply(modelRotationBepu, BEPUutilities.Matrix.CreateFromAxisAngle(modelRotationBepu.Up, angularChange));
                ModelRotation *= Matrix.CreateFromAxisAngle(ModelRotation.Up, angularChange);
            }

            if (keyState.IsKeyDown(Keys.D))
            {
                modelRotationBepu=BEPUutilities.Matrix.Multiply(modelRotationBepu, BEPUutilities.Matrix.CreateFromAxisAngle(modelRotationBepu.Up, -angularChange));
                ModelRotation *= Matrix.CreateFromAxisAngle(ModelRotation.Up, -angularChange);
            }


            if (keyState.IsKeyDown(Keys.Q))
            {
                modelRotationBepu=BEPUutilities.Matrix.Multiply(modelRotationBepu, BEPUutilities.Matrix.CreateFromAxisAngle(modelRotationBepu.Backward, angularChange));
                ModelRotation *= Matrix.CreateFromAxisAngle(ModelRotation.Backward, angularChange);
            }

            if (keyState.IsKeyDown(Keys.E))
            {
                modelRotationBepu=BEPUutilities.Matrix.Multiply(modelRotationBepu, BEPUutilities.Matrix.CreateFromAxisAngle(modelRotationBepu.Backward, -angularChange));
                ModelRotation *= Matrix.CreateFromAxisAngle(ModelRotation.Backward, -angularChange);
            }

            if (keyState.IsKeyDown(Keys.Z))
            {
                modelRotationBepu=BEPUutilities.Matrix.Multiply(modelRotationBepu, BEPUutilities.Matrix.CreateFromAxisAngle(modelRotationBepu.Right, angularChange));
                ModelRotation *= Matrix.CreateFromAxisAngle(ModelRotation.Right, angularChange);
            }

            if (keyState.IsKeyDown(Keys.C))
            {
                modelRotationBepu=BEPUutilities.Matrix.Multiply(modelRotationBepu, BEPUutilities.Matrix.CreateFromAxisAngle(modelRotationBepu.Right, -angularChange));
                ModelRotation *= Matrix.CreateFromAxisAngle(ModelRotation.Right, -angularChange);
            }


            //modelPosition += modelVelocity;
            //CameraPosition += modelVelocity;
            physCapsule.Position += modelVelocity;
            //physCapsule.LinearVelocity += modelVelocity;
            base.Update(gameTime);
            this.Services.GetService<Space>().Update();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[ship.Bones.Count];
            ship.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.

            //CameraPosition = modelPosition + 5*Vector3.Normalize(ModelRotation.Backward);
            /*
            CameraPosition = modelPosition + 5 * ModelRotation.Backward;
            if (Vector3.Normalize(ModelRotation.Backward)== Vector3.Up){
                ModelRotation *= Matrix.CreateFromAxisAngle(ModelRotation.Right, 0.05f);
            }
            CameraPosition = CameraPosition + 1.5f*Vector3.Normalize(ModelRotation.Up);
            */
            cameraPositionBepu = physCapsule.Position + 5 * BEPUutilities.Vector3.Normalize(modelRotationBepu.Backward);
            if (BEPUutilities.Vector3.Normalize(modelRotationBepu.Backward) == BEPUutilities.Vector3.Up)
            {
                modelRotationBepu = BEPUutilities.Matrix.Multiply(modelRotationBepu, BEPUutilities.Matrix.CreateFromAxisAngle(modelRotationBepu.Right, 0.05f));
            }
            cameraPositionBepu = cameraPositionBepu + 1.5f * BEPUutilities.Vector3.Normalize(modelRotationBepu.Up);
            //physCapsule.OrientationMatrix = modelRotationBepu;s

            foreach (ModelMesh mesh in ship.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    /*
                    //effect.EnableDefaultLighting();
                    effect.LightingEnabled = false;
                    effect.World = Matrix.CreateScale(0.001f) * transforms[mesh.ParentBone.Index]  *  ModelRotation
                        * Matrix.CreateTranslation(modelPosition);
                    effect.View = Matrix.CreateLookAt(CameraPosition,
                        modelPosition, ModelRotation.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1.0f, 10000.0f);
                    */
                    effect.LightingEnabled = false;
                    effect.World = Matrix.CreateScale(0.001f) * MathConverter.Convert(BEPUutilities.Matrix.Multiply(modelRotationBepu, physCapsule.WorldTransform));// MathConverter.Convert(physCapsule.WorldTransform);
                    effect.View = Matrix.CreateLookAt(MathConverter.Convert(cameraPositionBepu),
                        MathConverter.Convert(physCapsule.Position), MathConverter.Convert(modelRotationBepu.Up));
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1.0f, 10000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            base.Draw(gameTime);
        }
    }
}
