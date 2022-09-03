using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Casino
{
    public class KeyboardInput
    {
        private readonly bool[] capitalKeysReleased = new bool[34];
        private readonly bool[] digitKeysReleased = new bool[10];
        private readonly bool[] numPadKeysReleased = new bool[10];

        public bool isCapsLockOn { get; set; } = false;

        public bool isShiftOn { get; set; } = false;
        public KeyboardInput()
        {
            for (int i = 0; i < capitalKeysReleased.Length; i++)
            {
                capitalKeysReleased[i] = true;
            }

            for (int i = 0; i < digitKeysReleased.Length; i++)
            {
                digitKeysReleased[i] = true;
            }

            for (int i = 0; i < numPadKeysReleased.Length; i++)
            {
                numPadKeysReleased[i] = true;
            }
        }

        public Keys Update()
        {
            Keys returnKey = Keys.None;

            KeyboardState kState = Keyboard.GetState();

            if(kState.CapsLock)
            {
                isCapsLockOn = true;
            }
            else
            {
                isCapsLockOn = false;
            }

            if(kState.IsKeyDown(Keys.RightShift))
            {
                isShiftOn = true;
            }
            else
            {
                isShiftOn = false;
            }
            
            if(kState.IsKeyDown(Keys.Back) && capitalKeysReleased[26])
            {
                capitalKeysReleased[26] = false;
                returnKey = Keys.Back;
            }
            else if(kState.IsKeyDown(Keys.Space) && capitalKeysReleased[27])
            {
                capitalKeysReleased[27] = false;
                returnKey = Keys.Space;
            }
            else if(kState.IsKeyDown(Keys.OemQuestion) && capitalKeysReleased[28])
            {
                capitalKeysReleased[28] = false;
                returnKey = Keys.OemQuestion;
            }
            else if(kState.IsKeyDown(Keys.OemComma) && capitalKeysReleased[29])
            {
                capitalKeysReleased[29] = false;
                returnKey = Keys.OemComma;
            }
            else if(kState.IsKeyDown(Keys.OemQuotes) && capitalKeysReleased[30])
            {
                capitalKeysReleased[30] = false;
                returnKey = Keys.OemQuotes;
            }
            else if(kState.IsKeyDown(Keys.OemMinus) && capitalKeysReleased[31])
            {
                capitalKeysReleased[31] = false;
                returnKey = Keys.OemMinus;
            }
            else if(kState.IsKeyDown(Keys.OemPlus) && capitalKeysReleased[32])
            {
                capitalKeysReleased[32] = false;
                returnKey = Keys.OemPlus;
            }
            else if(kState.IsKeyDown(Keys.Enter)&& capitalKeysReleased[33])
            {
                capitalKeysReleased[33] = false;
                returnKey = Keys.Enter;
            }

            if(kState.IsKeyUp(Keys.Back))
            {
                capitalKeysReleased[26] = true;
            }

            if(kState.IsKeyUp(Keys.Space))
            {
                capitalKeysReleased[27] = true;
            }

            if(kState.IsKeyUp(Keys.OemQuestion))
            {
                capitalKeysReleased[28] = true;
            }

            if (kState.IsKeyUp(Keys.OemComma))
            {
                capitalKeysReleased[29] = true;
            }

            if (kState.IsKeyUp(Keys.OemQuotes))
            {
                capitalKeysReleased[30] = true;
            }

            if(kState.IsKeyUp(Keys.OemMinus))
            {
                capitalKeysReleased[31] = true;
            }

            if(kState.IsKeyUp(Keys.OemPlus))
            {
                capitalKeysReleased[32] = true;
            }

            if(kState.IsKeyUp(Keys.Enter))
            {
                capitalKeysReleased[33] = true;
            }

            for (int i = (int)Keys.A; i <= (int)Keys.Z; i++)
            {
                if(kState.IsKeyUp((Keys)i))
                {
                    capitalKeysReleased[i - (int)Keys.A] = true;
                }
            }

            for (int i = (int)Keys.D0; i <= (int)Keys.D9; i++)
            {
                if(kState.IsKeyUp((Keys)i))
                {
                    digitKeysReleased[i - (int)Keys.D0] = true;
                }
            }

            for (int i = (int)Keys.NumPad0; i <= (int)Keys.NumPad9; i++)
            {
                if (kState.IsKeyUp((Keys)i))
                {
                    numPadKeysReleased[i - (int)Keys.NumPad0] = true;
                }
            }

            return returnKey;
        }
    }
}
