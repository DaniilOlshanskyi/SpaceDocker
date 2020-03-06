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
        Model ship;
        float aspectRatio;
        BEPUutilities.Vector3 modelVelocity = BEPUutilities.Vector3.Zero;
        Skybox skybox;
        List<Asteroid> asteroids = new List<Asteroid>();
        SpriteBatch spriteBatch;
        SpriteFont dfont;
        float speed = 0;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        public static Vector3 modelPosition { get; private set; }
        public static BEPUphysics.Entities.Prefabs.Capsule physCapsule { get; private set; }
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
            // modelPosition = new Vector3(0, 0, -5);

            // Create skybox, mothership, and some randomly-generated asteroids
            skybox = new Skybox(this, graphics.GraphicsDevice.Viewport.AspectRatio);
            new Mothership(this, new Vector3(0,0,-200), "Mothership", 400f);
            Random rnd = new Random();
            for (int i = 0; i<500; i++)
            {
                new Asteroid(this, new Vector3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 200)), "Asteroid_" + i, 3, new Vector3(rnd.Next(1, 3), rnd.Next(1, 3), rnd.Next(1, 3)),
                   new Vector3((float)rnd.NextDouble() - 0.5f, (float)rnd.NextDouble() -0.5f, (float) rnd.NextDouble() - 0.5f));
            }

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
            dfont = Content.Load<SpriteFont>("File");
            // Load the ship model and make a physics object for it
            ship = Content.Load<Model>("Models\\p1_wedge");
            physCapsule = new BEPUphysics.Entities.Prefabs.Capsule(MathConverter.Convert(modelPosition), 1.2f, 0.9f, 1f);
            physCapsule.AngularDamping = 0f;
            physCapsule.LinearDamping = 0f;
            physCapsule.Tag = "Spaceship";
            // Assign a collision event to the physics object
            physCapsule.CollisionInformation.Events.InitialCollisionDetected += Events_InitialCollisionDetected;
            this.Services.GetService<Space>().Add(physCapsule);
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
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

            Services.GetService<Space>().Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            // Set W and S as forward and backward controls with applying linear impulses on the ship physics object 
            BEPUutilities.Vector3 modelForwardMove = 0.1f * physCapsule.OrientationMatrix.Forward;
            BEPUutilities.Vector3 modelBackwardMove = 0.1f * physCapsule.OrientationMatrix.Backward;
            if (keyState.IsKeyDown(Keys.W))
            {
                physCapsule.ApplyLinearImpulse(ref modelForwardMove);
            }

            if (keyState.IsKeyDown(Keys.S))
            {
                physCapsule.ApplyLinearImpulse(ref modelBackwardMove);
            }
            
            // Set roll, pitch, yaw controls with applying angular impulses on the ship physics object 
            float angularChange = 0.02f;
            BEPUutilities.Vector3 modelUp = angularChange * physCapsule.OrientationMatrix.Up;
            BEPUutilities.Vector3 modelUpNeg = -angularChange * physCapsule.OrientationMatrix.Up;
            BEPUutilities.Vector3 modelBackward = angularChange * physCapsule.OrientationMatrix.Backward;
            BEPUutilities.Vector3 modelBackwardNeg = -angularChange * physCapsule.OrientationMatrix.Backward;
            BEPUutilities.Vector3 modelRight = angularChange * physCapsule.OrientationMatrix.Right;
            BEPUutilities.Vector3 modelRightNeg = -angularChange * physCapsule.OrientationMatrix.Right;
            if (keyState.IsKeyDown(Keys.A))
            {
                physCapsule.ApplyAngularImpulse(ref modelUp);
            }

            if (keyState.IsKeyDown(Keys.D))
            {
                physCapsule.ApplyAngularImpulse(ref modelUpNeg);
            }


            if (keyState.IsKeyDown(Keys.Q))
            {
                physCapsule.ApplyAngularImpulse(ref modelBackward);
            }

            if (keyState.IsKeyDown(Keys.E))
            {
                physCapsule.ApplyAngularImpulse(ref modelBackwardNeg);
            }

            if (keyState.IsKeyDown(Keys.Z))
            {
                physCapsule.ApplyAngularImpulse(ref modelRight);
            }

            if (keyState.IsKeyDown(Keys.C))
            {
                physCapsule.ApplyAngularImpulse(ref modelRightNeg);
            }

            // Rese the physics ship object to become dynamic again (since it may become static if not moving for some time)
            physCapsule.BecomeDynamic(1f);
            speed = physCapsule.LinearVelocity.Length();
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
            //GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[ship.Bones.Count];
            ship.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            cameraPositionBepu = physCapsule.Position + 5 * BEPUutilities.Vector3.Normalize(physCapsule.OrientationMatrix.Backward);
            if (BEPUutilities.Vector3.Normalize(physCapsule.OrientationMatrix.Backward) == BEPUutilities.Vector3.Up)
            {
                var modelRight = -0.01f * physCapsule.OrientationMatrix.Right;
                physCapsule.ApplyAngularImpulse(ref modelRight);
            } 
            cameraPositionBepu = cameraPositionBepu + 1.5f * BEPUutilities.Vector3.Normalize(physCapsule.OrientationMatrix.Up);

            foreach (ModelMesh mesh in ship.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Alpha = 1f;
                    effect.LightingEnabled = false;
                    effect.World = Matrix.CreateScale(0.001f) * MathConverter.Convert(physCapsule.WorldTransform);//MathConverter.Convert(BEPUutilities.Matrix.Multiply(modelRotationBepu, physCapsule.WorldTransform));// MathConverter.Convert(physCapsule.WorldTransform);
                    effect.View = Matrix.CreateLookAt(MathConverter.Convert(cameraPositionBepu),
                        MathConverter.Convert(physCapsule.Position), MathConverter.Convert(physCapsule.OrientationMatrix.Up));
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1.0f, 10000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            base.Draw(gameTime);
            // Draw info about current speed
            spriteBatch.Begin();
            spriteBatch.DrawString(dfont, "Current speed:" + speed+ " m/s", Vector2.Zero, Color.White);
            spriteBatch.DrawString(dfont, "Land at <6 m/s! ", new Vector2(0,15), Color.White);
            spriteBatch.End();
        }
        /// <summary>
        /// Event to detect and process collision of the spaceship with something.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="other"></param>
        /// <param name="pair"></param>
        private void Events_InitialCollisionDetected(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair)
        {
            //System.Console.WriteLine(pair.EntityA.Tag);
            //System.Console.WriteLine(pair.EntityB.Tag);
            if (speed > 6)
            {
                System.Console.WriteLine("BIG BUMP " + speed);
            } else
            {
                System.Console.WriteLine("SMOOOOOOOOOL BUMP " + speed);
            }
        }
    }
}
