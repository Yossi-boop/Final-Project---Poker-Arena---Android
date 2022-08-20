using System;
using System.Collections.Generic;
using System.Text;

namespace Casino
{
    public class CharcterSprite
    {
        public PlayerSkin playerSkin;
        public int playerWidth;
        public int playerHeight;

        public CharcterSprite(PlayerSkin i_playerSkin, int i_playerWidth, int i_playerHeight)
        {
            playerSkin = i_playerSkin;
            playerWidth = i_playerWidth;
            playerHeight = i_playerHeight;
        }
    }
}
