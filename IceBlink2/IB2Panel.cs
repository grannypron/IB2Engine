﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class IB2Panel
    {
        //this class is handled differently than Android version
        public GameView gv;
        public string name = "";
        public string backgroundImageFilename = "";
        public bool hiding = false;
        public bool showing = false;
        public int shownLocX = 0;
        public int shownLocY = 0;
        public int currentLocX = 0;
        public int currentLocY = 0;
        public int hiddenLocX = 0;
        public int hiddenLocY = 0;
        public int hidingXIncrement = 0;
        public int hidingYIncrement = 0;
        public int Width = 0;
        public int Height = 0;
        public List<IB2Button> buttonList = new List<IB2Button>();
        public List<IB2ToggleButton> toggleList = new List<IB2ToggleButton>();
        public List<IB2Portrait> portraitList = new List<IB2Portrait>();
        public List<IbbHtmlLogBox> logList = new List<IbbHtmlLogBox>();


        public IB2Panel(GameView g)
        {
            gv = g;
            currentLocX = shownLocX;
            currentLocY = shownLocY;
        }

        public void Draw()
        {
            IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Width, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Height);
            IbRect dst = new IbRect(currentLocX, currentLocY, Width, Height);
            gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst);

            //iterate over all controls and draw
            foreach (IB2Button btn in buttonList)
            {
                btn.Draw(this);
            }
            foreach (IB2ToggleButton btn in toggleList)
            {
                btn.Draw();
            }
            foreach (IB2Portrait btn in portraitList)
            {
                btn.Draw();
            }
            foreach (IbbHtmlLogBox log in logList)
            {
                log.onDrawLogBox();
            }
        }

        public void Update(int elapsed)
        {
            //animate hiding panel
            if (hiding)
            {
                currentLocX += hidingXIncrement * elapsed;
                currentLocY += hidingYIncrement * elapsed;
                //hiding left and passed
                if ((hidingXIncrement < 0) && (currentLocX < hiddenLocX))
                {
                    currentLocX = hiddenLocX;
                    hiding = false;
                }
                //hiding right and passed
                if ((hidingXIncrement > 0) && (currentLocX > hiddenLocX))
                {
                    currentLocX = hiddenLocX;
                    hiding = false;
                }
                //hiding down and passed
                if ((hidingYIncrement > 0) && (currentLocY > hiddenLocY))
                {
                    currentLocY = hiddenLocY;
                    hiding = false;
                }
                //hiding up and passed
                if ((hidingYIncrement < 0) && (currentLocY < hiddenLocY))
                {
                    currentLocY = hiddenLocY;
                    hiding = false;
                }
            }
            else if (showing)
            {
                currentLocX -= hidingXIncrement * elapsed;
                currentLocY -= hidingYIncrement * elapsed;
                //showing right and passed
                if ((hidingXIncrement < 0) && (currentLocX > shownLocX))
                {
                    currentLocX = shownLocX;
                    showing = false;
                }
                //showing left and passed
                if ((hidingXIncrement > 0) && (currentLocX < shownLocX))
                {
                    currentLocX = shownLocX;
                    showing = false;
                }
                //showing up and passed
                if ((hidingYIncrement > 0) && (currentLocY < shownLocY))
                {
                    currentLocY = shownLocY;
                    showing = false;
                }
                //showing down and passed
                if ((hidingYIncrement < 0) && (currentLocY > shownLocY))
                {
                    currentLocY = shownLocY;
                    showing = false;
                }
            }
        }
    }
}
