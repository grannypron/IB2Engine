﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Color = SharpDX.Color;

namespace IceBlink2
{
    public class ScreenCastSelector 
    {
	   
	    private GameView gv;
        //private gv.module gv.mod = gv.mod;

        public int castingPlayerIndex = 0;
	    private int spellSlotIndex = 0;
	    private int slotsPerPage = 20;

        private int maxPages = 20;
        private int tknPageIndex = 0;

        private List<IbbButton> btnSpellSlots = new List<IbbButton>();

        private IbbButton btnTokensLeft = null;
        private IbbButton btnTokensRight = null;
        private IbbButton btnPageIndex = null;

        private IbbButton btnHelp = null;
	    private IbbButton btnSelect = null;
	    private IbbButton btnExit = null;
	    private string stringMessageCastSelector = "";
        private IbbHtmlTextBox description;
        public bool isInCombat = false;

        List<string> backupKnownSpellTagsInCombat = new List<string>();
        List<string> backupKnownSpellTagsOutsideCombat = new List<string>();

        public ScreenCastSelector(Module m, GameView g) 
	    {
		    //mod = m;
		    gv = g;
		    stringMessageCastSelector = gv.cc.loadTextToString("data/MessageCastSelector.txt");
	    }

        public void sortTraitsForLevelUp(Player pc)
        {
            //clear
            backupKnownSpellTagsInCombat.Clear();
            backupKnownSpellTagsOutsideCombat.Clear();

            List<string> spellsForLearningTags = new List<string>();
            List<SpellAllowed> spellsForLearning = new List<SpellAllowed>();

            if (!isInCombat)
            {
                SpellAllowed tempSA = new SpellAllowed();

                //add the known outside battle useable traits
                foreach (string s in pc.knownSpellsTags)
                {
                    foreach (Spell sp in gv.mod.moduleSpellsList)
                    {
                        if (sp.tag == s)
                        {
                            
                            if (sp.useableInSituation.Equals("Always") || sp.useableInSituation.Equals("OutOfCombat"))
                            {
                                spellsForLearningTags.Add(s);
                            }
                        }
                    }
                }

                //sort the known outside battle useable spells
                int levelCounter = 0;
                while (spellsForLearningTags.Count > 0)
                {
                    for (int i = spellsForLearningTags.Count - 1; i >= 0; i--)
                    {
                        foreach (SpellAllowed sa in pc.playerClass.spellsAllowed)
                        {
                            if (sa.tag == spellsForLearningTags[i])
                            {
                                tempSA = sa.DeepCopy();
                                break;
                            }
                        }
                        if (levelCounter == tempSA.atWhatLevelIsAvailable)
                        {
                            backupKnownSpellTagsOutsideCombat.Add(spellsForLearningTags[i]);
                            spellsForLearningTags.RemoveAt(i);
                        }
                    }
                    levelCounter++;
                }

            }
            //inside combat
            else
            {
                SpellAllowed tempSA = new SpellAllowed();

                //add the known in battle useable traits
                foreach (string s in pc.knownSpellsTags)
                {
                    foreach (Spell sp in gv.mod.moduleSpellsList)
                    {
                        if (sp.tag == s)
                        {

                            if (sp.useableInSituation.Equals("Always") || sp.useableInSituation.Equals("InCombat"))
                            {
                                spellsForLearningTags.Add(s);
                            }
                        }
                    }
                }

                //sort the known inside battle useable spells
                int levelCounter = 0;
                while (spellsForLearningTags.Count > 0)
                {
                    for (int i = spellsForLearningTags.Count - 1; i >= 0; i--)
                    {
                        foreach (SpellAllowed sa in pc.playerClass.spellsAllowed)
                        {
                            if (sa.tag == spellsForLearningTags[i])
                            {
                                tempSA = sa.DeepCopy();
                                break;
                            }
                        }
                        if (levelCounter == tempSA.atWhatLevelIsAvailable)
                        {
                            backupKnownSpellTagsInCombat.Add(spellsForLearningTags[i]);
                            spellsForLearningTags.RemoveAt(i);
                        }
                    }
                    levelCounter++;
                }
            }
        }


