#region File Description
//-----------------------------------------------------------------------------
// InputComponent.cs
//
// 
// Copyright (C) Jeff Sipko. All rights reserved.
// Licensed under Microsoft Permissive License
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace Duet.Input_System
{
    /// <summary>
    /// This is the argument to GamepadEventHandler
    /// </summary>
    public class GamepadEventArgs : System.EventArgs
    {
        public GamePadState state;

        public GamepadEventArgs(GamePadState state)
        {
            this.state = state;
        }
    }

    /// <summary>
    /// GamepadEventHandler delegate
    /// </summary>
    public delegate void GamepadEventHandler(object sender, GamepadEventArgs e);

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class InputComponent : Microsoft.Xna.Framework.GameComponent
    {
        #region private data
        private Vector2 cursorPos;
        private Vector2 cursorVelocity;
        private float cursorMaxSpeed;
        private Texture2D cursorTex;
        private GamePadState state;
        private GamePadCapabilities caps;
        #endregion

        #region properties
        public Vector2 CursorPos
        {
            get
            {
                return cursorPos;
            }
        }

        public float CursorMaxSpeed
        {
            get
            {
                return cursorMaxSpeed;
            }
            set
            {
                cursorMaxSpeed = value;
            }
        }

        public GamePadState State
        {
            get
            {
                return state;
            }
        }
        #endregion

        
        #region events
        public event GamepadEventHandler LStickMoved;
        #endregion

        public InputComponent(Game game)
            : base(game)
        {
        }

        public InputComponent(Game game, Texture2D tex, float maxSpeed)
            : base(game)
        {
            cursorTex = tex;
            cursorMaxSpeed = maxSpeed;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // Initialize the Cursor
            if (null != cursorTex)
            {
                cursorPos = new Vector2(
                    this.Game.GraphicsDevice.Viewport.Width / 2 - cursorTex.Width / 2,
                    this.Game.GraphicsDevice.Viewport.Width / 2 - cursorTex.Width / 2);
            }

            caps = GamePad.GetCapabilities(PlayerIndex.One);

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            state = GamePad.GetState(PlayerIndex.One);

            float xvel = Math.Abs(state.ThumbSticks.Left.X) > 0.1f ? state.ThumbSticks.Left.X : 0;
            float yvel = Math.Abs(state.ThumbSticks.Left.Y) > 0.1f ? state.ThumbSticks.Left.Y : 0;

            if (xvel > 0 || yvel > 0)
                if (null!=LStickMoved) LStickMoved(this, new GamepadEventArgs(state));


            cursorVelocity.X = xvel * cursorMaxSpeed;
            cursorVelocity.Y = yvel * -cursorMaxSpeed;
            cursorPos += cursorVelocity;

            // Keep it inside the window
            if (cursorPos.X < 0)
                cursorPos.X = 0;
            else if (cursorPos.X > Game.GraphicsDevice.Viewport.Width)
                cursorPos.X = Game.GraphicsDevice.Viewport.Width;
            if (cursorPos.Y < 0)
                cursorPos.Y = 0;
            else if (cursorPos.Y > Game.GraphicsDevice.Viewport.Height)
                cursorPos.Y = Game.GraphicsDevice.Viewport.Height;

            base.Update(gameTime);
        }

        /// <summary>
        /// Raises the ButtonClicked event
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        //public override void OnButtonClicked(GameTime gameTime) { }
    }
}