        public void setControlsStart()
        {

            List<string> spellsForLearningTags = new List<string>();
            List<SpellAllowed> spellsForLearning = new List<SpellAllowed>();

            int pW = (int)((float)gv.screenWidth / 100.0f);
            int pH = (int)((float)gv.screenHeight / 100.0f);
            int padW = gv.squareSize / 6;

            description = new IbbHtmlTextBox(gv, 320, 100, 500, 300);
            description.showBoxBorder = false;

            //added
            if (btnTokensLeft == null)
            {
                btnTokensLeft = new IbbButton(gv, 1.0f);
                btnTokensLeft.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnTokensLeft.Img2 = gv.cc.LoadBitmap("ctrl_left_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_left_arrow);
                btnTokensLeft.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnTokensLeft.X = (int)(5 * gv.squareSize) + (3 * pW);
                btnTokensLeft.Y = (2 * gv.squareSize);
                btnTokensLeft.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnTokensLeft.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            //added
            if (btnPageIndex == null)
            {
                btnPageIndex = new IbbButton(gv, 1.0f);
                btnPageIndex.Img = gv.cc.LoadBitmap("btn_small_off"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
                btnPageIndex.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnPageIndex.Text = "1/20";
                btnPageIndex.X = (int)(6 * gv.squareSize) + (3 * pW);
                btnPageIndex.Y = (2 * gv.squareSize);
                btnPageIndex.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnPageIndex.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            //added
            if (btnTokensRight == null)
            {
                btnTokensRight = new IbbButton(gv, 1.0f);
                btnTokensRight.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnTokensRight.Img2 = gv.cc.LoadBitmap("ctrl_right_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_right_arrow);
                btnTokensRight.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnTokensRight.X = (int)(7f * gv.squareSize) + (3 * pW);
                btnTokensRight.Y = (2 * gv.squareSize);
                btnTokensRight.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnTokensRight.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }

            if (btnSelect == null)
            {
                btnSelect = new IbbButton(gv, 0.8f);
                btnSelect.Text = gv.mod.playerList[gv.screenCastSelector.castingPlayerIndex].playerClass.labelForUseTraitAction.ToUpper() + " SELECTED " + gv.mod.playerList[gv.screenCastSelector.castingPlayerIndex].playerClass.spellLabelSingular.ToUpper();
                btnSelect.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnSelect.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnSelect.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
                btnSelect.Y = 9 * gv.squareSize + pH * 2;
                btnSelect.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnSelect.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }
            if (btnHelp == null)
            {
                btnHelp = new IbbButton(gv, 0.8f);
                btnHelp.Text = "HELP";
                btnHelp.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnHelp.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnHelp.X = 5 * gv.squareSize + padW * 1 + gv.oXshift;
                btnHelp.Y = 9 * gv.squareSize + pH * 2;
                btnHelp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHelp.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnExit == null)
            {
                btnExit = new IbbButton(gv, 0.8f);
                btnExit.Text = "EXIT";
                btnExit.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnExit.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnExit.X = (15 * gv.squareSize) - padW * 1 + gv.oXshift;
                btnExit.Y = 9 * gv.squareSize + pH * 2;
                btnExit.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnExit.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            for (int y = 0; y < slotsPerPage; y++)
            {
                IbbButton btnNew = new IbbButton(gv, 1.0f);
                btnNew.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnNew.ImgOff = gv.cc.LoadBitmap("btn_small_off");
                btnNew.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);

                int x = y % 5;
                int yy = y / 5;
                btnNew.X = ((x + 4) * gv.squareSize) + (padW * (x + 1)) + gv.oXshift;
                btnNew.Y = (3 + yy) * gv.squareSize + (padW * yy + padW);

                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);

                btnSpellSlots.Add(btnNew);
            }
            //DRAW ALL SPELL SLOTS
            if (isInCombat)
            {
                int cntSlot = 0;
                foreach (IbbButton btn in btnSpellSlots)
                {
                    Player pc = getCastingPlayer();

                    if (cntSlot == spellSlotIndex) { btn.glowOn = true; }
                    else { btn.glowOn = false; }

                    sortTraitsForLevelUp(pc);

                    //show only spells for the PC class
                    //if (cntSlot < pc.playerClass.spellsAllowed.Count)
                    if ((cntSlot + (tknPageIndex * slotsPerPage)) < backupKnownSpellTagsInCombat.Count)
                    {
                        //SpellAllowed sa = pc.playerClass.spellsAllowed[cntSlot];
                        Spell sp = gv.mod.getSpellByTag(backupKnownSpellTagsInCombat[cntSlot + (tknPageIndex * slotsPerPage)]);

                        btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                        btn.Img2Off = gv.cc.LoadBitmap(sp.spellImage + "_off");
                        btn.btnState = buttonState.Normal;

                        /*
                        if (pc.knownSpellsTags.Contains(sp.tag))
                        {
                            if (isInCombat) //all spells can be used in combat
                            {
                                //btn.Img = gv.cc.LoadBitmap("btn_small");
                                btn.btnState = buttonState.Normal;
                            }
                            //not in combat so check if spell can be used on adventure maps
                            else if ((sp.useableInSituation.Equals("Always")) || (sp.useableInSituation.Equals("OutOfCombat")))
                            {
                                //btn.Img = gv.cc.LoadBitmap("btn_small");
                                btn.btnState = buttonState.Normal;
                                //btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                            }
                            else //can't be used on adventure map
                            {
                                btn.btnState = buttonState.Off;
                                //btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                                //btn.Img2Off = gv.cc.LoadBitmap(sp.spellImage + "_off");
                            }
                        }
                        else //spell not known
                        {
                            btn.btnState = buttonState.Off;
                            //btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                            //btn.Img2Off = gv.cc.LoadBitmap(sp.spellImage + "_off");
                        }
                        */
                    }
                    else //slot is not in spells allowed index range
                    {
                        btn.btnState = buttonState.Off;
                        btn.Img2 = null;
                        btn.Img2Off = null;
                    }
                    cntSlot++;
                }
            }
            //ouside combat
            else
            {
                int cntSlot = 0;
                foreach (IbbButton btn in btnSpellSlots)
                {
                    Player pc = getCastingPlayer();

                    if (cntSlot == spellSlotIndex) { btn.glowOn = true; }
                    else { btn.glowOn = false; }

                    sortTraitsForLevelUp(pc);

                    //show only spells for the PC class
                    //if (cntSlot < pc.playerClass.spellsAllowed.Count)
                    if ((cntSlot + (tknPageIndex * slotsPerPage)) < backupKnownSpellTagsOutsideCombat.Count)
                    {
                        //SpellAllowed sa = pc.playerClass.spellsAllowed[cntSlot];
                        Spell sp = gv.mod.getSpellByTag(backupKnownSpellTagsOutsideCombat[cntSlot + (tknPageIndex * slotsPerPage)]);

                        btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                        btn.Img2Off = gv.cc.LoadBitmap(sp.spellImage + "_off");
                        btn.btnState = buttonState.Normal;
                    }
                    else //slot is not in spells allowed index range
                    {
                        btn.btnState = buttonState.Off;
                        btn.Img2 = null;
                        btn.Img2Off = null;
                    }
                }
                cntSlot++;
            }
        }
	
	    //CAST SELECTOR SCREEN (COMBAT and MAIN)
        public void redrawCastSelector(bool inCombat)
        {
            Player pc = getCastingPlayer();
            isInCombat = inCombat;
    	    //IF CONTROLS ARE NULL, CREATE THEM
    	    if (btnSelect == null)
    	    {
    		    setControlsStart();
    	    }
            btnSelect.Text = pc.playerClass.labelForCastAction.ToUpper() + " SELECTED " + gv.mod.getPlayerClass(getCastingPlayer().classTag).spellLabelSingular.ToUpper();
            int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = 0;
    	    int locX = pW * 4;
            //int textH = (int)gv.cc.MeasureString("GetHeight", gv.drawFontReg, gv.Width).Height;
            int textH = (int)gv.drawFontRegHeight;
            int spacing = textH; 
            //int spacing = (int)gv.mSheetTextPaint.getTextSize() + pH;
    	    int tabX = pW * 4;
    	    int noticeX = pW * 5;
    	    int noticeY = pH * 3 + spacing;
    	    int leftStartY = pH * 3;
    	    int tabStartY = 4 * gv.squareSize + pW * 10;

            //DRAW TEXT		
		    locY = (gv.squareSize * 0) + (pH * 2);
		    //gv.mSheetTextPaint.setColor(Color.LTGRAY);
		    gv.DrawText("Select a " + gv.mod.getPlayerClass(pc.classTag).spellLabelSingular + " to " + gv.mod.getPlayerClass(pc.classTag).labelForCastAction, noticeX, pH * 3);
            //gv.DrawText("Select a " + gv.mod.getPlayerClass(pc.classTag).spellLabelSingular + " to Cast", noticeX, pH * 3, "wh");
            //gv.mSheetTextPaint.setColor(Color.YELLOW);
            gv.DrawText(getCastingPlayer().name + " SP: " + getCastingPlayer().sp + "/" + getCastingPlayer().spMax, pW * 55, leftStartY);
            gv.DrawText(getCastingPlayer().name + "  HP: " + getCastingPlayer().hp + "/" + getCastingPlayer().hpMax, pW * 55, leftStartY + (int)(gv.squareSize / 3));

            //DRAW NOTIFICATIONS
            //here insert
            sortTraitsForLevelUp(pc);

            if (isSelectedSpellSlotInKnownSpellsRange())
		    {
			    Spell sp = GetCurrentlySelectedSpell();
                //Player pc = getCastingPlayer();	

                if ((pc.sp >= sp.costSP) && ((pc.hp - 1) >= sp.costHP))
                {
                    //gv.mSheetTextPaint.setColor(Color.GREEN);
                    gv.DrawText("Available", noticeX, noticeY, 1.0f, Color.Lime);
                }
                else //if known but not enough spell points, "Insufficient SP to Cast" in yellow
                {
                    //gv.mSheetTextPaint.setColor(Color.YELLOW);
                    gv.DrawText("Insufficient SP or HP", noticeX, noticeY, 1.0f, Color.Red);
                }
		    }		
		
		    //DRAW ALL SPELL SLOTS		
		    int cntSlot = 0;
		    foreach (IbbButton btn in btnSpellSlots)
		    {			
			    //Player pc = getCastingPlayer();						
			
			    if (cntSlot == spellSlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}
                if (isInCombat)
                {
                    if ((cntSlot + (tknPageIndex * slotsPerPage)) < backupKnownSpellTagsInCombat.Count)
                    {
                        Spell sp = gv.mod.getSpellByTag(backupKnownSpellTagsInCombat[cntSlot + (tknPageIndex * slotsPerPage)]);
                        //TraitAllowed ta = backupTraitsAllowed[cntSlot + (tknPageIndex * slotsPerPage)];
                        gv.cc.DisposeOfBitmap(ref btn.Img2);
                        btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                        gv.cc.DisposeOfBitmap(ref btn.Img2Off);
                        btn.Img2Off = gv.cc.LoadBitmap(sp.spellImage + "_off");
                        btn.btnState = buttonState.Normal;
                    }
                    else
                    {
                        gv.cc.DisposeOfBitmap(ref btn.Img);
                        btn.Img = gv.cc.LoadBitmap("btn_small_off");
                        btn.Img2 = null;
                        btn.Img2Off = null;
                        btn.Img3 = null;
                        btn.btnState = buttonState.Off;
                    }
                }

                //outside combat
                else
                {
                    if ((cntSlot + (tknPageIndex * slotsPerPage)) < backupKnownSpellTagsOutsideCombat.Count)
                    {
                        Spell sp = gv.mod.getSpellByTag(backupKnownSpellTagsOutsideCombat[cntSlot + (tknPageIndex * slotsPerPage)]);
                        //TraitAllowed ta = backupTraitsAllowed[cntSlot + (tknPageIndex * slotsPerPage)];
                        gv.cc.DisposeOfBitmap(ref btn.Img2);
                        btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                        gv.cc.DisposeOfBitmap(ref btn.Img2Off);
                        btn.Img2Off = gv.cc.LoadBitmap(sp.spellImage + "_off");
                        btn.btnState = buttonState.Normal;
                    }
                    else
                    {
                        gv.cc.DisposeOfBitmap(ref btn.Img);
                        btn.Img = gv.cc.LoadBitmap("btn_small_off");
                        btn.Img2 = null;
                        btn.Img2Off = null;
                        btn.Img3 = null;
                        btn.btnState = buttonState.Off;
                    }
                }

                btn.Draw();
			    cntSlot++;
		    }
		
		    //DRAW DESCRIPTION BOX
		    locY = tabStartY;		
		    if (isSelectedSpellSlotInKnownSpellsRange())
		    {
			    Spell sp = GetCurrentlySelectedSpell();
			    //string textToSpan = "<u>Description</u>" + "<BR>" + "<BR>";
	            string textToSpan = "<b><i><big>" + sp.name + "</big></i></b><BR>";
	            textToSpan += "SP Cost: " + sp.costSP + "<BR>";
                textToSpan += "HP Cost: " + sp.costHP + "<BR>";
                textToSpan += "Target Range: " + sp.range + "<BR>";
	            textToSpan += "Area of Effect Radius: " + sp.aoeRadius + "<BR>";
	            textToSpan += "Available at Level: " + getLevelAvailable(sp.tag) + "<BR>";
	            textToSpan += "<BR>";
	            textToSpan += sp.description;

                description.tbXloc = 11 * gv.squareSize;
                description.tbYloc = 2 * gv.squareSize;
                description.tbWidth = pW * 40;
                description.tbHeight = pH * 80;
                description.logLinesList.Clear();
                description.AddHtmlTextToLog(textToSpan);
                description.onDrawLogBox();
		    }
		
		    btnHelp.Draw();	
		    btnExit.Draw();	
		    btnSelect.Draw();
            btnTokensLeft.Draw();
            btnTokensRight.Draw();
            btnPageIndex.Draw();
        }
        public void onTouchCastSelector(MouseEventArgs e, MouseEventType.EventType eventType, bool inCombat)
	    {
		    btnHelp.glowOn = false;
		    btnExit.glowOn = false;
		    btnSelect.glowOn = false;
            btnTokensLeft.glowOn = false;
            btnTokensRight.glowOn = false;
            btnPageIndex.glowOn = false;

            switch (eventType)
		    {
		    case MouseEventType.EventType.MouseDown:
		    case MouseEventType.EventType.MouseMove:
			    int x = (int) e.X;
			    int y = (int) e.Y;
			    if (btnHelp.getImpact(x, y))
			    {
				    btnHelp.glowOn = true;
			    }
			    else if (btnSelect.getImpact(x, y))
			    {
				    btnSelect.glowOn = true;
			    }
			    else if (btnExit.getImpact(x, y))
			    {
				    btnExit.glowOn = true;
			    }
                    else if (btnTokensLeft.getImpact(x, y))
                    {
                        btnTokensLeft.glowOn = true;
                    }
                    else if (btnTokensRight.getImpact(x, y))
                    {
                        btnTokensRight.glowOn = true;
                    }
                    else if (btnPageIndex.getImpact(x, y))
                    {
                        btnPageIndex.glowOn = true;
                    }
                    break;
			
		    case MouseEventType.EventType.MouseUp:
			    x = (int) e.X;
			    y = (int) e.Y;
			
			    btnHelp.glowOn = false;
			    //btnInfo.glowOn = false;
			    btnExit.glowOn = false;
			    btnSelect.glowOn = false;
                    btnTokensLeft.glowOn = false;
                    btnTokensRight.glowOn = false;
                    btnPageIndex.glowOn = false;

                    for (int j = 0; j < slotsPerPage; j++)
			    {
				    if (btnSpellSlots[j].getImpact(x, y))
				    {
					    spellSlotIndex = j;
				    }
			    }

                    if (btnTokensLeft.getImpact(x, y))
                    {
                        if (tknPageIndex > 0)
                        {
                            tknPageIndex--;
                            btnPageIndex.Text = (tknPageIndex + 1) + "/" + maxPages;
                        }
                    }
                    else if (btnTokensRight.getImpact(x, y))
                    {
                        if (tknPageIndex < maxPages)
                        {
                            tknPageIndex++;
                            btnPageIndex.Text = (tknPageIndex + 1) + "/" + maxPages;
                        }
                    }

                    else if (btnHelp.getImpact(x, y))
			    {
				    tutorialMessageCastingScreen();
			    }
			    else if (btnSelect.getImpact(x, y))
			    {
				    doSelectedSpell(inCombat);
			    }
			    else if (btnExit.getImpact(x, y))
			    {
				    if (inCombat)
				    {
					    if (gv.screenCombat.canMove)
					    {
						    gv.screenCombat.currentCombatMode = "move";
					    }
					    else
					    {
						    gv.screenCombat.currentCombatMode = "attack";
					    }
					    gv.screenType = "combat";
					    doCleanUp();
				    }
				    else
				    {
					    gv.screenType = "main";	
					    doCleanUp();
				    }							
			    }
			    break;		
		    }
	    }
    
        public void doCleanUp()
	    {
    	    btnSpellSlots.Clear();
    	    btnHelp = null;
    	    btnSelect = null;
    	    btnExit = null;
            btnTokensLeft = null;
            btnTokensRight = null;
            btnPageIndex = null;
        }
    
        public void doSelectedSpell(bool inCombat)
	    {            
		    if (isSelectedSpellSlotInKnownSpellsRange())
		    {
			    //only allow to cast spells that you know and are usable on this map
			    if (getCastingPlayer().knownSpellsTags.Contains(GetCurrentlySelectedSpell().tag))
			    {
				    if (inCombat) //Combat Map
				    {
					
					    if ((getCastingPlayer().sp >= GetCurrentlySelectedSpell().costSP) && (getCastingPlayer().hp > GetCurrentlySelectedSpell().costHP))
                        {
                            //if (GetCurrentlySelectedSpell().castTimeInTurns == 0)
                            //{
                                gv.cc.currentSelectedSpell = GetCurrentlySelectedSpell();
                                gv.screenType = "combat";
                                gv.screenCombat.currentCombatMode = "cast";
                                doCleanUp();
                            //}
                            //a spell requirign a turn or more is cast
                            //else
                            //{
                                //getCastingPlayer().sp = -GetCurrentlySelectedSpell().costSP;
                                //getCastingPlayer().hp = -GetCurrentlySelectedSpell().costHP;
                                //endturn
                            //}
					    }
					    else
					    {
						    //Toast.makeText(gv.gameContext, "Not Enough SP for that spell", Toast.LENGTH_SHORT).show();
					    }
				    }
				    else //Adventure Map
				    {
					    //only cast if useable on adventure maps
					    if ((GetCurrentlySelectedSpell().useableInSituation.Equals("Always")) || (GetCurrentlySelectedSpell().useableInSituation.Equals("OutOfCombat")))
					    {						
						    if ((getCastingPlayer().sp >= GetCurrentlySelectedSpell().costSP) && (getCastingPlayer().hp > GetCurrentlySelectedSpell().costHP))
						    {
							    gv.cc.currentSelectedSpell = GetCurrentlySelectedSpell();
                                //ask for target
                                // selected to USE ITEM

                                //********************************************

                                //if target is SELF then just do doSpellTarget(self) 
                                if (gv.cc.currentSelectedSpell.spellTargetType.Equals("Self"))
                                {
                                    doSpellTarget(getCastingPlayer(), getCastingPlayer());
                                }

                                //********************************************
                                else
                                {

                                    List<string> pcNames = new List<string>();
                                    pcNames.Add("cancel");
                                    foreach (Player p in gv.mod.playerList)
                                    {
                                        pcNames.Add(p.name);
                                    }

                                    //If only one PC, do not show select PC dialog...just go to cast selector
                                    if (gv.mod.playerList.Count == 1)
                                    {
                                        try
                                        {
                                            Player target = gv.mod.playerList[0];
                                            gv.cc.doSpellBasedOnScriptOrEffectTag(gv.cc.currentSelectedSpell, target, target, true, false);
                                            gv.screenType = "main";
                                            doCleanUp();
                                            return;
                                        }
                                        catch (Exception ex)
                                        {
                                            gv.errorLog(ex.ToString());
                                        }
                                    }

                                    using (ItemListSelector pcSel = new ItemListSelector(gv, pcNames, gv.mod.getPlayerClass(getCastingPlayer().classTag).spellLabelSingular + " Target"))
                                    {
                                        pcSel.ShowDialog();
                                        Player pc = getCastingPlayer();
                                        if (pcSel.selectedIndex > 0)
                                        {
                                            try
                                            {
                                                Player target = gv.mod.playerList[pcSel.selectedIndex - 1];
                                                gv.cc.doSpellBasedOnScriptOrEffectTag(gv.cc.currentSelectedSpell, pc, target, true, false);
                                                gv.screenType = "main";
                                                doCleanUp();
                                            }
                                            catch (Exception ex)
                                            {
                                                IBMessageBox.Show(gv, "error with Pc Selector screen: " + ex.ToString());
                                                gv.errorLog(ex.ToString());
                                            }
                                        }
                                        else if (pcSel.selectedIndex == 0) // selected "cancel"
                                        {
                                            //do nothing
                                        }
                                    }
                                }//closing else or target self
						    }
						    else
						    {
							    //Toast.makeText(gv.gameContext, "Not Enough SP for that spell", Toast.LENGTH_SHORT).show();
						    }
					    }
				    }
			    }
		    }            
	    }

        public void doSpellTarget(Player pc, Player target)
        {  
        try  
        {  
                 gv.cc.doSpellBasedOnScriptOrEffectTag(gv.cc.currentSelectedSpell, pc, target, !isInCombat, false);  
                 gv.screenType = "main";  
                 doCleanUp();  
        }  
        catch (Exception ex)  
        {  
                 gv.sf.MessageBoxHtml("error with Pc Selector screen: " + ex.ToString());  
                 gv.errorLog(ex.ToString());  
        }  
        }  

    
        public Spell GetCurrentlySelectedSpell()
	    {
            //SpellAllowed sa = getCastingPlayer().playerClass.spellsAllowed[spellSlotIndex];
            //return gv.mod.getSpellByTag(sa.tag);
            if (isInCombat)
            {
                return gv.mod.getSpellByTag(backupKnownSpellTagsInCombat[spellSlotIndex + (tknPageIndex * slotsPerPage)]);
            }
            else
            {
                return gv.mod.getSpellByTag(backupKnownSpellTagsOutsideCombat[spellSlotIndex + (tknPageIndex * slotsPerPage)]);
            }
	    }
        public bool isSelectedSpellSlotInKnownSpellsRange()
	    {
            //return spellSlotIndex < getCastingPlayer().playerClass.spellsAllowed.Count;
            if (isInCombat)
            {
                return ((spellSlotIndex + (tknPageIndex * slotsPerPage)) < backupKnownSpellTagsInCombat.Count);
            }
            else
            {
                return ((spellSlotIndex + (tknPageIndex * slotsPerPage)) < backupKnownSpellTagsOutsideCombat.Count);
            }
        }
        public int getLevelAvailable(String tag)
	    {
		    SpellAllowed sa = getCastingPlayer().playerClass.getSpellAllowedByTag(tag);
		    if (sa != null)
		    {
			    return sa.atWhatLevelIsAvailable;
		    }
		    return 0;
	    }
	    public Player getCastingPlayer()
	    {
		    return gv.mod.playerList[castingPlayerIndex];
	    }
	    public void tutorialMessageCastingScreen()
        {
		    gv.sf.MessageBoxHtml(this.stringMessageCastSelector);	
        }
    }
}
